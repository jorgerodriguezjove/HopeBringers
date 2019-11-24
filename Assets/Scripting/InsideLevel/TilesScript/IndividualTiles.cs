using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IndividualTiles : MonoBehaviour
{
    #region VARIABLES

    [Header("INIT")]

    [HideInInspector]
    public Vector3 initialPosition;

    [Header("STATS TILE")]

    //Coordenadas del tile
    [HideInInspector]
    public int tileX;
    [HideInInspector]
    public int tileZ;

    //Coste que tiene una casilla.
    [SerializeField]
    public int MovementCost;

    //Altura del tile
    public int height;

    //Bool que determina si es un tile vacío
    [SerializeField]
    public bool isEmpty;

    //Bool que determina si es un tile con obstáculo
    [SerializeField]
    public bool isObstacle;

    //Bool que determina si es un tile que puede usarse para colocar personajes al comienzo.
    [SerializeField]
    public bool isAvailableForCharacterColocation;

    //Unidad encima del tile. Cada unidad se encarga de rellenar esta variable en el start ya que en el editor a cada unidad le paso una referencia del tile en el que esta.
    [SerializeField]
    public UnitBase unitOnTile;

    [Header("TILES VECINOS")]

    //Lista con los 4 vecinos adyacentes. Esto se usa para el pathfinding.
    [HideInInspector]
    public List<IndividualTiles> neighbours;

    //Número tiles vecinos ocupados por una unidad.
    //Acordarse de ocultar en editor y acordarse de actualizar este valor cada vez que se mueva una unidad.
    [SerializeField]
    public int neighboursOcuppied;

    //Los tiles que están en línea (cómo si fuese movimiento de torre) con este tile en función de en que dirección se encuentran.
    [HideInInspector]
    public List<IndividualTiles> tilesInLineUp;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineDown;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineRight;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineLeft;

    [Header("REFERENCIAS")]

    [HideInInspector]
    public TileManager TM;
    [HideInInspector]
    public LevelManager LM;


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
    private string tileInfo;


    #endregion

    #region INIT

    //Guardo la posición inicial y elevo el tile para que haga el efecto de caída.
    private void Awake()
    {
        initialPosition = gameObject.transform.position;

        //gameObject.transform.position = new Vector3(initialPosition.x, initialPosition.y + 20, initialPosition.z);
    }


    public void FallAnimation()
    {
        gameObject.transform.DOMove(initialPosition, 0.1f).SetEase(Ease.OutBounce);

    }

    private void Start()
    {
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
        if (isEmpty)
        {
            LM.UIM.ShowTooltip("");
        }
        else if (!unitOnTile)
        {
            LM.UIM.ShowTooltip(tileInfo);
        }

    }

    //Hover exit
    void OnMouseExit()
    {
        LM.UIM.ShowTooltip("");
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

}
