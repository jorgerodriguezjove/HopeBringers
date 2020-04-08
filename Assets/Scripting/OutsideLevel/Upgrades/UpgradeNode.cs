using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    //Estos ids se setean automáticamente desde el UITableManager la primera vez que se setea el árbol.
    [HideInInspector]
    public int idUpgrade;

    //[SerializeField]
    //public bool isBlocked;
    [SerializeField]
    public bool isBought;

    //Imágen que indica si la mejora ha sido comprada
    [SerializeField]
    GameObject imageFeedbackIsBought;

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

        if (isBought)
        {
            imageFeedbackIsBought.SetActive(true);
            imageFeedbackIsBought.GetComponent<ImageRadiaUpgrade>().SetImageFill(false);
        }
    }

    #endregion

    //Al hacer click sobre el upgrade
    public void onClickUpgrade()
    {
        if(!isBought)
        {
            FindObjectOfType<TableManager>().BuyUpgrade(GetComponent<UpgradeNode>());
        }
    }

    //Al comprar el upgrade
    public void UpgradeBought()
    {
        Debug.Log("Mejora: " + upgradeName  + " Comprada");

        //Actualizo la info de la mejora comprada
        //isBlocked = false;
        isBought = true;

        //Doy feedback de que la mejora ha sido comprada
        imageFeedbackIsBought.SetActive(true);
        imageFeedbackIsBought.GetComponent<ImageRadiaUpgrade>().SetImageFill(true);

        //Desbloqueo las siguientes mejoras
        if (nodesUnlockedAfterBuying.Count > 0)
        {
            for (int i = 0; i < nodesUnlockedAfterBuying.Count; i++)
            {
                nodesUnlockedAfterBuying[i].UnlockUpgrade();
            }
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
        //isBlocked = false;
        gameObject.SetActive(true);
    }
}
