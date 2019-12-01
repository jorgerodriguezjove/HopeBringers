using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndTurnButton : MonoBehaviour
{
	#region VARIABLES

	[SerializeField]
	UnityEvent AnEvent;

	#endregion

	#region INTERACTION

	private void OnMouseDown()
	{
		AnEvent.Invoke();
	}

	#endregion
}
