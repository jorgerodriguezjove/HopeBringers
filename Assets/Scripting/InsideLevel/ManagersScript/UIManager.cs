using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region VARIABLES

	[Header("HUD")]

    [SerializeField]
    GameObject endTurnButton;
	[HideInInspector]
	public Material endTurnBttnInitMaterial;
	[SerializeField]
	Material noMoreActionMaterial;
	[SerializeField]
	GameObject noMoreActionsPanel;

    //Botón para acabar de colocar unidades y empezar el juego.
    [SerializeField]
    public GameObject finishUnitPlacement;

	//Este int lo uso para saber cuantos personajes han acabado sus acciones. Al acabar todos, el panel ya tendría que salir.
	int numberOfCharactersFinished;

	[SerializeField]
	private GameObject optionsScreen;

	[SerializeField]
	public GameObject optionsButton;

    //Texto de cartel superior con las acciones
	[SerializeField]
	private TextMeshProUGUI tooltipAccionesText;

    //Duración de la animación
	[SerializeField]
	private float animationDuration;
	[SerializeField]
	private float durationEndTurnRotation;
    //Posición original del cuadro para la animación.
	private Vector3 characterInfoOriginalPosition;

    //Lista que guarda los paneles en la parte superior izquierda con la info de los players
	[SerializeField]
	public List<GameObject> panelesPJ;
	//Padre de los paneles de los enemigos
	[SerializeField]
	public GameObject padrePanelesEnemigos;
	//Prefab de los paneles enemigos para instanciarlo
	[SerializeField]
	public GameObject panelesEnemigosPrefab;
	//Lista que guarda los paneles enemigos para la lista de orden de enemigos
	[HideInInspector]
	public List<GameObject> panelesEnemigos;

    [Header("EXTRAS")]
    //Están solo de placeholder, en el futuro quizás tienen que ser imagenes y tendrán animación.
    [SerializeField]
    GameObject enemyBanner;
    [SerializeField]
    GameObject playerbanner;
    [SerializeField]
    public float bannerTime;

    [Header("TOOLTIPS")]

	[SerializeField]
	public GameObject tooltipPanel;
	[SerializeField]
	public TextMeshProUGUI attackInfoTextInTooltip;
    [SerializeField]
    public TextMeshProUGUI pasiveInfoTextInTooltip;

    [SerializeField]
	public Image activeIconTooltip;

    [SerializeField]
    public Image pasiveIconTooltip;

    [Header("CURSORES")]
	[SerializeField]
	public Texture2D attackCursor;
	public Texture2D movementCursor;

    [Header("SCROLL")]
    //Bools que indican si se estan pulsando los botones
    private bool isScrollButtonDownBeingPressed;
    private bool isScrollButtonUpBeingPressed;
	private bool scrollUpToEnemy, scrollDownToEnemy;

	//Bool para saber si la unidad tiene alguna unidad a rango o no
	
	private bool hasCharacterUnitInRange;

    //Velocidad de scroll
    [SerializeField]
    private float scrollSpeed;
	[SerializeField]
	private float autoScrollSpeed;
    //Separación entre retratos de personajes (el 69 no va en coña, de verdad que está bien separado así)
    [SerializeField]
    private int enemyPortraitSeparation;

    //Posición inicial a la que vuelve la barra cuando se acaba el turno enemigo
    private Vector3 initialScrollPosition;

    //Topes que no puede superar la barra
    [SerializeField]
    private GameObject topScrollUp, topScrollDown, buttonUp, buttonDown, buttonUpHighlight, buttonDownHighlight;

    [Header("TURN")]
    [SerializeField]
    TextMeshProUGUI currentTurn;
    [SerializeField]
    TextMeshProUGUI turnLimit;

    [Header("UNIT PLACEMENT")]
    [SerializeField]
    GameObject unitsToPlaceParent;
    [SerializeField]
    TextMeshProUGUI currentUnitsPlaced;
    [SerializeField]
    TextMeshProUGUI maxUnitsToPlace;

    [Header("TUTORIAL")]
	int turnNumber = 1;
	[SerializeField]
	bool tutorialLevel;
	[SerializeField]
	GameObject panelTutorial;
	[SerializeField]
	TextMeshProUGUI textTutorial;
	[SerializeField]
	[@TextAreaAttribute(5, 10)]
	string tutorialText1;
	[SerializeField]
	[@TextAreaAttribute(5, 10)]
	string tutorialText2;
	[SerializeField]
	[@TextAreaAttribute(5, 10)]
	string tutorialText3;

    [Header("VICTORIA")]
    [SerializeField]
    GameObject victoryPanel;

    [SerializeField]
    TextMeshProUGUI baseXp;
    [SerializeField]
    TextMeshProUGUI bonusXpTotal;
    [SerializeField]
    TextMeshProUGUI turnsLeftXp;
    [SerializeField]
    TextMeshProUGUI charactersAliveXp;
    [SerializeField]
    TextMeshProUGUI totalXp;

    [Header("BOSS")]
    [SerializeField]
    PortraitBoss bossPortrait;

    [Header("REFERENCIAS")]

    [SerializeField]
    GameObject hudParentObject;
    [SerializeField]
    GameObject hud3DInGame;
    [SerializeField]
    GameObject hud3DInDialog;
    [SerializeField]
    GameObject hud3DUnitPlacement;
    [SerializeField]
    GameObject hideDuringUnitPlacementHud;
    [SerializeField]
    GameObject hideDuringUnitPlacement3DHud;

    //Level Manager
    [HideInInspector]
    public LevelManager LM;

	[Header("TUTORIAL")]

	bool firstTimeAction, secondTimeAction, thirdTimeAction, fourthTimeAction, secondUnit; 


    #endregion

    #region INIT

    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
	}

    public void ActivateHudUnitPlacement()
    {
        hud3DInDialog.SetActive(false);
        hud3DUnitPlacement.SetActive(true);
        hideDuringUnitPlacementHud.SetActive(false);
        hudParentObject.SetActive(true);
        hideDuringUnitPlacement3DHud.SetActive(false);

        unitsToPlaceParent.SetActive(true);
        maxUnitsToPlace.SetText(GameManager.Instance.maxUnitsInThisLevel.ToString());
    }

    public void ActivateDialogHud(bool _shouldShow)
    {
        hud3DInDialog.SetActive(_shouldShow);
        hideDuringUnitPlacementHud.SetActive(!_shouldShow);
        hudParentObject.SetActive(!_shouldShow);
        hideDuringUnitPlacement3DHud.SetActive(_shouldShow);
    }

    public void UpdateUnitsPlaced(int _currentUnitsPlaced)
    {
        currentUnitsPlaced.SetText(_currentUnitsPlaced.ToString());
    }

    //Lo pongo en una variable en vez de en el start para que lo pueda llamar el Level Manager
    public void InitializeUI()
    {
        hudParentObject.SetActive(true);
        hud3DInGame.SetActive(true);
        hideDuringUnitPlacementHud.SetActive(true);
        hideDuringUnitPlacement3DHud.SetActive(true);

        hud3DInDialog.SetActive(false);
        hud3DUnitPlacement.SetActive(false);

        unitsToPlaceParent.SetActive(false);

		endTurnBttnInitMaterial = endTurnButton.GetComponent<MeshRenderer>().material;
        //Guardo la posición inicial de la lista para poder volver a ponerla en esta posición al terminar el turno enemigo.
        initialScrollPosition = padrePanelesEnemigos.transform.position;

		if (tutorialLevel)
		{
            if (panelTutorial != null)
            {
                panelTutorial.SetActive(true);
                textTutorial.text = tutorialText1;
                GameManager.Instance.isGamePaused = true;
            }
		}

		for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
        {
            //Activamos los retratos necesarios y les asignamos su jugador
            panelesPJ[i].SetActive(true);
            panelesPJ[i].GetComponent<Portraits>().assignedPlayer = LM.charactersOnTheBoard[i];
			panelesPJ[i].GetComponent<Tooltips>().tooltipAssignedPlayer = LM.charactersOnTheBoard[i];
            LM.charactersOnTheBoard[i].myPanelPortrait = panelesPJ[i];

            //Actualizamos las barras de vida
            panelesPJ[i].GetComponent<Portraits>().InitializeHealth();
            panelesPJ[i].GetComponent<Portraits>().RefreshTokens();

			//Línea para comprobar lo que tiene que hacer el special token en cada unidad
			panelesPJ[i].GetComponent<Portraits>().assignedPlayer.CheckWhatToDoWithSpecialsTokens();
		}

        if (FindObjectOfType<MechaBoss>() || FindObjectOfType<DarkLord>() ||FindObjectOfType<BossMultTile>())
        {
            bossPortrait.gameObject.SetActive(true);
        }

        else
        {
            Debug.Log("no boss");
        }

        tooltipAccionesText.text = "Selecciona una unidad";

    }

    public void HideGameHud()
    {
        Debug.Log("HideGameHud en ui manager no hace nada");
        //hudParentObject.SetActive(false);
        //hud3DInGame.SetActive(false);
        //hideDuringUnitPlacementHud.SetActive(false);
        //hideDuringUnitPlacement3DHud.SetActive(false);
        //optionsButton.SetActive(false);
    }

    #endregion

    #region END_TURN

    //Se llama desde el botón de finalizar turno
    public void EndTurn()
    {
        //He cambiado esta parte para que el end turn también borre los tiles pintados
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {

            if (LM.CheckIfFinishingTilesReached())
            {
                LM.InstaWin(false);
            }

            if (LM.selectedCharacter == null)
            {
                if (LM.selectedEnemy != null)
                {
                    LM.DeselectEnemy();
                }
				if (tutorialLevel)
				{
					turnNumber++;
				}
				if(turnNumber == 2)
				{
					textTutorial.text = tutorialText2;
				}
				else if(turnNumber == 3)
				{
					textTutorial.text = tutorialText3;
				}
				else
				{
                    if (panelTutorial != null)
                    {
                        panelTutorial.SetActive(false);
                    }
                }
                RotateButtonEndPhase();
                LM.ChangePhase();
            }
            else if (!LM.selectedCharacter.isMovingorRotating)
            {
                if (LM.selectedEnemy != null)
                {
                    LM.DeselectEnemy();
                }

                LM.DeSelectUnit();

                RotateButtonEndPhase();
                LM.ChangePhase();
            }
        }
    }

    //Función que activa o desactiva el botón de pasar turno en función de si es la fase del player o del enemigo
    public void ActivateDeActivateEndButton()
    {
        //endTurnButton.GetComponent<BoxCollider>().enabled = !endTurnButton.GetComponent<BoxCollider>().enabled;
        endTurnButton.GetComponent<MeshCollider>().enabled=true;
    }

	public void RotateButtonEndPhase()
	{
		//endTurnButton.transform.DORotate(new Vector3(-180, 0, 0), durationEndTurnRotation);
		//StartCoroutine("ButtonAnimationWaitEnd");
		endTurnButton.GetComponent<Animator>().Play("EndTurnFlip");
	}

	public void RotateButtonStartPhase()
	{
		//waitingButton.transform.DORotate(new Vector3(-180, 0, 0), durationEndTurnRotation);
		//StartCoroutine("ButtonAnimationWaitStart");
		endTurnButton.GetComponent<Animator>().Play("ResetEndTurnFlip");
	}

	IEnumerator ButtonAnimationWaitEnd()
	{
		yield return new WaitForSeconds(durationEndTurnRotation);
		endTurnButton.gameObject.SetActive(false);
		endTurnButton.transform.DORotate(new Vector3(0, 0, 0), durationEndTurnRotation);
		//waitingButton.gameObject.SetActive(true);
	}
	IEnumerator ButtonAnimationWaitStart()
	{
		yield return new WaitForSeconds(durationEndTurnRotation);
		//waitingButton.gameObject.SetActive(false);
		//waitingButton.transform.DORotate(new Vector3(0, 0, 0), durationEndTurnRotation);
		endTurnButton.gameObject.SetActive(true);
	}

	public void ActivateEndTurnHover()
	{
		for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
		{
			if(LM.charactersOnTheBoard[i].hasMoved == false)
			{
				LM.charactersOnTheBoard[i].actionAvaliablePanel.SetActive(true);
			}
			else if (LM.charactersOnTheBoard[i].hasAttacked == false && hasCharacterUnitInRange)
			{
				LM.charactersOnTheBoard[i].actionAvaliablePanel.SetActive(true);
			}
		}
	}

	public void DeactivateEndTurnHover()
	{
		for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
		{
			LM.charactersOnTheBoard[i].actionAvaliablePanel.SetActive(false);
		}
	}

	public void CheckActionsAvaliable()
	{
        numberOfCharactersFinished = 0;

        for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
		{
            LM.charactersOnTheBoard[i].CheckUnitsAndTilesInRangeToAttack(false);

            if (LM.charactersOnTheBoard[i].GetComponent<MageDecoy>())
            {
                numberOfCharactersFinished++;   
            }

            else
            {
                if (LM.charactersOnTheBoard[i].hasAttacked)
                {
                    numberOfCharactersFinished++;
                }

                else if (LM.charactersOnTheBoard[i].hasMoved && LM.charactersOnTheBoard[i].hasAttacked == false
                    && LM.charactersOnTheBoard[i].currentUnitsAvailableToAttack.Count == 0)
                {
                    numberOfCharactersFinished++;
                }

                else
                {
                    Debug.Log(LM.charactersOnTheBoard[i] + " Ha pasado ");
                }
            }	
		}

        if (numberOfCharactersFinished == LM.charactersOnTheBoard.Count)
		{
			endTurnButton.GetComponent<MeshRenderer>().material = noMoreActionMaterial;
			noMoreActionsPanel.SetActive(true);
			UndoTooltip();
        }
        else
        {
            ResetActionsAvaliable();
        }
      
      
	}
	public void ResetActionsAvaliable()
	{
		endTurnButton.GetComponent<MeshRenderer>().material = endTurnBttnInitMaterial;
		noMoreActionsPanel.SetActive(false);
	}

    #endregion

    #region FAST_FORWARD

    bool isGameAccelerated;
    [SerializeField]
    GameObject fastForwardButton;
    [SerializeField]
    TextMeshProUGUI changeSpeedText;
    [SerializeField]
    GameObject undoButton;

    //Hago aparecer o desaparecer, el botón de undo, fast forward...
    public void HideShowEnemyUi(bool _shouldShow)
    {
        //fastForwardButton.SetActive(_shouldShow);

        //undoButton.SetActive(!_shouldShow);
    }

    #endregion

    #region ROTATION_ARROWS

   

    [SerializeField]
    public void RotatePlayerInNewDirection(UnitBase.FacingDirection newDirection)
    {
        UnitBase currentPlayer = LM.selectedCharacter;

        ICommand command = new MoveCommand(newDirection, currentPlayer.currentFacingDirection,
                                           currentPlayer.myCurrentTile, LM.tileToMoveAfterRotate,
                                           LM.TM.currentPath, currentPlayer,
                                           currentPlayer.buffbonusStateDamage, currentPlayer.turnsWithBuffOrDebuff, currentPlayer.movementUds, currentPlayer.turnsWithMovementBuffOrDebuff);
        CommandInvoker.AddCommand(command);
    }

	#endregion

	#region RETRATOS
	//Avisa a los retratos activos de que refresquen las barras de vida
	public void RefreshHealth()
	{
		for (int i = 0; i < panelesPJ.Count; i++)
		{
            panelesPJ[i].GetComponent<Portraits>().RefreshHealth();
		}
	}
    //Avisa a los retratos activos de que refresquen los tokens
    public void RefreshTokens()
	{
        for (int i = 0; i < panelesPJ.Count; i++)
        {
            panelesPJ[i].GetComponent<Portraits>().RefreshTokens();
        }
	}

	public void PortraitCharacterSelect(PlayerUnit characterToSelect)
	{
		LM.SelectUnit(characterToSelect.movementUds, characterToSelect);
	}
	
	public void PortraitEnemySelect(EnemyUnit enemyToSelect)
	{
		LM.selectedEnemy = enemyToSelect;
	}

	public void HighlightCharacter(PlayerUnit characterToHighlight)
	{
		characterToHighlight.SelectedColor();
	}

	public void UnHighlightCharacter(PlayerUnit characterToUnhighlight)
	{
        if (LM.selectedCharacter == null)
        {
        
            characterToUnhighlight.ResetColor();
        }
	}
    #endregion

    #region ENEMY_INFO

    public void SetEnemyOrder()
    {
		for (int i = 0; i < panelesEnemigos.Count; i++)
		{
			Destroy(panelesEnemigos[i].gameObject);
		}

		panelesEnemigos.Clear();

        for (int i = 0; i < LM.enemiesOnTheBoard.Count; i++)
        {
            if (!LM.enemiesOnTheBoard[i].isDead)
            {
                GameObject enemyPanel = Instantiate(panelesEnemigosPrefab, padrePanelesEnemigos.transform, false);
                //enemyPanel.transform.position = new Vector3(enemyPanel.transform.position.x, enemyPanel.transform.position.y - i* enemyPortraitSeparation, enemyPanel.transform.position.z);
                panelesEnemigos.Add(enemyPanel);

                //IMPORTANTE. El contador de paneles enemigos no puede ser i ya que puede ser que haya un enemigo muerto y por tanto i sea demasiado grande.
                panelesEnemigos[panelesEnemigos.Count-1].GetComponent<EnemyPortraits>().assignedEnemy = LM.enemiesOnTheBoard[i];
				panelesEnemigos[panelesEnemigos.Count-1].GetComponent<EnemyPortraits>().enemyPortraitSprite = LM.enemiesOnTheBoard[i].characterImage;

                //Número y estado dormido/despierto
                panelesEnemigos[panelesEnemigos.Count - 1].GetComponent<EnemyPortraits>().UpdateOrder(i+1, LM.enemiesOnTheBoard[i].haveIBeenAlerted, LM.enemiesOnTheBoard[i].isGoingToBeAlertedOnEnemyTurn);

                //Asigo el tooltip
                panelesEnemigos[panelesEnemigos.Count - 1].GetComponent<EnemyTooltip>().tooltipAssignedEnemy = LM.enemiesOnTheBoard[i];
                
				LM.enemiesOnTheBoard[i].GetComponent<EnemyUnit>().myPortrait = panelesEnemigos[panelesEnemigos.Count - 1].GetComponent<EnemyPortraits>();
            }
        }

        LM.CheckIfGameOver();
    }

    public void ShowEnemyOrder(bool show_hide)
    {
        for (int i = 0; i < LM.enemiesOnTheBoard.Count; i++)
        {
            //LM.enemiesOnTheBoard[i].thisUnitOrder.SetActive(show_hide);
            LM.enemiesOnTheBoard[i].HealthBarOn_Off(show_hide);
        }
    }

    //Scrolear la barra de lista hacia arriba
    public void ScrollUp()
    {
        isScrollButtonUpBeingPressed = true;
    }

    //Scrolear la barra de lista hacia abajo
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Literalmente mueve la lista hacia abajo y en los botones está invertido!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void ScrollDown()
    {
        isScrollButtonDownBeingPressed = true;
    }

    //Parar de scrollear la barra al soltar el botón
    public void StopScroll()
    {
        isScrollButtonDownBeingPressed = false;
        isScrollButtonUpBeingPressed = false;
    }

    //Función que hace subir a la lista cada vez que se pasa de turno enemigo
    public void ScrollUpOnce()
    {
        if (panelesEnemigos[panelesEnemigos.Count - 1].transform.position.y <= topScrollDown.transform.position.y)
        {
            padrePanelesEnemigos.transform.position = new Vector3(padrePanelesEnemigos.transform.position.x, padrePanelesEnemigos.transform.position.y + enemyPortraitSeparation, padrePanelesEnemigos.transform.position.z);
        }
    }

    //Vuelvo a poner la lista arriba del todo
    public void ResetScrollPosition()
    {
        padrePanelesEnemigos.transform.position = initialScrollPosition;
    }

    //Función que se 
    public void MoveScrollToEnemy(EnemyUnit selectedEnemy)
    {
        if (selectedEnemy != null && selectedEnemy.myPortrait != null && !selectedEnemy.GetComponent<Crystal>())
        {
            if (selectedEnemy.myPortrait != null && selectedEnemy.myPortrait.transform.position.y >= topScrollUp.transform.position.y)
            {
                scrollDownToEnemy = true;
            }

            else if (selectedEnemy.myPortrait != null && selectedEnemy.myPortrait.transform.position.y <= topScrollUp.transform.position.y)
            {
                scrollUpToEnemy = true;
            }

            //Comprobar si el retrato enemigo esta entre el tope inferior y superior
            //Comprobar si esta por encima de ambos topes o por debajo de ambos topes para decidir si subo o bajo la lista
            //Contar el número de retratos que hay hasta que haya uno dentro del cuadro para saber cuánta distancia tengo que bajar o subir la barra.
        }
    }

    private void Update()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (isScrollButtonDownBeingPressed)
            {
                if (panelesEnemigos.Count > 0 && panelesEnemigos[0] != null && panelesEnemigos[0].transform.position.y >= topScrollUp.transform.position.y)
                {
                    padrePanelesEnemigos.transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
					buttonDownHighlight.SetActive(true);
					//buttonUp.gameObject.transform.DORotate(buttonUp.gameObject.transform.rotation.eulerAngles + new Vector3(0, 0, 10), 0.2f);
					//buttonDown.gameObject.transform.DORotate(buttonDown.gameObject.transform.rotation.eulerAngles + new Vector3(0, 0, 10), 0.2f);
				}
            }
			else
			{
				buttonDownHighlight.SetActive(false);
			}

            if (isScrollButtonUpBeingPressed)
            {
                if (panelesEnemigos.Count > 1 && panelesEnemigos[panelesEnemigos.Count - 1] && panelesEnemigos[panelesEnemigos.Count - 1].transform.position.y <= topScrollDown.transform.position.y)
                {
                    padrePanelesEnemigos.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
					buttonUpHighlight.SetActive(true);
					//buttonUp.gameObject.transform.DORotate(buttonUp.gameObject.transform.rotation.eulerAngles + new Vector3(0, 0, -10), 0.2f);
					//buttonDown.gameObject.transform.DORotate(buttonDown.gameObject.transform.rotation.eulerAngles + new Vector3(0, 0, -10), 0.2f);
				}
            }

			else
			{
				buttonUpHighlight.SetActive(false);
			}

			if(scrollDownToEnemy && LM.selectedEnemy != null)
			{
				if (panelesEnemigos.Count > 0 && panelesEnemigos[0] != null && panelesEnemigos[0].transform.position.y >= topScrollUp.transform.position.y &&
                    LM.selectedEnemy != null && !LM.selectedEnemy.isDead && LM.selectedEnemy.myPortrait != null && 
                    LM.selectedEnemy.myPortrait.transform.position.y >= topScrollUp.transform.position.y)
				{
					padrePanelesEnemigos.transform.Translate(Vector3.down * autoScrollSpeed * Time.deltaTime);
					buttonDownHighlight.SetActive(true);
				}

				else
				{
					scrollDownToEnemy = false;
				}
			}

			if (scrollUpToEnemy && LM.selectedEnemy != null)
			{
				if (panelesEnemigos.Count > 1 &&  panelesEnemigos[panelesEnemigos.Count - 1] != null && panelesEnemigos[panelesEnemigos.Count - 1].transform.position.y <= topScrollDown.transform.position.y &&
                    LM.selectedEnemy != null && !LM.selectedEnemy.isDead && LM.selectedEnemy.myPortrait != null && LM.selectedEnemy.myPortrait.transform.position.y <= topScrollUp.transform.position.y)
				{
					padrePanelesEnemigos.transform.Translate(Vector3.up * autoScrollSpeed * Time.deltaTime);
					buttonUpHighlight.SetActive(true);
				}

				else
				{
					scrollUpToEnemy = false;
				}
			}
		}

        if (skipButtonHolding)
        {
            skipTimer += Time.deltaTime;

            if (skipTimer > timeToSkipNextEnemy)
            {
                LM.SkipEnemyAnimation();
                skipTimer = 0;
            }
        }
    }

    #endregion

    #region TOOLTIP ACCTIONS

    public void TooltipMove()
	{
		tooltipAccionesText.text = "Mueve la unidad";
		if (LM.tutorialLevel1 && !firstTimeAction)
		{
			LM.tutorialGameObject.SetActive(true);
			firstTimeAction = true;
		}
	}
	public void TooltipAttack()
	{
		tooltipAccionesText.text = "Ataca a una unidad";
		hasCharacterUnitInRange = true;
		if (LM.tutorialLevel1 && !thirdTimeAction)
		{
			LM.tutorialGameObject.SetActive(true);
			thirdTimeAction = true;
			secondUnit = true;
			return;
		}
		if (LM.tutorialLevel1 && !fourthTimeAction && secondUnit)
		{
			LM.tutorialGameObject.SetActive(true);
			fourthTimeAction = true;
		}
	}
	public void TooltipNoAttackable()
	{
		tooltipAccionesText.text = "Esta unidad no tiene ningún enemigo a rango";
		hasCharacterUnitInRange = false;
		CheckActionsAvaliable();
		if (LM.tutorialLevel1 && !thirdTimeAction)
		{
			LM.tutorialGameObject.SetActive(true);
			thirdTimeAction = true;
			secondUnit = true;
			return;
		}
		if (LM.tutorialLevel1 && !fourthTimeAction && secondUnit)
		{
			LM.tutorialGameObject.SetActive(true);
			fourthTimeAction = true;
		}
	}
	public void TooltipRotate()
	{
		tooltipAccionesText.text = "Selecciona la rotación de la unidad";
		//if(LM.tutorialLevel1 && !secondTimeAction)
		//{
		//	LM.tutorialGameObject.SetActive(true);
		//	secondTimeAction = true;
		//}

	}
	public void UndoTooltip()
	{
		if(LM.tutorialLevel2 && !secondTimeAction)
		{
			LM.tutorialGameObject.SetActive(true);
			secondTimeAction = true;
		}
	}
	public void TooltipDefault()
	{
		tooltipAccionesText.text = "Selecciona una unidad";
	}
	public void TooltipMoveorAttack()
	{
		tooltipAccionesText.text = "Mueve la unidad o ataca a una unidad";
	}

	#endregion

	#region OPTIONS
	//Abre/cierra el panel de opciones y Desactiva/Activa el botón de opciones, respectivamente
	public void Activate_DeactivateOptions(bool isActivated)
	{
		optionsScreen.SetActive(isActivated);
		optionsButton.SetActive(!isActivated);
        GameManager.Instance.isGamePaused = isActivated;
	}
    #endregion

    #region SKIP

    bool skipButtonHolding = false;

    float skipTimer;
    [SerializeField]
    float timeToSkipNextEnemy;
    
   
    public void ClickDownSkipButton()
    {
        Debug.Log("click");
        LM.SkipEnemyAnimation();
        skipButtonHolding = true;
    }

    public void ClickUpSkipButton()
    {
        Debug.Log("suelto");
        skipButtonHolding = false;
        skipTimer = 0;
    }

    #endregion

    #region CHANGE_PHASE_BANNER

    public void PlayerTurnBanner(bool _isActivate)
    {
        playerbanner.SetActive(_isActivate);
    }

    public void EnemyTurnBanner(bool _isActivate)
    {
        enemyBanner.SetActive(_isActivate);
    }
    #endregion

    #region VICTORY

    public void Victory(int _baseXp, int _charactersAlifeXp, int _turnsLeftXp)
    {
        victoryPanel.SetActive(true);
        baseXp.SetText(_baseXp.ToString());
        charactersAliveXp.SetText(_charactersAlifeXp.ToString());
        turnsLeftXp.SetText(_turnsLeftXp.ToString());

        int bonus = _charactersAlifeXp + _turnsLeftXp;

        bonusXpTotal.SetText(bonus.ToString());

        int total = bonus + _baseXp;

        totalXp.SetText(total.ToString());
    }

    public void BackToLevelSelectionButton()
    {
        LM.UnPauseGame();

        //ESTE IF NO PUEDE IR DENTRO DEL EXIT LEVEL(Ahora mismo esta puesto así)
        if (GameManager.Instance.isInterlude)
        {
            GameManager.Instance.CheckEndLevel(GameManager.Instance.interludeSceneName);
        }

        else
        {
            GameManager.Instance.CheckEndLevel(AppScenes.MAP_SCENE);
        }
    }

    public void BackToLevelSelectionButtonLosingGame()
    {
        //Lo pongo en null para que no desbloquee el personaje
        GameManager.Instance.newCharacterToUnlock = null;
        GameManager.Instance.CheckEndLevel(AppScenes.MAP_SCENE);
    }

    #endregion

    public void UpdateTurnNumber(int _currentTurn, int _turnLimit)
    {
        currentTurn.SetText(_currentTurn.ToString());
        turnLimit.SetText(_turnLimit.ToString());
    }
}
