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

    //Bool que determina si en el tile se pueden colocar unidades al comienzo del nivel.
    [SerializeField]
    public bool isAvailableForCharacterColocation;

    //Por defecto todos los tiles tienen un coste de 1 unidad de movimiento.
    //En principio esto no debería cambiar nunca.
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

    //Este bool sirve para saber si el tile estaba con feedback de ataque antes para volver a ponerse
    [HideInInspector]
    public bool isUnderAttack;

    //Bool que sirve para saber si el tile estaba con feedback de movimiento antes para volver a ponerse
    private bool isMovementTile;

    //Bool que indica si el tile tiene que avisar a la balista del movimiento del caballero
    [HideInInspector]
    public bool lookingForKnightToWarnBalista;

    //Lista de balistaque tengo que avisar si el caballero entra o sale del tile
    [HideInInspector]
    public List<EnBalista> allBalistasToWarn = new List<EnBalista>();

    [Header("TILES VECINOS")]
    //Lista con los 4 vecinos adyacentes. Esto se usa para el pathfinding.
    [SerializeField]
    public List<IndividualTiles> neighbours = new List<IndividualTiles>();

    //Lista con los 8 vecinos adyacentes incluyendo las diagonales. Esto se usa para el pathfinding del dragón.
    [SerializeField]
    public List<IndividualTiles> surroundingNeighbours = new List<IndividualTiles>();

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

    //Estos dos tiles sirven para pasar los tiles adyacentes dependiendo de la dirección recibida (Se usa en la función GetLateralTilesBasedOnDirection)
    private IndividualTiles translatedRightTile;
    private IndividualTiles translatedLeftTile;
    //Lista que recibe la unidad con los tiles traducidos
    [HideInInspector]
    public List<IndividualTiles> translatedLateralTiles;

    [Header("FEEDBACK")]

    //Estas son las variables locales. LOS MATERIALES SE SETEAN EN EL TILE MANAGER
    [SerializeField]
    public Material moveInteriorColor;
    [SerializeField]
    public Material moveBorderColor;


    [SerializeField]
    public Material hoverTileBorderColor;

    [SerializeField]
    public Material attackBorderColor;
    [SerializeField]
    public Material chargingAttackBorderColor;
    [SerializeField]
    public Material actionRangeBorderColor;

    //Material inicial del tile
    private Material initialColor;

    [SerializeField]
    [@TextAreaAttribute(15, 20)]
    private string tileInfo = "Falta poder setearlo desde el editor";
    [SerializeField]
    private Sprite tileImage;

    [Header("REFERENCIAS")]

    //Referencia al Level Manager, se setea en el constructor
    [HideInInspector]
    public LevelManager LM;

    MeshRenderer meshRendererUsing;

    [SerializeField]
    MeshRenderer tileBorder;
    [SerializeField]
    MeshRenderer tileInterior;

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
    public void SetVariables(bool _isObstacle, bool _empty, bool _noTilesInThisColumn, bool _startingTile,
                             Vector3 _worldPos, int xPos, int yPos, int zPos, 
                             GameObject tilePref, LevelManager LMRef)
    {
        //Inicializo las variables que le pasa Grid
        isObstacle = _isObstacle;
        isEmpty = _empty;
        noTilesInThisColumn = _noTilesInThisColumn;
        worldPosition = _worldPos;
        isAvailableForCharacterColocation = _startingTile;

        //Coordenadas del tile
        tileX = xPos;
        height = yPos;
        tileZ = zPos;

        //Ref al levelmanager
        LM = LMRef;

        //Nombre en el editor del tile
        gameObject.name = string.Join("_", tileX.ToString(), tileZ.ToString(), height.ToString());

        //Si es empty o un obstáculo le quito el mesh
        if (isEmpty || isObstacle || noTilesInThisColumn)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }

        //Seteo los materiales
        meshRendererUsing = GetComponent<MeshRenderer>();

        initialColor = meshRendererUsing.material;

        meshRendererUsing.enabled = false;
        tileBorder.enabled = false;
        tileInterior.enabled = false;

        //Aviso a los vecinos de la ocupación del tile
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

        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && LM.tilesAvailableForMovement.Contains(this) && LM.selectedCharacter != null && !LM.selectedCharacter.hasMoved)
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
        //Con este if evito que se pinten los tiles de los personajes con el pathfinding del goblin
        if (unitOnTile == null)
        {
            tileInterior.enabled = true;
            tileInterior.material = moveInteriorColor;

            tileBorder.enabled = true;
            tileBorder.material = moveBorderColor;


            isMovementTile = true;
        }

        else
        {
            Debug.Log("Soy tile " + name + "y me han dicho que me pinte aunque tengo una unidad");
        }
    }

    public void ColorCurrentTileHover()
    {
        //tileInterior.enabled = true;
        //tileInterior.material = hoverTileBorderColor;

        tileBorder.enabled = true;
        tileBorder.material = hoverTileBorderColor;
    }

    //Cambiar a color normal
    public void ColorDeselect()
    {
        if (isUnderAttack)
        {
            tileInterior.enabled = true;
            tileInterior.material = attackBorderColor;

            tileBorder.enabled = true;
            tileBorder.material = attackBorderColor;
        }
        else
        {
            tileInterior.enabled = false;
            tileBorder.enabled = false;
        }

        isMovementTile = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //Cambiar a color de rango de acción dormido
    public void ColorActionRange()
    {
        if (!tileBorder.enabled)
        {
            tileBorder.enabled = true;
            tileBorder.material = actionRangeBorderColor;
        }
        
    }






    //Cambiar el color a ataque
    public void ColorAttack()
    {
        //tileInterior.enabled = true;
        //tileInterior.material = attackBorderColor;

        tileBorder.enabled = true;
        tileBorder.material = attackBorderColor;

        isUnderAttack = true;
    }

    //Cambiar el color a ataque
    public void ColorChargingAttack()
    {
        if (!isUnderAttack)
        {
            //tileInterior.enabled = true;
            //tileInterior.material = chargingAttackBorderColor;

            tileBorder.enabled = true;
            tileBorder.material = chargingAttackBorderColor;
        }
    }

    //Quitar el color de ataque y avisar de que ya no está bajo ataque el tile
    public void ColorDesAttack()
    {
        tileBorder.enabled = false;
        //tileInterior.enabled = false;

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



    public void WarnBalista(UnitBase _playerMovedToThisTile)
    {
        if (lookingForKnightToWarnBalista)
        {
            if (_playerMovedToThisTile.GetComponent<Knight>())
            {
                //Avisar a las balistas
                for (int i = 0; i < allBalistasToWarn.Count; i++)
                {
                    allBalistasToWarn[i].BeWarnedByTile();
                }

                Debug.Log("Balistaaaaaa");
            }
        }
    }


    //Decide que tile es derecha, izquierda, arriba o abajo en función de la dirección que recibe.
    //Por defecto las listas de tile dan por hecho que el norte es arriba, este derecha y así.
    //Al pasarle otra dirección como por ejemplo el este, arriba pasaría a ser el este y derecha el norte. Con esto podemos calcular los tiles laterales con una única función.
    public List<IndividualTiles> GetLateralTilesBasedOnDirection(UnitBase.FacingDirection _referenceDirection)
    {
        translatedLateralTiles.Clear();

        if (_referenceDirection == UnitBase.FacingDirection.North)
        {
            translatedRightTile = tilesInLineRight[0];
            translatedLeftTile = tilesInLineLeft[0];
        }

        else if (_referenceDirection == UnitBase.FacingDirection.East)
        {
            translatedRightTile = tilesInLineDown[0];
            translatedLeftTile = tilesInLineUp[0];
        }

        else if (_referenceDirection == UnitBase.FacingDirection.South)
        {
            translatedRightTile = tilesInLineLeft[0];
            translatedLeftTile = tilesInLineRight[0];
        }

        else if (_referenceDirection == UnitBase.FacingDirection.West)
        {
            translatedRightTile = tilesInLineUp[0];
            translatedLeftTile = tilesInLineDown[0];
        }

        translatedLateralTiles.Add(translatedRightTile);
        translatedLateralTiles.Add(translatedLeftTile);

        return translatedLateralTiles;
    }
}

