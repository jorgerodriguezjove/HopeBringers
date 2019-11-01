using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualTiles : MonoBehaviour
{
    #region VARIABLES

    //STATS DEL TILE---------------------------------------------------------------------

    //Coordenadas del tile
    [HideInInspector]
    public int tileX;
    [HideInInspector]
    public int tileZ;

    //Coste inicial del tile
    [SerializeField]
    private int initialMovementCost;

    //Coste máximo de movimiento para que ningún personaje pueda entrar
    private int maximumMovementCost = 9999;

    //Coste que tiene una casilla.
    [HideInInspector]
    public int currentMovementCost;

    //Altura del tile
    public int height;

    //Bool que determina si es un tile vacío
    [SerializeField]
    public bool isEmpty;

    //Bool que determina si es un tile con obstáculo
    [SerializeField]
    public bool isObstacle;

    //Unidad encima del tile. Cada unidad se encarga de rellenar esta variable en el start ya que en el editor a cada unidad le paso una referencia del tile en el que esta.
    [HideInInspector]
    public UnitBase unitOnTile;

    //TILES VECINOS---------------------------------------------------------------------

    //Lista con los 4 vecinos adyacentes. Esto se usa para el pathfinding.
    [HideInInspector]
    public List<IndividualTiles> neighbours;

    //Los tiles que están en línea (cómo si fuese movimiento de torre) con este tile en función de en que dirección se encuentran.
    [HideInInspector]
    public List<IndividualTiles> tilesInLineUp;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineDown;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineRight;
    [HideInInspector]
    public List<IndividualTiles> tilesInLineLeft;

    //REFERENCIAS---------------------------------------------------------------------

    [HideInInspector]
    public TileManager TM;
    [HideInInspector]
    public LevelManager LM;

    //MATERIALES. CAMBIAR ESTO POR SHADERS
    [SerializeField]
    private Material colorTest;
    private Material initialColor;

    #endregion

    #region INIT

    private void Awake()
    {
        //Posible problema con orden de Awakes
        if (isEmpty || isObstacle || unitOnTile != null)
        {
            currentMovementCost = maximumMovementCost;
        }

        else
        {
            currentMovementCost = initialMovementCost;
        }
    }

    private void Start()
    {
        initialColor = GetComponent<MeshRenderer>().material;
    }

    #endregion

    #region INTERACTION

    private void OnMouseDown()
    {
        LM.MoveUnit(this);
    }

    #endregion

    #region COLORS

    //Cambiar a color movimiento
    public void ColorSelect()
    {
        GetComponent<MeshRenderer>().material = colorTest;
    }

    //Cambiar a color normal
    public void ColorDeselect()
    {
        GetComponent<MeshRenderer>().material = initialColor;
    }

    #endregion

    //Está función se tiene que llamar cada vez que se rompa un obstáculo y cuándo un enemigo se mueva de unidad
    public void ChangeMovementCostToMinimum()
    {
        currentMovementCost = initialMovementCost;
    }

    //Esta función se tiene que llamar cada vez que un enemigo entra en un tile.
    public void ChangeMovementCostToMaximum()
    {
        currentMovementCost = maximumMovementCost;
    }
}
