using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueData : CharacterData
{

    public override void UpdateMyUnitStatsForTheLevel()
    {
        //Referencia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Rogue>();

        //Aztualizo las mejoras genéricas
        base.UpdateMyUnitStatsForTheLevel();

        //Inicializo las variables especificas del personaje
        //CUANDO SE METAN MEJORAS ESPECIFICAS DEL ROGUE DESCOMENTAR ESTA LÍNEA
        //myUnitReferenceOnLevel.GetComponent<Rogue>().SetSpecificStats();
    }

    //Esto se llama en el INIT del characterData (padre de este script)
    protected override void InitializeSpecificUpgrades()
    {
        //Mejoras Tipo BOOL


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
