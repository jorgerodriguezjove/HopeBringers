using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    #region VARIABLES

    //CREACIÓN DE MAPA--------------------------------------------

    //Array donde se meten los tiles en el editor
    [SerializeField]
    private GameObject[] tilesInScene;

    //2D array con las coordenadas de los tiles. (Básicamente convierte el array tilesInScene en un array 2D)
    private GameObject[,] tilesCoord;

    [HideInInspector]
    public int mapSizeX = 10;
    [HideInInspector]
    public int mapSizeZ = 10;

    //Array con script de tiles que voy a usar para calcular el pathfinding
    [HideInInspector]
    public IndividualTiles[,] graph;

    //PATHFINDING--------------------------------------------------

    //Variable que se usa para almacenar el resultado del pathfinding y enviarlo.
    float tempCurrentPathCost;

    //Lista de tiles que forman el path desde un tile hasta otro. Al igual que temCurrentPathCost se resetea cada vez que se llama a la función CalculatePathForMovement
    [HideInInspector]
    public List<IndividualTiles> currentPath = new List<IndividualTiles>();

    //Si es true se mueve en diagonal, si no se mueve en torre.
    [SerializeField]
    public bool isDiagonalMovement;

    //Personaje actualmente seleccionado
    private PlayerUnit selectedCharacter;

    //Tiles que actualmente están dispoibles para el movimiento de la unidad seleccionada.
    List<IndividualTiles> tilesAvailableForMovement = new List<IndividualTiles>();

    //REFERENCIAS--------------------------------------------------

    private LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        SaveTilePosition();
        GeneratePathFindingGraph();
    }

    //Ordeno el array tilesInScene con los 100 tiles en un array 2D 10x10
    void SaveTilePosition()
    {
        tilesCoord = new GameObject[mapSizeZ, mapSizeX];
        int k = 0;
        for (int i = 0; i < mapSizeZ; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                tilesCoord[j, i] = tilesInScene[k];
                k++;
            }
        }
    }

    //Genero el graph con los nodos que voy a usar para calcular el pathfinding.
    void GeneratePathFindingGraph()
    {
        //Inicializo el array
        graph = new IndividualTiles[mapSizeX, mapSizeZ];

        //Obtengo una referencia del script de cada tile, lo guardo en la lista y le paso sus coordenadas y una referncia al GM
        for (int i = 0; i < mapSizeZ; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                graph[j, i] = tilesCoord[j, i].GetComponent<IndividualTiles>();
                graph[j, i].GetComponent<IndividualTiles>().TM = this;
                graph[j, i].GetComponent<IndividualTiles>().LM = LM;
                graph[j, i].tileX = j;
                graph[j, i].tileZ = i;
            }
        }

        //Una vez que todos los tiles en el array existen y saben sus coordenadas, calculo los nodos vecinos y se los paso a cada tile.
        for (int i = 0; i < mapSizeZ; i++)
        {
            for (int j = 0; j < mapSizeX; j++)

            {
                //Casilla vecina de la izquierda
                if (j > 0)
                {
                    graph[j, i].neighbours.Add(graph[j - 1, i]);

                    for (int k = 1; j - k >= 0 ; k++)
                    {
                        graph[j, i].tilesInLineLeft.Add(graph[j - k, i]);
                    }
                }

                //Casilla vecina de la derecha
                if (j < mapSizeX - 1)
                {
                    graph[j, i].neighbours.Add(graph[j + 1, i]);

                    for (int k = 1; k < mapSizeX - j ; k++)
                    {
                        graph[j, i].tilesInLineRight.Add(graph[j + k, i]);
                    }
                }

                //Casilla vecina de abajo
                if (i > 0)
                {
                    graph[j, i].neighbours.Add(graph[j, i - 1]);

                    for (int k = 1; i - k >= 0; k++)
                    {
                        graph[j, i].tilesInLineDown.Add(graph[j, i- k]);
                    }
                }

                //Casilla vecina de arriba
                if (i < mapSizeZ - 1)
                {
                    graph[j, i].neighbours.Add(graph[j, i + 1]);

                    for (int k = 1; k < mapSizeZ - i; k++)
                    {
                        graph[j, i].tilesInLineUp.Add(graph[j, i + k]);
                    }
                }
            }
        }

      
    }

    #endregion

    #region PATHFINDING

    //Calculo el coste de una casilla
    float CostToEnterTile(int x, int z)
    {
        return graph[x, z].currentMovementCost;
    }

    //Doy feedback de que casillas están al alcance del personaje.
    public List<IndividualTiles> checkAvailableTilesForMovement(int movementUds, PlayerUnit selectedUnit)
    {
        selectedCharacter = selectedUnit;
        tilesAvailableForMovement.Clear();
        tempCurrentPathCost = 0;

        for (int i = 0; i < mapSizeZ; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                
                CalculatePathForMovementCost(j, i);
                if (tempCurrentPathCost <= movementUds)
                {
                    if (graph[j, i].unitOnTile == null)
                    {
                        graph[j, i].ColorSelect();
                        tilesAvailableForMovement.Add(graph[j, i]);
                    }
                }
                tempCurrentPathCost = 0;
            }
        }

        return tilesAvailableForMovement;
    }

    [SerializeField]
    public List<IndividualTiles> unvisited;

    //Calculo el coste que tiene el personaje por ir a cada casilla.
    public void CalculatePathForMovementCost(int x, int z)
    {
        currentPath.Clear();

        //Diccionario con distancia a nodos
        Dictionary<IndividualTiles, float> dist = new Dictionary<IndividualTiles, float>();
        //Diccionario con nodos que forman el camino para llegar al objetivo.
        Dictionary<IndividualTiles, IndividualTiles> prev = new Dictionary<IndividualTiles, IndividualTiles>();

        //Lista con los nodos que todavía no han sido comprobados al buscar el camino.

        unvisited.Clear();
        unvisited = new List<IndividualTiles>();

        //Punto de origen (Nodo en el que está el personaje).
        IndividualTiles source = graph[selectedCharacter.GetComponent<PlayerUnit>().myCurrentTile.tileX, selectedCharacter.GetComponent<PlayerUnit>().myCurrentTile.tileZ];

        //Casilla objetivo a la que queremos llegar.
        IndividualTiles target = graph[x, z];

        //La distancia que hay desde el origen hasta el origen es 0. Por lo que en el diccionario, el nodo que coincida con el origen, su float valdrá 0.
        dist[source] = 0;
        //No hay ningún nodo antes que el origen por lo que el valor de source en el diccionario es null.
        prev[source] = null;


        //Inicializamos para que pueda llegar hasta alcance infinito ya que no se la distancia hasta el objetivo. Al ponerlos todos en infinitos menos el source, me aseguro que empieza desde ahí.
        //En principio no llegará nunca hasta el infinito porque encontrará antes el objetivo y entonces se cortará el proceso.
        //También sirve para contemplar las casillas a las que no se puede llegar (es cómo si tuviesen valor infinito).
        foreach (IndividualTiles node in graph)
        {
            //Si el nodo no ha sido quitado de los nodos sin visitar
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }

            //Todos los nodos se añaden a la lista de unvisited, incluido el origen.

            if (!isDiagonalMovement)
            {
                if (node.tileX == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileX || node.tileZ == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileZ)
                {
                    unvisited.Add(node);
                }
            }

            //else
            //{
            //    unvisited.Add(node);
            //}

        }

        //Mientras que haya nodos que no hayan sido visitados...
        while (unvisited.Count > 0)
        {
            //currentNode se corresponde con el nodo no visitado con la distancia más corta
            //La primera vez va a ser source ya que es el único nodo que no tiene valor infinito
            //Después de eso sólo podrá coger una de las casillas vecinas y así irá repitiendo el ciclo.
            IndividualTiles currentNode = null;

            foreach (IndividualTiles possibleNode in unvisited)
            {
                if (currentNode == null || dist[possibleNode] < dist[currentNode])
                {
                    if (!isDiagonalMovement)
                    {
                        if (possibleNode.tileX == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileX || possibleNode.tileZ == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileZ)
                        {
                            currentNode = possibleNode;
                        }
                    }

                    //else
                    //{
                    //    currentNode = possibleNode;
                    //}
                }
            }

            //Si el nodo coincide con el objetivo, terminamos la busqueda.
            if (currentNode == target)
            {
                break;
            }

            unvisited.Remove(currentNode);

            foreach (IndividualTiles node in currentNode.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[currentNode] + CostToEnterTile(node.tileX, node.tileZ);

                if (alt < dist[node])
                {
                    if (Mathf.Abs(node.height - currentNode.height) <= 1)
                    {
                        dist[node] = alt;
                        prev[node] = currentNode;
                    }
                }
            }
        }

        if (prev[target] == null)
        {
            //Si llega aquí significa que no hay ninguna ruta disponible desde el origen hasta el objetivo.
            tempCurrentPathCost = Mathf.Infinity;
        }

        //Si llega hasta aquí si que hay un camino hasta el objetivo.

        IndividualTiles curr = target;


        //Recorre la cadena de Prev y la añade a la lista que guarda el camino.
        //Esta ruta está al reves, va desde el objetivo hasta el origen.
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        //Le damos la vuelta a la lista para que vaya desde el orgien hasta el objetivo.
        currentPath.Reverse();

        //Calcular coste del path
        for (int i = 0; i < currentPath.Count; i++)
        {
            //Sumo el coste de todas las casillas que forman el path excepto la primera (ya que es la casilla sobre la que se encuentra la unidad).
            if (i != 0)
            {
                tempCurrentPathCost += CostToEnterTile(currentPath[i].tileX, currentPath[i].tileZ);
            }
        }
    }



    #endregion


}
