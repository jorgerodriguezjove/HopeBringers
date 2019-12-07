using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    [Header("STATE MACHINE")]

    //Rango de acción del enemigo
    [SerializeField]
    public int initialRangeOfAction;

    [SerializeField]
    private float timeWaitingMovement;
    [SerializeField]
    private float timeWaitingAttacking;
    [SerializeField]
    private float timeWaitingEnded;

    //Estado actual del enemigo
    [SerializeField]
    protected enemyState myCurrentEnemyState;

    //Posibles estados del enemigo
    protected enum enemyState {Waiting, Searching, Moving, Attacking, Ended}

    //Posibles estados del enemigo
    protected enum TierLevel { LevelBase1, Level2 }

    [SerializeField]
    protected TierLevel myTierLevel;

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

    //Bool que sirve para que la corrutina solo se llame una vez (por tema de que el state machine esta en el update y si no lo haría varias veces)
    private bool corroutineDone;

    //Bool que indica si el enemigo ha sido despertado o si solo tiene que comprobar su rango inicial.
    [SerializeField]
    protected bool haveIBeenAlerted;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector (Ya está cambiado)
    [SerializeField]
    public GameObject LevelManagerRef;
    protected LevelManager LM;


    [Header("FEEDBACK")]
    //Flecha que indica que enemigo está realizando su acción.
    [SerializeField]
    private GameObject arrowEnemyIndicator;

    [SerializeField]
    private Material selectedMaterial;

    //Referencia al retrato en la lista
    [HideInInspector]
    public EnemyPortraits myPortrait;

    #endregion


    #region INIT

    private void Awake()
    {
        //Le digo al enemigo cual es el LevelManager del nivel actual
        LevelManagerRef = FindObjectOfType<LevelManager>().gameObject;

        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.enemiesOnTheBoard.Add(this);
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
        myPortrait.HighlightMyself();
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
                arrowEnemyIndicator.SetActive(true);
                SearchingObjectivesToAttack();

                break;

            case (enemyState.Moving):
                if (!corroutineDone)
                {
                    StartCoroutine("WaitBeforeNextState");
                }
                break;

            case (enemyState.Attacking):
                if (!corroutineDone)
                {
                    StartCoroutine("WaitBeforeNextState");
                }
                break;

            case (enemyState.Ended):
                if (!corroutineDone)
                {
                    
                    StartCoroutine("WaitBeforeNextState");
                   
                }
                break;
        }


        //if (currentUnitsAvailableToAttack.Count == 0)
        //{
        //    Debug.Log("EMPTY");
        //}
    }

    IEnumerator WaitBeforeNextState()
    {
        corroutineDone = true;

        if (myCurrentEnemyState == enemyState.Moving)
        {
            yield return new WaitForSeconds(timeWaitingMovement);
            MoveUnit();
        }

        else if (myCurrentEnemyState == enemyState.Attacking)
        {
            yield return new WaitForSeconds(timeWaitingAttacking);
            Attack();
        }

        else if (myCurrentEnemyState == enemyState.Ended)
        {
            yield return new WaitForSeconds(timeWaitingEnded);
            arrowEnemyIndicator.SetActive(false);
            FinishMyActions();
        }

        corroutineDone = false;
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

    //Función que se encarga de hacer que el personaje este despierto/alerta
    public void AlertEnemy()
    {
        haveIBeenAlerted = true;
        initialRangeOfAction = 1000;
    }


    //Para acabar el turno de la unnidad
    public virtual void FinishMyActions()
    {
        hasMoved = false;
        hasAttacked = false;
        myCurrentEnemyState = enemyState.Waiting;
        myPortrait.UnHighlightMyself();
        LM.NextEnemyInList();
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        if (LM.selectedCharacter != null)
        {
            LM.SelectUnitToAttack(GetComponent<UnitBase>());
        }
        
        else
        {
            if (!isDead)
            {
                if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                {
                    if (LM.selectedEnemy != null)
                    {
                        LM.HideEnemyHover(LM.selectedEnemy);
                        //Llamo a LevelManager para desactivar hover
                        if (LM.selectedCharacter != null)
                        {
                            LM.selectedCharacter.HideDamageIcons();
                        }
                        LM.HideHover(LM.selectedEnemy);
                        LM.selectedEnemy.HealthBarOn_Off(false);
                        LM.UIM.HideUnitInfo("");
                        //LM.UIM.HideCharacterInfo("");
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        LM.tilesAvailableForMovement.Clear();

                        LM.DeSelectUnit();

                        if (!haveIBeenAlerted)
                        {
                            LM.ShowEnemyHover(initialRangeOfAction, this);
                        }
                        else
                        {
                            LM.ShowEnemyHover(movementUds, this);
                        }
                       
                        LM.selectedEnemy = this;
                        //Llamo a LevelManager para activar hover

                        LM.CheckIfHoverShouldAppear(this);
                        LM.UIM.ShowUnitInfo(this.unitInfo, this);
                        //LM.UIM.ShowCharacterInfo(LM.selectedEnemy.unitInfo, LM.selectedEnemy);
                        HealthBarOn_Off(true);
                        gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();
                    }
                    else
                    {
                        LM.DeSelectUnit();

                        if (!haveIBeenAlerted)
                        {
                            LM.ShowEnemyHover(initialRangeOfAction, this);
                        }
                        else
                        {
                            LM.ShowEnemyHover(movementUds, this);
                        }

                        LM.selectedEnemy = this;
                        //Llamo a LevelManager para activar hover

                        LM.CheckIfHoverShouldAppear(this);
                        LM.UIM.ShowUnitInfo(this.unitInfo, this);
                        //LM.UIM.ShowCharacterInfo(LM.selectedEnemy.unitInfo, LM.selectedEnemy);
                        HealthBarOn_Off(true);
                        gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();
                    }
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy == null && LM.selectedCharacter == null)
            {
                if (!isDead)
                {
                    OnHoverEnterFunctionality();
                }
            }
            else if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                if (!isDead)
                {
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                    LM.CheckIfHoverShouldAppear(this);
                    HealthBarOn_Off(true);

                }
            }
        }
	}

    //Creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public void OnHoverEnterFunctionality()
    {
        //Muestro el rango de acción del personaje.
        if (!haveIBeenAlerted)
        {
            LM.ShowEnemyHover(initialRangeOfAction, this);
        }
        else
        {
            LM.ShowEnemyHover(movementUds, this);
        }

        //Llamo a LevelManager para activar hover				
        LM.UIM.ShowUnitInfo(this.unitInfo, this);

        //LM.UIM.ShowCharacterInfo(unitInfo, this); 
        HealthBarOn_Off(true);
        gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

        //Cambio el color del personaje
        SelectedColor();

        myPortrait.HighlightMyself();
    }


    private void OnMouseExit()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy == null)
            {
                OnHoverExitFunctionality();
            }
        }
    }

    //Al igual que con enter creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public void OnHoverExitFunctionality()
    {
        LM.HideEnemyHover(this);
        //Llamo a LevelManager para desactivar hover
        if (LM.selectedCharacter != null)
        {
            LM.selectedCharacter.HideDamageIcons();
        }
        LM.HideHover(this);
        HealthBarOn_Off(false);
        LM.UIM.HideUnitInfo("");
        //LM.UIM.HideCharacterInfo("");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        ResetColor();


        myPortrait.UnHighlightMyself();

    }

    public void SelectedColor()
    {
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
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

        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación, sonido y partículas de muerte
        myAnimator.SetTrigger("Death");
        SoundManager.Instance.PlaySound(AppSounds.EN_DEATH);
        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        LM.HideHover(this);
        HealthBarOn_Off(false);
		LM.UIM.HideTileInfo();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //Cambios en la lógica para indicar que ha muerto
        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();
        
        Destroy(unitModel);

        
        isDead = true;
        LM.UIM.SetEnemyOrder();
        //Tiene que ir despues del bool de isdead = true



        //No uso FinishMyActions porque no me interesa que pase turno, sólo que  se quede en waiting por si acaso se muere en su turno.
        myCurrentEnemyState = enemyState.Waiting;

    }

    #endregion

    #region CHECKS

    //Esta función es el equivalente al chequeo de objetivos del jugador.Charger y balista usan versiones diferentes por eso el virtual. Es distinta de la del player y en principio no se puede reutilizar la misma debido a estas diferencias.
    protected virtual void CheckCharactersInLine()
    {
        
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
