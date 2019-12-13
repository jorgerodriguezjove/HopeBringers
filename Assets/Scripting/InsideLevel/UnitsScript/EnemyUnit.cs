using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    [Header("STATE MACHINE")]

    //Rango de acción del enemigo
    [SerializeField]
    public int rangeOfAction;

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
    public enum TierLevel { LevelBase1, Level2 }

    [SerializeField]
    public TierLevel myTierLevel;

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
    [HideInInspector]
    public bool haveIBeenAlerted = false;

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

    //Referencia al LineRenderer hijo para indicar el movimiento del enemigo
    [SerializeField]
    protected LineRenderer myLineRenderer;

    //Referencia al gameobject que actua como hover de los enemigos.
    [SerializeField]
    public GameObject shaderHover;

    [SerializeField]
    private GameObject sleepParticle;

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
        Destroy(sleepParticle);
        rangeOfAction = 1000;
    }

    //Función que se encarga de pintar el line renderer y el tile de ataque
    public virtual void ShowActionPathFinding(bool shouldToShow)
    {
        //Cada enemigo realiza su propio path
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public virtual void SearchingObjectivesToAttackShowActionPathFinding()
    {
        //Cada enemigo realiza su propioa búsqueda
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
                    LM.SelectEnemy(GetComponent<EnemyUnit>().unitInfo, GetComponent<EnemyUnit>());
                }
            }
        }
    }


    //Función que guarda todo lo que ocurre cuando se selecciona un personaje. Esta función sirve para no repetir codigo y además para poder llamarla desde el Level Manager.
    public void SelectedFunctionality()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy != null && LM.selectedEnemy != GetComponent<EnemyUnit>())
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
            }

            else
            {
                LM.DeSelectUnit();

                if (!haveIBeenAlerted)
                {
                    LM.ShowEnemyHover(rangeOfAction, true, this);
                }
                else
                {
                    LM.ShowEnemyHover(movementUds, false, this);
                }

                LM.selectedEnemy = GetComponent<EnemyUnit>();

                LM.CheckIfHoverShouldAppear(GetComponent<EnemyUnit>());
                LM.UIM.ShowUnitInfo(GetComponent<EnemyUnit>().unitInfo, GetComponent<EnemyUnit>());
                myPortrait.HighlightMyself();

                //Activo la barra de vida
                HealthBarOn_Off(true);

                //Cambio el color del personaje
                SelectedColor();
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
					LM.UIM.ShowUnitInfo(LM.selectedCharacter.attackInfo, LM.selectedCharacter);
                    LM.CheckIfHoverShouldAppear(this);
                    HealthBarOn_Off(true);
                }
            }
            else if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                //Llamo a LevelManager para activar hover				
                LM.UIM.ShowUnitInfo(this.unitInfo, this);

                //LM.UIM.ShowCharacterInfo(unitInfo, this); 
                HealthBarOn_Off(true);
                //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

                myPortrait.HighlightMyself();

                //Cambio el color del personaje
                SelectedColor();
            }
            else if (LM.selectedEnemy != null && LM.selectedEnemy != this)
            {
                //Llamo a LevelManager para activar hover				
                LM.UIM.ShowUnitInfo(this.unitInfo, this);

                //LM.UIM.ShowCharacterInfo(unitInfo, this); 
                HealthBarOn_Off(true);
                //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

                myPortrait.HighlightMyself();

                //Cambio el color del personaje
                SelectedColor();
            }
        }
    }

    //Creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public void OnHoverEnterFunctionality()
    {
        //Muestro el rango de acción del personaje.
        if (!haveIBeenAlerted)
        {
            LM.ShowEnemyHover(rangeOfAction, true ,this);
        }
        else
        {
            LM.ShowEnemyHover(movementUds, false ,this);
        }

        //if (hoverUnit.GetComponent<EnemyUnit>().myTierLevel == EnemyUnit.TierLevel.LevelBase1)
        //{
        //    for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
        //    {
        //        tilesAvailableForMovementEnemies[i].ColorSelect();
        //    }
        //}

        //else if (hoverUnit.GetComponent<EnemyUnit>().myTierLevel == EnemyUnit.TierLevel.Level2 && hoverUnit.GetComponent<EnemyUnit>().isAlerted)
        //{
        //    for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
        //    {
        //        tilesAvailableForMovementEnemies[i].ColorActionRange();
        //    }
        //}

        //Llamo a LevelManager para activar hover				
        LM.UIM.ShowUnitInfo(this.unitInfo, this);

		//LM.UIM.ShowCharacterInfo(unitInfo, this); 
		HealthBarOn_Off(true);
        //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

        myPortrait.HighlightMyself();

        //Cambio el color del personaje
        SelectedColor();
    }

    private void OnMouseExit()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy == null)
            {
                OnHoverExitFunctionality();
            }

            else if (LM.selectedEnemy != null && LM.selectedEnemy != this)
            {
                HealthBarOn_Off(false);
                LM.UIM.HideUnitInfo("");

                ResetColor();

                myPortrait.UnHighlightMyself();

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

		
        LM.HideHover(this);
        HealthBarOn_Off(false);
		//LM.UIM.HideCharacterInfo("");
		if (LM.selectedCharacter == null)
		{
			LM.UIM.HideUnitInfo("");
		}

		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		if(LM.selectedCharacter != null)
		{
			LM.UIM.ShowUnitInfo(LM.selectedCharacter.unitInfo, LM.selectedCharacter);
		}

        if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Count > 0 && LM.selectedCharacter.currentUnitsAvailableToAttack[0] == GetComponent<EnemyUnit>())
        {
            Debug.Log("rojo");
            
        }

        else
        {
            Debug.Log("reset");
            ResetColor();
        }
		

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
    public virtual void CheckCharactersInLine()
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
