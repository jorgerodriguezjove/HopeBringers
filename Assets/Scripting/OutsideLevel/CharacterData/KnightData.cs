using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : CharacterData
{
    #region VARIABLES

    //La referencia a mi unidad dentro del nivel. (No puedo usar myUnit porque es  una referencia al prefab).
    PlayerUnit myUnitReferenceOnLevel;

    #endregion

    public override void InitializeMyUnitStats()
    {
        //Referncia al personaje en el nivel
        myUnitReferenceOnLevel = FindObjectOfType<Knight>();

        //Inicializo los stats genéricos
        myUnitReferenceOnLevel.SetMyGenericStats(maxHealth, movementUds, 
                                                        baseDamage, bonusBackAttack,
                                                        bonusMoreHeight, bonusLessHeight, damageMadeByPush, damageMadeByFall,
                                                        range, maxHeightDifferenceToAttack, maxHeightDifferenceToMove);

        //Inicializo los stats propios del personaje
        //myUnitReferenceOnLevel.InitializeMyStats();
    }

}
