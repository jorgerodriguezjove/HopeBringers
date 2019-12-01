using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridVisualization : MonoBehaviour
{
    #region VARIABLES

    public LayerMask unwalkableMask;
    //Tamaño del área dónde va a haber tiles
    public Vector3 gridWorldSize;
    //Radio de los tiles
    public float nodeRadius;
    //Array de 2 dimensiones con los tiles
    GameObject[,,] gridObject;
    Node[,,] gridNode;

    //Prefab del tile
    [SerializeField]
    GameObject tilePref;

    float nodeDiameter;
    int gridSizeX, gridSizeZ;

    int gridSizeY;

    [Header("FUNCIÓN CREAR PATH")]

    //Diccionario con distancia a nodos
    Dictionary<Node, float> dist = new Dictionary<Node, float>();
    //Diccionario con nodos que forman el camino para llegar al objetivo.
    Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
    //Lista con los nodos que todavía no han sido comprobados al buscar el camino.
    [SerializeField]
    public List<Node> unvisited = new List<Node>();

    //Punto de origen (Nodo en el que está el personaje).
    Node source;

    //Casilla objetivo a la que queremos llegar.
    Node target;

    //Current tile que se está comprobando para hacer el path (antes de invertir el path).
    Node curr;

    [Header("PATHFINDING")]

    //Variable que se usa para almacenar el resultado del pathfinding y enviarlo.
    float tempCurrentPathCost;

    //Personaje actualmente seleccionado
    private UnitBase selectedCharacter;

    //Lista de tiles que forman el path desde un tile hasta otro. Al igual que temCurrentPathCost se resetea cada vez que se llama a la función CalculatePathForMovement
    [HideInInspector]
    public List<Node> currentPath = new List<Node>();

    //Almaceno el tile wue estoy comprobando aora mismo para no acceder todo el rato desde el selected character
    private Node currentTileCheckingForMovement;

    //Tiles que actualmente están dispoibles para el movimiento de la unidad seleccionada.
    List<Node> tilesAvailableForMovement = new List<Node>();

    [Header("REFERENCIAS")]
    [SerializeField]
    LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        //Inicializo el array y la posición en la que se inicia a comprobar el grid
        gridObject = new GameObject[gridSizeX, gridSizeY, gridSizeZ];
        gridNode = new Node[gridSizeX, gridSizeY ,gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y/2 - Vector3.forward * gridWorldSize.z / 2;

        //Creo y guardo los tiles
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool isObstacle = false;
                    bool empty = false;
                    if (y == 0)
                    {
                        isObstacle = (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    }

                    else
                    {
                        if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                        {
                            empty = false;
                            isObstacle = true;
                        }

                        else if (gridNode[x, y - 1, z].isObstacle && !gridNode[x, y - 1, z].isEmpty)
                        {
                            empty = false;
                            isObstacle = false;
                        }

                        else
                        {
                            empty = true;
                            isObstacle = false;
                        }
                    }

                    gridObject[x, y, z] = Instantiate(tilePref, new Vector3(worldPoint.x, worldPoint.y - 0.5f, worldPoint.z), Quaternion.identity);

                    gridObject[x, y, z].AddComponent<Node>();

                    gridObject[x, y, z].GetComponent<Node>().SetVariables(isObstacle, empty, worldPoint, x, y, z, tilePref, LM);

                    gridNode[x, y, z] = gridObject[x, y, z].GetComponent<Node>();

                 

                }
            }
        }


        SetTilesNeighbours();
    }

    int prevY;

    //NO ESTÁ PENSADO PARA QUE HAYA TILES ENCIMA DE OTROS A DIFERENTES ALTURAS.
    void SetTilesNeighbours()
    {
        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                //Guardo la y del tile que estoy comprobando.
                for (int _prevY = 0; _prevY < gridSizeY; _prevY++)
                {
                    if (!gridNode[x, _prevY, z].isEmpty && !gridNode[x, _prevY, z].isObstacle)
                    {
                        prevY = _prevY;
                    }
                }

                #region Izquierda
                if (x > 0)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; x - k >= 0; k++)
                    {
                        //Compruebo todas las alturas de un tile hasta que encuentrao un tile en el que me puedo poner
                        for (int y = 0; y < gridSizeY; y++)
                        {
                            if (!gridNode[x-k, y, z].isEmpty && !gridNode[x-k, y, z].isObstacle)
                            {
                                if (k == 1)
                                {
                                    gridNode[x, prevY, z].neighbours.Add(gridNode[x - 1, y, z]);
                                }

                                if (!gridNode[x, prevY, z].isEmpty && !gridNode[x, prevY, z].isObstacle)
                                {
                                    gridNode[x, prevY, z].tilesInLineLeft.Add(gridNode[x - k, y, z]);
                                }

                                break;
                            }
                        }
                    }
                }

                #endregion

                #region Derecha

                if (x < gridSizeX -1)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; k < gridSizeX - x; k++)
                    {
                        //Compruebo todas las alturas de un tile hasta que encuentro un tile en el que me puedo poner
                        for (int y = 0; y < gridSizeY; y++)
                        {
                            if (!gridNode[x + k, y, z].isEmpty && !gridNode[x + k, y, z].isObstacle)
                            {
                                if (k == 1)
                                {
                                    gridNode[x, prevY, z].neighbours.Add(gridNode[x + 1, y, z]);
                                }

                                if (!gridNode[x, prevY, z].isEmpty && !gridNode[x, prevY, z].isObstacle)
                                {
                                    gridNode[x, prevY, z].tilesInLineRight.Add(gridNode[x + k, y, z]);
                                }

                                break;
                            }
                        }
                    }
                }

                #endregion

                #region Abajo

                if (z > 0)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; z - k >= 0; k++)
                    {
                        //Compruebo todas las alturas de un tile hasta que encuentrao un tile en el que me puedo poner
                        for (int y = 0; y < gridSizeY; y++)
                        {
                            if (!gridNode[x, y, z - k].isEmpty && !gridNode[x, y, z - k].isObstacle)
                            {
                                if (k == 1)
                                {
                                    gridNode[x, prevY, z].neighbours.Add(gridNode[x, y, z - 1]);
                                }

                                if (!gridNode[x, prevY, z].isEmpty && !gridNode[x, prevY, z].isObstacle)
                                {
                                    gridNode[x, prevY, z].tilesInLineDown.Add(gridNode[x, y, z - k]);
                                }

                                break;
                            }
                        }
                    }
                }

                #endregion

                #region Arriba

                if (z < gridSizeZ - 1)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; k < gridSizeZ - z; k++)
                    {
                        //Compruebo todas las alturas de un tile hasta que encuentrao un tile en el que me puedo poner
                        for (int y = 0; y < gridSizeY; y++)
                        {
                            if (!gridNode[x, y, z + k].isEmpty && !gridNode[x, y, z + k].isObstacle)
                            {
                                if (k == 1)
                                {
                                    gridNode[x, prevY, z].neighbours.Add(gridNode[x, y, z + 1]);
                                }

                                if (!gridNode[x, prevY, z].isEmpty && !gridNode[x, prevY, z].isObstacle)
                                {
                                    gridNode[x, prevY, z].tilesInLineUp.Add(gridNode[x, y, z + k]);
                                }

                                break;
                            }
                        }
                    }
                }

                #endregion





            }
        }
    }

    #endregion

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);
        percentY= Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return gridNode[x, y, z];
    }



    //#region PATHFINDING

    ////Calculo el coste de una casilla
    //float CostToEnterTile(int x, int y, int z)
    //{
    //    return gridNode[x, y, z].movementCost;
    //}

    ////Calculo tiles a los que se puede mover una unidad o en los que puede un enemigo buscar objetivos
    //public List<Node> OptimizedCheckAvailableTilesForMovement(int movementUds, UnitBase selectedUnit)
    //{
    //    selectedCharacter = selectedUnit;
    //    tilesAvailableForMovement.Clear();

    //    //Recorro de izquierda a derecha los tiles que pueden estar disponibles para moverse (Va moviendose en X columna a columna)
    //    for (int i = -movementUds; i < (movementUds * 2) + 1; i++)
    //    {
    //        //Al restar a losMovementUds el i actual obtengo los tiles que hay por encima de la posición del personaje en dicha columna
    //        //Este número me sirve para calcular la posición en z de los tiles
    //        int tilesInZ = movementUds - Mathf.Abs(i);

    //        //Esto significa que es el extremo del rombo y sólo hay 1 tile en vertical
    //        if (tilesInZ == 0)
    //        {
    //            //Compruebo si existe un tile con esas coordenadas
    //            if (selectedCharacter.myNode.xPosition + i < gridSizeX   && selectedCharacter.myNode.xPosition + i >= 0 &&
    //                selectedCharacter.myNode.zPosition < gridSizeZ       && selectedCharacter.myNode.zPosition >= 0     &&
    //                selectedCharacter.myNode.yPosition < gridSizeZ)
    //            {

    //                //Almaceno el tile en una variable
    //                //Al igual que en la funcion de setear los vecinos tengo que usar un for para encontrar la y del tile
    //                for (int _prevY = 0; _prevY < gridSizeY; _prevY++)
    //                {
    //                    if (!gridNode[selectedCharacter.myNode.xPosition + i, _prevY, selectedCharacter.myNode.zPosition].isEmpty && !gridNode[selectedCharacter.myNode.xPosition + i, _prevY, selectedCharacter.myNode.zPosition].isObstacle)
    //                    {
    //                        currentTileCheckingForMovement = gridNode[selectedCharacter.myNode.xPosition + i, prevY ,selectedCharacter.myNode.zPosition];
    //                        break;
    //                    }
    //                }

    //                //Compruebo si el tile está ocupado, tiene un obstáculo o es un tile vacío
    //                if (!currentTileCheckingForMovement.isEmpty && !currentTileCheckingForMovement.isObstacle)
    //                {
    //                    if (selectedCharacter.GetComponent<EnemyUnit>() || (selectedCharacter.GetComponent<PlayerUnit>() && currentTileCheckingForMovement.unitOnTile == null))
    //                    {
    //                        //Compruebo si existe un camino hasta el tile
    //                        CalculatePathForMovementCost(currentTileCheckingForMovement.xPosition, currentTileCheckingForMovement.yPosition, currentTileCheckingForMovement.zPosition);
    //                        if (tempCurrentPathCost <= movementUds)
    //                        {
    //                            tilesAvailableForMovement.Add(currentTileCheckingForMovement);
    //                        }
    //                        tempCurrentPathCost = 0;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            for (int j = tilesInZ; j >= -tilesInZ; j--)
    //            {
    //                //Compruebo si existe un tile con esas coordenadas
    //                if (selectedCharacter.myNode.xPosition + i < gridSizeX && selectedCharacter.myNode.xPosition + i >= 0 &&
    //                    selectedCharacter.myNode.zPosition + j < gridSizeZ && selectedCharacter.myNode.zPosition + j >= 0 &&
    //                    selectedCharacter.myNode.yPosition < gridSizeY)
    //                {

    //                    //Almaceno el tile en una variable
    //                    //Al igual que en la funcion de setear los vecinos tengo que usar un for para encontrar la y del tile
    //                    for (int _prevY = 0; _prevY < gridSizeY; _prevY++)
    //                    {
    //                        if (!gridNode[selectedCharacter.myNode.xPosition + i, _prevY, selectedCharacter.myNode.zPosition].isEmpty && !gridNode[selectedCharacter.myNode.xPosition + i, _prevY, selectedCharacter.myNode.zPosition].isObstacle)
    //                        {
    //                            currentTileCheckingForMovement = gridNode[selectedCharacter.myNode.xPosition + i, prevY, selectedCharacter.myNode.zPosition];
    //                            break;
    //                        }
    //                    }

    //                    //Compruebo si el tile está ocupado, tiene un obstáculo o es un tile vacío
    //                    if (!currentTileCheckingForMovement.isEmpty && !currentTileCheckingForMovement.isObstacle)
    //                    {
    //                        if (selectedCharacter.GetComponent<EnemyUnit>() || (selectedCharacter.GetComponent<PlayerUnit>() && currentTileCheckingForMovement.unitOnTile == null))
    //                        {
    //                            //Compruebo si existe un camino hasta el tile
    //                            CalculatePathForMovementCost(currentTileCheckingForMovement.xPosition, currentTileCheckingForMovement.yPosition, currentTileCheckingForMovement.zPosition);
    //                            if (tempCurrentPathCost <= movementUds)
    //                            {
    //                                tilesAvailableForMovement.Add(currentTileCheckingForMovement);
    //                            }
    //                            tempCurrentPathCost = 0;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return tilesAvailableForMovement;
    //}

    //void CalculatePathForMovementCost (int x, int y ,int z)
    //{
    //    currentPath.Clear();
    //    unvisited.Clear();

    //    //Origen y target
    //    source = gridNode[selectedCharacter.myNode.xPosition, selectedCharacter.myNode.yPosition, selectedCharacter.myNode.zPosition];
    //    target = gridNode[x,y,z];

    //    //La distancia que hay desde el origen hasta el origen es 0. Por lo que en el diccionario, el nodo que coincida con el origen, su float valdrá 0.
    //    dist[source] = 0;
    //    //No hay ningún nodo antes que el origen por lo que el valor de source en el diccionario es null.
    //    prev[source] = null;

    //    //Inicializamos para que pueda llegar hasta alcance infinito ya que no se la distancia hasta el objetivo. Al ponerlos todos en infinitos menos el source, me aseguro que empieza desde ahí.
    //    //En principio no llegará nunca hasta el infinito porque encontrará antes el objetivo y entonces se cortará el proceso.
    //    //También sirve para contemplar las casillas a las que no se puede llegar (es cómo si tuviesen valor infinito).
    //    foreach (Node node in gridNode)
    //    {
    //        //Si el nodo no ha sido quitado de los nodos sin visitar
    //        if (node != source)
    //        {
    //            dist[node] = Mathf.Infinity;
    //            prev[node] = null;
    //        }

    //        //Todos los nodos se añaden a la lista de unvisited, incluido el origen.
    //        unvisited.Add(node);
    //    }

    //    //Mientras que haya nodos que no hayan sido visitados...
    //    while (unvisited.Count > 0)
    //    {
    //        //currentNode se corresponde con el nodo no visitado con la distancia más corta
    //        //La primera vez va a ser source ya que es el único nodo que no tiene valor infinito
    //        //Después de eso sólo podrá coger una de las casillas vecinas y así irá repitiendo el ciclo.
    //        Node currentNode = null;

    //        foreach (Node possibleNode in unvisited)
    //        {
    //            if (currentNode == null || dist[possibleNode] < dist[currentNode])
    //            {
    //                currentNode = possibleNode;
    //            }
    //        }

    //        //Si el nodo coincide con el objetivo, terminamos la busqueda.
    //        if (currentNode == target)
    //        {
    //            break;
    //        }

    //        unvisited.Remove(currentNode);

    //        foreach (Node node in currentNode.neighbours)
    //        {
    //            if (selectedCharacter.GetComponent<EnGiant>())
    //            {
    //                float alt = dist[currentNode] + CostToEnterTile(node.xPosition, node.yPosition ,node.zPosition);

    //                if (alt < dist[node])
    //                {
    //                    if (Mathf.Abs(node.yPosition - currentNode.yPosition) <= selectedCharacter.maxHeightDifferenceToMove)
    //                    {
    //                        dist[node] = alt;
    //                        prev[node] = currentNode;
    //                    }
    //                }
    //            }

    //            else if (selectedCharacter.GetComponent<EnGoblin>())
    //            {
    //                //Si el nodo no está vacío o un obstáculo puedo seguir comprobando el path
    //                if (!node.isEmpty && !node.isObstacle)
    //                {
    //                    //Exceptuando el target que siempre va a tener una unidad, compruebo si los tiles para formar el path no están ocupados por enemigos
    //                    if ((node != target && node.unitOnTile == null) || node == target)
    //                    {
    //                        float alt = dist[currentNode] + CostToEnterTile(node.xPosition, node.yPosition ,node.zPosition);

    //                        if (alt < dist[node])
    //                        {
    //                            if (Mathf.Abs(node.yPosition - currentNode.yPosition) <= selectedCharacter.maxHeightDifferenceToMove)
    //                            {
    //                                dist[node] = alt;
    //                                prev[node] = currentNode;
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            else
    //            {
    //                if (node.unitOnTile == null && !node.isEmpty && !node.isObstacle)
    //                {
    //                    float alt = dist[currentNode] + CostToEnterTile(node.xPosition, node.yPosition, node.zPosition);

    //                    if (alt < dist[node])
    //                    {
    //                        if (Mathf.Abs(node.yPosition - currentNode.yPosition) <= selectedCharacter.maxHeightDifferenceToMove)
    //                        {
    //                            dist[node] = alt;
    //                            prev[node] = currentNode;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    if (prev[target] == null)
    //    {
    //        //Si llega aquí significa que no hay ninguna ruta disponible desde el origen hasta el objetivo.
    //        tempCurrentPathCost = Mathf.Infinity;
    //    }

    //    //Si llega hasta aquí si que hay un camino hasta el objetivo.
    //    curr = target;

    //    //Recorre la cadena de Prev y la añade a la lista que guarda el camino.
    //    //Esta ruta está al reves, va desde el objetivo hasta el origen.
    //    while (curr != null)
    //    {
    //        currentPath.Add(curr);
    //        curr = prev[curr];
    //    }

    //    //Le damos la vuelta a la lista para que vaya desde el orgien hasta el objetivo.
    //    currentPath.Reverse();

    //    //Calcular coste del path
    //    for (int i = 0; i < currentPath.Count; i++)
    //    {
    //        //Sumo el coste de todas las casillas que forman el path excepto la primera (ya que es la casilla sobre la que se encuentra la unidad).
    //        if (i != 0)
    //        {
    //            tempCurrentPathCost += CostToEnterTile(currentPath[i].xPosition,currentPath[i].yPosition,currentPath[i].zPosition);
    //        }
    //    }
    //}

    //#endregion



    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));


        if (gridNode != null)
        {
            foreach (Node n in gridNode)
            {
                Gizmos.color = (n.isEmpty) ? Color.clear : (n.isObstacle) ? Color.clear : Color.white;
                Gizmos.DrawCube(new Vector3(n.worldPosition.x, n.worldPosition.y - nodeRadius, n.worldPosition.z), new Vector3(1, 0.1f, 1) * (nodeDiameter - .1f));
            }
        }
    }
}

