using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    [Header("COLOCACIÓN DE UNIDAD")]
    [SerializeField]
    public bool characterStartedOnTheLevel;

    [HideInInspector]
    public Transform initialPosInBox;

    [Header("LOGICA PLAYER")]
 
    [HideInInspector]
    public bool isMovingorRotating = false;

    //Bool para saber si puedo hacer hover a las unidades 
    [HideInInspector]
    public bool canHover;

    //Lista de posibles unidades a las que atacar
    [SerializeField]
    public List<UnitBase> currentUnitsAvailableToAttack;

    //Lista de tiles al hacer hover en enemigos
    [HideInInspector]
    public List<IndividualTiles> tilesInEnemyHover;

    [Header("MOVIMIENTO")]

    //Camino que tiene que seguir la unidad para moverse
    protected List<IndividualTiles> myCurrentPath = new List<IndividualTiles>();

    [Header("PLAYER_UNIT")]
    //Tiempo a esperar tras atacar
    [SerializeField]
    protected float timeWaitAfterAttack;

	[Header("INFO")]
    [HideInInspector]
	public string activeSkillInfo;
    [HideInInspector]
	public string pasiveSkillInfo;

    [HideInInspector]
    public Sprite activeTooltipIcon;
    [HideInInspector]
	public Sprite pasiveTooltipIcon;

    [Header("FEEDBACK")]

    [SerializeField]
    public Material selectedMaterial;

    [SerializeField]
    private Material finishedMaterial;

    protected GameObject movementTokenInGame;

    private GameObject attackTokenInGame;

    [HideInInspector]
    public GameObject myPanelPortrait;

    [Header("PORTRAITS & REF")]

    //Flecha que indica al jugador si la unidad aún pueda realizar acciones.
    [SerializeField]
    private GameObject arrowIndicator;

    [SerializeField]
	public Sprite portraitImage;

	[SerializeField]
	public GameObject actionAvaliablePanel;

	[SerializeField]
	private Image inGamePortrait2;

    [SerializeField]
    public Canvas canvasWithRotationArrows;

    //Objeto que sirve de padre para todo el hud in game del personaje
    [SerializeField]
    public GameObject insideGameInfoObject;

    [Header("HOVER PARTICLE")]

    //Escudo que bloquea el daño completo
    [SerializeField]
    public GameObject shieldBlockAllDamage;

    //Escudo que no bloquea el daño completo
    [SerializeField]
    public GameObject shieldBlockPartialDamage;

    //Para el tooltip de ataque

    [Header("REFERENCIAS")]

    [HideInInspector]
    public LevelManager LM;
    [HideInInspector]
    public UIManager UIM;
    //Añado esto para que el mage pueda acceder a la función de GetSurroundingTiles()
    [HideInInspector]
    public TileManager TM;
    [HideInInspector]
    public Monk pjMonkUnitReference;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = FindObjectOfType<LevelManager>();

        TM = FindObjectOfType<TileManager>();

		//Referencia al UIM 
		UIM = FindObjectOfType<UIManager>();

        //Inicializo componente animator, material inicial y particula mov
        myAnimator = GetComponent<Animator>();
        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;
        movementParticle.SetActive(false);

        fMovementUds = movementUds;

		//Asigno a la imagen dentro del juego el retrato del personaje
		if(characterImage != null)
		{
			inGamePortrait.sprite = characterImage;
            inGamePortrait.preserveAspect = true;
            inGamePortrait2.sprite = characterImage;
            inGamePortrait2.preserveAspect = true;
        }
		


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

        //Compruebo si los tiles de daño tienen que hacer daño. 
        for (int i = 0; i < LM.damageTilesInBoard.Count; i++)
        {
            if (LM.damageTilesInBoard[i].unitToDoDamage != null 
                && LM.damageTilesInBoard[i].unitToDoDamage.GetComponent<PlayerUnit>() != null 
                && LM.damageTilesInBoard[i].unitToDoDamage.GetComponent<PlayerUnit>() == this)
            {
                
                LM.damageTilesInBoard[i].CheckHasToDoDamage();
                LM.damageTilesInBoard[i].damageDone = true;
                UIM.RefreshHealth();
                RefreshHealth(false);
                break;
            }
        }

        if (!isDead)
        {
          
                if (turnStunned <= 0)
                {
                    isStunned = false;
                    turnStunned = 0;
                    SetStunIcon(this, false, false);
                }
                turnStunned--;

                if (!isStunned)
                {
                    turnsWithBuffOrDebuff--;
                    if (turnsWithBuffOrDebuff <= 0)
                    {
                        turnsWithBuffOrDebuff = 0;
                        if (GetComponent<Druid>())
                        {
                            GetComponent<Druid>().healedLife -= GetComponent<Druid>().buffHeal;
                        }
                        else
                        {
                            buffbonusStateDamage = 0;
                        }

                        SetBuffDebuffIcon(0, this, false);
                    }

                    turnsWithMovementBuffOrDebuff--;
                    if (turnsWithMovementBuffOrDebuff <= 0)
                    {
                        turnsWithMovementBuffOrDebuff = 0;
                        movementUds = fMovementUds;
                        SetMovementIcon(0, this, false);
                    }

                    if (arrowIndicator != null)
                    {
                        arrowIndicator.SetActive(true);
                    }
                    hasMoved = false;
                    if (movementTokenInGame != null)
                    {
                        movementTokenInGame.SetActive(true);
                    }
                    hasAttacked = false;
                    if (attackTokenInGame != null)
                    {
                        attackTokenInGame.SetActive(true);
                    }

                    //Refresco de los tokens para resetearlos en pantalla
                    UIM.RefreshTokens();
                    CheckWhatToDoWithSpecialsTokens();
                    isMovingorRotating = false;
                    unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;
                    ResetSpecificVariables();
                }
                else
                {
                    turnsWithBuffOrDebuff--;
                    if (turnsWithBuffOrDebuff <= 0)
                    {
                        turnsWithBuffOrDebuff = 0;
                        buffbonusStateDamage = 0;
                    }
                    if (arrowIndicator != null)
                    {
                        arrowIndicator.SetActive(false);
                    }
                    hasMoved = true;

                    if (movementTokenInGame != null)
                    {
                        movementTokenInGame.SetActive(false);
                    }
                    hasAttacked = true;
                    if (attackTokenInGame != null)
                    {
                        attackTokenInGame.SetActive(false);
                    }
                    //Refresco de los tokens para resetearlos en pantalla
                    UIM.RefreshTokens();
                    CheckWhatToDoWithSpecialsTokens();
                    isMovingorRotating = false;
                    unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = finishedMaterial;
                    ResetSpecificVariables();


                }
            

        }

    }

    //La unidad ha atacado y por tanto no puede hacer nada más.
    private void FinishMyActions()
    {
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
        }
        
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

    public virtual void ResetSpecificVariables()
    {
        //Cada unidad resetea si tiene variables específicas
    }
    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    //Es virtual para el decoy del mago.
    protected virtual void OnMouseDown()
    {
        if (!GameManager.Instance.isGamePaused)
        {
            if (LM.selectedCharacter == this)
            {
                LM.TileClicked(this.myCurrentTile);
            }

            else
            {
                Valkyrie valkyrieRef = FindObjectOfType<Valkyrie>();

                if (valkyrieRef != null && valkyrieRef.changePositions && LM.selectedCharacter == valkyrieRef && !valkyrieRef.hasMoved)
                {
                    if (currentHealth <= valkyrieRef.numberCanChange)
                    {
                        valkyrieRef.ChangePosition(this);
                    }
                }

                else
                {
                    LM.SelectUnit(movementUds, this);
                }
            }
        }      
    }

    //Es virtual para el decoy del mago.
    protected virtual void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.Initializing && !characterStartedOnTheLevel)
        {
            SelectedColor();
        }

        else if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && !GameManager.Instance.isGamePaused)
        {
            if (LM.selectedEnemy == null)
            {
                //Ataque
                if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    LM.CalculatePreviousActionPlayer(LM.selectedCharacter, this);
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                }

                else if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {

                    //ESTO LO HE QUITADO PORQUE MOSTRABA EL NÚMERO DE DAÑO DEL OBJETIVO SOLO CON HACER HOVER AL SAMURAI.
                    //No se si hace algo más asi que de momento lo comento

                    //Samurai samuraiRef = FindObjectOfType<Samurai>();
                    //if (samuraiRef != null && LM.selectedCharacter == samuraiRef)
                    //{
                    //    LM.CalculatePreviousActionPlayer(LM.selectedCharacter, this);
                    //}

                    Valkyrie valkyrieRef = FindObjectOfType<Valkyrie>();
                    if (valkyrieRef != null && LM.selectedCharacter == valkyrieRef)
                    {

                        LM.CalculatePreviousActionPlayer(LM.selectedCharacter, this);
                    }
                }

                if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    myPanelPortrait.GetComponent<Portraits>().HighlightPortrait();
                }

                if (!hasAttacked && LM.selectedCharacter == null)
                {
                    myPanelPortrait.GetComponent<Portraits>().HighlightPortrait();
                    SelectedColor();
                    LM.ShowUnitHover(movementUds, this);
                }
            }

            else if (LM.selectedEnemy != null)
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

        if (LM.currentLevelState == LevelManager.LevelState.Initializing)
        {
            if (LM.currentCharacterPlacing != GetComponent<UnitBase>())
            {
                ResetColor();
            }
        }

        else if (LM.currentLevelState != LevelManager.LevelState.Initializing)
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

            if (LM.selectedCharacter != null && LM.selectedCharacter.sombraHoverUnit != null)
            {
                LM.selectedCharacter.sombraHoverUnit.SetActive(false);

                if (LM.selectedCharacter.tilesInEnemyHover.Count > 0)
                {

                    for (int i = 0; i < LM.selectedCharacter.tilesInEnemyHover.Count; i++)
                    {
                        if (LM.tilesAvailableForMovement.Count > 0 && LM.tilesAvailableForMovement.Contains(LM.selectedCharacter.tilesInEnemyHover[i]))
                        {
                            LM.selectedCharacter.tilesInEnemyHover[i].ColorDesAttack();
                            LM.selectedCharacter.tilesInEnemyHover[i].ColorMovement();
                        }
                        else
                        {
                            LM.selectedCharacter.tilesInEnemyHover[i].ColorDesAttack();
                        }

                        if (LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile != null)
                        {
                            LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.ResetColor();
                            LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.sombraHoverUnit.SetActive(false);

                        }
                    }
                }

                LM.selectedCharacter.HideAttackEffect(this);
            }


            if (sombraHoverUnit != null)
            {
                if (LM.selectedEnemy == null)
                {
                    sombraHoverUnit.SetActive(false);
                }
            }
            Druid druidRef = FindObjectOfType<Druid>();

            if (druidRef != null && LM.selectedCharacter == druidRef)
            {
                // Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                druidRef.previsualizeAttackIcon.SetActive(false);
                druidRef.canvasHover.SetActive(false);
            }


            //Tiene que ir después del HideAttackEffect
            if (LM.selectedCharacter != null)
            {
                LM.selectedCharacter.tilesInEnemyHover.Clear();
            }
        }

        //Quito el healthbar de los objetivos a los que puedo atacar al salir del hover
        //Aunque lo desactivo en el hover exit, se activan en el CheckUnits en vez de en el hover enter
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].HealthBarOn_Off(false);
        }
    }

    #endregion

    #region MOVEMENT_&_ROTATION

    //El LevelManager avisa a la unidad de que debe moverse.
    //Esta función tiene que ser override para que el mago pueda instanciar decoys.
    public virtual void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        //Compruebo la dirección en la que se mueve para girar a la unidad
        //   CheckTileDirection(tileToMove);
        hasMoved = true;
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

        Samurai samuraiRef = FindObjectOfType<Samurai>();

        if (samuraiRef != null )
        {
            samuraiRef.CheckIfIsLonely();

        }

      
        //Al acabar al movimiento aviso a levelManager de que avise a los enemigos para ver si serán alertados.
        LM.AlertEnemiesOfPlayerMovement();
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

            //Estas líneas las añado para comprobar si el caballero tiene que defender
            Knight knightDef = FindObjectOfType<Knight>();

            if (knightDef != null && !GetComponent<Knight>())
            {
                CheckIfKnightIsDefendingAfterUnitMovement(knightDef, currentFacingDirection);
            }

            //Desactivo el trail de partículas de movimiento
            movementParticle.SetActive(false);
        }

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

        yield return new WaitForSeconds(timeDurationRotation);

        isMovingorRotating = false;
    }

    public void RotateUnitFromButton(FacingDirection newDirection, IndividualTiles _tileToMove, List<IndividualTiles> _currentPath)
    {
        hasMoved = true;

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

        //IMPORTANTE: HE PUESTO ESTO DESPUÉS PARA QUE FUNCIONE EL MOSTRAR RANGO DE ATAQUE Y PARECE NO DAR PROBLEMAS. REVISAR EN EL FUTURO
        //Esto tiene que ir antes del  LM.UnitHasFinishedMovementAndRotation() para que función de UnitHasFinishedMovementAndRotation() sepa si hay
        // enemigos a los que atacar
        CheckUnitsAndTilesInRangeToAttack(false);

        UIM.CheckActionsAvaliable();
    }

    public override void UndoMove(MoveCommand _moveCommand ,IndividualTiles tileToMoveBack, FacingDirection rotationToTurnBack, bool shouldResetMovement)
    {
        base.UndoMove(_moveCommand, tileToMoveBack, rotationToTurnBack, shouldResetMovement);

        if (shouldResetMovement)
        {
            isMovingorRotating = false;
            hasMoved = false;
        }

        Samurai samuraiRef = FindObjectOfType<Samurai>();

        if (samuraiRef != null)
        {
            samuraiRef.CheckIfIsLonely();
        }

        //Bufos y debufos

        //Druida es especial y modifica healedlife
        if (!GetComponent<Druid>())
        {
            buffbonusStateDamage = _moveCommand.pj_damageBuffDebuff;
        }

        turnsWithBuffOrDebuff = _moveCommand.pj_turnsDamageBuffDebuff;

        movementUds = _moveCommand.pj_movementBuffDebuff;
        turnsWithMovementBuffOrDebuff = _moveCommand.pj_turnsMovementBuffDebuff;

        UIM.RefreshTokens();
        UIM.CheckActionsAvaliable();
        //Estas líneas las añado para comprobar si hay samurai y si hay que actualizar el honor
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null)
        {
            samuraiUpgraded.RefreshHonorOnPortrait();
        }
    }

    public override void UndoAttack(AttackCommand lastAttack, bool _isThisUnitTheAttacker)
    {
        base.UndoAttack(lastAttack, _isThisUnitTheAttacker);

        if (_isThisUnitTheAttacker)
        {
            //Permitirle otra vez atacar
            hasAttacked = lastAttack.pjHasAttacked;
            hasMoved = lastAttack.pjHasMoved;

            //Resetear el material
            ResetColor();

            #region Rotation

            if (lastAttack.pjPreviousRotation == FacingDirection.North)
            {
                unitModel.transform.DORotate(new Vector3(0, 0, 0), 0);
                currentFacingDirection = FacingDirection.North;
            }

            else if (lastAttack.pjPreviousRotation == FacingDirection.South)
            {
                unitModel.transform.DORotate(new Vector3(0, 180, 0), 0);
                currentFacingDirection = FacingDirection.South;
            }

            else if (lastAttack.pjPreviousRotation == FacingDirection.East)
            {
                unitModel.transform.DORotate(new Vector3(0, 90, 0), 0);
                currentFacingDirection = FacingDirection.East;
            }

            else if (lastAttack.pjPreviousRotation == FacingDirection.West)
            {
                unitModel.transform.DORotate(new Vector3(0, -90, 0), 0);
                currentFacingDirection = FacingDirection.West;
            }
            #endregion

            //Mover de tile
            transform.DOMove(lastAttack.pjPreviousTile.transform.position, 0);
            UpdateInformationAfterMovement(lastAttack.pjPreviousTile);

            //Vida
            currentHealth = lastAttack.pjPreviousHealth;

            currentArmor = lastAttack.pjArmor;

            isStunned = lastAttack.pjIsStunned;

            isMarked = lastAttack.pjIsMarked;
            numberOfMarks = lastAttack.pjnumberOfMarks;

            if (!isMarked)
            {
                QuitMarks();
            }

            if (numberOfMarks >= 1)
            {
                isMarked = true;
                monkMarkUpgrade.SetActive(false);
                monkMark.SetActive(true);
            }

            else if (numberOfMarks == 2)
            {
                isMarked = true;
                monkMarkUpgrade.SetActive(true);
                monkMark.SetActive(false);
            }

            //Bufos y debufos

            //Druida es especial y modifica healedlife
            if (!GetComponent<Druid>())
            {
                buffbonusStateDamage = lastAttack.pj_damageBuffDebuff;
            }
            
            turnsWithBuffOrDebuff = lastAttack.pj_turnsDamageBuffDebuff;

            movementUds = lastAttack.pj_movementBuffDebuff;
            turnsWithMovementBuffOrDebuff = lastAttack.pj_turnsMovementBuffDebuff;
        }

        else
        {
            //Permitirle otra vez atacar
            hasMoved = lastAttack.objHasMoved;
            hasAttacked = lastAttack.objHasAttacked;

            //Resetear el material
            ResetColor();

            #region Rotation

            if (lastAttack.objPreviousRotation == FacingDirection.North)
            {
                unitModel.transform.DORotate(new Vector3(0, 0, 0), 0);
                currentFacingDirection = FacingDirection.North;
            }

            else if (lastAttack.objPreviousRotation == FacingDirection.South)
            {
                unitModel.transform.DORotate(new Vector3(0, 180, 0), 0);
                currentFacingDirection = FacingDirection.South;
            }

            else if (lastAttack.objPreviousRotation == FacingDirection.East)
            {
                unitModel.transform.DORotate(new Vector3(0, 90, 0), 0);
                currentFacingDirection = FacingDirection.East;
            }

            else if (lastAttack.objPreviousRotation == FacingDirection.West)
            {
                unitModel.transform.DORotate(new Vector3(0, -90, 0), 0);
                currentFacingDirection = FacingDirection.West;
            }
            #endregion

            //Mover de tile
            transform.DOMove(lastAttack.objPreviousTile.transform.position, 0);
            UpdateInformationAfterMovement(lastAttack.objPreviousTile);

            //Vida
            currentHealth = lastAttack.objPreviousHealth;

            currentArmor = lastAttack.objArmor;

            isStunned = lastAttack.objIsStunned;

            isMarked = lastAttack.objIsMarked;
            numberOfMarks = lastAttack.objnumberOfMarks;

            if (!isMarked)
            {
                QuitMarks();
            }

            if (numberOfMarks >= 1)
            {
                isMarked = true;
                monkMarkUpgrade.SetActive(false);
                monkMark.SetActive(true);
            }

            else if (numberOfMarks == 2)
            {
                isMarked = true;
                monkMarkUpgrade.SetActive(true);
                monkMark.SetActive(false);
            }


            //Bufos y debufos
            if (!GetComponent<Druid>())
            {
                buffbonusStateDamage = lastAttack.obj_damageBuffDebuff;
            }
                
            turnsWithBuffOrDebuff = lastAttack.obj_turnsDamageBuffDebuff;

            movementUds = lastAttack.obj_movementBuffDebuff;
            turnsWithMovementBuffOrDebuff = lastAttack.obj_turnsMovementBuffDebuff;
        }

        //Actualizar hud
        RefreshHealth(false);
        UIM.RefreshHealth();
        UIM.RefreshTokens();
        //Estas líneas las añado para comprobar si hay samurai y si hay que actualizar el honor
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null)
        {
            samuraiUpgraded.RefreshHonorOnPortrait();
        }
        UIM.CheckActionsAvaliable();
    }

    #endregion

    #region ATTACK_&_HEALTH

    //Función de ataque que se hace override en cada clase
    //El daño y la animación no lo pongo aquí porque tiene que ser lo primero que se calcule.
    //Cada unidad se encargará de aplicar su efecto en su override.
    public virtual void Attack(UnitBase unitToAttack)
    {
        //Esto es para que cuando ataquen los personajes exploten las marcas
        if (pjMonkUnitReference != null)
        {  
            if (!GetComponent<Monk>())
            {
                if (unitToAttack.isMarked)
                {
                    if (pjMonkUnitReference.suplex2 && unitToAttack.numberOfMarks == 2)
                    {
                        //Curar al personaje marca mejorada
                        currentHealth += pjMonkUnitReference.healWithUpgradedMark;
                    }

                    else
                    {
                        //Curar al personaje normal
                        currentHealth += pjMonkUnitReference.healerBonus;
                    }

                    RefreshHealth(false);
                    UIM.RefreshHealth();

                    unitToAttack.isMarked = false;
                    unitToAttack.numberOfMarks = 0;
                }
            }
        }

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null )
        {
            samuraiUpgraded.RefreshHonorOnPortrait();

        }
        
        //Añado esto para saber si tengo que resetear el honor
         if (unitToAttack != null && unitToAttack.currentFacingDirection == currentFacingDirection)
         {
             LM.honorCount = 0;
         }
                                    
        UIM.CheckActionsAvaliable();

        //La unidad ha atacado y por tanto no puede hacer nada más. Así que espero a que acabe la animación y finalizo su turno.
        StartCoroutine("AttackWait");
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(timeWaitAfterAttack);
        FinishMyActions();
    }

    public void CheckIfKnightIsDefending(Knight knightThatDef, FacingDirection unitThatIsAttackingDirection)
    {
        if (knightThatDef != null)
        {
            //Este es el valor que queremos que tenga para defender unidades
            knightThatDef.shieldDef = 1;

            //Si tiene la mejora
            if (knightThatDef.isBlockingNeighbours)
            {
                //Compruebo si el caballero tiene como vecino a la unidad que esta comprobando
                if (knightThatDef.myCurrentTile.neighbours.Contains(myCurrentTile))
                {
                    //Si esta en los vecinos compruebo si la direccion es la correcta (compruebo si el atacante y el caballero están opuestos)
                    if ((knightThatDef.currentFacingDirection == FacingDirection.North && unitThatIsAttackingDirection == FacingDirection.South)
                        || (knightThatDef.currentFacingDirection == FacingDirection.South && unitThatIsAttackingDirection == FacingDirection.North)
                        || (knightThatDef.currentFacingDirection == FacingDirection.West && unitThatIsAttackingDirection == FacingDirection.East)
                        || (knightThatDef.currentFacingDirection == FacingDirection.East && unitThatIsAttackingDirection == FacingDirection.West))
                    {
                        
                            //Si tiene la segunda mejora el valor es 999 porque bloquea todo el daño 
                            if (knightThatDef.isBlockingNeighboursFull)
                            {
                                knightThatDef.shieldDef = 999;
                            }

                            //Si solo tiene la primera lo dejo con el valor mínimo (podría quitar este else en realidad pero lo dejo por claridad)
                            else
                            {
                                knightThatDef.shieldDef = 1;
                            }                                               
                    }

                    //Si la dirección no coincide no protege nada
                    else
                    {
                        knightThatDef.shieldDef = 0;
                    }
                }

                //Si no está en los vecinos el caballero no le protege de ningún daño
                else
                {
                    knightThatDef.shieldDef = 0;
                }
            }

            //Si no tiene la mejora no reduce el daño
            else
            {
                knightThatDef.shieldDef = 0;
            }
        }
    }

    public void CheckIfKnightIsDefendingAfterUnitMovement(Knight knightThatDef, FacingDirection unitThatIsAttackingDirection)
    {
        if (knightThatDef != null)
        {
            //Este es el valor que queremos que tenga para defender unidades
            knightThatDef.shieldDef = 1;

            //Si tiene la mejora
            if (knightThatDef.isBlockingNeighbours)
            {
                //Compruebo si el caballero tiene como vecino a la unidad que esta comprobando
                if (knightThatDef.myCurrentTile.neighbours.Contains(myCurrentTile))
                {
                    if ((knightThatDef.currentFacingDirection == FacingDirection.North && unitThatIsAttackingDirection == FacingDirection.North)
                      && myCurrentTile == knightThatDef.myCurrentTile.tilesInLineLeft[0] || myCurrentTile == (knightThatDef.myCurrentTile.tilesInLineRight[0]))
                    {
                        if (knightThatDef.isBlockingNeighboursFull)
                        {
                            ShowHideFullShield(true);
                        }
                        else
                        {
                            ShowHidePartialShield(true);
                        }

                        StartCoroutine("HideShields");
                    }else if ((knightThatDef.currentFacingDirection == FacingDirection.South && unitThatIsAttackingDirection == FacingDirection.South)
                      && myCurrentTile == knightThatDef.myCurrentTile.tilesInLineLeft[0] || myCurrentTile == (knightThatDef.myCurrentTile.tilesInLineRight[0]))
                    {
                        if (knightThatDef.isBlockingNeighboursFull)
                        {
                            ShowHideFullShield(true);
                        }
                        else
                        {
                            ShowHidePartialShield(true);
                        }

                        StartCoroutine("HideShields");
                    }
                    else if ((knightThatDef.currentFacingDirection == FacingDirection.East && unitThatIsAttackingDirection == FacingDirection.East)
                     && myCurrentTile == knightThatDef.myCurrentTile.tilesInLineUp[0] || myCurrentTile == (knightThatDef.myCurrentTile.tilesInLineDown[0]))
                    {
                        if (knightThatDef.isBlockingNeighboursFull)
                        {
                            ShowHideFullShield(true);
                        }
                        else
                        {
                            ShowHidePartialShield(true);
                        }

                        StartCoroutine("HideShields");
                    }
                    else if ((knightThatDef.currentFacingDirection == FacingDirection.West && unitThatIsAttackingDirection == FacingDirection.West)
                     && myCurrentTile == knightThatDef.myCurrentTile.tilesInLineUp[0] || myCurrentTile == (knightThatDef.myCurrentTile.tilesInLineDown[0]))
                    {
                        if (knightThatDef.isBlockingNeighboursFull)
                        {
                            ShowHideFullShield(true);
                        }
                        else
                        {
                            ShowHidePartialShield(true);
                        }

                        StartCoroutine("HideShields");
                    }
                }
            }
        }
    }

    IEnumerator HideShields()
    {

        yield return new WaitForSeconds(0.5f);
        
            ShowHideFullShield(false);       
            ShowHidePartialShield(false);
        
    }


    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        SoundManager.Instance.PlaySound(AppSounds.RECEIVEDAMAGE);

        //Animación de ataque
        myAnimator.SetTrigger("Damage");

            //Estas líneas las añado para comprobar si el caballero tiene que defender
            Knight knightDef = FindObjectOfType<Knight>();


            if (knightDef != null)
            {
                if (unitAttacker != null)
                {
                    CheckIfKnightIsDefending(knightDef, unitAttacker.currentFacingDirection);
                    damageReceived -= knightDef.shieldDef;
                }
            }

            if (damageReceived < 0)
            {
                damageReceived = 0;
            }

            if (currentArmor > 0)
            {
                currentArmor -= damageReceived;
                if (currentArmor < 0)
                {
                    damageReceived = currentArmor * -1;
                    currentHealth -= damageReceived;
                    currentArmor = 0;
                }
            }

            else
            {
                currentHealth -= damageReceived;
            }

            Debug.Log("Soy " + name + " me han hecho daño");

            if (currentHealth <= 0)
            {
                //Logro matar aliado
                if (unitAttacker != null && unitAttacker.GetComponent<PlayerUnit>())
                {
                    GameManager.Instance.UnlockAchievement(AppAchievements.ACHV_NOBODY);
                }

                Die();
                return;
            }

            //Cuando me hacen daño refresco la información en la interfaz
            UIM.RefreshHealth();


            //Estas líneas las añado para comprobar si el halo de la valquiria tiene que salir
            Valkyrie valkyrieRef = FindObjectOfType<Valkyrie>();
            if (valkyrieRef != null && valkyrieRef != this)
            {
                if (currentHealth <= valkyrieRef.numberCanChange)
                {
                    valkyrieRef.myHaloUnits.Add(this);
                    valkyrieRef.CheckValkyrieHalo();


                }
                else if (valkyrieRef.myHaloUnits.Contains(this))
                {
                    valkyrieRef.myHaloUnits.Remove(this);
                }

            }

            base.ReceiveDamage(damageReceived, unitAttacker);        
    }

    public override void Die()
    {
        monkMark.SetActive(false);
        monkMarkUpgrade.SetActive(false);

        if (GetComponent<Monk>())
        {
            for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
            {
                LM.charactersOnTheBoard[i].pjMonkUnitReference = null;
            }
        }

        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación de ataque
        myAnimator.SetTrigger("Death");

        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();

        LM.charactersOnTheBoard.Remove(this);
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

    public void ShowHideFullShield(bool _shouldShow)
    {
        shieldBlockAllDamage.SetActive(_shouldShow);
    }

    public void ShowHidePartialShield(bool _shouldShow)
    {
        shieldBlockPartialDamage.SetActive(_shouldShow);
    }

    public override void CalculateDamage(UnitBase unitToDealDamage)
	{
		//Reseteo la variable de daño a realizar
		damageWithMultipliersApplied = baseDamage;

		//Si estoy en desventaja de altura hago menos daño
		if (unitToDealDamage.myCurrentTile.height > myCurrentTile.height)
		{
			damageWithMultipliersApplied -= penalizatorDamageLessHeight;
            healthBar.SetActive(true);
            downToUpDamageIcon.SetActive(true);
		}

		//Si estoy en ventaja de altura hago más daño
		else if (unitToDealDamage.myCurrentTile.height < myCurrentTile.height)
		{
			damageWithMultipliersApplied += bonusDamageMoreHeight;
            healthBar.SetActive(true);
            upToDownDamageIcon.SetActive(true);
		}

		//Si le ataco por la espalda hago más daño
		if (unitToDealDamage != null && unitToDealDamage.currentFacingDirection == currentFacingDirection)
		{
            if (unitToDealDamage != null && unitToDealDamage.GetComponent<EnDuelist>()
               && unitToDealDamage.GetComponent<EnDuelist>().hasTier2
               && hasAttacked)
            {
                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.South;
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.North;
                }

                else if (currentFacingDirection == FacingDirection.East)
                {

                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.West;
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.East;
                }

            }

            else
            {
                //Añado este if para que, cada vez que ataque un jugador y si le va a realizar daño por la espalda, el count del honor se resetea
                if (hasAttacked)
                {
                    LM.honorCount = 0;
                }

                //Ataque por la espalda
                damageWithMultipliersApplied += bonusDamageBackAttack;
                healthBar.SetActive(true);
                backStabIcon.SetActive(true);
            }
		}

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null && samuraiUpgraded.itsForHonorTime2)
        {
            damageWithMultipliersApplied += LM.honorCount;

        }

        damageWithMultipliersApplied += buffbonusStateDamage;

        if (unitToDealDamage.GetComponent<MechaBoss>() && !unitToDealDamage.GetComponent<MechaBoss>().isCharging)
        {
            damageWithMultipliersApplied = 0;
        }

        Debug.Log("Daño base: " + baseDamage + " Daño con multiplicadores " + damageWithMultipliersApplied);
	}

	public override void HealthBarOn_Off(bool isOn)
	{

        if (shouldLockHealthBar && isOn)
        {
            if (healthBar != null)
            {
                healthBar.SetActive(isOn);
            }

            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(!isOn);
            }
        }
        else if (!shouldLockHealthBar)
        {
            if (healthBar != null)
            {
                healthBar.SetActive(isOn);
            }

            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(!isOn);
            }
        }
    }

    //Función que llama el LevelManager al hacer hover sobre un objetivo al que poder atacar
    public virtual void ShowAttackEffect(UnitBase _unitToAttack)
    {
        HealthBarOn_Off(true);
        //Cada personaje hace una cosa distinta

        if (pjMonkUnitReference != null)
        {
            pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, true);

            if (pjMonkUnitReference.rotatorTime)
            {
                if (pjMonkUnitReference.rotatorTime2)
                {
                    if (_unitToAttack.isMarked)
                    {
                        TM.surroundingTiles.Clear();

                        TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, 1, true, false);

                        //Marco a las unidades adyacentes si no están marcadas
                        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                        {
                            if (TM.surroundingTiles[i].unitOnTile != null)
                            {
                                if (TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>()
                                    && !TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>().isMarked)
                                {
                                    pjMonkUnitReference.PutQuitMark(TM.surroundingTiles[i].unitOnTile, this, false, true);
                                }
                            }
                        }
                    }
                }

                pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, true);
                _unitToAttack.hoverRotateIcon.SetActive(true);
            }
        }
    }

    public virtual void HideAttackEffect(UnitBase _unitToAttack)
    {
        //Cada personaje hace una cosa distinta
       
        if (pjMonkUnitReference != null)
        {
            pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, false);

            if (pjMonkUnitReference.rotatorTime)
            {
                if (pjMonkUnitReference.rotatorTime2)
                {
                    if (_unitToAttack.isMarked)
                    {
                        pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, false);

                        //COMPROBAR QUE NO DE ERROR EN OTRAS COSAS
                        TM.surroundingTiles.Clear();

                        TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, 1, true, false);

                        //Marco a las unidades adyacentes si no están marcadas
                        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                        {
                            if (TM.surroundingTiles[i].unitOnTile != null)
                            {
                                if (TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>()
                                    && !TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>().isMarked)
                                {
                                    if (!hasAttacked)
                                    {
                                        pjMonkUnitReference.PutQuitMark(TM.surroundingTiles[i].unitOnTile, this, false, false);
                                    }
                                }
                            }
                        }
                    }

                    _unitToAttack.hoverRotateIcon.SetActive(false);
                }

                else
                {
                    if (!hasAttacked)
                    {
                        pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, false);
                    }

                    _unitToAttack.hoverRotateIcon.SetActive(false);
                }
            }

            //Enemigo
            pjMonkUnitReference.PutQuitMark(_unitToAttack, this, false, false);
            _unitToAttack.ResetColor();
            _unitToAttack.DisableCanvasHover();
        }

        //Para que al quitar el hover si va a explotar una marca que se quite el número de curación
        DisableCanvasHover();
        ResetColor();
        myCurrentTile.ColorDeselect();

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
    public virtual void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
    {
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();

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
                if (!myCurrentTile.tilesInLineUp[i].isEmpty && !myCurrentTile.tilesInLineUp[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[i]);
                }

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
                if (!myCurrentTile.tilesInLineDown[i].isEmpty && !myCurrentTile.tilesInLineDown[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[i]);
                }

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
                if (!myCurrentTile.tilesInLineRight[i].isEmpty && !myCurrentTile.tilesInLineRight[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[i]);
                }

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
                if (!myCurrentTile.tilesInLineLeft[i].isEmpty && !myCurrentTile.tilesInLineLeft[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[i]);
                }

                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    break;
                }
            }	
		}

        if (_shouldPaintEnemiesAndShowHealthbar)
        {
            //Marco las unidades disponibles para atacar de color rojo
            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                CalculateDamage(currentUnitsAvailableToAttack[i]);
                currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
                currentUnitsAvailableToAttack[i].myCurrentTile.ColorInteriorRed();
            }
        }

        if (LM.selectedCharacter == this || LM.selectedCharacter == null)
        {

            for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
            {
                currentTilesInRangeForAttack[i].ColorBorderRed();
            }
        }

    }

    public void CheckIfUnitHasMarks(UnitBase _unitToCheck)
    {
        if (_unitToCheck.isMarked)
        {

            Monk monkref = FindObjectOfType<Monk>();

            if(monkref != null)
            {
                _unitToCheck.QuitMarks();
                currentHealth += monkref.healerBonus * _unitToCheck.numberOfMarks;
                RefreshHealth(false);

                _unitToCheck.numberOfMarks = 0;

                if (_unitToCheck.isMarked && monkref.debuffMark)
                {
                    ApplyBuffOrDebuffDamage(_unitToCheck, 0, 0);
                }

                if (monkref.debuffMark2)
                {
                    if (!_unitToCheck.isStunned)
                    {
                        StunUnit(_unitToCheck);
                    }
                }
                else if (monkref.healerMark2)
                {
                    ApplyBuffOrDebuffDamage(this, 1, 3);
                }

                UIM.RefreshTokens();
            }
          
        }
    }

    #endregion


    //Si estamos haciendo pruebas y no cargamos desde el nivel de mapa tiene que hacer el raycast. 
    //Si probamos desde el mapa de seleccion el tile se setea al colocar las unidades.
    protected override void FindAndSetFirstTile()
    {
        if (LM.FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa || characterStartedOnTheLevel)
        {
            base.FindAndSetFirstTile();
        }
    }

    public virtual void CheckWhatToDoWithSpecialsTokens()
    {
        //Cada character realiza su comprobación


    }

    //Crear AttackCommand para Undo
    public virtual void CreateAttackCommand(UnitBase obj)
    {
        ICommand command = new AttackCommand(obj.currentFacingDirection, currentFacingDirection,
                                      obj.myCurrentTile, myCurrentTile,
                                      obj.currentHealth, currentHealth,
                                      GetComponent<UnitBase>(), obj,
                                      currentArmor, obj.currentArmor,
                                      isStunned, obj.isStunned,
                                      isMarked, obj.isMarked, numberOfMarks, obj.numberOfMarks,
                                      hasMoved, obj.hasMoved, hasAttacked, obj.hasAttacked,
                                      buffbonusStateDamage, obj.buffbonusStateDamage,movementUds, obj.movementUds,
                                      turnsWithBuffOrDebuff, obj.turnsWithBuffOrDebuff, turnsWithMovementBuffOrDebuff, obj.turnsWithMovementBuffOrDebuff);
        CommandInvoker.AddCommand(command);
    }
    
}
