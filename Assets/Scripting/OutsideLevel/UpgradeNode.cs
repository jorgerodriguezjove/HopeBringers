using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour
{
    #region VARIABLES

    [Header("STATS MEJORA")]
    //Cantidad que se añade al power level del personaje tras comprar la habilidad
    [SerializeField]
    public int upgradeCost;

    [SerializeField]
    public string tooltip;

    //Si es resta o suma
    [Tooltip ("Activar si la mejora reduce el valor en vez de sumar. Por ej reducir cooldown")]
    [SerializeField]
    private bool isItMinus;

    //Si la mejora es activar un bool
    [Tooltip("Activar si la mejora otorga un nuevo comportamiento/habilidad")]
    [SerializeField]
    private bool isItBoolUpgrade;

    [Tooltip ("Este nombre tiene que ser el mismo usado en AppUpgrades")]
    [SerializeField]
    private string upgradeName;

    //Valor añadido
    [SerializeField]
    private int valueAdded;

    [Header("LÓGICA")]

    //Se usa para comprobar que nodos han sido comprados al recargar el árbol de habilidades. NO SE USA PARA SABER A QUE MEJORA SE APLICA
    //Lo ideal supongo que sería que se generase automaticamente en vez de poner en el editor el id (esto puede dar lugar a repeticiones)
    [SerializeField]
    public int idUpgrade;

    [SerializeField]
    public bool isBlocked;
    [SerializeField]
    public bool isBought;

    [SerializeField]
    List<UpgradeNode> nodesUnlockedAfterBuying;

    [HideInInspector]
    public CharacterData myUnit;

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

        //Si la mejora es de tipo bool busco la mejora en el diccionario y la aplico
        if (isItBoolUpgrade)
        {
           if(myUnit.specificBoolCharacterUpgrades.ContainsKey(upgradeName))
           {
               myUnit.specificBoolCharacterUpgrades[upgradeName] = true;
           }

           //Por si esta mal puesto el nombre
           else
           {
               Debug.LogError("Diccionario Bool no contiene el nombre: " + upgradeName);
           }

        }

        else
        {
            //Si hay que restar el valor en vez de sumarlo
            if (isItMinus)
            {
                valueAdded *= -1;
            }

            //Si la mejora es genérica para todos los personajes
            if (myUnit.genericUpgrades.ContainsKey(upgradeName))
            {
                myUnit.genericUpgrades[upgradeName] += valueAdded;
            }

            //Si la mejora es específica del personaje
            else if (myUnit.genericUpgrades.ContainsKey(upgradeName))
            {
                myUnit.specificIntCharacterUpgrades[upgradeName] += valueAdded;
            }

            //Por si esta mal puesto el nombre
            else
            {
                Debug.LogError("Diccionario Generico y especifico Int no contienen el nombre: " + upgradeName);
            }
        }
    }

    //Al desbloquear upgrades por comprar
    public void UnlockUpgrade()
    {
        isBlocked = false;
        gameObject.SetActive(true);
    }

    


}
