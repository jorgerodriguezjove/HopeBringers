using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    //Lista con todas las upgrades dentro del árbol
    [SerializeField]
    public List<UpgradeNode> allUpgradesInTree = new List<UpgradeNode>();
}
