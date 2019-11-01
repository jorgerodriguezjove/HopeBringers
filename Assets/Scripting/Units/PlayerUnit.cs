using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    [Header("STATS ESPECÍFICO")]

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [HideInInspector]
    public bool hasMoved = false;
    [HideInInspector]
    public bool hasAttacked = false;
    [HideInInspector]
    public bool isMoving = false;

    [Header("MOVIMIENTO")]

    //Camino que tiene que seguir la unidad para moverse
    private List<IndividualTiles> myCurrentPath;

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    [SerializeField]
    private float timeMovementAnimation;

    //Tiempo que tarda en rotar a la unidad.
    [SerializeField]
    protected float timeDurationRotation;

    [Header("ATAQUE")]

    //Lista de posibles unidades a las que atacar
    [SerializeField]
    public List<UnitBase> currentUnitsAvailableToAttack;

    //Variable que guarda el número más pequeño al comparar el rango del personaje con el número de tiles disponibles para atacar.
    int rangeVSTilesInLineLimitant;

    //Variable en la que guardo el daño a realizar
    private float damageWithMultipliersApplied;

    [SerializeField]
    private float maxHeightDifferenceToAttack;

    [Header("FEEDBACK")]
    
    [SerializeField]
    private Material selectedMaterial;
    [SerializeField]
    private Material finishedMaterial;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.characthersOnTheBoard.Add(this);
        //Aviso al tile en el que empiezo que soy su unidad.
        myCurrentTile.unitOnTile = this;
        initMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        LM.SelectUnit(movementUds, this);
    }

    //El LevelManager avisa a la unidad de que debe moverse.
    public void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(tileToMove);

        hasMoved = true;
        myCurrentPath = pathReceived;

        StartCoroutine("MovingUnitAnimation");
        myCurrentTile = tileToMove;
       
    }

    IEnumerator MovingUnitAnimation()
    {
        isMoving = true;

        //Animación de movimiento
        for (int j = 1; j < myCurrentPath.Count; j++)
        {
            currentTileVectorToMove = new Vector3(myCurrentPath[j].transform.position.x, myCurrentPath[j].transform.position.y + 1, myCurrentPath[j].transform.position.z);

            transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

            yield return new WaitForSeconds(timeMovementAnimation);
        }

        CheckUnitsInRangeToAttack();
        isMoving = false;
    }
    #endregion

    #region ATTACK_&_HEALTH

    //Calcula y aplica el daño a la unidad elegida
    protected void DoDamage(UnitBase unitToDealDamage)
    {
        //Reseteo la variable de daño a realizar
        damageWithMultipliersApplied = baseDamage;

        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > myCurrentTile.height)
        {
            damageWithMultipliersApplied *= multiplicatorLessHeight;
        }

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < myCurrentTile.height)
        {
            damageWithMultipliersApplied *= multiplicatorMoreHeight;
        }
        
        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
        {
            //Ataque por la espalda
            damageWithMultipliersApplied *= multiplicatorBackAttack;
        }

        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied));
    }

    //Función de ataque que se hace override en cada clase
    public virtual void Attack(UnitBase unitToAttack)
    {
        //El daño no lo pongo aquí porque tiene que ser lo primero que se calcule.

        //Cada unidad se encargará de aplicar su efecto.

        //La unidad ha atacado y por tanto no puede hacer nada más.
        FinishMyActions();
    }

    //La unidad ha atacado y por tanto no puede hacer nada más.
    private void FinishMyActions()
    {
        //La unidad ha atacado
        hasAttacked = true;

        //Aviso al LM que deseleccione la unidad
        LM.DeSelectUnit();

        //Doy feedback de que esa unidad no puede hacer nada
        GetComponent<MeshRenderer>().material = finishedMaterial;
    }

    //Función que muestra el efecto del ataque y que se hace override en cada clase.
    public virtual void ShowHover()
    {
        //Cada unidad muestra su efecto
    }

    public override void ReceiveDamage(int damageReceived)
    {
        currentHealth -= damageReceived;

        Debug.Log("me han hecho daño");
        Debug.Log(gameObject.name);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");
    }

    #endregion

    #region FEEDBACK

    public void SelectedColor()
    {
        GetComponent<MeshRenderer>().material = selectedMaterial;
    }

    public void InitialColor()
    {
        GetComponent<MeshRenderer>().material = initMaterial;
    }

    #endregion

    #region CHECKS

    //En caso de querer generalizar la comprobación de en que dirección está un tile en comparación a mi posición, lo que se puede hacer es que la función no sea un void, si no que 
    //devuelva un valor de un enum como el de la rotación del personaje, de tal forma que los 4 ifs solo se ponen una vez y siempre devuelven una dirección

    //De momento esta función simplemente sirve para girar al personaje.
    public void CheckTileDirection(IndividualTiles tileToCheck)
    {
        //Arriba o abajo
        if (tileToCheck.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (tileToCheck.tileZ > myCurrentTile.tileZ)
            {
                transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
            //Abajo
            else
            {
                transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileToCheck.tileX > myCurrentTile.tileX)
            {
                transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
            //Izquierda
            else
            {
                transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
        }
    }

    //Comprueba las unidades (tanto aliadas como enemigas) que están en alcance para ser atacadas.
    public void CheckUnitsInRangeToAttack()
    {
        currentUnitsAvailableToAttack.Clear();

        if (currentFacingDirection == FacingDirection.North || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
           else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height -myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    break;
                }
            }
        }

        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttacked();
        }
    }


    #endregion

}
