using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{


    private void Start()
    {
        myCurrentTile.unitOnTile = this;
    }
}
