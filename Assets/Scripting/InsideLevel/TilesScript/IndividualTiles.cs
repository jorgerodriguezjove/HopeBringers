using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualTiles : MonoBehaviour, IHeapItem<IndividualTiles>
{
    #region VARIABLES
    [Header("LÓGICA")]

    public bool isObstacle;
    public bool isEmpty;
    public bool noTilesInThisColumn;

    //PROBLEMA CON NUEVO SISTEMA DE TILES
    [SerializeField]
    public bool isAvailableForCharacterColocation;

    [HideInInspector]
    public int movementCost = 1;

    //Posición real en la escena del tile
    [HideInInspector]
    public Vector3 worldPosition;
    //Posiciones de coordenada dentro del Grid
    [SerializeField]
    public int tileX, height, tileZ;

    [HideInInspector]
    public UnitBase unitOnTile;

    [Header("TILES VECINOS")]
    //Lista con los 4 vecinos adyacentes. Esto se usa para el pathfinding.
    [SerializeField]
    public List<IndividualTiles> neighbours = new List<IndividualTiles>();

    //Número tiles vecinos ocupados por una unidad.
    //Acordarse de ocultar en editor y acordarse de actualizar este valor cada vez que se mueva una unidad.
    [SerializeField]
    public int neighboursOcuppied;

    //Los tiles que están en línea (cómo si fuese movimiento de torre) con este tile en función de en que dirección se encuentran.
    [SerializeField]
    public List<IndividualTiles> tilesInLineUp = new List<IndividualTiles>();
    [SerializeField]
    public List<IndividualTiles> tilesInLineDown = new List<IndividualTiles>();
    [SerializeField]
    public List<IndividualTiles> tilesInLineRight = new List<IndividualTiles>();
    [SerializeField]
    public List<IndividualTiles> tilesInLineLeft = new List<IndividualTiles>();


    [Header("FEEDBACK")]
    [SerializeField]
    private Material availableForMovementColor;
    [SerializeField]
    private Material currentTileHoverMovementColor;
    [SerializeField]
    private Material attackColor;
    private Material initialColor;

    //Este bool sirve para saber si el tile estaba con feedback de ataque antes para volver a ponerse
    private bool isUnderAttack;

    //Bool que sirve para saber si el tile estaba con feedback de movimiento antes para volver a ponerse
    private bool isMovementTile;

    [SerializeField]
    [@TextAreaAttribute(15, 20)]
    private string tileInfo = "Falta poder setearlo desde el editor";
    [SerializeField]
    private Sprite tileImage;

    [Header("REFERENCIAS")]

    //Referencia al Level Manager, se setea en el constructor
    [HideInInspector]
    public LevelManager LM;

    [Header("OPTIMIZATION")]

    //Distancia hasta el source
    public int gCost;
    //Distancia hasta el target
    public int hCost;
    //Suma de ambas distancias
    private int fCost;

    //Esto era antes el diccioanrio de prev de tileManger. Ahora el propio tile almacena el tile anterior del path.
    public IndividualTiles parent;

    int heapIndex;

    ////Este bool sirve para saber si el tile estaba con feedback de ataque antes para volver a ponerse
    //private bool isUnderAttack;

    ////Bool que sirve para saber si el tile estaba con feedback de movimiento antes para volver a ponerse
    //private bool isMovementTile;

    //[SerializeField]
    //[@TextAreaAttribute(15, 20)]
    //private string tileInfo;
    //[SerializeField]
    //private Sprite tileImage;

    #endregion

    #region INIT

    //Constructor
    public void SetVariables(bool _isObstacle, bool _empty, bool _noTilesInThisColumn ,Vector3 _worldPos, int xPos, int yPos, int zPos, GameObject tilePref, LevelManager LMRef)
    {
        //Inicializo las variables que le pasa Grid
        isObstacle = _isObstacle;
        isEmpty = _empty;
        noTilesInThisColumn = _noTilesInThisColumn;
        worldPosition = _worldPos;

        //Coordenadas del tile
        tileX = xPos;
        height = yPos;
        tileZ = zPos;

        //Ref al levelmanager
        LM = LMRef;

        gameObject.name = string.Join("_", tileX.ToString(), tileZ.ToString(), height.ToString());

        if (isEmpty || isObstacle || noTilesInThisColumn)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }

        initialColor = GetComponent<MeshRenderer>().material;

        UpdateNeighboursOccupied();

        if (LM.currentLevelState == LevelManager.LevelState.Initializing && isAvailableForCharacterColocation)
        {
            //Cambio el color del tile
            LM.tilesForCharacterPlacement.Add(GetComponent<IndividualTiles>());
            ColorCurrentTileHover();
        }
    }

    #endregion

    #region INTERACTION

    private void OnMouseDown()
    {
        LM.TileClicked(this);
    }

    //Hover enter
    void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.Initializing && isAvailableForCharacterColocation)
        {
            //HACER QUE SE PINTE EL TILE Y APAREZCA LA UNIDAD QUE SE VA A COLOCAR
        }

        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && LM.tilesAvailableForMovement.Contains(this))
        {
            //Cambio el cursor
            Cursor.SetCursor(LM.UIM.movementCursor, Vector2.zero, CursorMode.Auto);

            //Cambio el color del tile
            ColorCurrentTileHover();
        }
        //if (isEmpty)
        //{
        //    LM.UIM.HideTileInfo();
        //}
        //else if (!unitOnTile)
        //{
        //    LM.UIM.ShowTileInfo(tileInfo, tileImage);
        //}

    }

    //Hover exit
    void OnMouseExit()
    {
        //LM.UIM.HideTileInfo();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (isMovementTile)
        {
            ColorSelect();
        }

        else if (isUnderAttack)
        {
            ColorAttack();
        }
    }

    #endregion

    #region NEIGHBOURS

    //Esta función comprueba las casillas vecinas en busca de unidades para saber cuantas unidades rodean al tile.
    public void UpdateNeighboursOccupied()
    {
        neighboursOcuppied = 0;

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].unitOnTile != null)
            {
                neighboursOcuppied++;
            }
        }
    }

    //Esta función se usa cuando una unidad muere o se mueve para avisar a los vecinos de que actualicen sus tiles. 
    //Con esto hay que tener cuidado ya que puede entrar en un bucle en el que todos los tiles comprueben los vecinos constantemente.
    public void WarnInmediateNeighbours()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].UpdateNeighboursOccupied();
        }
    }

    #endregion

    #region COLORS

    //Cambiar a color movimiento
    public void ColorSelect()
    {
        GetComponent<MeshRenderer>().material = availableForMovementColor;
        isMovementTile = true;
    }

    public void ColorCurrentTileHover()
    {
        GetComponent<MeshRenderer>().material = currentTileHoverMovementColor;
    }

    //Cambiar a color normal
    public void ColorDeselect()
    {
        if (isUnderAttack)
        {
            GetComponent<MeshRenderer>().material = attackColor;
        }
        else
        {
            GetComponent<MeshRenderer>().material = initialColor;
        }

        isMovementTile = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //Cambiar el color a ataque
    public void ColorAttack()
    {
        GetComponent<MeshRenderer>().material = attackColor;
        isUnderAttack = true;
    }

    //Quitar el color de ataque y avisar de que ya no está bajo ataque el tile
    public void ColorDesAttack()
    {
        GetComponent<MeshRenderer>().material = initialColor;
        isUnderAttack = false;
    }

    #endregion

    #region OPTIMIZATION

    public int CalculateFCost
    {
        get { return gCost + hCost; }
    }

    //Al ser T un elemento genérico no se si va a poder compararse asi que uso una interfaz para asegurarme de que tiene un index.
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(IndividualTiles tileToCompare)
    {
        int compare = fCost.CompareTo(tileToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(tileToCompare.hCost);

        }

        return -compare;
    }

    public void ClearPathfindingVariables()
    {
        gCost = 0;
        hCost = 0;
        fCost = 0;
        parent = null;
    }

    #endregion

}

