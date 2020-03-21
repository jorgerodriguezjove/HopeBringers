using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Druid>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Actualizo las merjoas especificas del personaje
            //HAY QUE CAMBIARLO EN SU SCRIPT
            //myUnitReferenceOnLevel.GetComponent<Druid>().SetSpecificStats(specificBoolCharacterUpgrades[AppKnightUpgrades.pushFurther1], specificBoolCharacterUpgrades[AppKnightUpgrades.pushWider1]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        //HAY QUE CAMBIARLO EN SU SCRIPT
        //specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushFurther1, false);
        //specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushWider1, false);

        //Mejoras tipo INT
        //specificIntCharacterUpgrades.Add(AppKnightUpgrades.pushFurther1, myUnitReferenceOnLevel.GetComponent<Knight>().tilesToPush);
    }

}

