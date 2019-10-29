using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    //Vida actual de la unidad.
    [HideInInspector]
    public int currentHealth;

    //Ahora mismo se setea desde el inspector
    //ACORDARSE CAMBIAR POR LEVEL MANAGER
    public TileManager TM;

    #endregion

    private void Start()
    {
        //Asignar tile en el que empieza
        currentHealth = maxHealth;

        TM.checkAvailableTilesForMovement(movementUds);
    }



}
