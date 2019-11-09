using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    [Header("STATS ESPECÍFICO")]

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [SerializeField]
    public bool hasMoved = false;
    [SerializeField]
    public bool hasAttacked = false;
    [SerializeField]
    public bool isMovingorRotating = false;

    [Header("MOVIMIENTO")]

    //Camino que tiene que seguir la unidad para moverse
    private List<IndividualTiles> myCurrentPath;

    [Header("FEEDBACK")]
    
    [SerializeField]
    private Material selectedMaterial;
    [SerializeField]
    private Material finishedMaterial;

    [SerializeField]
    private Canvas canvasWithRotationArrows;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;
	public GameObject UIManagerRef;
	private UIManager UIM;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.characthersOnTheBoard.Add(this);
		//Referencia al UIM 
		UIM = UIManagerRef.GetComponent<UIManager>();
        //Aviso al tile en el que empiezo que soy su unidad.
        myCurrentTile.unitOnTile = this;
        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        movementParticle.SetActive(false);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    #endregion

    #region TURN_STATE

    //Reseteo las variables
    public void ResetUnitState()
    {
        hasMoved = false;
        hasAttacked = false;
		//Refresco de los tokens para resetearlos en pantalla
		UIM.RefreshTokens();
        isMovingorRotating = false;
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;
    }

    //La unidad ha atacado y por tanto no puede hacer nada más.
    private void FinishMyActions()
    {
        //La unidad ha atacado
        hasAttacked = true;
		hasMoved = true;
		//Refresco de los tokens de ataque
		UIM.RefreshTokens();
		//Aviso al LM que deseleccione la unidad
		LM.DeSelectUnit();

        //Doy feedback de que esa unidad no puede hacer nada
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = finishedMaterial;
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        LM.SelectUnit(movementUds, this);
    }

    #endregion

    #region MOVEMENT_&_ROTATION

    //El LevelManager avisa a la unidad de que debe moverse.
    public void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(tileToMove);
        hasMoved = true;
		//Refresco los tokens para reflejar el movimiento
		UIM.RefreshTokens();
        myCurrentPath = pathReceived;

        StartCoroutine("MovingUnitAnimation");

        myCurrentTile.unitOnTile = null;
        myCurrentTile = tileToMove;
        myCurrentTile.unitOnTile = this;
       
    }

    IEnumerator MovingUnitAnimation()
    {
        //Activo el trail de particulas de movimiento
        movementParticle.SetActive(true);

        isMovingorRotating = true;

        //Animación de movimiento
        for (int j = 1; j < myCurrentPath.Count; j++)
        {
            SoundManager.Instance.PlaySound(AppSounds.MOVEMENT);

            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = new Vector3(myCurrentPath[j].transform.position.x, myCurrentPath[j].transform.position.y, myCurrentPath[j].transform.position.z);

            Debug.Log(currentTileVectorToMove);
            //Muevo y roto a la unidad
            unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation);
            transform.DOMove(currentTileVectorToMove, timeMovementAnimation);
            
            //Espera entre casillas
            yield return new WaitForSeconds(timeMovementAnimation);
        }

        //Desactivo el trail de partículas de movimiento
        movementParticle.SetActive(false);

        //Esto es solo para la prueba de movimiento para ver cual elegimos.
        if (!LM.TM.isDiagonalMovement)
        {
            //Movimiento con torre sin giro
            if (!LM.TM.isChooseRotationIfTower)
            {
                CheckUnitsInRangeToAttack();
                LM.UnitHasFinishedMovementAndRotation();
                isMovingorRotating = false;
            }

            //Movimiento con torre con giro
            else
            {
                //Hacer que aparezcan los botones
                canvasWithRotationArrows.gameObject.SetActive(true);
            }
        }

        //Movimiento en diagonal
        else
        {
            Debug.Log("Choose Arrow");
            //Hacer que aparezcan los botones
            canvasWithRotationArrows.gameObject.SetActive(true);
        }
    }

    public void RotateUnitFromButton(FacingDirection newDirection)
    {

        //Arriba o abajo
        if (newDirection == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.North;
        }

        else if (newDirection == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.South;
        }

        else if (newDirection == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.East;
        }

        else if (newDirection == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.West;
        }

        canvasWithRotationArrows.gameObject.SetActive(false);
        CheckUnitsInRangeToAttack();
        isMovingorRotating = false;

        LM.UnitHasFinishedMovementAndRotation();
    }

    #endregion

    #region ATTACK_&_HEALTH

    //Función de ataque que se hace override en cada clase
    public virtual void Attack(UnitBase unitToAttack)
    {
        //El daño y la animación no lo pongo aquí porque tiene que ser lo primero que se calcule.

        //Cada unidad se encargará de aplicar su efecto.

        //La unidad ha atacado y por tanto no puede hacer nada más.
        FinishMyActions();
		
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Animación de ataque
        myAnimator.SetTrigger("Damage");

        currentHealth -= damageReceived;
		//Cuando me hacen daño refresco la información en la interfaz
		UIM.RefreshHealth();

        Debug.Log("Soy " + name + "me han hecho daño");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación de ataque
        myAnimator.SetTrigger("Death");

        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);
    }

    #endregion

    #region FEEDBACK

    //Función que muestra el efecto del ataque y que se hace override en cada clase.
    public virtual void ShowHover(UnitBase enemyToAttack)
    {
        //Cada unidad muestra su efecto

        CalculateDamage(enemyToAttack);

        //Mostrar el daño es común a todos
        enemyToAttack.EnableCanvasHover(Mathf.RoundToInt(damageWithMultipliersApplied));
    }

    public void SelectedColor()
    {
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
    }

    public void InitialColor()
    {
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;
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
                unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
            //Abajo
            else
            {
                unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileToCheck.tileX > myCurrentTile.tileX)
            {
                unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
            //Izquierda
            else
            {
                unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
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
