using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltips : MonoBehaviour
{
	[HideInInspector]
	public PlayerUnit tooltipAssignerPlayer;
	//Contador para retrasar la aparición del tooltip
	public float timeToShowTooltip = 1f;
	float timeToShowTooltipTimer;

	//Creamos un vector3 para que guarde la posición del ratón
	Vector3 lastMouseCoordinate = Vector3.zero;

	//Bool para controlar que el código del update solo se ejecute al entrar en un panel
	bool startTooltip = false;

	[SerializeField]
	bool generalTooltip;
	[SerializeField]
	bool activeSkill;
	[SerializeField]
	bool pasiveSkill;
	[SerializeField]
	GameObject tooltipPanel;
	[SerializeField]
	TextMeshProUGUI textPanel;
	[SerializeField]
	Image imagePanel;

	#region INIT
	private void Start()
	{
		timeToShowTooltipTimer = timeToShowTooltip;
	}
	#endregion
	#region UPDATE
	private void Update()
	{
		if (startTooltip)
		{
			Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
			if (mouseDelta.x == 0 && mouseDelta.y == 0)
			{
				timeToShowTooltipTimer -= Time.deltaTime;
				Debug.Log("Timer");
				if (timeToShowTooltipTimer <= 0)
				{
					tooltipPanel.SetActive(true);
					textPanel.text = tooltipAssignerPlayer.unitInfo;
					imagePanel.sprite = tooltipAssignerPlayer.attackTooltipImage;
					//Mostrar el tooltip
					Debug.Log("Tooltip Aparece");
				}
			}
			else
			{
				timeToShowTooltipTimer = timeToShowTooltip;
				tooltipPanel.SetActive(false);
				//Tooltip desaparece
				Debug.Log("Tooltip Desaparece");
			}
			lastMouseCoordinate = Input.mousePosition;
		}

	}
	#endregion

	#region INTERACTION
	public void StartTooltip()
	{
		startTooltip = true;
	}
	public void StopTooltip()
	{
		startTooltip = false;
	}
	#endregion
}
