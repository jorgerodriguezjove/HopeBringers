using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    //Vida actual de la unidad.
    [HideInInspector]
    public int currentHealth;

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [HideInInspector]
    public bool hasMoved = false;
    [HideInInspector]
    public bool hasAttacked = false;

    //REFERENCIAS
    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;

    #endregion

    #region INIT

    private void Start()
    {
        currentHealth = maxHealth;
        LM = LevelManagerRef.GetComponent<LevelManager>();
    }

    #endregion

    #region INTERACTION


    private void OnMouseUp()
    {
        if (!hasMoved)
        {
            LM.SelectUnit(movementUds, this);
        }

        





        if (!hasAttacked && !hasMoved)
        {
            //Avisa al LM
            //LM Comprueba que no hay unidad seleccionada actualmente y la selecciona
        }

        //Primero compruebo si me he movido

        //Avisar a LM de click

    }

    #endregion


}
