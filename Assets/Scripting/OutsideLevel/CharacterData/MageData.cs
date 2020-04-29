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

        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Activas
        specificStringCharacterUpgrades.Add("lightningChain1Text", AppMageUpgrades.lightningChain1Text);
        specificStringCharacterUpgrades.Add("lightningChain2Text", AppMageUpgrades.lightningChain2Text);

        specificStringCharacterUpgrades.Add("crossAreaAttack1Text", AppMageUpgrades.crossAreaAttack1Text);
        specificStringCharacterUpgrades.Add("crossAreaAttack2Text", AppMageUpgrades.crossAreaAttack2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("bombDecoy1Text", AppMageUpgrades.bombDecoy1Text);
        specificStringCharacterUpgrades.Add("bombDecoy2Text", AppMageUpgrades.bombDecoy2Text);

        specificStringCharacterUpgrades.Add("mirrorDecoy1Text", AppMageUpgrades.mirrorDecoy1Text);
        specificStringCharacterUpgrades.Add("mirrorDecoy2Text", AppMageUpgrades.mirrorDecoy2Text);
    }
}
