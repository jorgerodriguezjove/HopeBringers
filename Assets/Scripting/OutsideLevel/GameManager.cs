using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    #region VARIABLES

    //Lista de unidades que se tienen que cargar en el nivel
    [HideInInspector]
    public List<PlayerUnit> unitsForCurrentLevel = new List<PlayerUnit>();

    #endregion

    #region INIT

    #endregion

}
