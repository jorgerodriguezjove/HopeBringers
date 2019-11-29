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


	#endregion

	#region INIT
	private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();
	}
	private void Start()
	{
		GetComponent<Image>().sprite = enemyPortraitSprite;
	}

	#endregion

	#region INTERACTION

	public void ShowEnemyPortraitFromPanel()
	{
		if(UIM.LM.selectedCharacter == null)
		{
			UIM.ShowCharacterImage(assignedEnemy);
		}	
	}

	public void HideEnemyPortraitFromPanel()
	{
		UIM.HideCharacterImage();
	}

	

	#endregion
}
