using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPortraits : MonoBehaviour
{
	#region VARIABLES

	[HideInInspector]
	public Sprite enemyPortraitSprite;
	[HideInInspector]
	public EnemyUnit assignedEnemy;
	[HideInInspector]
	public UIManager UIM;

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
		GetComponent<Image>().sprite = enemyPortraitSprite;
	}

	#endregion

	#region INTERACTION

	public void ShowEnemyPortraitFromPanel()
	{

        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (UIM.LM.selectedCharacter == null && UIM.LM.selectedEnemy == null)
            {
                UIM.ShowCharacterImage(assignedEnemy);
                UIM.LM.ShowEnemyHover(assignedEnemy.range, assignedEnemy);
            }
        }
	}

	public void HideEnemyPortraitFromPanel()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (UIM.LM.selectedEnemy == null)
            {
                UIM.HideCharacterImage();
                UIM.LM.HideEnemyHover(assignedEnemy);
                UIM.LM.HideEnemyHover(assignedEnemy);
            }
        }

	}
	
	public void SelectEnemyFromPanel()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            UIM.ShowCharacterImage(assignedEnemy);
            UIM.LM.selectedEnemy = assignedEnemy;
        }
	}

	#endregion
}
