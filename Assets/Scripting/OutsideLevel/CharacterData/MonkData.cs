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


        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Base
        specificStringCharacterUpgrades.Add("MonkDataBaseActive", AppMonkUpgrades.MonkDataBaseActive);
        specificStringCharacterUpgrades.Add("MonkDataBasePasive", AppMonkUpgrades.MonkDataBasePasive);

        //Activas
        specificStringCharacterUpgrades.Add("turn1Text", AppMonkUpgrades.turn1Text);
        specificStringCharacterUpgrades.Add("turn2Text", AppMonkUpgrades.turn2Text);

        specificStringCharacterUpgrades.Add("suplex1Text", AppMonkUpgrades.suplex1Text);
        specificStringCharacterUpgrades.Add("suplex2Text", AppMonkUpgrades.suplex2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("markDebuff1Text", AppMonkUpgrades.markDebuff1Text);
        specificStringCharacterUpgrades.Add("markDebuff2Text", AppMonkUpgrades.markDebuff2Text);

        specificStringCharacterUpgrades.Add("markBuff1Text", AppMonkUpgrades.markBuff1Text);
        specificStringCharacterUpgrades.Add("markBuff2Text", AppMonkUpgrades.markBuff2Text);
    }

}

