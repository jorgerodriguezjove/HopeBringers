using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour

{
    #region VARIABLES

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private LayerMask noTileHereMask;

    [SerializeField]
    private LayerMask startignTileMask;


    //Tamaño del área dónde va a haber tiles
    public Vector3 gridWorldSize;
    //Radio de los tiles
    public float nodeRadius;
    //Array de 3 dimensiones con los objetos que van a tener el script de node.
    GameObject[,,] gridObject;
    //Array de 3 dimensiones con los scripts. IMPORTANTE ESTE SOLO SE USA AL COMIENZO LUEGO SE USA EL DE 2 DIMENSIONES
    IndividualTiles[,,] grid3DNode;

    //Array de 2 Dimensiones que se usa para el pathfinding.
    IndividualTiles[,] grid2DNode;

    //Prefab del tile
    [SerializeField]
    GameObject tilePref;

    float nodeDiameter;
    int gridSizeX, gridSizeZ;

    int gridSizeY;

    [Header("FUNCIÓN CREAR PATH")]

    //Diccionario con distancia a nodos
    Dictionary<IndividualTiles, float> dist = new Dictionary<IndividualTiles, float>();
    //Diccionario con nodos que forman el camino para llegar al objetivo.
    Dictionary<IndividualTiles, IndividualTiles> prev = new Dictionary<IndividualTiles, IndividualTiles>();
    //Lista con los nodos que todavía no han sido comprobados al buscar el camino.
    [SerializeField]
    public List<IndividualTiles> unvisited = new List<IndividualTiles>();

    //Punto de origen (Nodo en el que está el personaje).
    IndividualTiles source;

    //Casilla objetivo a la que queremos llegar.
    IndividualTiles target;

    //Current tile que se está comprobando para hacer el path (antes de invertir el path).
    IndividualTiles curr;

    [Header("PATHFINDING")]

    //Variable que se usa para almacenar el resultado del pathfinding y enviarlo.
    float tempCurrentPathCost;

    //Personaje actualmente seleccionado
    private UnitBase selectedCharacter;

    //Lista de tiles que forman el path desde un tile hasta otro. Al igual que temCurrentPathCost se resetea cada vez que se llama a la función CalculatePathForMovement
    [HideInInspector]
    public List<IndividualTiles> currentPath = new List<IndividualTiles>();

    //Almaceno el tile wue estoy comprobando aora mismo para no acceder todo el rato desde el selected character
    private IndividualTiles currentTileCheckingForMovement;

    //Tiles que actualmente están dispoibles para el movimiento de la unidad seleccionada.
    //La pongo pública para que el enemigo pueda acceder.
    public List<IndividualTiles> tilesAvailableForMovement = new List<IndividualTiles>();

    //Lista de tiles sin visitar
    List<IndividualTiles> openList = new List<IndividualTiles>();
    //HasSet de tiles visitados. (En el HashSet no puede haber elementos repetidos aunque no puedes acceder directametne a un elemento con [])
    HashSet<IndividualTiles> closedHasSet = new HashSet<IndividualTiles>();

    [Header("DRAGON")]

    //Bool que sirve para saber si en el caso del dragón el foreach que comprueba los surroundings tiles ha encontrado uno ocupado por el que no pasa el dragón
    bool shouldSkipSurroundingTile;

    //Si el dragón ha encontrado al target en uno de sus surrounding tiles. Sirve para excepción de obstáculo vecino al target.
    bool targetFounInNeighbourTiles;

    //Lista con los obstáculos que rodean al dragón. Si el count es 0 entonces no hay obstáculos y puede colocarse en ese tile
    private List<IndividualTiles> surroundingObstacles = new List<IndividualTiles>();

    bool noTargetInNeighbours;

    //Tile que guarda la altura del tile que estoy comprobando para moverme
    int currentDragonNeighbourCheckingHeight;

    [Header("ENEMY_PATHFINDING")]

    ////Enemigo actual que está calculando su pathfinding
    //private EnemyUnit selectedEnemy;

    //Lista de posibles objetivos del enemigo actual
    List<UnitBase> charactersAvailableForAttack = new List<UnitBase>();

    //Variable que se usa para almacenar la distancia con el objetivo
    float tempCurrentObjectiveCost;

    //Es el equivalente a currentTileCheckingForMovement para buscar enemigos en rango para el goblin
    private IndividualTiles currentTileCheckingForUnit;

    //Lista que le paso al goblin con los enemigos en rango
    private List<UnitBase> unitsInRangeWithoutPathfinding = new List<UnitBase>();

    //Las mismas variables que para el CheckAvailableMovement pero para las acciones enemigas
    public List<IndividualTiles> tilesAvailableForEnemyAction = new List<IndividualTiles>();
    private IndividualTiles currentTileCheckingForEnemyAction;

    [Header("UTILITIES")]

    //Lista que guarda los tiles en un área con forma de rombo alrededor del tile
    [HideInInspector]
    public List<IndividualTiles> rhombusTiles;

    [Header("REFERENCIAS")]
    [SerializeField]
    LevelManager LM;

    #endregion

    #region INIT
    //Esto se llama desde el awake del level manager
    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        //Inicializo el array y la posición en la que se inicia a comprobar el grid
        gridObject = new GameObject[gridSizeX, gridSizeY, gridSizeZ];
        grid3DNode = new IndividualTiles[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position;

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
                    bool noTileInThisColumn = false;
                    bool startingTile = false;

                    //Si es la primera fila (el suelo vamos) únicamente compruebo obstáculos
                    if (y == 0)
                    {
                        //Compruebo si es un tile en el que se puede colocar a un personaje
                        if (Physics.CheckSphere(worldPoint, nodeRadius, startignTileMask))
                        {
                            startingTile = true;
                        }

                        else if (Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask))
                        {
                            empty = false;
                            isObstacle = true;
                            noTileInThisColumn = false;
                        }

                        else if (Physics.CheckSphere(worldPoint, nodeRadius, noTileHereMask))
                        {
                            empty = false;
                            isObstacle = true;
                            noTileInThisColumn = true;
                        }
                    }

                    //Si ya hay altura
                    else
                    {
                        //Compruebo si es un tile en el que se puede colocar a un personaje
                        if (Physics.CheckSphere(worldPoint, nodeRadius, startignTileMask))
                        {
                            startingTile = true;
                        }

                        //Compruebo si hay obstáculos
                        else if (Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask))
                        {
                            empty = false;
                            isObstacle = true;
                            noTileInThisColumn = false;
                        }

                        //Compruebo si es un obstáculos que no tiene tiles encima
                        else if (Physics.CheckSphere(worldPoint, nodeRadius, noTileHereMask))
                        {
                            empty = false;
                            isObstacle = true;
                            noTileInThisColumn = true;
                        }

                        else if (grid3DNode[x, y - 1, z].noTilesInThisColumn)
                        {
                            empty = true;
                            isObstacle = false;
                            noTileInThisColumn = false;
                        }

                        //Si no compruebo si en el tile de abajo hay un obstáculo
                        else if (grid3DNode[x, y - 1, z].isObstacle && !grid3DNode[x, y - 1, z].isEmpty)
                        {
                            empty = false;
                            isObstacle = false;
                            noTileInThisColumn = false;
                        }

                        //Si no se da ningúno de los dos casos entonces es un tile vacío
                        else
                        {
                            empty = true;
                            isObstacle = false;
                            noTileInThisColumn = false;
                        }
                    }

                    gridObject[x, y, z] = Instantiate(tilePref, new Vector3(worldPoint.x, worldPoint.y - 0.5f, worldPoint.z), Quaternion.identity);

                    gridObject[x, y, z].GetComponent<IndividualTiles>().SetVariables(isObstacle, empty, noTileInThisColumn, startingTile, worldPoint, x, y, z, tilePref, LM);

                    grid3DNode[x, y, z] = gridObject[x, y, z].GetComponent<IndividualTiles>();
                }
            }
        }

        Create2DGrid();

    }

    //Una vez he creado el array de 3 dimensiones con todos los tiles, lo voy a transformar en un array de 2 donde solo guardo la x y la z.
    //La altura va como variable extra que no se usa dentro del array o de las coordenadas.
    void Create2DGrid()
    {
        grid2DNode = new IndividualTiles[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    //Añado al grid2D únicamente los tiles por los que se puede mover unidades
                    if (!grid3DNode[x, y, z].isEmpty && !grid3DNode[x, y, z].isObstacle && !grid3DNode[x, y, z].noTilesInThisColumn)
                    {
                        grid2DNode[x, z] = grid3DNode[x, y, z];
                        break;
                    }

                    if (y == gridSizeY-1)
                    {
                        grid2DNode[x, z] = grid3DNode[x, y, z];
                        break;
                    }
                }
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (grid3DNode[x, y, z].isEmpty || (grid3DNode[x, y, z].isObstacle && !grid3DNode[x, y, z].noTilesInThisColumn))
                    {
                        Destroy(gridObject[x, y, z].gameObject);
                    }
                }
            }
        }

        SetTilesNeighbours();
    }

    //NO ESTÁ PENSADO PARA QUE HAYA TILES ENCIMA DE OTROS A DIFERENTES ALTURAS.
    void SetTilesNeighbours()
    {
        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                #region Izquierda
                if (x > 0)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; x - k >= 0; k++)
                    {
                        if (k == 1)
                        {
                            grid2DNode[x, z].neighbours.Add(grid2DNode[x - 1, z]);
                        }

                        grid2DNode[x, z].tilesInLineLeft.Add(grid2DNode[x - k, z]);
                    }
                }

                #endregion

                #region Derecha

                if (x < gridSizeX - 1)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; k < gridSizeX - x; k++)
                    {
                        if (k == 1)
                        {
                            grid2DNode[x, z].neighbours.Add(grid2DNode[x + 1, z]);
                        }

                        grid2DNode[x, z].tilesInLineRight.Add(grid2DNode[x + k, z]);

                    }
                }

                #endregion

                #region Abajo

                if (z > 0)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; z - k >= 0; k++)
                    {
                        if (k == 1)
                        {
                            grid2DNode[x, z].neighbours.Add(grid2DNode[x, z - 1]);
                        }

                        grid2DNode[x, z].tilesInLineDown.Add(grid2DNode[x, z - k]);
                    }
                }

                #endregion

                #region Arriba

                if (z < gridSizeZ - 1)
                {
                    //Compruebo todos los tiles en linea
                    for (int k = 1; k < gridSizeZ - z; k++)
                    {
                        if (k == 1)
                        {
                            grid2DNode[x, z].neighbours.Add(grid2DNode[x, z + 1]);
                        }
                        grid2DNode[x, z].tilesInLineUp.Add(grid2DNode[x, z + k]);
                    }
                }

                #endregion

            }
        }

        //Seteo los tiles que rodean (la lista que incluye las diagonales vamos)
        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GetSurroundingTiles(grid2DNode[x, z], 1,true,false);
                for (int j = 0; j < surroundingTiles.Count; j++)
                {
                    grid2DNode[x, z].surroundingNeighbours.Add(surroundingTiles[j]);
                }
            }
        }
    }

    #endregion

    #region PATHFINDING

    //Calculo el coste de una casilla
    float CostToEnterTile(int x, int z)
    {
        return grid2DNode[x, z].movementCost;
    }

    //Calculo tiles a los que se puede mover una unidad o en los que puede un enemigo buscar objetivos
    public List<IndividualTiles> OptimizedCheckAvailableTilesForMovement(int movementUds, UnitBase selectedUnit, bool _shouldUseHeap)
    {
        selectedCharacter = selectedUnit;
        tilesAvailableForMovement.Clear();

        //Recorro de izquierda a derecha los tiles que pueden estar disponibles para moverse (Va moviendose en X columna a columna)
        for (int i = -movementUds; i < (movementUds * 2) + 1; i++)
        {
            //Al restar a losMovementUds el i actual obtengo los tiles que hay por encima de la posición del personaje en dicha columna
            //Este número me sirve para calcular la posición en z de los tiles
            int tilesInZ = movementUds - Mathf.Abs(i);

            //Esto significa que es el extremo del rombo y sólo hay 1 tile en vertical
            if (tilesInZ == 0)
            {
                //Compruebo si existe un tile con esas coordenadas
                if (selectedCharacter.myCurrentTile.tileX + i < gridSizeX && selectedCharacter.myCurrentTile.tileX + i >= 0 &&
                    selectedCharacter.myCurrentTile.tileZ < gridSizeZ && selectedCharacter.myCurrentTile.tileZ >= 0)
                {

                    currentTileCheckingForMovement = grid2DNode[selectedCharacter.myCurrentTile.tileX + i, selectedCharacter.myCurrentTile.tileZ];

                    //Compruebo si el tile está ocupado, tiene un obstáculo o es un tile vacío
                    //IMPORTANTE NO COMPROBAR LA ALTURA. ESO SE HACE EN EL PATHFINDING. La altura se tiene que comprobar de un tile respecto a sus vecinos, no tiene sentido comprobar el tile en el que esta el player con el que quiere llegar.
                    if (currentTileCheckingForMovement != null && !currentTileCheckingForMovement.isEmpty && !currentTileCheckingForMovement.isObstacle)
                    {
                        //El enemigo no puede excluir los tiles que tienen personajes de jugador porque los necesita para encontrar el número de objetivos.
                        //Para que no se pinten sus tiles en la propia función de pintar he puesto un if que evita que se pintan.
                        if (currentTileCheckingForMovement.unitOnTile != null)
                        {
                            if (selectedCharacter.GetComponent<EnemyUnit>() && currentTileCheckingForMovement.unitOnTile.GetComponent<EnemyUnit>())
                            {
                                continue;
                            }

                            else if (selectedCharacter.GetComponent<PlayerUnit>())
                            {
                                continue;
                            }
                        }
                        
                        CalculatePathForMovementCost(currentTileCheckingForMovement.tileX, currentTileCheckingForMovement.tileZ, false, _shouldUseHeap);

                        if (tempCurrentPathCost <= movementUds)
                        {
                            tilesAvailableForMovement.Add(currentTileCheckingForMovement);
                        }

                        tempCurrentPathCost = 0;
                    }
                }
            }
            else
            {
                for (int j = tilesInZ; j >= -tilesInZ; j--)
                {
                    //Compruebo si existe un tile con esas coordenadas
                    if (selectedCharacter.myCurrentTile.tileX + i < gridSizeX && selectedCharacter.myCurrentTile.tileX + i >= 0 &&
                        selectedCharacter.myCurrentTile.tileZ + j < gridSizeZ && selectedCharacter.myCurrentTile.tileZ + j >= 0)
                    {

                        //Almaceno el tile en una variable
                        currentTileCheckingForMovement = grid2DNode[selectedCharacter.myCurrentTile.tileX + i, selectedCharacter.myCurrentTile.tileZ + j];


                        //Compruebo si el tile está ocupado, tiene un obstáculo o es un tile vacío
                        //IMPORTANTE NO COMPROBAR LA ALTURA. ESO SE HACE EN EL PATHFINDING. La altura se tiene que comprobar de un tile respecto a sus vecinos, no tiene sentido comprobar el tile en el que esta el player con el que quiere llegar.
                        if (currentTileCheckingForMovement != null && !currentTileCheckingForMovement.isEmpty && !currentTileCheckingForMovement.isObstacle)
                        {
                            //El enemigo no puede excluir los tiles que tienen personajes de jugador porque los necesita para encontrar el número de objetivos.
                            //Para que no se pinten sus tiles en la propia función de pintar he puesto un if que evita que se pintan.
                            if (currentTileCheckingForMovement.unitOnTile != null)
                            {
                                if (selectedCharacter.GetComponent<EnemyUnit>() && currentTileCheckingForMovement.unitOnTile.GetComponent<EnemyUnit>())
                                {
                                    continue;
                                }

                                else if (selectedCharacter.GetComponent<PlayerUnit>())
                                {
                                    continue;
                                }
                            }

                            //Compruebo si existe un camino hasta el tile
                            CalculatePathForMovementCost(currentTileCheckingForMovement.tileX, currentTileCheckingForMovement.tileZ, false, _shouldUseHeap);


                            if (tempCurrentPathCost <= movementUds)
                            {
                                tilesAvailableForMovement.Add(currentTileCheckingForMovement);
                            }

                            tempCurrentPathCost = 0;
                        }
                    }
                }
            }
        }
       
        return tilesAvailableForMovement;
    }

    int numberOfNeighboursWithDifferentHeight;

    //Calcula el tamaño del grid para que el heap pueda pillarlo al construirlo.
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

    public void CalculatePathForMovementCost(int x, int z, bool _shouldIgnoreUnitsForRepath, bool _shouldUseHeap)
    {
        openList.Clear();
        //La variable se tiene que quedar aqui porque si no el Heap da error y pide que sea static.
        Heap<IndividualTiles> openHeap = new Heap<IndividualTiles>(MaxSize);
        closedHasSet.Clear();

        //Origen y target
        source = grid2DNode[selectedCharacter.myCurrentTile.tileX, selectedCharacter.myCurrentTile.tileZ];
        target = grid2DNode[x, z];

        //Si no uso heap lo añado a la lista
        if(!_shouldUseHeap)
        {
            openList.Add(source);
        }

        else
        {
            openHeap.Add(source);
        }
       

        //Mientras que haya nodos que no hayan sido visitados...
        while (openList.Count > 0)
        {
            IndividualTiles currentNode;

            //Esto solo es necesario si no uso heap
            if (!_shouldUseHeap)
            {
                currentNode = openList[0];

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].CalculateFCost < currentNode.CalculateFCost ||
                        openList[i].CalculateFCost == currentNode.CalculateFCost && openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
            }

            //Empiezo el heap
            else
            {
                currentNode = openHeap.RemoveFirst();
            }

            closedHasSet.Add(currentNode);

            #region LAST_PART

            //Si el nodo coincide con el objetivo, terminamos la busqueda.
            if (currentNode == target)
            {
                LastPartOfPathfinding();
                return;
            }

            #endregion

            foreach (IndividualTiles neighbour in grid2DNode[currentNode.tileX, currentNode.tileZ].neighbours)
            {
                //No le he puesto !_shouldIgnoreUnitsForRepath. Solo está en el enemigo normal
                #region DRAGON
                //Dragón.
                if (selectedCharacter.GetComponent<BossMultTile>())
                {
                    if (neighbour == null || neighbour.isEmpty || neighbour.isObstacle || closedHasSet.Contains(neighbour) || Mathf.Abs(neighbour.height - grid2DNode[currentNode.tileX, currentNode.tileZ].height) > selectedCharacter.maxHeightDifferenceToMove)
                    {
                        continue;
                    }

                    //Exceptuando el target que siempre va a tener una unidad, compruebo si los tiles para formar el path no están ocupados por enemigos
                    else if (neighbour != target && neighbour.unitOnTile != null || neighbour.unitOnTile != null && neighbour.unitOnTile.GetComponent<EnemyUnit>())
                    {
                        //Solo hago continue si la unidad no es el propio dragón,
                        if (neighbour.unitOnTile != selectedCharacter)
                        {
                            continue;
                        }
                    }

                    //Guardo la altura del tile para comprobar que los 8 tiles que le rodean tienen la misma altura
                    currentDragonNeighbourCheckingHeight = neighbour.height;

                    surroundingObstacles.Clear();
                    shouldSkipSurroundingTile = false;
                    targetFounInNeighbourTiles = false;
                    noTargetInNeighbours = true;

                    if (neighbour != target)
                    {
                        foreach (IndividualTiles surrTile in neighbour.surroundingNeighbours)
                        {
                            //Check 1 por si el tile no es valido
                            if (surrTile == null || surrTile.isEmpty || surrTile.isObstacle || Mathf.Abs(surrTile.height - grid2DNode[currentNode.tileX, currentNode.tileZ].height) > selectedCharacter.maxHeightDifferenceToMove)
                            {
                                surroundingObstacles.Add(surrTile);
                                //shouldSkipSurroundingTile = true;
                                continue;
                            }

                            //Check 2 por si el tile no es valido para colocarse
                            else if (surrTile != target && surrTile.unitOnTile != null || surrTile.unitOnTile != null && surrTile.unitOnTile.GetComponent<EnemyUnit>())
                            {
                                //Solo hago continue si la unidad no es el propio dragón,
                                if (surrTile.unitOnTile != selectedCharacter)
                                {
                                    surroundingObstacles.Add(surrTile);
                                    //shouldSkipSurroundingTile = true;
                                    continue;
                                }
                            }

                            numberOfNeighboursWithDifferentHeight = 0;

                            #region HEIGHTS
                            ////Si la altura del tile surrounding no coincide con el del neighbour cuenta como si fuese un obstáculo.
                            //if (surrTile.height != currentDragonNeighbourCheckingHeight)
                            //{
                            //    //COMPROBAR SI ESTOS COMENTARIOS ESTÁ BIEN
                            //    //En estos for funciona asi:
                            //    //neighbour = tile al que se va a mover el centro del dragón.
                            //    //neighbour.neighbours[i] es el primer tile con cambio de altura y que tiene 3 vecinos con su misma altura
                            //    //neighbour.neighbours[i].neighbours[j] es cada uno de estos vecinos que tienen la misma altura
                            //    //neighbour.neighbours[i].neighbours[j].surroundingNeighbours[k] cada uno de los tiles surrounding de estos vecinos con la misma altura 

                            //    //Compruebo si el tile con diferente altura es adyacente al tile que estoy comprobando
                            //    for (int i = 0; i < surrTile.neighbours.Count; i++)
                            //    {
                            //        //Sólo puede existir un vecino que sea igual al surrTile si son adyacentes
                            //        if (surrTile.neighbours[i] == neighbour)
                            //        {
                            //            //Si es adyacente entonces paso a la siguiente fase.
                            //            //Compruebo si este tile con diferente altura que es adyacente tiene 3 vecinos con su misma altura
                            //            //Aunque este for sea igual que el anterior necesito volver a hacerlo ya que necesito saber en orden que es adyacente y luego si tiene 3 tiles a la misma altura.
                            //            for (int j = 0; j < surrTile.neighbours.Count; j++)
                            //            {
                            //                //Comprobar si hay 3 a la misma altura
                            //                if (surrTile.neighbours[j].height == currentDragonNeighbourCheckingHeight)
                            //                {
                            //                    continue;
                            //                }

                            //                else
                            //                {
                            //                    numberOfNeighboursWithDifferentHeight++;
                            //                }
                            //            }

                            //            //Si únicamente hay 1 tile con diferente altura entonces paso a la siguiente fase
                            //            if (numberOfNeighboursWithDifferentHeight == 1)
                            //            {
                            //                for (int j = 0; j < surrTile.neighbours.Count; j++)
                            //                {
                            //                    for (int k = 0; k < surrTile.neighbours[j].surroundingNeighbours.Count; k++)
                            //                    {
                            //                        if (surrTile.neighbours[j].surroundingNeighbours[k].height != surrTile.height)
                            //                        {
                            //                            shouldSkipSurroundingTile = true;

                            //                            //Manual break;
                            //                            j = 100;
                            //                            break;
                            //                        }
                            //                    }

                            //                    //Si llega aqui puede aceptar el tile
                            //                    //Manual break
                            //                    i = 100;
                            //                    break;
                            //                }
                            //            }

                            //            //Si hay más de uno entonces es un obstáculo y salgo del for
                            //            else
                            //            {
                            //                break;
                            //            }
                            //        }
                            //    }

                            //    //Si sale del for significa que entonces no era adyacente y entonces cuenta como obstáculo sin mas.
                            //    surroundingObstacles.Add(surrTile);

                            //    //CREO QUE SOBRA Y JODE LA DETECCIÓN DEL TARGET
                            //    //continue;
                            //}
                            #endregion

                            for (int i = 0; i < neighbour.neighbours.Count; i++)
                            {
                                //Si el target se encuentra entre los tiles NEIGHBOUR NO SURROUNDING marco el bool para que la excepción de obstáculo vecino a target se tenga en cuenta.
                                //Surrounding no puede ser porque la excepción solo se aplica en los tiles vecinos, no en las diagonales
                                if (neighbour.neighbours[i] == target)
                                {
                                    targetFounInNeighbourTiles = true;
                                }
                            }
                        }
                    }

                    if (targetFounInNeighbourTiles)
                    {
                        for (int i = 0; i < surroundingObstacles.Count; i++)
                        {
                            for (int j = 0; j < surroundingObstacles[i].neighbours.Count; j++)
                            {
                                //Si uno de los vecinos del tile con obstáculo es el target entonces no me impide colocarme en este tile y paso al siguiente.
                                //Esto es porque solo pueden existir simultaneamente 2 tiles que sean vecinos del target y al mismo tiempo surrounding del tile que estoy comprobando
                                //En ambos casos ninguno de estos tiles impide que me coloque para atacar al target
                                if (surroundingObstacles[i].neighbours[j] == target)
                                {
                                    noTargetInNeighbours = false;
                                    break;
                                }
                            }

                            //Como al resetearlo lo pongo en verdadero, si no se ha puesto en falso significa que no ha encontrado vecino con target.
                            if (noTargetInNeighbours)
                            {
                                //Si he encontrado un obstáculo que no es vecino del target significa que no puedo colocarme en este tile
                                shouldSkipSurroundingTile = true;

                                break;
                            }
                        }
                    }

                    else
                    {
                        if (surroundingObstacles.Count > 0)
                        {
                            shouldSkipSurroundingTile = true;
                        }
                    }

                    if (shouldSkipSurroundingTile)
                    {
                        continue;
                    }
                }

                #endregion

                #region ENEMIGOS_Y_JUGADOR

                //Goblin Y  gigante.
                //Importante que sea else if para que el dragón no entre en la condición
                else if (selectedCharacter.GetComponent<EnemyUnit>())
                {
                    if (neighbour == null || neighbour.isEmpty || neighbour.isObstacle || closedHasSet.Contains(neighbour) || Mathf.Abs(neighbour.height - grid2DNode[currentNode.tileX, currentNode.tileZ].height) > selectedCharacter.maxHeightDifferenceToMove)
                    {
                        //if (neighbour.unitOnTile != null)
                        //{
                        //    Debug.Log( "First" + neighbour.unitOnTile.name);
                        //}
                        continue;
                    }

                    //Exceptuando el target que siempre va a tener una unidad, compruebo si los tiles para formar el path no están ocupados por enemigos
                    else if (!_shouldIgnoreUnitsForRepath && (neighbour != target && neighbour.unitOnTile != null || neighbour.unitOnTile != null && neighbour.unitOnTile.GetComponent<EnemyUnit>()))
                    {
                        //if (neighbour.unitOnTile != null)
                        //{
                        //    Debug.Log("Second" + neighbour.unitOnTile.name);
                        //}

                        continue;
                    }
                }

                //Player
                if ((selectedCharacter.GetComponent<PlayerUnit>() && (neighbour == null || neighbour.isEmpty || neighbour.isObstacle || neighbour.unitOnTile != null)) || Mathf.Abs(neighbour.height - grid2DNode[currentNode.tileX, currentNode.tileZ].height) > selectedCharacter.maxHeightDifferenceToMove || closedHasSet.Contains(neighbour))
                {
                    continue;
                }

                //El gigante se tiene en cuenta al no ponerle condiciones de tile vacio o tile obstaculo

                #endregion

                int newMovemntCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovemntCostToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = newMovemntCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, target);

                    neighbour.parent = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            //Si llega hasta aqui significa que no hay tiles a los que moverse por lo que el coste es infinito.
            //NO PONER RETURN, puede darse el caso de que este llegando al ultimo tile de una fila y si estan todos los lados bloqueados menos el tile anterior,
            //entraria aqui porque el tile anterior no puede revisitarlo, asi que cortariamos el proceso de seguir comprobando el resto de tiles de Open antes de tiempo.
            

            if (openList.Count == 0)
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //ES MUY POSIBLE QUE ESTE SEGUNDO IF HAGA LO MISMO QUE EL RETURN Y ROMPA LO QUE PONGO EN EL COMENTARIO DE ARRIBA
                if (!_shouldIgnoreUnitsForRepath && !recalculatePathAtTheEndOfTheFunction)
                {
                    recalculatePathAtTheEndOfTheFunction = true;
                }

                else if (_shouldIgnoreUnitsForRepath)
                {
                    recalculatePathAtTheEndOfTheFunction = false;
                    tempCurrentPathCost = Mathf.Infinity;
                }
              
            }
        }

        if (recalculatePathAtTheEndOfTheFunction)
        {
            RecalculateUnreachablePath(x, z);
        }

    }

    //Este bool indica que el objetivo que busca el path es inalcanzable y que por tanto al final de la función tiene que llamar a recalcular el path sin tener en cuenta los enemigos
    private bool recalculatePathAtTheEndOfTheFunction;

    public void RecalculateUnreachablePath(int x, int z)
    {
        CalculatePathForMovementCost(x, z, true, false);

        //Le resto 2 porque no quiero tener en cuenta el ultimo tile ya que es en el que esta el player y otro más porque count sin más se sale de la lista (ya que el último elemento es siempre count-1).
        for (int i = currentPath.Count-2; i > 0; i--)
        {
            if (currentPath[i].unitOnTile != null)
            {
                CalculatePathForMovementCost(currentPath[i].tileX, currentPath[i].tileZ, false, false);
                return;
            }
        }

        tempCurrentPathCost = Mathf.Infinity;
    }


    private void LastPartOfPathfinding()
    {
        currentPath.Clear();
        //Si llega hasta aquí si que hay un camino hasta el objetivo.
        curr = target;

        //Recorre la cadena de Prev y la añade a la lista que guarda el camino.
        //Esta ruta está al reves, va desde el objetivo hasta el origen.
        while (curr != null)
        {
            if (!currentPath.Contains(curr))
            {
                currentPath.Add(curr);
                curr = curr.parent;
            }

            else
            {
                Debug.LogError("ERROR DE LOOP. TILES REPETIDOS EN EL CURRENTPATH");

                for (int i = 0; i < currentPath.Count; i++)
                {
                    Debug.Log(currentPath[i].name);
                }

                Debug.Log(curr.name);
                break;
            }
        }

        //Le damos la vuelta a la lista para que vaya desde el orgien hasta el objetivo.
        currentPath.Reverse();

        //Debug.Log("REAL " + currentPath.Count);

        //Calcular coste del path
        for (int i = 0; i < currentPath.Count; i++)
        {
            //Sumo el coste de todas las casillas que forman el path excepto la primera (ya que es la casilla sobre la que se encuentra la unidad).
            if (i != 0)
            {
                tempCurrentPathCost += CostToEnterTile(currentPath[i].tileX, currentPath[i].tileZ);
            }
        }

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                grid2DNode[i, j].ClearPathfindingVariables();
            }
        }
    }


    int GetDistance(IndividualTiles nodeA, IndividualTiles nodeB)
    {
       return(Mathf.Abs(nodeA.tileX - nodeB.tileX) + Mathf.Abs(nodeA.tileZ - nodeB.tileZ));
    }

    #endregion

    #region ENEMY_PATHFINDING

    //Esta lista guarda todos los personajes del jugador en el tablero (No puedo usar la del Level Manager porque no incluye la copia del mago)
    PlayerUnit[] playersOnTheBoard;

    //Estas funciones son bastante parecidas a las del pathfinding normal salvo que devuelven personajes a los que pueden atacar en vez de tiles.
    //El gigante llama a esta función pero no a las anteriores.
    //Sin embargo el goblin llama a esta por el objetivo y a las anteriores por el path

    public List<UnitBase> OnlyCheckClosestPathToPlayer(EnemyUnit _selectedUnit)
    {
        selectedCharacter = _selectedUnit;

        charactersAvailableForAttack.Clear();
        tempCurrentObjectiveCost = 0;
        tempCurrentPathCost = 0;

        //Actualizo la lista de personajes por si alguno ha muerto o se ha añadido la copia del mago
        playersOnTheBoard = FindObjectsOfType<PlayerUnit>();

        for (int i = 0; i < playersOnTheBoard.Length; i++)
        {
            CalculatePathForMovementCost(playersOnTheBoard[i].myCurrentTile.tileX, playersOnTheBoard[i].myCurrentTile.tileZ, false, false);

            if (tempCurrentObjectiveCost == 0 || tempCurrentObjectiveCost >= tempCurrentPathCost)
            {
                //Si se da el caso que temCurrentPathCost es 0 significa que no ha encontrado un camino hasta el enemigo (creo)
                if (tempCurrentPathCost != 0)
                {
                    if (tempCurrentObjectiveCost > tempCurrentPathCost)
                    {
                        //Limpio la lista de objetivos y añado
                        charactersAvailableForAttack.Clear();
                    }

                    //Me guardo la distancia para checkear
                    tempCurrentObjectiveCost = tempCurrentPathCost;

                    charactersAvailableForAttack.Add(playersOnTheBoard[i]);
                }
            }

            tempCurrentPathCost = 0;
        }

        //Reset
        return charactersAvailableForAttack;
    }
                
    //Función que sirve para encontrar a todos los enemigos en rango y que el goblin pueda alertarlos
    public List<UnitBase> GetAllUnitsInRangeWithoutPathfinding(int rangeToCheck, UnitBase selectedUnit)
    {
        unitsInRangeWithoutPathfinding.Clear();

        //Recorro de izquierda a derecha los tiles que pueden estar disponibles para moverse (Va moviendose en X columna a columna)
        for (int i = -rangeToCheck; i < (rangeToCheck * 2) + 1; i++)
        {
            //Al restar a losMovementUds el i actual obtengo los tiles que hay por encima de la posición del personaje en dicha columna
            //Este número me sirve para calcular la posición en z de los tiles
            int tilesInZ = rangeToCheck - Mathf.Abs(i);

            //Esto significa que es el extremo del rombo y sólo hay 1 tile en vertical
            if (tilesInZ == 0)
            {
                //Compruebo si existe un tile con esas coordenadas
                if (selectedUnit.myCurrentTile.tileX + i < gridSizeX && selectedUnit.myCurrentTile.tileX + i >= 0 &&
                    selectedUnit.myCurrentTile.tileZ < gridSizeZ && selectedUnit.myCurrentTile.tileZ >= 0)
                {
                    currentTileCheckingForUnit = grid2DNode[selectedUnit.myCurrentTile.tileX + i, selectedUnit.myCurrentTile.tileZ];

                    if (currentTileCheckingForUnit != null && currentTileCheckingForUnit.unitOnTile != null && currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>())
                    {
                        if (currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>() != selectedUnit)
                        {
                            unitsInRangeWithoutPathfinding.Add(currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>());
                        }
                    }
                }
            }
            else
            {
                for (int j = tilesInZ; j >= -tilesInZ; j--)
                {
                    //Compruebo si existe un tile con esas coordenadas
                    if (selectedUnit.myCurrentTile.tileX + i < gridSizeX && selectedUnit.myCurrentTile.tileX + i >= 0 &&
                        selectedUnit.myCurrentTile.tileZ + j < gridSizeZ && selectedUnit.myCurrentTile.tileZ + j >= 0)
                    {
                        //Almaceno el tile en una variable
                        currentTileCheckingForUnit = grid2DNode[selectedUnit.myCurrentTile.tileX + i, selectedUnit.myCurrentTile.tileZ + j];

                        if (currentTileCheckingForUnit != null && currentTileCheckingForUnit.unitOnTile != null && currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>())
                        {
                            if (currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>() != selectedUnit)
                            {
                                unitsInRangeWithoutPathfinding.Add(currentTileCheckingForUnit.unitOnTile.GetComponent<UnitBase>());
                            }
                        }
                    }
                }
            }
        }

        return unitsInRangeWithoutPathfinding;
    }

    //Esta función es una copia de CheckAvailableTiles for Movement solo que no calcula el pathfinding de los tiles, simplemente los pinta.
    //Esto sirve para que el rango de los enemigos no dependa de pathfinding ya que siempre es el mismo
    public List<IndividualTiles> CheckAvailableTilesForEnemyAction(int movementUds, EnemyUnit selectedUnit)
    {
        tilesAvailableForEnemyAction.Clear();

        //Recorro de izquierda a derecha los tiles que pueden estar disponibles para moverse (Va moviendose en X columna a columna)
        for (int i = -movementUds; i < (movementUds * 2) + 1; i++)
        {
            //Al restar a losMovementUds el i actual obtengo los tiles que hay por encima de la posición del personaje en dicha columna
            //Este número me sirve para calcular la posición en z de los tiles
            int tilesInZ = movementUds - Mathf.Abs(i);

            for (int j = tilesInZ; j >= -tilesInZ; j--)
            {

                //Compruebo si existe un tile con esas coordenadas
                if (selectedUnit.myCurrentTile.tileX + i < gridSizeX && selectedUnit.myCurrentTile.tileX + i >= 0 &&
                    selectedUnit.myCurrentTile.tileZ + j < gridSizeZ && selectedUnit.myCurrentTile.tileZ + j >= 0)
                {
                    if (!grid2DNode[selectedUnit.myCurrentTile.tileX + i, selectedUnit.myCurrentTile.tileZ + j].isObstacle && !grid2DNode[selectedUnit.myCurrentTile.tileX + i, selectedUnit.myCurrentTile.tileZ + j].isEmpty)
                    {
                        tilesAvailableForEnemyAction.Add(grid2DNode[selectedUnit.myCurrentTile.tileX + i, selectedUnit.myCurrentTile.tileZ + j]);
                    }
                }
            }
        }

        return tilesAvailableForEnemyAction;
    }
    #endregion


    //Esta función se usa para calcular áreas de tiles (básicamente un rombo de tiles)
    //No mira si hay obstáculos o está vacío, únicamente si hay tiles
    public List<IndividualTiles> GetRhombusTiles(IndividualTiles rhombusCenter, int radius)
    {
        //radius equivale a la distancia entre el centro del rombo (tile que esta usando esta función) y el tile más alejado en una dirección, es decir el extremo del rombo (que al ser un rombo es la misma distancia hasta cualquier extremo)
        rhombusTiles.Clear();

        //Recorro de izquierda a derecha los tiles que pueden estar disponibles para moverse (Va moviendose en X columna a columna)
        for (int i = -radius; i < (radius * 2) + 1; i++)
        {
            //Al restar a losMovementUds el i actual obtengo los tiles que hay por encima de la posición del personaje en dicha columna
            //Este número me sirve para calcular la posición en z de los tiles
            int tilesInZ = radius - Mathf.Abs(i);

            //Esto significa que es el extremo del rombo y sólo hay 1 tile en vertical
            if (tilesInZ == 0)
            {
                //Compruebo si existe un tile con esas coordenadas
                if (rhombusCenter.tileX + i < gridSizeX && rhombusCenter.tileX + i >= 0 &&
                    rhombusCenter.tileZ < gridSizeZ && rhombusCenter.tileZ >= 0)
                {

                    //currentTileCheckingForMovement = grid2DNode[selectedCharacter.myCurrentTile.tileX + i, selectedCharacter.myCurrentTile.tileZ];

                    rhombusTiles.Add(grid2DNode[rhombusCenter.tileX + i, rhombusCenter.tileZ]);
                }
            }
        }

        return rhombusTiles;
    }

    public List<IndividualTiles> surroundingTiles = new List<IndividualTiles>();

    //Obtener los vecinos + las diagonales de un tile
    //Si radius es igual a uno se obtienen los 8 tiles que rodean al tile (neighbours + diagonales)
    //Si radius es 2 se ignoran estos 8 tiles y se obtienen los 14 tiles que rodean a estos 8. Básicamente lo uso para saber los tiles a los que puede atacar el dragón.
    public List<IndividualTiles> GetSurroundingTiles(IndividualTiles centerTile, int radius, bool shouldIAddCorners, bool shouldAddOnlyBorder)
    {
        surroundingTiles.Clear();

        //Filas.
        //Resto en vez de sumar para que vaya de arriba a abajo. Por eso es -Radius
        for (int i = radius; i >= -radius; i--)
        {
            //Columnas.
            //Aqui sumo para que vaya de izquierda a derecha.
            for (int j = -radius; j <= radius; j++)
            {  
               if (centerTile.tileZ + j < gridSizeZ && centerTile.tileZ + j >= 0 &&  
                    centerTile.tileX + i < gridSizeX && centerTile.tileX + i >= 0 &&
                    grid2DNode[centerTile.tileX + i, centerTile.tileZ + j] != centerTile)
               {
                    //Si añado las esquinas
                    if (shouldIAddCorners)
                    {
                        //Si cojo solo el borde
                        if (shouldAddOnlyBorder)
                        {
                            if (Mathf.Abs(j) == radius || Mathf.Abs(i) == radius)
                            {
                                surroundingTiles.Add(grid2DNode[centerTile.tileX + i, centerTile.tileZ + j]);
                            }
                        }

                        //Si cojo toda el área
                        else
                        {
                            surroundingTiles.Add(grid2DNode[centerTile.tileX + i, centerTile.tileZ + j]);
                        }
                    }

                    //Si no quiero añadir las esquinas
                    else
                    {
                        //Si coincide que la j y la i estan a la vez en uno de los máximos o de los mínimos entonces es una esquina
                        if (j == -radius && (i == radius || i == -radius) ||
                            j == radius && (i == radius || i == -radius))
                        {
                            continue;
                        }

                        else
                        {
                            //Si cojo solo el borde
                            if (shouldAddOnlyBorder)
                            {
                                if (Mathf.Abs(j) == radius || Mathf.Abs(i) == radius)
                                {
                                    surroundingTiles.Add(grid2DNode[centerTile.tileX + i, centerTile.tileZ + j]);
                                }
                            }

                            //Si cojo toda el área
                            else
                            {
                                surroundingTiles.Add(grid2DNode[centerTile.tileX + i, centerTile.tileZ + j]);
                            }

                        }
                    }
               }
            }
        }
        return surroundingTiles;
    }


    public List<IndividualTiles> coneTiles = new List<IndividualTiles>();
    int coneIndex;

    public List<IndividualTiles> GetConeTiles(List<IndividualTiles> middleLine, UnitBase.FacingDirection _direction)
    {
        coneIndex = 0;

        for (int i = 0; i < middleLine.Count; i++)
        {
            for (int j = 0; j < middleLine[i].GetLateralTilesBasedOnDirection(_direction, coneIndex).Count; j++)
            {
                coneTiles.Add(middleLine[i].GetLateralTilesBasedOnDirection(_direction, coneIndex)[j]);
            }
            coneTiles.Add(middleLine[i]);

            coneIndex += 1;
        }


        for (int i = 0; i < coneTiles.Count; i++)
        {
            coneTiles[i].ColorAttack();
        }

        return coneTiles;

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3 (transform.position.x + gridWorldSize.x/2, transform.position.y + gridWorldSize.y / 2, transform.position.z + gridWorldSize.z / 2), new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));
    }
}
