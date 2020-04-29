using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UpgradeNode : MonoBehaviour
{
    #region VARIABLES

    [Header("STATS MEJORA")]
    //Cantidad que se añade al power level del personaje tras comprar la habilidad
    [SerializeField]
    public int upgradeCost;

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
    //Nombre de la mejora a nivel lógico y del sprite
    private string upgradeName;

    //Es coger el upgrade name y añadirle "Text". Por ej: PusFurther1 pasa a ser PushFurther1Text
    private string descriptionText;

    //Valor añadido
    [SerializeField]
    private int valueAdded;

    [Header("LÓGICA")]

    //Se usa para comprobar que nodos han sido comprados al recargar el árbol de habilidades. NO SE USA PARA SABER A QUE MEJORA SE APLICA
    //Estos ids se setean automáticamente desde el UITableManager la primera vez que se setea el árbol.
    [HideInInspector]
    public int idUpgrade;

    [SerializeField]
    public bool isBought;

    //Imágen que indica si la mejora ha sido comprada
    [SerializeField]
    GameObject imageFeedbackIsBought;

    //Imágen que indica que todavía no se puede comprar la mejora
    [SerializeField]
    GameObject imageFeedbackIsBlocked;

    [SerializeField]
    List<UpgradeNode> nodesUnlockedAfterBuying;

    [HideInInspector]
    public CharacterData myUnit;

    //Bool que indica si se puede intentar comprar la mejora.
    public bool isInteractuable = true;

    [Header("REFERENCIAS")]
    [HideInInspector]
    public TableManager TM;

    [SerializeField]
    private GameObject feedbackBranchBlocked;

    [SerializeField]
    private TextMeshProUGUI descriptionTextBoxReference;

    [SerializeField]
    private Image iconHolder;

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

    public void UpdateIconAndDescription()
    {
        descriptionText = upgradeName + "Text";

        Debug.Log(Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + upgradeName));

        iconHolder.sprite = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + upgradeName);

        if (myUnit.specificStringCharacterUpgrades.ContainsKey(descriptionText))
        {
            descriptionTextBoxReference.SetText(myUnit.specificStringCharacterUpgrades[descriptionText]);
        }
        
        else
        {
            Debug.Log("No se ha cargado la descripción porque no hay key con ese nombre.");
            descriptionTextBoxReference.SetText(myUnit.specificStringCharacterUpgrades["pushFurther1Text"]);
        }
    }

    #endregion

    //Al hacer click sobre el upgrade
    public void onClickUpgrade()
    {
        if(!isBought && isInteractuable)
        {
            //Aviso comprar mejora
            if (GameManager.Instance.currentExp >= myUnit.unitPowerLevel)
            {
                FindObjectOfType<UITableManager>().ConfirmateUpgrade(true, GetComponent<UpgradeNode>());
            }

            //Aviso no suficiente xp
            else
            {
                FindObjectOfType<UITableManager>().NotEnoughXp();
            }
        }

        // Mensaje Ya ha sido comprada
        else if (isInteractuable)
        {
            FindObjectOfType<UITableManager>().AlreadyBought();
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
            Debug.Log(myUnit);

            Debug.Log(upgradeName);

            Debug.Log(myUnit.specificBoolCharacterUpgrades);

           if (myUnit.specificBoolCharacterUpgrades.ContainsKey(upgradeName))
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

        FindObjectOfType<UITableManager>().UpdateUpgradesBlocked();
    }

    //Al desbloquear upgrades por comprar
    public void UnlockUpgrade()
    {
        imageFeedbackIsBlocked.SetActive(false);
        isInteractuable = true;
    }

    public void ShowHideFeedbackBlockedBranch(bool _shouldShow)
    {
        feedbackBranchBlocked.SetActive(_shouldShow);
        isInteractuable = !_shouldShow;
    }

    public void ShowFeedbackBlockedUpgrade()
    {
        imageFeedbackIsBlocked.SetActive(true);
        isInteractuable = false;
    }

    private void OnMouseEnter()
    {
        //Hover highlight
        if (isInteractuable)
        {

        }
    }

    private void OnMouseExit()
    {
        
    }



}
