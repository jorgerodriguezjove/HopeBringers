using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageData : CharacterData
{ 

	#region VARIABLES

	//La referencia a mi unidad dentro del nivel. (No puedo usar myUnit porque es  una referencia al prefab).
	PlayerUnit myUnitReferenceOnLevel;

	#endregion

	//CAMBIAR LAS COSAS PARA QUE SEAN LAS DEL MAGO (Ahora mismo es una copia del Knight)

	public override void InitializeMyUnitStats()
	{
		//Referencia al personaje en el nivel
		myUnitReferenceOnLevel = FindObjectOfType<Knight>();

		//Inicializo los stats genéricos
		myUnitReferenceOnLevel.SetMyGenericStats(genericUpgrades[AppGenericUpgrades.maxHealth], genericUpgrades[AppGenericUpgrades.baseDamage]);
		//movementUds, bonusBackAttack,
		//bonusMoreHeight, bonusLessHeight, damageMadeByPush, damageMadeByFall,
		//range, maxHeightDifferenceToAttack, maxHeightDifferenceToMove);

		//Inicializo las variables especificas del personaje
		myUnitReferenceOnLevel.GetComponent<Knight>().SetSpecificStats(specificBoolCharacterUpgrades[AppKnightUpgrades.pushFurther1], specificBoolCharacterUpgrades[AppKnightUpgrades.pushWider1]);
	}


	//Esto se llama en el INIT del characterData (padre de este script)
	protected override void InitializeSpecificUpgrades()
	{
		//Mejoras Tipo BOOL
		specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushFurther1, false);
		specificBoolCharacterUpgrades.Add(AppKnightUpgrades.pushWider1, false);


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
