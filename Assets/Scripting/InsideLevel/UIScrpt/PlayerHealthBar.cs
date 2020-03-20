using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
	#region VARIABLES
	[SerializeField]
	private Slider healthBar;
	[HideInInspector]
	private UnitBase unit;
	#endregion

	#region INIT
	private void Awake()
	{
		//unit = gameObject.GetComponent<UnitBase>();
		//healthBar.maxValue = unit.maxHealth;
		//healthBar.value = unit.currentHealth;
	}
	#endregion

	#region HEALTHCONTROL
	//public void ReloadHealth()
	//{
	//	healthBar.value = unit.currentHealth;
	//}

	#endregion
}
