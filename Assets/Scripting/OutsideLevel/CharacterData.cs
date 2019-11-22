using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    #region VARIABLES

    //Referencia a la unidad que representa la figura
    [SerializeField]
    public PlayerUnit myUnit;

    [SerializeField]
    public GameObject skillTreePrefab;

    //Coste actual de la siguiente mejora del personaje
    [SerializeField]
    public int powerLevel;

    //Lista que guarda las habilidades que han sido compradas
    [SerializeField]
    public List<int> idSkillsBought;

    [Header("Referencias")]
    [SerializeField]
    private TableManager TM;


    #endregion

    //TEMPORAL
    private void OnMouseDown()
    {
        TM.OnClickCharacter(GetComponent<CharacterData>());
        
    }

    public void UpgradeAcquired(int upgradeCost,int idSkill)
    {
        powerLevel += upgradeCost;
        idSkillsBought.Add(idSkill);
    }
}
