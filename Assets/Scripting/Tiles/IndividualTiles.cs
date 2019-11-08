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

    //Unidad encima del tile. Cada unidad se encarga de rellenar esta variable en el start ya que en el editor a cada unidad le paso una referencia del tile en el que esta.
    [HideInInspector]
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
    private Material colorTest;
    private Material initialColor;
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
    }

    #endregion

    #region INTERACTION

    private void OnMouseDown()
    {
        LM.MoveUnit(this);
    }

	void OnMouseEnter()
	{
		if(isEmpty)
		{
			LM.UIM.ShowTooltip("");
		}
		else if (!unitOnTile)
		{
			LM.UIM.ShowTooltip(tileInfo);
		}
		
	}
	void OnMouseExit()
	{
		LM.UIM.ShowTooltip("");
	}

	#endregion

	public void UpdateNeighboursOccupied()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].unitOnTile != null)
            {
                neighboursOcuppied++;
            }
        }
    }


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

}
