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
	bool activeSkill;
	[SerializeField]
	bool pasiveSkill;
	[SerializeField]
	GameObject tooltipPanel;
	[SerializeField]
	GameObject fatherTooltip;
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
					if (activeSkill)
					{
						textPanel.text = fatherTooltip.GetComponent<Tooltips>().tooltipAssignerPlayer.activeSkilllInfo;
						imagePanel.sprite = fatherTooltip.GetComponent<Tooltips>().tooltipAssignerPlayer.attackTooltipImage;
					}
					else if (pasiveSkill)
					{
						textPanel.text = fatherTooltip.GetComponent<Tooltips>().tooltipAssignerPlayer.pasiveSkillInfo;
						imagePanel.sprite = fatherTooltip.GetComponent<Tooltips>().tooltipAssignerPlayer.pasiveTooltipImage;
					}
					else
					{
						textPanel.text = tooltipAssignerPlayer.unitGeneralInfo;
						imagePanel.sprite = tooltipAssignerPlayer.attackTooltipImage;
					}				
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
