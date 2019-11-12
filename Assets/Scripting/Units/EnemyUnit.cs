using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    [Header("STATE MACHINE")]

    //Estado actual del enemigo
    [SerializeField]
    protected enemyState myCurrentEnemyState;

    //Posibles estados del enemigo
    protected enum enemyState {Waiting, Searching, Moving, Attacking, Ended}

    //Distancia en tiles con el enemigo más lejano
    protected int furthestAvailableUnitDistance;

    //Bool que comprueba si la balista se ha movido
    protected bool hasMoved = false;

    protected bool hasAttacked = false;

    //Orden en la lista de enemigos. Según su velocidad cambiará el orden en el que actúa.
    [HideInInspector]
    public int orderToShow;

    [SerializeField]
    public GameObject thisUnitOrder;

    [HideInInspector]
    public List<UnitBase> currentUnitsAvailableToAttack;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector
    [SerializeField]
    public GameObject LevelManagerRef;
    protected LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
		//Referencia al LM y me incluyo en la lista de enemiogos
		LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.enemiesOnTheBoard.Add(this);
        myCurrentTile.unitOnTile = this;
        myCurrentTile.WarnInmediateNeighbours();
        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        myCurrentEnemyState = enemyState.Waiting;
        currentHealth = maxHealth;

        movementParticle.SetActive(false);
    }

    #endregion

    #region ENEMY_STATE

    public void MyTurnStart()
    {
        myCurrentEnemyState = enemyState.Searching;
    }

    private void Update()
    {
        //Debug.Log(myCurrentEnemyState);

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


        //if (currentUnitsAvailableToAttack.Count == 0)
        //{
        //    Debug.Log("EMPTY");
        //}
    }

    public virtual void SearchingObjectivesToAttack()
    {
        //Cada enemigo busca enemigos a su manera
    }


    public virtual void MoveUnit()
    {
       //Acordarse de que cada enemigo debe actualizar al moverse los tiles (vacíar el tile anterior y setear el nuevo tile y la unidad del nuevo tile)
    }


    public virtual void Attack()
    {
        //Cada enemigo realiza su propio ataque
    }

    //Para acabar el turno de la unnidad
    public virtual void FinishMyActions()
    {
        hasMoved = false;
        hasAttacked = false;
        myCurrentEnemyState = enemyState.Waiting;
        LM.NextEnemyInList();
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
        //Llamo a LevelManager para activar hover
        LM.CheckIfHoverShouldAppear(this);
		LM.UIM.ShowTooltip(unitInfo);
		HealthBarOn_Off(true);
		gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();
    }

    private void OnMouseExit()
    {
        //Llamo a LevelManager para desactivar hover
        LM.HideHover(this);
		HealthBarOn_Off(false);
		LM.UIM.ShowTooltip("");
	}

    #endregion

    #region DAMAGE

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        currentHealth -= damageReceived;

        Debug.Log("Soy " + gameObject.name + "y me han hecho " + damageReceived + " de daño");
        Debug.Log("Mi vida actual es " + currentHealth);

        myAnimator.SetTrigger("Damage");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación, sonido y partículas de muerte
        myAnimator.SetTrigger("Death");
        SoundManager.Instance.PlaySound(AppSounds.EN_DEATH);
        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        //Cambios en la lógica para indicar que ha muerto

        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();
        
        Destroy(unitModel);
        isDead = true;
    }

    #endregion

    #region CHECKS

    //Esta función es el equivalente al chequeo de objetivos del jugador. Es distinta y en principio no se puede reutilizar la misma debido a estas diferencias.
    protected void CheckCharactersInLine()
    {
        if (!isDead)
        {
            currentUnitsAvailableToAttack.Clear();

            if (currentFacingDirection == FacingDirection.North || GetComponent<EnCharger>())
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
                    //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                    if (myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        break;
                    }

                    //Sólo el charger para si encuentra un tile empty o con unidad
                    else if (GetComponent<EnCharger>() && (myCurrentTile.tilesInLineUp[i].isEmpty || (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<EnemyUnit>())))
                    {
                        break;
                    }

                    //Independientemente de que sea charger o balista este código sirve para ambos
                    else if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades.
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                        furthestAvailableUnitDistance = i;

                        break;
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.East || GetComponent<EnCharger>())
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
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                    {
                        //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                        if (myCurrentTile.tilesInLineRight[i].isObstacle)
                        {
                            break;
                        }

                        //Sólo el charger para si encuentra un tile empty o con unidad
                        else if (GetComponent<EnCharger>() && (myCurrentTile.tilesInLineRight[i].isEmpty || (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<EnemyUnit>())))
                        {
                            break;
                        }

                        //Independientemente de que sea charger o balista este código sirve para ambos

                        //Si la distancia es mayor que la distancia con el enemigo ya guardado, me deshago de la unidad anterior y almaceno esta cómo objetivo.
                        else if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                        {
                            currentUnitsAvailableToAttack.Clear();
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                            furthestAvailableUnitDistance = i;
                        }

                        //Si tienen la misma distancia almaceno a las dos
                        else if (furthestAvailableUnitDistance == i)
                        {
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        }

                        break;
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.South || GetComponent<EnCharger>())
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
                    if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                    {
                        //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                        if (myCurrentTile.tilesInLineDown[i].isObstacle)
                        {
                            break;
                        }

                        //Sólo el charger para si encuentra un tile empty o con unidad
                        else if (GetComponent<EnCharger>() && (myCurrentTile.tilesInLineDown[i].isEmpty || (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<EnemyUnit>())))
                        {
                            break;
                        }

                        //Independientemente de que sea charger o balista este código sirve para ambos

                        //Si la distancia es mayor que la distancia con el enemigo ya guardado, me deshago de la unidad anterior y almaceno esta cómo objetivo.
                        if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                        {
                            currentUnitsAvailableToAttack.Clear();
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                            furthestAvailableUnitDistance = i;
                        }

                        //Si tienen la misma distancia almaceno a las dos
                        else if (furthestAvailableUnitDistance == i)
                        {
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                        }

                        break;
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.West || GetComponent<EnCharger>())
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
                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                    {
                        //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                        if (myCurrentTile.tilesInLineLeft[i].isObstacle)
                        {
                            break;
                        }

                        //Sólo el charger para si encuentra un tile empty o con unidad
                        else if (GetComponent<EnCharger>() && (myCurrentTile.tilesInLineLeft[i].isEmpty || (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<EnemyUnit>())))
                        {
                            break;
                        }

                        //Independientemente de que sea charger o balista este código sirve para ambos

                        //Si la distancia es mayor que la distancia con el enemigo ya guardado, me deshago de la unidad anterior y almaceno esta cómo objetivo.
                        if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                        {
                            currentUnitsAvailableToAttack.Clear();
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                            furthestAvailableUnitDistance = i;
                        }

                        //Si tienen la misma distancia almaceno a las dos
                        else if (furthestAvailableUnitDistance == i)
                        {
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        }
                        break;
                    }
                }
            }
        }
    }

        

    /// <summary>
    /// Adaptando la función de pathfinding del Tile Manager usamos eso para al igual que hicimos con el charger guardar los enemigos con la menor distancia
    /// (En esta funcion la distancia equivale al coste que va sumando en la función para calcular el path)</summary>
    ///  Una vez guardados los enemigos determinamos las reglas para eligr al que atacamos
    ///  Una vez elegido calculamos a donde tiene que moverse de forma manual (para poder hacer que se choque con los bloques (movimiento tonto))
    ///  En el caso del goblin es igual salvo que este ultimo paso no se hace de forma mnaual si no que usamos una función parecida a la que llama el levelmanager para
    ///  pedir el path del movimiento del jugador.
    /// <summary>


    #endregion
}
