using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    #region VARIABLES
    [Header("LÓGICA")]

    public bool isObstacle;
    public bool isEmpty;

    public int movementCost = 1;

    //Posición real en la escena del tile
    public Vector3 worldPosition;
    //Posiciones de coordenada dentro del Grid
    [HideInInspector]
    public int xPosition, yPosition, zPosition;

    [HideInInspector]
    public UnitBase unitOnTile;

    [Header("TILES VECINOS")]
    //Lista con los 4 vecinos adyacentes. Esto se usa para el pathfinding.
    [SerializeField]
    public List<Node> neighbours = new List<Node>();

    //Número tiles vecinos ocupados por una unidad.
    //Acordarse de ocultar en editor y acordarse de actualizar este valor cada vez que se mueva una unidad.
    [SerializeField]
    public int neighboursOcuppied;

    //Los tiles que están en línea (cómo si fuese movimiento de torre) con este tile en función de en que dirección se encuentran.
    [SerializeField]
    public List<Node> tilesInLineUp = new List<Node>();
    [SerializeField]
    public List<Node> tilesInLineDown = new List<Node>();
    [SerializeField]
    public List<Node> tilesInLineRight = new List<Node>();
    [SerializeField]
    public List<Node> tilesInLineLeft = new List<Node>();


    [Header("FEEDBACK")]
    [SerializeField]
    private Material availableForMovementColor;
    [SerializeField]
    private Material currentTileHoverMovementColor;
    [SerializeField]
    private Material attackColor;
    private Material initialColor;

    [Header("FEEDBACK")]

    //Referencia al Level Manager, se setea en el constructor
    [HideInInspector]
    public LevelManager LM;

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
    public void SetVariables (bool _isObstacle, bool _empty, Vector3 _worldPos, int xPos, int yPos, int zPos, GameObject tilePref, LevelManager LMRef)
    {
        //Inicializo las variables que le pasa Grid
        isObstacle = _isObstacle;
        isEmpty = _empty;
        worldPosition = _worldPos;

        //Coordenadas del tile
        xPosition = xPos;
        yPosition = yPos;
        zPosition = zPos;

        //Ref al levelmanager
        LM = LMRef;

        gameObject.name = string.Join("_", xPosition.ToString(), zPosition.ToString(), yPosition.ToString());

        if (isEmpty || isObstacle)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        //AÑADIR UN COLLIDER PARA EL RATÓN O CAMBIAR A RAYCAST

    }


    #endregion
}
