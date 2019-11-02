using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    //[Header("STATE MACHINE")]

    //Posibles estados del enemigo
    private enum enemyState {Waiting, Searching, Moving, Attacking, Ended}

    //Estado actual del enemigo
    private enemyState myCurrentEnemyState;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector
    [SerializeField]
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

        myCurrentEnemyState = enemyState.Waiting;
        currentHealth = maxHealth;
    }

    #endregion

    #region ENEMY_STATE

    public void MyTurnStart()
    {
        myCurrentEnemyState = enemyState.Searching;
    }

    private void Update()
    {
        switch (myCurrentEnemyState)
        {
            case (enemyState.Waiting):
                break;

            case (enemyState.Searching):
                SearchingObjectivesToAttack();
                break;

            case (enemyState.Moving):
                MoveUnit();
                break;

            case (enemyState.Attacking):
                Attack();
                break;

            case (enemyState.Ended):
                FinishMyActions();
                break;
        }
    }

    public virtual void SearchingObjectivesToAttack()
    {
        //myCurrentEnemyState = enemyState.Searching;
        myCurrentEnemyState = enemyState.Ended;
    }


    public virtual void MoveUnit()
    {
        //myCurrentEnemyState = enemyState.Searching;
    }


    public virtual void Attack()
    {
        //myCurrentEnemyState = enemyState.Searching;
    }

    public virtual void FinishMyActions()
    {
        LM.NextEnemyInList();
        myCurrentEnemyState = enemyState.Waiting;
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

    #region DAMAGE

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

    #region CHECKS

    //Esta función es exactamente igual que la del player con la excepción de que solo tiene en cuenta a personajes del jugador e ignora enemigos.
    private void CheckCharactersInLine()
    {
        currentUnitsAvailableToAttack.Clear();

        if (currentFacingDirection == FacingDirection.North || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<EnemyUnit>() && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<EnemyUnit>() && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<EnemyUnit>() && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West || GetComponent<Rogue>())
        {
            if (range <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<EnemyUnit>() && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    break;
                }
            }
        }

        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            Debug.Log(currentUnitsAvailableToAttack[i].name);
        }
    }

    #endregion
}
