using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour
{
    #region VARIABLES

    [Header("FUNCIONAMIENTO DE LA MEJORA")]
    //Cantidad que se añade al power level del personaje tras comprar la habilidad
    [SerializeField]
    public int upgradeCost;

    [SerializeField]
    public string tooltip;

    [Header("LÓGICA")]

    //[SerializeField]
    //public int idUpgrade;

    [SerializeField]
    public UpgradeType thisUpgradeType;
    public enum UpgradeType { SMALL, MEDIUM, BIG }

    [SerializeField]
    public bool isBlocked;
    [SerializeField]
    public bool isBought;

    [SerializeField]
    List<UpgradeNode> nodesUnlockedAfterBuying;

    [Header("REFERENCIAS")]
    [HideInInspector]
    public TableManager TM;

    #endregion

    #region INIT

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(onClickUpgrade);
    }

    #endregion

    //Al hacer click sobre el upgrade
    public void onClickUpgrade()
    {
        FindObjectOfType<TableManager>().BuyUpgrade(GetComponent<UpgradeNode>());
    }

    //Al comprar el upgrade
    public void UpgradeBought()
    {
        isBlocked = false;
        isBought = true;

        for (int i = 0; i < nodesUnlockedAfterBuying.Count; i++)
        {
            nodesUnlockedAfterBuying[i].UnlockUpgrade();
        }


        if (thisUpgradeType == UpgradeType.BIG)
        {

        }


    }

    //Al desbloquear upgrades por comprar
    public void UnlockUpgrade()
    {
        isBlocked = false;
        gameObject.SetActive(true);
    }

    


}
