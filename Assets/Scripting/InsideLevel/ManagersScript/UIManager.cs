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
    private GameObject endTurnButton;

	[SerializeField]
	private GameObject optionsScreen;

	[SerializeField]
	private GameObject optionsButton;

    //Texto de cuadro inferior derecha (tiles)
	[SerializeField]
	private TextMeshProUGUI tooltipText;
	[SerializeField]
	private Image tooltipImage;

    //Texto de cartel superior con las acciones
	[SerializeField]
	private TextMeshProUGUI tooltipAccionesText;

    //Cuadro inferior izquierda. Referencia para animaciones
	[SerializeField]
	private GameObject characterInfo;
    //Texto cuadro inferior izquierda
	[SerializeField]
	private TextMeshProUGUI characterInfoText;
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
	
    //Cursores
	[SerializeField]
	public Texture2D attackCursor, movementCursor;

    //Imagen de cada player explicando sus acciones
	[SerializeField]
	public Image playersImageExplanation;

	[SerializeField]
	public Image imageCharacterInfo;
  

    [Header("REFERENCIAS")]

    //Level Manager
	[HideInInspector]
    public LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
    }
	private void Start()
	{
		characterInfoOriginalPosition = characterInfo.transform.position;

		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
            //Activamos los retratos necesarios y les asignamos su jugador
            panelesPJ[i].SetActive(true);
            panelesPJ[i].GetComponent<Portraits>().assignedPlayer = LM.characthersOnTheBoard[i];
            LM.characthersOnTheBoard[i].myPanelPortrait = panelesPJ[i];
            //Hacer que el player sepa cuál es su retrato?

            //Actualizamos las barras de vida
            panelesPJ[i].GetComponent<Portraits>().RefreshHealth();
            panelesPJ[i].GetComponent<Portraits>().RefreshTokens();
        }

		tooltipAccionesText.text = "Selecciona una unidad";
	}

    #endregion

    #region END_TURN

    //Se llama desde el botón de finalizar turno
    public void EndTurn()
    {
        //He cambiado esta parte para que el end turn también borre los tiles pintados
        if (LM.selectedCharacter == null)
        {
            Debug.Log("Soy EndTurn");
            RotateButtonEndPhase();
            LM.ChangePhase();
            if (LM.selectedEnemy != null)
            {
                LM.DeselectEnemy();
            }
        }
        else if ( !LM.selectedCharacter.isMovingorRotating)
        {
            Debug.Log("Soy EndTurn");
            RotateButtonEndPhase();
            LM.ChangePhase();

            if (LM.selectedEnemy != null)
            {
                LM.DeselectEnemy();
            }

            LM.DeSelectUnit();
        }
        
    }

    //Función que activa o desactiva el botón de pasar turno en función de si es la fase del player o del enemigo
    public void ActivateDeActivateEndButton()
    {
        //endTurnButton.GetComponent<BoxCollider>().enabled = !endTurnButton.GetComponent<BoxCollider>().enabled;
        endTurnButton.GetComponent<BoxCollider>().enabled=true;
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
		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
			if(LM.characthersOnTheBoard[i].hasAttacked == false)
			{
				LM.characthersOnTheBoard[i].actionAvaliablePanel.SetActive(true);
			}
		}
	}

	public void DeactivateEndTurnHover()
	{
		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
			LM.characthersOnTheBoard[i].actionAvaliablePanel.SetActive(false);
		}
	}

	#endregion

	#region UNDO_MOVE

	////Se llama desde el botón de finalizar turno
	//public void UndoMove()
	//{
	//    LM.UndoMove();
	//}

	#endregion

	#region ROTATION_ARROWS

	[SerializeField]
    public void RotatePlayerInNewDirection(UnitBase.FacingDirection newDirection)
    {
        LM.selectedCharacter.RotateUnitFromButton(newDirection);
    }


	#endregion

	#region RETRATOS
	//Avisa a los retratos activos de que refresquen las barras de vida
	public void RefreshHealth()
	{
		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
            panelesPJ[i].GetComponent<Portraits>().RefreshHealth();
		}
	}
    //Avisa a los retratos activos de que refresquen los tokens
    public void RefreshTokens()
	{
        for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
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

    #region CHARACTER_INFO

	public void ShowCharacterImage(UnitBase characterImage)
	{
		if(characterImage.characterImage != null)
		{
			imageCharacterInfo.gameObject.SetActive(true);
			imageCharacterInfo.sprite = characterImage.characterImage;
		}
	}

	public void HideCharacterImage()
	{
		imageCharacterInfo.gameObject.SetActive(false);
		imageCharacterInfo.sprite = null;
	}

    public void ShowCharacterInfo(string textToPrint, UnitBase unitTooltipImage)
    {
        characterInfo.transform.DOMove(characterInfo.transform.parent.position, animationDuration);
        characterInfoText.text = textToPrint;
        if (unitTooltipImage.tooltipImage !=null)
        {
            playersImageExplanation.sprite = unitTooltipImage.tooltipImage;
        }
    }

    public void HideCharacterInfo(string textToPrint)
	{
		characterInfo.transform.DOMove(characterInfoOriginalPosition, animationDuration);
		characterInfoText.text = textToPrint;
		playersImageExplanation.sprite = null;
	}
 
	#endregion

	#region TOOLTIP

	public void ShowTileInfo(string textToPrint, Sprite tileImageToShow)
	{
		tooltipText.text = textToPrint;
		tooltipImage.gameObject.SetActive(true);
		tooltipImage.sprite = tileImageToShow;
	}

	public void HideTileInfo()
	{
		tooltipText.text = "";
		tooltipImage.gameObject.SetActive(false);
		tooltipImage.sprite = null;
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
                panelesEnemigos.Add(enemyPanel);

                //IMPORTANTE. El contador de paneles enemigos no puede ser i ya que puede ser que haya un enemigo muerto y por tanto i sea demasiado grande.
                panelesEnemigos[panelesEnemigos.Count-1].GetComponent<EnemyPortraits>().assignedEnemy = LM.enemiesOnTheBoard[i];
                panelesEnemigos[panelesEnemigos.Count-1].GetComponent<EnemyPortraits>().enemyPortraitSprite = LM.enemiesOnTheBoard[i].characterImage;

                //LM.enemiesOnTheBoard[i].orderToShow = i + 1;
                //LM.enemiesOnTheBoard[i].thisUnitOrder.GetComponent <TextMeshPro>().text = "" + LM.enemiesOnTheBoard[i].orderToShow;
                //LM.enemiesOnTheBoard[i].GetComponent<PlayerHealthBar>().ReloadHealth();
            }
        }
    }

    public void ShowEnemyOrder(bool show_hide)
    {
        for (int i = 0; i < LM.enemiesOnTheBoard.Count; i++)
        {
            //LM.enemiesOnTheBoard[i].thisUnitOrder.SetActive(show_hide);
            LM.enemiesOnTheBoard[i].HealthBarOn_Off(show_hide);
        }
    }

	#endregion

	#region TOOLTIP ACCTIONS

	public void TooltipMove()
	{
		tooltipAccionesText.text = "Mueve la unidad";
	}
	public void TooltipAttack()
	{
		tooltipAccionesText.text = "Ataca a una unidad";
	}
	public void TooltipNoAttackable()
	{
		tooltipAccionesText.text = "Esta unidad no tiene ningún enemigo a rango";
	}
	public void TooltipRotate()
	{
		tooltipAccionesText.text = "Selecciona la rotación de la unidad";
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
	}
	#endregion



}
