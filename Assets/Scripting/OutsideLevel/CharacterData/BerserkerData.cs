﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Berserker>();

        //Aztualizo las mejoras genéricas
        base.UpdateMyUnitStatsForTheLevel();

        //Inicializo las variables especificas del personaje
        myUnitReferenceOnLevel.GetComponent<Berserker>().SetSpecificStats(specificBoolCharacterUpgrades[AppBerserkUpgrades.doubleAttack1], specificBoolCharacterUpgrades[AppBerserkUpgrades.circularAttack1]);
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppBerserkUpgrades.doubleAttack1, false);
        specificBoolCharacterUpgrades.Add(AppBerserkUpgrades.circularAttack1, false);

        //Mejoras tipo INT
    }



























    //CODIGO ANTIGUO COMPROBAR SI SE PUEDE BORRAR
    //RogueData otherRogueInScene;



    //public override void Awake()
    //{
    //    otherRogueInScene = FindObjectOfType<RogueData>();

    //    if (otherRogueInScene != null && otherRogueInScene.gameObject != this.gameObject)
    //    {
    //        Destroy(otherRogueInScene.gameObject);
    //    }

    //    base.Awake();
    //}
}