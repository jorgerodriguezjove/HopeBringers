using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    //Vida actual de la unidad.
    [HideInInspector]
    public int currentHealth;

    //REFERENCIAS-------------------------------------------------------

    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.enemiesOnTheBoard.Add(this);
        myCurrentTile.unitOnTile = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        LM.SelectUnitToAttack(GetComponent<UnitBase>());
    }

    #endregion

    #region ATTACK

    public override void ReceiveDamage(int damageReceived)
    {
        currentHealth -= damageReceived;

        Debug.Log("Soy " + gameObject.name + "y me han hecho " + damageReceived + " de daño");
        Debug.Log("Mi vida actual es " + currentHealth);
    }

    #endregion
}
