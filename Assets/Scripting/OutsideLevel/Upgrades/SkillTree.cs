using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
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




}
