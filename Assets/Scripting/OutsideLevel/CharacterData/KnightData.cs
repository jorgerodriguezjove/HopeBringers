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

        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Activas
        specificStringCharacterUpgrades.Add("pushFurther1Text", AppKnightUpgrades.pushFurther1Text);
        specificStringCharacterUpgrades.Add("pushFurther2Text", AppKnightUpgrades.pushFurther2Text);

        specificStringCharacterUpgrades.Add("pushWider1Text", AppKnightUpgrades.pushWider1Text);
        specificStringCharacterUpgrades.Add("pushWider2Text", AppKnightUpgrades.pushWider2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("individualBlock1Text", AppKnightUpgrades.individualBlock2Text);
        specificStringCharacterUpgrades.Add("individualBlock2Text", AppKnightUpgrades.individualBlock1Text);

        specificStringCharacterUpgrades.Add("neighbourBlock1Text", AppKnightUpgrades.neighbourBlock1Text);
        specificStringCharacterUpgrades.Add("neighbourBlock2Text", AppKnightUpgrades.neighbourBlock2Text);
    }

}
