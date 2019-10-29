using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    #region VARIABLES

    //Array donde se meten los tiles en el editor
    [SerializeField]
    GameObject[] tilesInScene;

    //2D array con las coordenadas de los tiles. (Básicamente convierte el array tilesInScene en un array 2D)
    GameObject[,] tilesCoord;

    [HideInInspector]
    public int mapSizeX = 10;
    [HideInInspector]
    public int mapSizeZ = 10;

    //Array con script de tiles que voy a usar para calcular el pathfinding
    [HideInInspector]
    public IndividualTiles[,] graph;

    //Variable que se usa para almacenar el resultado del pathfinding y enviarlo.
    float tempCurrentPathCost;


    //ACORDARSE DE PONER EN LEVEL MANAGER. AHORA MISMO SOLO ESTÁ PARA PROBAR
    [SerializeField]
    GameObject selectedCharacter;

    //Si es true se mueve en diagonal, si no se mueve en torre.
    [SerializeField]
    private bool isDiagonalMovement;

    #endregion

    #region INIT

    private void Awake()
    {
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

        ////Le paso a cada tile ocupado una referencia de la unidad que lo ocupa.
        //for (int i = 0; i < units.Count; i++)
        //{
        //    TellTileIsOcuppied(units[i]);
        //}
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
                }

                //Casilla vecina de la derecha
                if (j < mapSizeX - 1)
                {
                    graph[j, i].neighbours.Add(graph[j + 1, i]);
                }

                //Casilla vecina de abajo
                if (i > 0)
                {
                    graph[j, i].neighbours.Add(graph[j, i - 1]);
                }

                //Casilla vecina de arriba
                if (i < mapSizeZ - 1)
                {
                    graph[j, i].neighbours.Add(graph[j, i + 1]);
                }
            }
        }
    }

    #endregion

    #region PATHFINDING

    //Calculo el coste de una casilla
    float CostToEnterTile(int x, int z)
    {
        return graph[x, z].movementCost;
    }

    //Doy feedback de que casillas están al alcance del personaje.
    public void checkAvailableTilesForMovement(int movementUds)
    {
        for (int i = 0; i < mapSizeZ; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                if (CalculatePathForMovementCost(j, i) <= movementUds )
                {
                    if (j == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileX || i == selectedCharacter.GetComponent<UnitBase>().myCurrentTile.tileZ)
                    {
                    }
                    //currentPosibleTiles.Add(tilesCoord[j, i]);
                    tilesCoord[j, i].GetComponent<IndividualTiles>().ColorSelect();
                }
                tempCurrentPathCost = 0;
            }
        }
    }


    //Calculo el coste que tiene el personaje por ir a cada casilla.
    public float CalculatePathForMovementCost(int x, int z)
    {
        //Diccionario con distancia a nodos
        Dictionary<IndividualTiles, float> dist = new Dictionary<IndividualTiles, float>();
        //Diccionario con nodos que forman el camino para llegar al objetivo.
        Dictionary<IndividualTiles, IndividualTiles> prev = new Dictionary<IndividualTiles, IndividualTiles>();

        //Lista con los nodos que todavía no han sido comprobados al buscar el camino.
        List<IndividualTiles> unvisited = new List<IndividualTiles>();

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
            unvisited.Add(node);
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
                    currentNode = possibleNode;
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
            return Mathf.Infinity;
        }

        //Si llega hasta aquí si que hay un camino hasta el objetivo.

        List<IndividualTiles> currentPath = new List<IndividualTiles>();
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

        return tempCurrentPathCost;
    }


#endregion


}
