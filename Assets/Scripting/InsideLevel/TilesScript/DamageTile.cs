using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTile : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    public bool damageDone;

    [SerializeField]
    public bool hasUnit;

    [SerializeField]
    public GameObject unitToDoDamage;

    [SerializeField]
    public int damageToDo;

	[SerializeField]
	[@TextAreaAttribute(15, 20)]
    public string tileInfo;

	[SerializeField]
    public Sprite tileImage;

	[Header("REFERENCIAS")]
    public LevelManager LM;


    #endregion

    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        LM.damageTilesInBoard.Add(this);
    }
    #endregion

    public  virtual void OnTriggerEnter(Collider unitOnTile)
    {

        if (unitOnTile.GetComponent<Druid>().tileSustitute)
        {
            Instantiate(unitOnTile.GetComponent<Druid>().healerTilePref, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
    public virtual void OnTriggerStay(Collider unitOnTile)
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

    public virtual void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = null;
            hasUnit = false;
        }
    }

    public virtual void CheckHasToDoDamage()
    {
        if(hasUnit && !damageDone)
        {
            if (unitToDoDamage != null && unitToDoDamage.GetComponent<Druid>())
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

	


	public virtual void Update()
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
