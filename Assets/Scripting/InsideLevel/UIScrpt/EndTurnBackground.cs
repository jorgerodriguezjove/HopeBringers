using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndTurnBackground : MonoBehaviour
{
	#region VARIABLES

	[SerializeField]
	UnityEvent MouseEnter;
	[SerializeField]
	UnityEvent MouseExit;

	#endregion

	#region INTERACTION

	private void OnMouseEnter()
	{
		MouseEnter.Invoke();
	}

	private void OnMouseExit()
	{
		MouseExit.Invoke();
	}

	#endregion
}
