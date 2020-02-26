using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltips : MonoBehaviour
{
	[HideInInspector]
	public PlayerUnit tooltipAssignedPlayer;
	//Contador para retrasar la aparición del tooltip
	public float timeToShowTooltip = 1f;
	float timeToShowTooltipTimer;

	//Creamos un vector3 para que guarde la posición del ratón
	Vector3 lastMouseCoordinate = Vector3.zero;

	//Bool para controlar que el código del update solo se ejecute al entrar en un panel
	bool startTooltip = false;

	private UIManager UIM;
	[SerializeField]
	bool activeSkill;
	[SerializeField]
	bool pasiveSkill;
	[SerializeField]
	GameObject fatherTooltip;


	#region INIT
	private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();
	}
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
					UIM.tooltipPanel.SetActive(true);
					if (activeSkill)
					{
						UIM.textPanel.text = fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.activeSkillInfo;
						if(fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.attackTooltipImage != null)
						{
							UIM.imagePanel.gameObject.SetActive(true);
							UIM.imagePanel.sprite = fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.attackTooltipImage;
						}
						else
						{
							UIM.imagePanel.gameObject.SetActive(false);
						}
						
					}
					else if (pasiveSkill)
					{
						UIM.textPanel.text = fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.pasiveSkillInfo;
						if (fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.attackTooltipImage != null)
						{
							UIM.imagePanel.gameObject.SetActive(true);
							UIM.imagePanel.sprite = fatherTooltip.GetComponent<Tooltips>().tooltipAssignedPlayer.pasiveTooltipImage;
						}
						else
						{
							UIM.imagePanel.gameObject.SetActive(false);
						}
					}
					else
					{
						UIM.textPanel.text = tooltipAssignedPlayer.unitGeneralInfo;
						if (tooltipAssignedPlayer.attackTooltipImage != null)
						{
							UIM.imagePanel.gameObject.SetActive(true);
							UIM.imagePanel.sprite = tooltipAssignedPlayer.attackTooltipImage;
						}
						else
						{
							UIM.imagePanel.gameObject.SetActive(false);
						}
						
					}				
					//Mostrar el tooltip
					Debug.Log("Tooltip Aparece");
				}
			}
			else
			{
				timeToShowTooltipTimer = timeToShowTooltip;
				UIM.tooltipPanel.SetActive(false);
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
