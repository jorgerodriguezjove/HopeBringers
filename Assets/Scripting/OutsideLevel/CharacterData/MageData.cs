using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageData : CharacterData
{ 

	public override void UpdateMyUnitStatsForTheLevel()
	{
		//Referencia al personaje en el nivel
		myUnitReferenceOnLevel = FindObjectOfType<Mage>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Inicializo las variables especificas del personaje 
            myUnitReferenceOnLevel.GetComponent<Mage>().SetSpecificStats(specificBoolCharacterUpgrades[AppMageUpgrades.lightningChain1], specificBoolCharacterUpgrades[AppMageUpgrades.lightningChain2],
                                                                         specificBoolCharacterUpgrades[AppMageUpgrades.crossAreaAttack1], specificBoolCharacterUpgrades[AppMageUpgrades.crossAreaAttack2],
                                                                         specificBoolCharacterUpgrades[AppMageUpgrades.bombDecoy1], specificBoolCharacterUpgrades[AppMageUpgrades.bombDecoy2],
                                                                         specificBoolCharacterUpgrades[AppMageUpgrades.mirrorDecoy1], specificBoolCharacterUpgrades[AppMageUpgrades.mirrorDecoy2]);
        }
    }

	//Esto se llama en el INIT del characterData (padre de este script)
	protected override void InitializeSpecificUpgrades()
	{
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.lightningChain1, false);
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.lightningChain2, false);

        specificBoolCharacterUpgrades.Add(AppMageUpgrades.crossAreaAttack1, false);
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.crossAreaAttack2, false);

        specificBoolCharacterUpgrades.Add(AppMageUpgrades.bombDecoy1, false);
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.bombDecoy2, false);

        specificBoolCharacterUpgrades.Add(AppMageUpgrades.mirrorDecoy1, false);
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.mirrorDecoy2, false);


        //Mejoras tipo INT
    }
}
