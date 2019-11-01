using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    [Header("REFERENCIAS")]

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
        initMaterial = GetComponent<MeshRenderer>().material;
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

    private void OnMouseEnter()
    {
        Debug.Log("enemy");
        //Llamo a LevelManager para activar hover
        LM.CheckIfHoverShouldAppear(this);
    }

    private void OnMouseExit()
    {
        //Llamo a LevelManager para desactivar hover
        LM.HideHover(this);
    }


    #endregion

    #region ATTACK

    public override void ReceiveDamage(int damageReceived)
    {
        currentHealth -= damageReceived;

        Debug.Log("Soy " + gameObject.name + "y me han hecho " + damageReceived + " de daño");
        Debug.Log("Mi vida actual es " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");
    }

    #endregion
}
