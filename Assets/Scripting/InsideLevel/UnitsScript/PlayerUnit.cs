using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    [Header("LOGICA PLAYER")]

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [HideInInspector]
    public bool hasMoved = false;
    [HideInInspector]
    public bool hasAttacked = false;
    [HideInInspector]
    public bool isMovingorRotating = false;

    //Bool para saber si puedo hacer hover a las unidades 
    [HideInInspector]
    public bool canHover;

    //Lista de posibles unidades a las que atacar
    [HideInInspector]
    public List<UnitBase> currentUnitsAvailableToAttack;

    //Solo lo uso para el Rogue
    [HideInInspector]
    public bool hasUsedExtraTurn;

    [Header("MOVIMIENTO")]

    //Camino que tiene que seguir la unidad para moverse
    protected List<IndividualTiles> myCurrentPath = new List<IndividualTiles>();

    [Header("FEEDBACK")]

    [SerializeField]
    public Material selectedMaterial;

    [SerializeField]
    private Material finishedMaterial;

	[SerializeField]
    public Canvas canvasWithRotationArrows;

    //Flecha que indica al jugador si la unidad aún pueda realizar acciones.
    [SerializeField]
    private GameObject arrowIndicator;

    [SerializeField]
    protected GameObject movementTokenInGame;

    [SerializeField]
    private GameObject attackTokenInGame;

    [HideInInspector]
    public GameObject myPanelPortrait;
    [SerializeField]
	public Sprite portraitImage;

	[SerializeField]
	public GameObject actionAvaliablePanel;
	[SerializeField]
	public GameObject backStabIcon, upToDownDamageIcon, downToUpDamageIcon;

	//Para el tooltip de ataque
	[SerializeField]
	[@TextAreaAttribute(15, 20)]
	public string attackInfo;

	[SerializeField]
	public Sprite attackTooltipImage;


	[Header("REFERENCIAS")]

    [HideInInspector]
    public LevelManager LM;
    [HideInInspector]
    public UIManager UIM;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = FindObjectOfType<LevelManager>();
        LM.characthersOnTheBoard.Add(this);
		//Referencia al UIM 
		UIM = FindObjectOfType<UIManager>();
        //Aviso al tile en el que empiezo que soy su unidad.

       ///SETEAR EL TILE AL COLOCAR A LA UNIDAD EN UNO DE LOS TILES DISPONIBLES PARA COLOCARLAS
       // myCurrentTile.unitOnTile = this;
       // myCurrentTile.WarnInmediateNeighbours();

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        movementParticle.SetActive(false);

        //if (LM.FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa)
        //{
        //    currentHealth = maxHealth;
        //}

        //Vigilar esto. He comentado lo anterior y puesto esto porque si no al colocar los personajes si se colocaban al principio del nivel  tenian 0 de vida.
        currentHealth = maxHealth;

    }

    //Stats genéricos que tienen todos los personajes.
    //Los stats especificos se ponen en cada personaje
    public void SetMyGenericStats(int _maxHealth, int _baseDamage, 
                                  int _attackRange)
    {
        maxHealth = _maxHealth;

        //Comprobar que esto no hace que el baseDamage se ponga a 0
        baseDamage = _baseDamage;

        attackRange = _attackRange;





        //Una vez seteadas todas las variables, inicializo mi vida actual
        currentHealth = maxHealth;
    }

    #endregion

    #region TURN_STATE

    //Reseteo las variables
    public void ResetUnitState()
    {
        arrowIndicator.SetActive(true);
        hasMoved = false;
        movementTokenInGame.SetActive(true);
        hasAttacked = false;
        attackTokenInGame.SetActive(true);
        //Refresco de los tokens para resetearlos en pantalla
        UIM.RefreshTokens();
        isMovingorRotating = false;
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;

        hasUsedExtraTurn = false;
    }

    //La unidad ha atacado y por tanto no puede hacer nada más.
    private void FinishMyActions()
    {
        arrowIndicator.SetActive(false);
        //La unidad ha atacado
        hasAttacked = true;
		hasMoved = true;
		//Refresco de los tokens de ataque
		UIM.RefreshTokens();
		//Aviso al LM que deseleccione la unidad
		LM.DeSelectUnit();
        UIM.ActivateDeActivateEndButton();
        //Doy feedback de que esa unidad no puede hacer nada
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = finishedMaterial;
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    //Es virtual para el decoy del mago.
    protected virtual void OnMouseDown()
    {
        if (LM.selectedCharacter == this)
        {
            LM.TileClicked(this.myCurrentTile);
        }
        else
        {
            LM.SelectUnit(movementUds, this);
        }
    }

    //Es virtual para el decoy del mago.
    protected virtual void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions) 
        {
            if (LM.selectedEnemy == null)
            {
                if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                }
                if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    myPanelPortrait.GetComponent<Portraits>().HighlightPortrait();
                }

                if (!hasAttacked)
                {
                    myPanelPortrait.GetComponent<Portraits>().HighlightPortrait();
                    SelectedColor();
                    LM.ShowUnitHover(movementUds, this);
                }
            }
            else if(LM.selectedEnemy != null)
            {
                if (LM.selectedCharacter == null)
                {
                    myPanelPortrait.GetComponent<Portraits>().HighlightPortrait();
                    SelectedColor();
                }

            }
        }
    }

    //Es virtual para el decoy del mago.
    protected virtual void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (LM.currentLevelState != LevelManager.LevelState.Initializing)
        {
            if (LM.selectedCharacter == null)
            {
                //Compruebo si no hay un enemigo seleccionado para no quitarle la info.
                if (LM.selectedEnemy == null)
                {
                    LM.HideUnitHover(this);
                }

                myPanelPortrait.GetComponent<Portraits>().UnHighlightPortrait();

                ResetColor();
            }

            else if (LM.selectedCharacter == this)
            {
                return;
            }

            else if (LM.selectedCharacter != GetComponent<PlayerUnit>())
            {
                LM.HideUnitHover(this);
                myPanelPortrait.GetComponent<Portraits>().UnHighlightPortrait();

                ResetColor();
            }
        }
    }

    #endregion

    #region MOVEMENT_&_ROTATION

    //El LevelManager avisa a la unidad de que debe moverse.
    public virtual void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        //Compruebo la dirección en la que se mueve para girar a la unidad
        //   CheckTileDirection(tileToMove);
        hasMoved = true;
        movementTokenInGame.SetActive(false);
        //Refresco los tokens para reflejar el movimiento
        UIM.RefreshTokens();

        //Limpio myCurrentPath y le añado las referencias de pathReceived 
        //(Como en el goblin no vale con hacer myCurrentPath = PathReceived porque es una referencia a la lista y necesitamos una referencia a los elementos dentro de la lista)
        myCurrentPath.Clear();

        for (int i = 0; i < pathReceived.Count; i++)
        {
            myCurrentPath.Add(pathReceived[i]);
        }

        if (myCurrentPath.Count > 0)
        {
            StartCoroutine("MovingUnitAnimation");
            UpdateInformationAfterMovement(tileToMove);
        }
        else
        {
            isMovingorRotating = false;
            LM.UnitHasFinishedMovementAndRotation();
            UpdateInformationAfterMovement(myCurrentTile);
        }
    }

    IEnumerator MovingUnitAnimation()
    {
        //Activo el trail de particulas de movimiento
        movementParticle.SetActive(true);

        //isMovingorRotating = true;

        if(myCurrentPath.Count > 0)
        {

            //Animación de movimiento
            for (int j = 1; j < myCurrentPath.Count; j++)
            {
                SoundManager.Instance.PlaySound(AppSounds.MOVEMENT);

                //Calcula el vector al que se tiene que mover.
                currentTileVectorToMove = myCurrentPath[j].transform.position; // new Vector3(myCurrentPath[j].transform.position.x, myCurrentPath[j].transform.position.y, myCurrentPath[j].transform.position.z);

                //Muevo y roto a la unidad
                unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation, AxisConstraint.Y);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Espera entre casillas
                yield return new WaitForSeconds(timeMovementAnimation);
            }

            //Desactivo el trail de partículas de movimiento
            movementParticle.SetActive(false);

        }
        
        isMovingorRotating = false;

        //Arriba o abajo
        if (currentFacingDirection == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
        }
    }

    public void RotateUnitFromButton(FacingDirection newDirection, IndividualTiles _tileToMove, List<IndividualTiles> _currentPath)
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
        
        isMovingorRotating = true;

        MoveToTile(_tileToMove, _currentPath);
        LM.UnitHasFinishedMovementAndRotation();

        CheckUnitsInRangeToAttack();
    }


    public void UndoMove(IndividualTiles tileToMoveBack, FacingDirection rotationToTurnBack)
    {
        #region Rotation

        if (rotationToTurnBack == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), 0);
            currentFacingDirection = FacingDirection.North;
        }

        else if (rotationToTurnBack == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), 0);
            currentFacingDirection = FacingDirection.South;
        }

        else if (rotationToTurnBack == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), 0);
            currentFacingDirection = FacingDirection.East;
        }

        else if (rotationToTurnBack == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), 0);
            currentFacingDirection = FacingDirection.West;
        }
        #endregion

        isMovingorRotating = false;
        hasMoved = false;

        transform.DOMove(tileToMoveBack.transform.position, 0);
        UpdateInformationAfterMovement(tileToMoveBack);


    }

    #endregion

    #region ATTACK_&_HEALTH

    //Función de ataque que se hace override en cada clase
    public virtual void Attack(UnitBase unitToAttack)
    {
        attackTokenInGame.SetActive(false);

        //El daño y la animación no lo pongo aquí porque tiene que ser lo primero que se calcule.

        //Cada unidad se encargará de aplicar su efecto.

        //La unidad ha atacado y por tanto no puede hacer nada más. Así que espero a que acabe la animación y finalizo su turno.
        StartCoroutine("AttackWait");
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(timeWaitAfterAttack);
        FinishMyActions();
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Animación de ataque
        myAnimator.SetTrigger("Damage");

        currentHealth -= damageReceived;

        Debug.Log("Soy " + name + " me han hecho daño");

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        //Cuando me hacen daño refresco la información en la interfaz
        UIM.RefreshHealth();

        base.ReceiveDamage(damageReceived,unitAttacker);
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación de ataque
        myAnimator.SetTrigger("Death");

        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();

        LM.characthersOnTheBoard.Remove(this);
        myPanelPortrait.SetActive(false);
        UIM.panelesPJ.Remove(myPanelPortrait);
        Destroy(gameObject);        
    }

    #endregion

    #region FEEDBACK

    //Función que muestra el efecto del ataque y que se hace override en cada clase.
    public virtual void ShowHover(UnitBase enemyToAttack)
    {
        //Cada unidad muestra su efecto
        CalculateDamage(enemyToAttack);
		//Mostrar el daño es común a todos
		//enemyToAttack.EnableCanvasHover(Mathf.RoundToInt(damageWithMultipliersApplied));
    }

    public void SelectedColor()
    {
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
    }

    //Override para que el personaje pueda volver a negro si ya ha atacado
    public override void ResetColor()
    {
        //Si ha atacado vuelve al color negro
        if (hasAttacked)
        {
            unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = finishedMaterial;
        }

        //Si no vuelve al inicial
        else
        {
            unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;
        }
    }

	protected override void CalculateDamage(UnitBase unitToDealDamage)
	{
		//Reseteo la variable de daño a realizar
		damageWithMultipliersApplied = baseDamage;

		//Si estoy en desventaja de altura hago menos daño
		if (unitToDealDamage.myCurrentTile.height > myCurrentTile.height)
		{
			damageWithMultipliersApplied -= penalizatorDamageLessHeight;
			downToUpDamageIcon.SetActive(true);
		}

		//Si estoy en ventaja de altura hago más daño
		else if (unitToDealDamage.myCurrentTile.height < myCurrentTile.height)
		{
			damageWithMultipliersApplied += bonusDamageMoreHeight;
			upToDownDamageIcon.SetActive(true);
		}

		//Si le ataco por la espalda hago más daño
		if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
		{
			//Ataque por la espalda
			damageWithMultipliersApplied += bonusDamageBackAttack;
			backStabIcon.SetActive(true);
		}

        Debug.Log("Daño base: " + baseDamage + " Daño con multiplicadores " + damageWithMultipliersApplied);
	}

	public void HideDamageIcons()
	{
		downToUpDamageIcon.SetActive(false);
		upToDownDamageIcon.SetActive(false);
		backStabIcon.SetActive(false);
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
    //Es virtual porque la comprobación del pícaro es diferente (tiene que tener en cuenta el tile en el que va a acabar tras el salto).
    public virtual void CheckUnitsInRangeToAttack()
    {
        currentUnitsAvailableToAttack.Clear();

        if (currentFacingDirection == FacingDirection.North)
        {
            if (attackRange <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
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

        if (currentFacingDirection == FacingDirection.South)
        {
            if (attackRange <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
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

        if (currentFacingDirection == FacingDirection.East)
        {
            if (attackRange <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
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

        if (currentFacingDirection == FacingDirection.West)
        {
            if (attackRange <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
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

        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttacked();
        }

		
    }

    #endregion


    //Si estamos haciendo pruebas y no cargamos desde el nivel de mapa tiene que hacer el raycast. 
    //Si probamos desde el mapa de seleccion el tile se setea al colocar las unidades.
    protected override void FindAndSetFirstTile()
    {
        if (LM.FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa)
        {
            base.FindAndSetFirstTile();
        }
    }
}
