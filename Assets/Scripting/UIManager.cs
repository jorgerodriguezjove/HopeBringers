using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
	private GameObject optionsScreen;

	[SerializeField]
	private GameObject optionsButton;

	[SerializeField]
	private List<TextMeshProUGUI> healthValues;

	[SerializeField]
	private List<Slider> healthBars;

	[SerializeField]
	private List<GameObject> attackTokens;

	[SerializeField]
	private List<GameObject> movementTokens;

	[SerializeField]
	private TextMeshProUGUI tooltipText;

	[SerializeField]
	private TextMeshProUGUI characterInfoText;


	#endregion

	#region INIT

	private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
    }
	private void Start()
	{
		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
			healthValues[i].text = LM.characthersOnTheBoard[i].currentHealth + "/" + LM.characthersOnTheBoard[i].maxHealth;
			healthBars[i].maxValue = LM.characthersOnTheBoard[i].maxHealth;
			healthBars[i].value = LM.characthersOnTheBoard[i].currentHealth;
		}
	}

	#endregion

	#region END_TURN

	//Se llama desde el botón de finalizar turno
	public void EndTurn()
    {
        LM.ChangePhase();
    }

    //Función que activa o desactiva el botón de pasar turno en función de si es la fase del player o del enemigo
    public void ActivateDeActivateEndButton()
    {
        endTurnButton.interactable = !endTurnButton.interactable;
    }

    #endregion

    #region UNDO_MOVE

    //Se llama desde el botón de finalizar turno
    public void UndoMove()
    {
        LM.UndoMove();
    }

    #endregion

    #region ROTATION_ARROWS

    [SerializeField]
    public void RotatePlayerInNewDirection()
    {
        LM.selectedCharacter.RotateUnitFromButton(EventSystem.current.currentSelectedGameObject.GetComponent<RotateButton>().newDirection);
    }


	#endregion

	#region RETRATOS
	//Refresca la información de las 4 barras de vida en pantalla
	public void RefreshHealth()
	{
		for (int i = 0; i < LM.characthersOnTheBoard.Count; i++)
		{
			healthValues[i].text = LM.characthersOnTheBoard[i].currentHealth + "/" + LM.characthersOnTheBoard[i].maxHealth;
			healthBars[i].maxValue = LM.characthersOnTheBoard[i].maxHealth;
			healthBars[i].value = LM.characthersOnTheBoard[i].currentHealth;
		}
	}
	//Refresco la información de todos los tokens para activar y desactivar los que correspondan
	public void RefreshTokens()
	{
		for (int i = 0; i < attackTokens.Count; i++)
		{
			attackTokens[i].SetActive(!LM.characthersOnTheBoard[i].hasAttacked);
		}
		for (int i = 0; i < movementTokens.Count; i++)
		{
			movementTokens[i].SetActive(!LM.characthersOnTheBoard[i].hasMoved);
		}
	}
	#endregion

	#region CHARACTER_INFO

	public void ShowCharacterInfo(string textToPrint)
	{
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
