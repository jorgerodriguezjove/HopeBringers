using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Rogue>();

        if (myUnitReferenceOnLevel != null)
        {
            //Aztualizo las mejoras genéricas
            base.UpdateMyUnitStatsForTheLevel();

            //Inicializo las variables especificas del personaje
            myUnitReferenceOnLevel.GetComponent<Rogue>().SetSpecificStats(specificBoolCharacterUpgrades[AppRogueUpgrades.multiJumpAttack1], specificIntCharacterUpgrades[AppRogueUpgrades.multiJumpAttack2],
                                                                          specificBoolCharacterUpgrades[AppRogueUpgrades.extraTurnAfterKill1], specificIntCharacterUpgrades[AppRogueUpgrades.extraTurnAfterKill2],
                                                                          specificBoolCharacterUpgrades[AppRogueUpgrades.smokeBomb1], specificBoolCharacterUpgrades[AppRogueUpgrades.smokeBomb2],
                                                                          specificBoolCharacterUpgrades[AppRogueUpgrades.buffDamageKill1], specificBoolCharacterUpgrades[AppRogueUpgrades.buffDamageKill2]);
        }
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL
        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.multiJumpAttack1, false);
        specificIntCharacterUpgrades.Add(AppRogueUpgrades.multiJumpAttack2, 0);

        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.extraTurnAfterKill1, false);
        specificIntCharacterUpgrades.Add(AppRogueUpgrades.extraTurnAfterKill2, 0);

        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.smokeBomb1, false);
        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.smokeBomb2, false);

        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.buffDamageKill1, false);
        specificBoolCharacterUpgrades.Add(AppRogueUpgrades.buffDamageKill2, false);

        //DESCRIPCIONES DE LAS MEJORAS. Estos no hace falta inicializarlos en la unidad ingame. Están aqui simplemente para los árboles de mejoras
        //Activas
        specificStringCharacterUpgrades.Add("multiJumpAttack1Text", AppRogueUpgrades.multiJumpAttack1Text);
        specificStringCharacterUpgrades.Add("multiJumpAttack2Text", AppRogueUpgrades.multiJumpAttack2Text);

        specificStringCharacterUpgrades.Add("extraTurnAfterKill1Text", AppRogueUpgrades.extraTurnAfterKill1Text);
        specificStringCharacterUpgrades.Add("extraTurnAfterKill2Text", AppRogueUpgrades.extraTurnAfterKill2Text);

        //Pasivas
        specificStringCharacterUpgrades.Add("smokeBomb1Text", AppRogueUpgrades.smokeBomb1Text);
        specificStringCharacterUpgrades.Add("smokeBomb2Text", AppRogueUpgrades.smokeBomb2Text);

        specificStringCharacterUpgrades.Add("buffDamageKill1Text", AppRogueUpgrades.buffDamageKill1Text);
        specificStringCharacterUpgrades.Add("buffDamageKill2Text", AppRogueUpgrades.buffDamageKill2Text);
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
