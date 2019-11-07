using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portraits : MonoBehaviour
{
	#region VARIABLES

	[HideInInspector]
	public PlayerUnit assignedPlayer;
	[HideInInspector]
	private UIManager UIM;

	#endregion

	#region INIT

	private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();
	}

	#endregion


	#region INTERACTION

	public void AssignClickerPlayer()
	{
		UIM.PortraitCharacterSelect(assignedPlayer);
	}

	#endregion

}
