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
    private float timeWaitingBeforeMovement;
    //Tiempo a esperar tras moverse
    [SerializeField]
    protected float timeWaitAfterMovement;
    [SerializeField]
    private float timeWaitingAttacking;
    [SerializeField]
    private float timeWaitingEnded;

    //La variable de time for movement que esta en unitbase determina el tiempo que tarda por tile al moverse.
    //Esta variable sirve para que cuando le de al skip el tiempo pase a ser 0 para que vaya rápido
    protected float currentTimeForMovement;

    //La variable timeWaitingAttacking ESTA MAL NOMBRADA y sirve de espera después de moverse haya atacado o no
    //Rsta variable al igual que currentTimeForMovement sirve para acelerar el turno enemigo
    protected float currentTimeWaitinAttacking;

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

	[Header("INFO")]

	[@TextAreaAttribute(15, 20)]
	public string enemyTierInfo;
	[SerializeField]
	public Sprite enemyTierImage;

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
    public LineRenderer myLineRenderer;

    //Bool que sirve para indicar si el tile que pinta para indicar el ataque ya estaba antes de pintarse bajo ataque para que al despintarlo se quede como estaba.
    protected List<bool> wereTilesAlreadyUnderAttack = new List<bool>();
    protected List<IndividualTiles> tilesAlreadyUnderAttack = new List<IndividualTiles>();

    //Referencia al gameobject que actua como hover de los enemigos.
    [SerializeField]
    public GameObject shaderHover;

    [SerializeField]
    private GameObject sleepParticle;

	//Variables del doble click
	int clicked;
	float clickTime;
	float clickDelay = 0.5f;

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

        currentTimeForMovement = timeMovementAnimation;
        currentTimeWaitinAttacking = timeWaitingAttacking;
    }

   
    #endregion

    #region ENEMY_STATE

    public void MyTurnStart()
    {
        if (myPortrait !=null)
        {
            myPortrait.HighlightMyself();
        }       
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
            yield return new WaitForSeconds(timeWaitingBeforeMovement);
            MoveUnit();
        }

        else if (myCurrentEnemyState == enemyState.Attacking)
        {
            yield return new WaitForSeconds(currentTimeWaitinAttacking);
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


       
        CalculateDamage(currentUnitsAvailableToAttack[0]);
        currentUnitsAvailableToAttack[0].ColorAvailableToBeAttacked(damageWithMultipliersApplied);
        
    }

    //Función que se encarga de hacer que el personaje este despierto/alerta
    public void AlertEnemy()
    {
        haveIBeenAlerted = true;
        Destroy(sleepParticle);
        rangeOfAction = 1000;
    }

    //Función que se encarga de pintar el line renderer y el tile de ataque
    public virtual void ShowActionPathFinding(bool shouldRecalculate)
    {
        //Cada enemigo realiza su propio path
    }

    public void HideActionPathfinding()
    {
        myLineRenderer.enabled = false;
        shaderHover.SetActive(false);

        for (int i = 0; i < tilesAlreadyUnderAttack.Count; i++)
        {
            if (!wereTilesAlreadyUnderAttack[i])
            {
                tilesAlreadyUnderAttack[i].ColorDesAttack();
                tilesAlreadyUnderAttack[i].unitOnTile.previsualizeAttackIcon.SetActive(false);
                
            }
        }

        wereTilesAlreadyUnderAttack.Clear();
        tilesAlreadyUnderAttack.Clear();
    }

    public virtual void ColorAttackTile()
    {
        //El goblin y el gigante lo usan para pintar el tile al que van a atacar al mostrar show action
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public virtual void SearchingObjectivesToAttackShowActionPathFinding()
    {
        //Cada enemigo realiza su propioa búsqueda
    }

    //Para acabar el turno de la unnidad
    public virtual void FinishMyActions()
    {
        LM.HideEnemyHover(GetComponent<EnemyUnit>());
        hasMoved = false;
        hasAttacked = false;
        myCurrentEnemyState = enemyState.Waiting;

        //Me aseguro de que el tiempo de movimiento vuelve a la normalidad por si le ha dado a acelerar
        currentTimeForMovement = timeMovementAnimation;
        currentTimeWaitinAttacking = timeWaitingAttacking;

        if (myPortrait != null)
        {
            myPortrait.UnHighlightMyself();
        }
        
        LM.NextEnemyInList();
    }

    public void SkipAnimation()
    {
        currentTimeForMovement = 0;
        currentTimeWaitinAttacking = 0;
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
                    LM.SelectEnemy(GetComponent<EnemyUnit>().unitGeneralInfo, GetComponent<EnemyUnit>());
                }
            }
        }
		//Doble click
		clicked++;
		if(clicked == 1)
		{
			clickTime = Time.time;
		}
		if(clicked > 1 && (Time.time - clickTime) < clickDelay)
		{
			clicked = 0;
			clickTime = 0;
			LM.UIM.EnemyCameraFocus(this);
			//Focus camera
		}
		else if (clicked > 2 || Time.time - clickTime > 1)
		{
			clicked = 0;
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
                    LM.selectedCharacter.HideDamageIcons(this);
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
                LM.UIM.ShowUnitInfo(GetComponent<EnemyUnit>().unitGeneralInfo, GetComponent<EnemyUnit>());
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
                if (!isDead)
                {
                    //Llamo a LevelManager para activar hover				
                    //LM.UIM.ShowUnitInfo(this.unitInfo, this);

                    //LM.UIM.ShowCharacterInfo(unitInfo, this); 
                    HealthBarOn_Off(true);
                    //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

                    myPortrait.HighlightMyself();

                    //Cambio el color del personaje
                    SelectedColor();
                }
            }
            else if (LM.selectedEnemy != null && LM.selectedEnemy != this)
            {
                if (!isDead)
                {
                    //Llamo a LevelManager para activar hover				
                    //LM.UIM.ShowUnitInfo(this.unitInfo, this);

                    //LM.UIM.ShowCharacterInfo(unitInfo, this); 
                    HealthBarOn_Off(true);
                    //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

                    myPortrait.HighlightMyself();

                    //Cambio el color del personaje
                    SelectedColor();
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
            //Pinto el rango de acción y de movimiento
            LM.ShowEnemyHover(rangeOfAction, true ,this);
        }

        //Pinto únicamente el rango de movimiento
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
        LM.UIM.ShowUnitInfo(this.unitGeneralInfo, this);

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
                ResetColor();
                HealthBarOn_Off(false);
                LM.UIM.ShowUnitInfo(LM.selectedEnemy.unitGeneralInfo, LM.selectedEnemy);
                //LM.UIM.HideUnitInfo("");

               

                myPortrait.UnHighlightMyself();

            }

        }
    }

    //Al igual que con enter creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public void OnHoverExitFunctionality()
    {
        LM.HideEnemyHover(this);

        

        if (LM.selectedEnemy == null)
        {
            LM.UIM.HideUnitInfo("");
            if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                ResetColor();
                myPortrait.UnHighlightMyself();
                LM.HideHover(this);
                HealthBarOn_Off(false);
            }


          

        }
        else
        {
            if (LM.selectedEnemy != this)
            {
                LM.HideHover(this);
                HealthBarOn_Off(false);
            }

            LM.UIM.HideUnitInfo("");
            LM.UIM.ShowUnitInfo(LM.selectedEnemy.unitGeneralInfo, LM.selectedEnemy);
            LM.selectedCharacter.HideDamageIcons(this);
            myCurrentTile.ColorDesAttack();
            previsualizeAttackIcon.SetActive(false);
        }
        
        //LM.UIM.HideCharacterInfo("");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

		
        
		//LM.UIM.HideCharacterInfo("");
		if (LM.selectedCharacter == null)
		{
			LM.UIM.HideUnitInfo("");
		}

		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		if(LM.selectedCharacter != null)
		{
			LM.UIM.ShowUnitInfo(LM.selectedCharacter.unitGeneralInfo, LM.selectedCharacter);
            
        }

        //if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Count > 0 && LM.selectedCharacter.currentUnitsAvailableToAttack[0] == GetComponent<EnemyUnit>())
        //{
        //    Debug.Log("rojo");
        //}

        else
        {
            ResetColor();
            LM.HideHover(this);
            HealthBarOn_Off(false);
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

        //Cambios en UI
        LM.HideHover(this);
        HealthBarOn_Off(false);
		LM.UIM.HideTileInfo();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //Cambios en la lógica para indicar que ha muerto
        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();

        //Hago que visualmente desaparezca aunque no lo destryuo todavía.
        unitModel.SetActive(false);
        if (sleepParticle != null)
        {
            sleepParticle.SetActive(false);
        }
        GetComponent<Collider>().enabled = false;

        //Aviso de que el enemigo está muerto
        isDead = true;

        //Estas dos llamadas tienen que ir despues del bool de isdead = true
        LM.UIM.SetEnemyOrder();

        //No uso FinishMyActions porque no me interesa que pase turno, sólo que se quede en waiting por si acaso se muere en su turno.
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

    public override void UndoMove(IndividualTiles tileToMoveBack, FacingDirection rotationToTurnBack, bool shouldResetMovement)
    {
        if (isDead)
        {
            ////Cambios en UI
            //LM.HideHover(this);
            //HealthBarOn_Off(false);
            //LM.UIM.HideTileInfo();
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            myAnimator.Play("Idle");

            //Cambios en la lógica para indicar que ha muerto
            myCurrentTile.unitOnTile = GetComponent<UnitBase>();
            myCurrentTile.WarnInmediateNeighbours();

            //Hago que visualmente desaparezca aunque no lo destryuo todavía.
            unitModel.SetActive(true);
            GetComponent<Collider>().enabled = true;

            //Aviso de que el enemigo está muerto
            isDead = false;

            //Estas dos llamadas tienen que ir despues del bool de isdead = true
            LM.UIM.SetEnemyOrder();

            //No uso FinishMyActions porque no me interesa que pase turno, sólo que se quede en waiting por si acaso se muere en su turno.
            myCurrentEnemyState = enemyState.Waiting;
        }


        base.UndoMove(tileToMoveBack, rotationToTurnBack, shouldResetMovement);
    }

    public override void UndoAttack(int previousHealth)
    {
        base.UndoAttack(previousHealth);

        RefreshHealth(true);
    }

}
