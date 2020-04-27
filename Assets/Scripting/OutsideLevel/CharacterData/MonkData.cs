using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Monk>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Actualizo las merjoas especificas del personaje
            //HAY QUE CAMBIARLO EN SU SCRIPT
              myUnitReferenceOnLevel.GetComponent<Monk>().SetSpecificStats(specificBoolCharacterUpgrades[AppMonkUpgrades.turn1], specificBoolCharacterUpgrades[AppMonkUpgrades.turn2],
                                                                           specificBoolCharacterUpgrades[AppMonkUpgrades.suplex1], specificBoolCharacterUpgrades[AppMonkUpgrades.suplex2],
                                                                           specificBoolCharacterUpgrades[AppMonkUpgrades.markBuff1], specificBoolCharacterUpgrades[AppMonkUpgrades.markBuff2],
                                                                           specificBoolCharacterUpgrades[AppMonkUpgrades.markDebuff1], specificBoolCharacterUpgrades[AppMonkUpgrades.markDebuff2]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.turn1, false);
        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.turn2, false);

        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.suplex1, false);
        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.suplex2, false);

        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.markBuff1, false);
        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.markBuff2, false);

        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.markDebuff1, false);
        specificBoolCharacterUpgrades.Add(AppMonkUpgrades.markDebuff2, false);
    }

}

