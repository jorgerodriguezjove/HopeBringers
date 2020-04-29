using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Samurai>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Actualizo las merjoas especificas del personaje
            //HAY QUE CAMBIARLO EN SU SCRIPT
            myUnitReferenceOnLevel.GetComponent<Samurai>().SetSpecificStats(specificBoolCharacterUpgrades[AppSamuraiUpgrades.parry1], specificBoolCharacterUpgrades[AppSamuraiUpgrades.parry2],
                                                                            specificBoolCharacterUpgrades[AppSamuraiUpgrades.multiAttack1], specificIntCharacterUpgrades[AppSamuraiUpgrades.multiAttack2],
                                                                            specificBoolCharacterUpgrades[AppSamuraiUpgrades.honor1], specificBoolCharacterUpgrades[AppSamuraiUpgrades.honor2],
                                                                            specificBoolCharacterUpgrades[AppSamuraiUpgrades.loneWolf1], specificBoolCharacterUpgrades[AppSamuraiUpgrades.loneWolf2]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        //HAY QUE CAMBIARLO EN SU SCRIPT
        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.parry1, false);
        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.parry2, false);

        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.multiAttack1, false);
        specificIntCharacterUpgrades.Add(AppSamuraiUpgrades.multiAttack2, 0);

        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.honor1, false);
        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.honor2, false);

        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.loneWolf1, false);
        specificBoolCharacterUpgrades.Add(AppSamuraiUpgrades.loneWolf2, false);

        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Activas
        specificStringCharacterUpgrades.Add("parry1Text", AppSamuraiUpgrades.parry1Text);
        specificStringCharacterUpgrades.Add("parry2Text", AppSamuraiUpgrades.parry2Text);

        specificStringCharacterUpgrades.Add("multiAttack1Text", AppSamuraiUpgrades.multiAttack1Text);
        specificStringCharacterUpgrades.Add("multiAttack2Text", AppSamuraiUpgrades.multiAttack2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("honor1Text", AppSamuraiUpgrades.honor1Text);
        specificStringCharacterUpgrades.Add("honor2Text", AppSamuraiUpgrades.honor2Text);

        specificStringCharacterUpgrades.Add("loneWolf1Text", AppSamuraiUpgrades.loneWolf1Text);
        specificStringCharacterUpgrades.Add("loneWolf2Text", AppSamuraiUpgrades.loneWolf2Text);
    }

}
