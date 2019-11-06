using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrder : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    private GameObject uiManager_Ref;

    private UIManager UIM;

    #endregion

    #region INIT
    private void Awake()
    {
        UIM = uiManager_Ref.GetComponent<UIManager>();
    }

    #endregion

    #region INTERACTION
    void OnMouseEnter()
	{
        UIM.ShowEnemyOrder(true);
	}
	void OnMouseExit()
	{
        UIM.ShowEnemyOrder(false);
	}
    #endregion
}
