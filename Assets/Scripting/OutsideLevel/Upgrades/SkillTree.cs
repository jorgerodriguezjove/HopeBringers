using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTree : MonoBehaviour
{
    //Esto lo uso para que cuando los character data vuelvan del nivel puedan encontrar su arbol de habilidades correpsondiente y volver a guardar la referencia
    [SerializeField]
    public int characterId;

    [Header("ALL UPGRADES")]
    //Lista con todas las upgrades dentro del árbol
    [SerializeField]
    public List<UpgradeNode> allUpgradesInTree = new List<UpgradeNode>();

    //Aqui van todas las upgrades que bloquean una rama al comprarse.
    [Header("First/Second Upgrades")]
    [SerializeField]
    public List<UpgradeNode> firstUpgradesInTree = new List<UpgradeNode>();

    //Aqui van todas las upgrades que son segundas mejoras
    [SerializeField]
    public List<UpgradeNode> secondUpgradesInTree = new List<UpgradeNode>();

    [Header("Branches Upgrades")]
    [SerializeField]
    public List<UpgradeNode> active1Upgrades = new List<UpgradeNode>();

    //Aqui van todas las upgrades que son segundas mejoras
    [SerializeField]
    public List<UpgradeNode> active2Upgrades = new List<UpgradeNode>();

    [SerializeField]
    public List<UpgradeNode> pasive1Upgrades = new List<UpgradeNode>();

    //Aqui van todas las upgrades que son segundas mejoras
    [SerializeField]
    public List<UpgradeNode> pasive2Upgrades = new List<UpgradeNode>();

    [SerializeField]
    Image baseActiveIcon;

    [SerializeField]
    TextMeshProUGUI baseActiveText;

    [SerializeField]
    Image basePasiveIcon;

    [SerializeField]
    TextMeshProUGUI basePasiveText;

    string activeName;
    string pasiveName;

    public void UpdateActiveAndPasive(CharacterData unitData)
    {
        activeName = unitData.name + "BaseActive";
        pasiveName = unitData.name + "BasePasive";


        Debug.Log("Nombre: " + pasiveName);
        //Icons
        baseActiveIcon.sprite = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + activeName);
        basePasiveIcon.sprite = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + pasiveName);

        //Texts
        if (unitData.specificStringCharacterUpgrades.ContainsKey(activeName))
        {
            baseActiveText.SetText(unitData.specificStringCharacterUpgrades[activeName]);
            Debug.Log("si text base");
        }

        else
        {
            Debug.Log("no text base");
        }
        
        if (unitData.specificStringCharacterUpgrades.ContainsKey(pasiveName))
        {
            basePasiveText.SetText(unitData.specificStringCharacterUpgrades[pasiveName]);
        }
    }
}
