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

    [Header("REFERENCIAS")]

    //Level Manager
    private LevelManager LM;


	[Header("HUD")]

    [SerializeField]
    private Button endTurnButton;
	[SerializeField]
	private Button waitingButton;

	[SerializeField]
	private GameObject optionsScreen;

	[SerializeField]
	private GameObject optionsButton;

    //AQUI
   


	[SerializeField]
	private TextMeshProUGUI tooltipText;

	[SerializeField]
	private GameObject characterInfo;
	[SerializeField]
	private TextMeshProUGUI characterInfoText;
	[SerializeField]
	private float animationDuration;
	[SerializeField]
	private float durationEndTurnRotation;

	private Vector3 characterInfoOriginalPosition;

	[SerializeField]
	public List<GameObject> panelesPJ;
	

	[SerializeField]
	public Texture2D attackCursor, movementCursor;


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
	}

	#endregion

	#region END_TURN

	//Se llama desde el botón de finalizar turno
	public void EndTurn()
    {
		RotateButtonEndPhase();
        LM.ChangePhase();
    }

    //Función que activa o desactiva el botón de pasar turno en función de si es la fase del player o del enemigo
    public void ActivateDeActivateEndButton()
    {
        endTurnButton.interactable = !endTurnButton.interactable;
    }

	public void RotateButtonEndPhase()
	{
		endTurnButton.transform.DORotate(new Vector3(-180, 0, 0), durationEndTurnRotation);
		StartCoroutine("ButtonAnimationWaitEnd");	
	}

	public void RotateButtonStartPhase()
	{
		waitingButton.transform.DORotate(new Vector3(-180, 0, 0), durationEndTurnRotation);
		StartCoroutine("ButtonAnimationWaitStart");	
	}

	IEnumerator ButtonAnimationWaitEnd()
	{
		yield return new WaitForSeconds(durationEndTurnRotation);
		endTurnButton.gameObject.SetActive(false);
		endTurnButton.transform.DORotate(new Vector3(0, 0, 0), durationEndTurnRotation);
		waitingButton.gameObject.SetActive(true);
	}
	IEnumerator ButtonAnimationWaitStart()
	{
		yield return new WaitForSeconds(durationEndTurnRotation);
		waitingButton.gameObject.SetActive(false);
		waitingButton.transform.DORotate(new Vector3(0, 0, 0), durationEndTurnRotation);
		endTurnButton.gameObject.SetActive(true);
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

	public void HighlightCharacter(PlayerUnit characterToHighlight)
	{
		characterToHighlight.SelectedColor();
	}
	public void UnHighlightCharacter(PlayerUnit characterToUnhighlight)
	{
		characterToUnhighlight.ResetColor();
	}
	#endregion

	#region CHARACTER_INFO

	public void ShowCharacterInfo(string textToPrint)
	{
		characterInfo.transform.DOMove(characterInfo.transform.parent.position, animationDuration);
		characterInfoText.text = textToPrint;
	}

	public void HideCharacterInfo(string textToPrint)
	{
		characterInfo.transform.DOMove(characterInfoOriginalPosition, animationDuration);
		characterInfoText.text = textToPrint;
	}
 
	#endregion

	#region TOOLTIP

	public void ShowTooltip(string textToPrint)
	{
		tooltipText.text = textToPrint;
	}

    #endregion

    #region ENEMY_INFO
    public void SetEnemyOrder()
    {
        for (int i = 0; i < LM.enemiesOnTheBoard.Count; i++)
        {
            LM.enemiesOnTheBoard[i].orderToShow = i + 1;
            LM.enemiesOnTheBoard[i].thisUnitOrder.GetComponent <TextMeshPro>().text = "" + LM.enemiesOnTheBoard[i].orderToShow;
            LM.enemiesOnTheBoard[i].GetComponent<PlayerHealthBar>().ReloadHealth();
        }
    }

    public void ShowEnemyOrder(bool show_hide)
    {
        for (int i = 0; i < LM.enemiesOnTheBoard.Count; i++)
        {
            LM.enemiesOnTheBoard[i].thisUnitOrder.SetActive(show_hide);
            LM.enemiesOnTheBoard[i].HealthBarOn_Off(show_hide);
        }
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
