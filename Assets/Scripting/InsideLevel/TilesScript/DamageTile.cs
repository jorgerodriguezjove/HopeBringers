using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTile : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    public bool damageDone;

    [SerializeField]
    private bool hasUnit;

    [SerializeField]
    private GameObject unitToDoDamage;

    [SerializeField]
    private int damageToDo;

	[SerializeField]
	[@TextAreaAttribute(15, 20)]
	private string tileInfo;

	[SerializeField]
	private Sprite tileImage;

	[Header("REFERENCIAS")]
    private LevelManager LM;


    #endregion

    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        LM.damageTilesInBoard.Add(this);
    }
	#endregion

	void OnTriggerStay(Collider unitOnTile)
	{
		//if (LM.currentLevelState == LevelManager.LevelState.ProcessingEnemiesActions && !damageDone)
		//{
		//	//Por si acaso pilla el tile en vez de a la unidad
		//	if (unitToDoDamage.GetComponent<UnitBase>())
		//	{
		//		unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);
		//		damageDone = true;
		//		Debug.Log("DAMAGE DONE");
		//	}
		//}

        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = unitOnTile.gameObject;
            hasUnit = true;
        }
	}

	void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = null;
            hasUnit = false;
        }
    }

    public void CheckHasToDoDamage()
    {
        if(hasUnit && !damageDone)
        {
            if (unitToDoDamage.GetComponent<Druid>())
            {
               
            }
            else
            {
                Debug.Log(unitToDoDamage);
                unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);

                damageDone = true;
                Debug.Log("DAMAGE DONE");
            }
            
            
        }
    }

	


	void Update()
	{
		if (LM.currentLevelState == LevelManager.LevelState.PlayerPhase)
		{
			damageDone = false;
		}
	}

	
	#region INTERACTION

	private void OnMouseEnter()
	{
		LM.UIM.ShowTileInfo(tileInfo, tileImage);
	}

	private void OnMouseExit()
	{
		LM.UIM.HideTileInfo();
	}

	#endregion
}
