using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : CharacterData
{
    #region VARIABLES

    //La referencia a mi unidad dentro del nivel. (No puedo usar myUnit porque es  una referencia al prefab).
    PlayerUnit myUnitReferenceOnLevel;

    #endregion


    public override void InitializeMyUnitStats()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Knight>();

        //Inicializo los stats genéricos
        myUnitReferenceOnLevel.SetMyGenericStats(genericUpgrades[AppGenericUpgrades.maxHealth]);
        //baseDamage,movementUds, bonusBackAttack,
        //bonusMoreHeight, bonusLessHeight, damageMadeByPush, damageMadeByFall,
        //range, maxHeightDifferenceToAttack, maxHeightDifferenceToMove);
        
        //Inicializo las variables especificas del personaje
        myUnitReferenceOnLevel.GetComponent<Knight>().SetSpecificStats(specificBoolCharacterUpgrades[AppKnightUpgrades.bigUpgradeFirstA], specificBoolCharacterUpgrades[AppKnightUpgrades.bigUpgradeFirstB]);
    }


    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.bigUpgradeFirstA, false);
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.bigUpgradeFirstB, false);


        //Mejoras tipo INT
    }

}
