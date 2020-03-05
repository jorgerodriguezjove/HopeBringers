using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageData : CharacterData
{ 

	public override void UpdateMyUnitStatsForTheLevel()
	{
		//Referencia al personaje en el nivel
		myUnitReferenceOnLevel = FindObjectOfType<Mage>();

        //Aztualizo las mejoras genéricas
        base.UpdateMyUnitStatsForTheLevel();

        //Inicializo las variables especificas del personaje 
        myUnitReferenceOnLevel.GetComponent<Mage>().SetSpecificStats(specificBoolCharacterUpgrades[AppMageUpgrades.lightningChain1], specificBoolCharacterUpgrades[AppMageUpgrades.crossAreaAttack1]);
    }

	//Esto se llama en el INIT del characterData (padre de este script)
	protected override void InitializeSpecificUpgrades()
	{
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.lightningChain1, false);
        specificBoolCharacterUpgrades.Add(AppMageUpgrades.crossAreaAttack1, false);


        //Mejoras tipo INT
    }
}
