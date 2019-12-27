using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyPortraits : MonoBehaviour
{
	#region VARIABLES

	[SerializeField]
	public Image enemyIconChild;
	[SerializeField]
	public TextMeshProUGUI unitNameInPortrait;
	[HideInInspector]
	public Sprite enemyPortraitSprite;
	[HideInInspector]
	public EnemyUnit assignedEnemy;
	[HideInInspector]
	public UIManager UIM;

    //Refernecia al panel con el highlight
    [SerializeField]
    private GameObject highlightPanelRef;

    //Añadido para hacer comprobaciones de turnos
    [HideInInspector]
    private LevelManager LM;

    #endregion

    #region INIT
    private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();

        //Añadido para hacer comprobaciones de turnos
        LM = FindObjectOfType<LevelManager>();
    }
	private void Start()
	{
		enemyIconChild.sprite = enemyPortraitSprite;
		unitNameInPortrait.text = assignedEnemy.unitName;
	}

	#endregion

	#region INTERACTION

	public void OnHoverEnterEnemyPortrait()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (UIM.LM.selectedCharacter == null && UIM.LM.selectedEnemy == null)
            {
                //Activo highlight de retrato y de personaje
                assignedEnemy.OnHoverEnterFunctionality();

                HighlightMyself();
            }
        }
	}

    //Función separada para que la llame el enemigo.
    public void HighlightMyself()
    {
        highlightPanelRef.SetActive(true);
    }

	public void OnHoverExitEnemyPortrait()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (UIM.LM.selectedEnemy == null)
            {
                assignedEnemy.OnHoverExitFunctionality();
                highlightPanelRef.SetActive(false);
            }
        }
	}

    //Función separada para que la llame el enemigo.
    public void UnHighlightMyself()
    {
        if (highlightPanelRef != null)
        {
            highlightPanelRef.SetActive(false);
        }
       
    }

    public void ClickOnEnemyPortrait()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            LM.SelectEnemy(assignedEnemy.unitInfo,assignedEnemy);

            UIM.ShowUnitInfo(assignedEnemy.unitInfo, assignedEnemy);

            HighlightMyself();


        }
	}

	#endregion
}
