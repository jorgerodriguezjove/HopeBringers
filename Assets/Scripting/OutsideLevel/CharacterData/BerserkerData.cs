using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Berserker>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Inicializo las variables especificas del personaje
            myUnitReferenceOnLevel.GetComponent<Berserker>().SetSpecificStats(specificBoolCharacterUpgrades[ AppBerserkUpgrades.areaAttack1], specificIntCharacterUpgrades[AppBerserkUpgrades.areaAttack2],
                                                                              specificBoolCharacterUpgrades[AppBerserkUpgrades.circularAttack1], specificIntCharacterUpgrades[AppBerserkUpgrades.circularAttack2],
                                                                              specificIntCharacterUpgrades[AppBerserkUpgrades.rageDamage1], specificIntCharacterUpgrades[AppBerserkUpgrades.rageDamage2],
                                                                              specificBoolCharacterUpgrades[AppBerserkUpgrades.fearRage1], specificIntCharacterUpgrades[AppBerserkUpgrades.fearRage2]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppBerserkUpgrades.circularAttack1, false);
        specificIntCharacterUpgrades.Add(AppBerserkUpgrades.circularAttack2, 0);

        specificBoolCharacterUpgrades.Add(AppBerserkUpgrades.areaAttack1, false);
        specificIntCharacterUpgrades.Add(AppBerserkUpgrades.areaAttack2, 0);

        specificIntCharacterUpgrades.Add(AppBerserkUpgrades.rageDamage1, 0);
        specificIntCharacterUpgrades.Add(AppBerserkUpgrades.rageDamage2, 0);

        specificBoolCharacterUpgrades.Add(AppBerserkUpgrades.fearRage1, false);
        specificIntCharacterUpgrades.Add(AppBerserkUpgrades.fearRage2, 0);

        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Activas
        specificStringCharacterUpgrades.Add("areaAttack1Text", AppBerserkUpgrades.areaAttack1Text);
        specificStringCharacterUpgrades.Add("areaAttack2Text", AppBerserkUpgrades.areaAttack2Text);

        specificStringCharacterUpgrades.Add("circularAttack1Text", AppBerserkUpgrades.circularAttack1Text);
        specificStringCharacterUpgrades.Add("circularAttack2Text", AppBerserkUpgrades.circularAttack2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("rageDamage1Text", AppBerserkUpgrades.rageDamage1Text);
        specificStringCharacterUpgrades.Add("rageDamage2Text", AppBerserkUpgrades.rageDamage2Text);

        specificStringCharacterUpgrades.Add("fearRage1Text", AppBerserkUpgrades.fearRage1Text);
        specificStringCharacterUpgrades.Add("fearRage2Text", AppBerserkUpgrades.fearRage2Text);
    }
}
