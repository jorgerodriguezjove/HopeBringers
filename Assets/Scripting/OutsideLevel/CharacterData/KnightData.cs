using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Knight>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Actualizo las merjoas especificas del personaje
            myUnitReferenceOnLevel.GetComponent<Knight>().SetSpecificStats(specificBoolCharacterUpgrades[AppKnightUpgrades.pushFurther1], specificBoolCharacterUpgrades[AppKnightUpgrades.pushFurther2],
                                                                           specificBoolCharacterUpgrades[AppKnightUpgrades.pushWider1], specificBoolCharacterUpgrades[AppKnightUpgrades.pushWider2],
                                                                           specificBoolCharacterUpgrades[AppKnightUpgrades.individualBlock1], specificBoolCharacterUpgrades[AppKnightUpgrades.individualBlock2],
                                                                           specificBoolCharacterUpgrades[AppKnightUpgrades.neighbourBlock1], specificBoolCharacterUpgrades[AppKnightUpgrades.neighbourBlock2]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        //Activas
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushFurther1, false);
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushFurther2, false);

        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushWider1, false);
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushWider2, false);

        //Pasivas
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.individualBlock1, false);
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.individualBlock2, false);

        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.neighbourBlock1, false);
        specificBoolCharacterUpgrades.Add(AppKnightUpgrades.neighbourBlock2, false);

        //Mejoras tipo INT
        //specificIntCharacterUpgrades.Add(AppKnightUpgrades.pushFurther1, myUnitReferenceOnLevel.GetComponent<Knight>().tilesToPush);
    }

}
