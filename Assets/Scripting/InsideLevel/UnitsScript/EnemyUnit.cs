using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    [Header("STATE MACHINE")]

    //Rango de acción del enemigo
    [SerializeField]
    public int rangeOfAction;

    //Tiempo antes de empezar a buscar, antes de empezar a moverse, antes de hacer la animación de ataque y antes de pasar al siguiente enemigo
    //Estas variables solo guardan el valor en el editor para ponerselo a las variables current
    [SerializeField]
    protected float timeWaitingBeforeStarting;
    [SerializeField]
    protected float timeWaitingBeforeMovement;
    [SerializeField]
    protected float timeWaitingBeforeAttacking;
    [SerializeField]
    protected float timeWaitingBeforeEnding;

    //La variable de time for movement que esta en unitbase determina el tiempo que tarda por tile al moverse.
    //Esta variable sirve para que cuando le de al skip el tiempo pase a ser 0 para que vaya rápido
    protected float currentTimeForMovement;

    //Estas variables son las que de verdad se usan en las funciones y pueden valer 0 por el skip o el valor de su version sin el current
    protected float currentTimeWaitingBeforeStarting;
    protected float currentTimeWaitinBeforeMovement;
    protected float currentTimeWaitinBeforeAttacking;
    protected float currentTimeWaitingBeforeEnding;

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

    //Tile donde se coloca la sombra y que uso de referencia para pintar el rango del enemigo si se mueve desde la somra.
    public IndividualTiles shadowTile;

    //Bool que sirve para que la corrutina solo se llame una vez (por tema de que el state machine esta en el update y si no lo haría varias veces)
    private bool corroutineDone;

    //Bool que indica si el enemigo va a ser despertado cuando llegue el turno enemigo. 
    [HideInInspector]
    public bool isGoingToBeAlertedOnEnemyTurn = false;

    //Bool que indica si el enemigo ha sido despertado o si solo tiene que comprobar su rango inicial.
    [SerializeField]
    public bool haveIBeenAlerted = false;

    [Header ("GOBLIN COMMON VARIABLES")]

    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    protected UnitBase myCurrentObjective;
    protected IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    public List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    //Lista que guarda los enmeigos y personajes que están dentro del rango de alerta del personaje (ya sea para comprobar personajes o alertar a enemigos)
    //LA USAN TODOS LOS ENEMIGOS MENOS EL CHARGER Y LA BALISTA
    [HideInInspector]
    public List<UnitBase> unitsInRange = new List<UnitBase>();

    //Se usa para moverse el número de tiles que toque (si el máximo o menos debido a obstaculos u otras cosas.
    protected int limitantNumberOfTilesToMove;

    //Bool que indica si almenos una de las unidades encontradas en rango de acción es un player
    protected bool keepSearching;

    [Header("REFERENCIAS")]

    //Ahora mismo se setea desde el inspector (Ya está cambiado)
    [SerializeField]
    public GameObject LevelManagerRef;
    protected LevelManager LM;
    protected UIManager UIM;

    [Header("INFO")]

	[@TextAreaAttribute(15, 20)]
	public string enemyTierInfo;
	[SerializeField]
	public Sprite enemyTierImage;

    [Header("ONLY BOSS")]
    [SerializeField]
    public int numberOfAttackTokens;
    [HideInInspector]
    public PortraitBoss bossPortrait;

    [SerializeField]
    private GameObject interrogationParticle;

    [Header("FEEDBACK")]

    //Flecha que indica que enemigo está realizando su acción.
    [SerializeField]
    private GameObject arrowEnemyIndicator;

    [SerializeField]
    protected Material selectedMaterial;

    //Referencia al retrato en la lista
    [HideInInspector]
    public EnemyPortraits myPortrait;

    //Referencia al LineRenderer hijo para indicar el movimiento del enemigo
    [SerializeField]
    public LineRenderer myLineRenderer;

    //Bool que sirve para indicar si el tile que pinta para indicar el ataque ya estaba antes de pintarse bajo ataque para que al despintarlo se quede como estaba.
    protected List<bool> wereTilesAlreadyUnderAttack = new List<bool>();
    protected List<IndividualTiles> tilesAlreadyUnderAttack = new List<IndividualTiles>();

    [SerializeField]
    private GameObject sleepParticle;

    [SerializeField]
    public GameObject exclamationIcon;

    //Variables del doble click
    int clicked;
	float clickTime;
	float clickDelay = 0.5f;

    #endregion

    #region INIT

    //El dark lord hace OVERRIDE
    protected virtual void Awake()
    {
        //Le digo al enemigo cual es el LevelManager del nivel actual
        LevelManagerRef = FindObjectOfType<LevelManager>().gameObject;
        UIM = FindObjectOfType<UIManager>();

        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.enemiesOnTheBoard.Add(this);

        if (GetComponent<MechaBoss>() || GetComponent<DarkLord>() || GetComponent<BossMultTile>())
        {
            bossPortrait = FindObjectOfType<PortraitBoss>();
        }

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        myCurrentEnemyState = enemyState.Waiting;
        currentHealth = maxHealth;

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        currentTimeForMovement = timeMovementAnimation;

        currentTimeWaitingBeforeStarting = timeWaitingBeforeStarting;
        currentTimeWaitinBeforeMovement = timeWaitingBeforeMovement;
        currentTimeWaitinBeforeAttacking = timeWaitingBeforeAttacking;
        currentTimeWaitingBeforeEnding = timeWaitingBeforeEnding;

		if(characterImage != null && inGamePortrait != null)
		{
			inGamePortrait.sprite = characterImage;
		}

        fMovementUds = movementUds;
    }

   
    #endregion

    #region ENEMY_STATE

    public void MyTurnStart()
    {
        if (myPortrait !=null)
        {
            myPortrait.HighlightMyself();
        }

        //Compruebo si los tiles de daño tienen que hacer daño. 
        for (int i = 0; i < LM.damageTilesInBoard.Count; i++)
        {
            if (LM.damageTilesInBoard[i].unitToDoDamage != null
                && LM.damageTilesInBoard[i].unitToDoDamage.GetComponent<EnemyUnit>() != null
                && LM.damageTilesInBoard[i].unitToDoDamage.GetComponent<EnemyUnit>() == this)
            {


                LM.damageTilesInBoard[i].CheckHasToDoDamage();
                LM.damageTilesInBoard[i].damageDone = true;
                break;
            }
        }

        StartCoroutine("WaitBeforeNextState");
    }

    private void Update()
    {
        switch (myCurrentEnemyState)
        {
            case (enemyState.Waiting):
                break;

            case (enemyState.Searching):

                //Aqui no hay wait, porque se tiene que esperar antes de empezar a buscar, no con cada busqueda.

                arrowEnemyIndicator.SetActive(true);


                turnsWithBuffOrDebuff--;
                if (turnsWithBuffOrDebuff<=0)
                {
                    turnsWithBuffOrDebuff = 0;
                    buffbonusStateDamage = 0;
                }

                if (isMarked && FindObjectOfType<Monk>().debuffMark)
                {
                    ApplyBuffOrDebuffDamage(this,-1,3);                   
                }

                turnsWithMovementBuffOrDebuff--;
                if (turnsWithMovementBuffOrDebuff <= 0)
                {
                    turnsWithMovementBuffOrDebuff = 0;
                    movementUds = fMovementUds;
                    SetMovementIcon(0, this, false);
                }


                //Añado esto para stunnear a los enemigos 
                if (!isStunned)
                {
                    SearchingObjectivesToAttack();
                }
                else
                {
                    
                    if (turnStunned <= 0)
                    {
                        isStunned = false;
                        turnStunned = 0;
                        SetStunIcon(this, false, false);
                        
                    }
                    turnStunned--;
                    myCurrentEnemyState = enemyState.Ended;
                }

                break;

            case (enemyState.Moving):
                if (!corroutineDone)
                {
                    StartCoroutine("WaitBeforeNextState");
                }
                break;

            case (enemyState.Attacking):
                //Aqui no hay wait, por que se tiene que esperar antes de hacer la animación de atque, no al entrar en la función attack.
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

        if (myCurrentEnemyState == enemyState.Waiting)
        {
            yield return new WaitForSeconds(currentTimeWaitingBeforeStarting);
            myCurrentEnemyState = enemyState.Searching;
        }

        if (myCurrentEnemyState == enemyState.Moving)
        {
            yield return new WaitForSeconds(currentTimeWaitinBeforeMovement);
            MoveUnit();
        }

        //No hay yield return porque va en la propia animación de ataque
        if (myCurrentEnemyState == enemyState.Attacking)
        {
            Attack();
        }

        else if (myCurrentEnemyState == enemyState.Ended)
        {
            yield return new WaitForSeconds(currentTimeWaitingBeforeEnding);
            arrowEnemyIndicator.SetActive(false);
            FinishMyActions();
        }

        corroutineDone = false;
    }

    public virtual void SearchingObjectivesToAttack()
    {
        //Cada enemigo busca enemigos a su manera
        
        //Añadido esto para saber si los jugadores están ocultos (Añadir a todos los enemigos despues de que compruebe posibles objetivos pero antes de que busque al último)
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            if (currentUnitsAvailableToAttack[i].isHidden)
            {
                currentUnitsAvailableToAttack.Remove(currentUnitsAvailableToAttack[i]);
            }
        }

        //Actualizo el panel de dormido de mi retrato en la lista de enemigos
        if (haveIBeenAlerted)
        {
            if (myPortrait != null)
                myPortrait.UpdateSleepState(true);
        }
    }

    //Función que se encarga de hacer que el personaje este despierto/alerta
    public virtual void AlertEnemy()
    {
        DesAlertEnemy();
        haveIBeenAlerted = true;
        Destroy(sleepParticle);
        rangeOfAction = 1000;
    }

    //Función que se encarga de pintar el line renderer + sombra y el número de daño
    public virtual void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //Cada enemigo realiza su propio path

        //AL IGUAL QUE CON EL MOVIMIENTO ESTO ES LA LÓGICA DEL GOBLIN QUE SE USA DE BASE
        //Si se tiene que mostrar la acción por el hover calculamos el enemigo
        if (_shouldRecalculate)
        {
            pathToObjective.Clear();

            SearchingObjectivesToAttackShowActionPathFinding();
            if (myCurrentObjectiveTile != null)
            {
                //Cada enemigo realiza su propio path
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

                //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                for (int i = 0; i < LM.TM.currentPath.Count; i++)
                {
                    pathToObjective.Add(LM.TM.currentPath[i]);
                }
            }
        }

        //Si se va a mostrar la acción en el turno enemigo entonces no recalculo y directamente enseño la acción.
        //Esta parte es común para cuando se hace desde el hover como cuando se hace en turno enemigo.
        if (myCurrentObjectiveTile != null)
        {
            myLineRenderer.positionCount = 0;

            if (pathToObjective.Count - 2 > movementUds)
            {
                limitantNumberOfTilesToMove = movementUds;
            }
            else
            {
                limitantNumberOfTilesToMove = pathToObjective.Count - 2;
            }

            myLineRenderer.enabled = true;

            if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && pathToObjective.Count > 2)
            {
                sombraHoverUnit.SetActive(true);
            }

            //Coge
            myLineRenderer.positionCount += (limitantNumberOfTilesToMove + 1);

            for (int i = 0; i < limitantNumberOfTilesToMove + 1; i++)
            {
                shadowTile = pathToObjective[i];

                Vector3 pointPosition = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);
                //COMPROBAR
                SetShadowRotation(this, pathToObjective[i], currentUnitsAvailableToAttack[0].myCurrentTile);

                if (i < pathToObjective.Count - 1)
                {
                    myLineRenderer.SetPosition(i, pointPosition);

                    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                    {
                        sombraHoverUnit.transform.position = pointPosition;

                        if ((pathToObjective[limitantNumberOfTilesToMove + 1]) == currentUnitsAvailableToAttack[0].myCurrentTile)
                        {
                            Debug.Log(name + " " + currentUnitsAvailableToAttack[0].name);
                            CalculateDamagePreviousAttack(currentUnitsAvailableToAttack[0], this, pathToObjective[limitantNumberOfTilesToMove], CheckTileDirection(pathToObjective[limitantNumberOfTilesToMove], pathToObjective[limitantNumberOfTilesToMove + 1], false));
                        }
                        else
                        {
                            damageWithMultipliersApplied = -999;
                        }

                        Vector3 positionToLook = new Vector3(myCurrentObjective.transform.position.x, myCurrentObjective.transform.position.y + 0.5f, myCurrentObjective.transform.position.z);
                        sombraHoverUnit.transform.DOLookAt(positionToLook, 0, AxisConstraint.Y);
                    }
                }
            }

            CheckTilesInRange(shadowTile, SpecialCheckRotation(shadowTile, false));

            ///En el gigante es importante que esta función vaya después de colocar la sombra. Por si acaso asegurarse de que este if nunca se pone antes que el reposicionamiento de la sombra

            //A pesar de que ya se llama a esta función desde el levelManager en caso de hover, si se tiene que mostrar porque el goblin está atacando se tiene que llamar desde aqui (ya que no pasa por el level manager)
            //Tiene que ser en falso porque si no pongo la condicion la función se cree que el tileya estaba pintado de antes
            if (!_shouldRecalculate)
            {
                ColorAttackTile();
            }
        }
    }

    public void HideActionPathfinding()
    {
        myLineRenderer.enabled = false;

        if (sombraHoverUnit != null)
        {
            sombraHoverUnit.SetActive(false);
        }

        else
        {
            Debug.Log(sombraHoverUnit.name + " no tiene sombraHoverUnit");
        }
        

        for (int i = 0; i < tilesAlreadyUnderAttack.Count; i++)
        {
            if (!wereTilesAlreadyUnderAttack[i])
            {
                tilesAlreadyUnderAttack[i].ColorDesAttack();

                if (tilesAlreadyUnderAttack[i].unitOnTile != null)
                {
                    tilesAlreadyUnderAttack[i].unitOnTile.previsualizeAttackIcon.SetActive(false);
                }
            }
        }

        wereTilesAlreadyUnderAttack.Clear();
        tilesAlreadyUnderAttack.Clear();
    }

    //Se llama desde el LevelManager. Al final del showAction se encarga de mostrar el tile al que va a atacar
    public virtual void ColorAttackTile()
    {
        //El goblin y el gigante lo usan para pintar el tile al que van a atacar al mostrar show 
        //Esta base es común para unos cuantos enemigos, el gigante técnicamente es también común pero necesita acceder al if por lo que no usa la base.

        if (pathToObjective.Count > 0 && pathToObjective.Count <= movementUds + 2 && myCurrentObjective != null)
        {
            wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.isUnderAttack);
            tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile);
            myCurrentObjectiveTile.ColorAttack();
        }
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public virtual void SearchingObjectivesToAttackShowActionPathFinding()
    {
        //ESTA BASE ES LA LÓGICA DEL GOBLIN
        myCurrentObjective = null;
        myCurrentObjectiveTile = null;

        //Si no ha sido alertado compruebo si hay players al alcance que van a hacer que se despierte y se mueva
        if (!haveIBeenAlerted)
        {
            //Comprobar las unidades que hay en mi rango de acción
            unitsInRange = LM.TM.GetAllUnitsInRangeWithoutPathfinding(rangeOfAction, GetComponent<UnitBase>());

            for (int i = 0; i < unitsInRange.Count; i++)
            {
                if (unitsInRange[i].GetComponent<PlayerUnit>())
                {
                    keepSearching = true;
                    currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
                    break;
                }
            }
        }

        //Si ha sido alertado compruebo simplemente hacia donde se va a mover
        else
        {
            //Determinamos el enemigo más cercano.
            //currentUnitsAvailableToAttack = LM.TM.OnlyCheckClosestPathToPlayer();
            currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
            //Debug.Log("Line 435 " + currentUnitsAvailableToAttack.Count);

            keepSearching = true;
        }

        if (keepSearching)
        {
            if (currentUnitsAvailableToAttack.Count == 1)
            {
                myCurrentObjective = currentUnitsAvailableToAttack[0];
                myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
            }

            //Si hay varios enemigos a la misma distancia, se queda con el que tenga más unidades adyacentes
            else if (currentUnitsAvailableToAttack.Count > 1)
            {
                //Ordeno la lista de posibles objetivos según el número de unidades dyacentes
                currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                {
                    return (b.myCurrentTile.neighboursOcuppied).CompareTo(a.myCurrentTile.neighboursOcuppied);
                });

                //Elimino a todos los objetivos de la lista que no tengan el mayor número de enemigos adyacentes
                for (int i = currentUnitsAvailableToAttack.Count - 1; i > 0; i--)
                {
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.neighboursOcuppied > currentUnitsAvailableToAttack[i].myCurrentTile.neighboursOcuppied)
                    {
                        currentUnitsAvailableToAttack.RemoveAt(i);
                    }
                }

                //Si sigue habiendo varios enemigos los ordeno segun la vida
                if (currentUnitsAvailableToAttack.Count > 1)
                {

                    //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                    currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                    {
                        return (a.currentHealth).CompareTo(b.currentHealth);

                    });
                }

                myCurrentObjective = currentUnitsAvailableToAttack[0];
                myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
            }
        }

        keepSearching = false;
    }

    //Para acabar el turno de la unnidad
    public virtual void FinishMyActions()
    {
        LM.HideEnemyHover(GetComponent<EnemyUnit>());
        hasMoved = false;
        hasAttacked = false;
        myCurrentEnemyState = enemyState.Waiting;

        //Me aseguro de que el tiempo de movimiento vuelve a la normalidad por si le ha dado a acelerar
        //currentTimeForMovement = timeMovementAnimation;

        //currentTimeWaitingBeforeStarting = timeWaitingBeforeStarting;
        //currentTimeWaitinBeforeMovement = timeWaitingBeforeMovement;
        //currentTimeWaitinBeforeAttacking = timeWaitingBeforeAttacking;
        //currentTimeWaitingBeforeEnding = timeWaitingBeforeEnding;

        if (bossPortrait != null)
        {
            bossPortrait.RefreshAllTokens();
        }

        if (myPortrait != null)
        {
            myPortrait.UnHighlightMyself();
        }
        
        LM.NextEnemyInList();
    }

    public void SkipAnimation()
    {
        currentTimeForMovement = 0;

        Debug.Log(5);
        currentTimeWaitingBeforeStarting = 0;
        currentTimeWaitinBeforeMovement = 0;
        currentTimeWaitinBeforeAttacking = 0;
        currentTimeWaitingBeforeEnding = 0;

    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        if (LM.selectedCharacter != null)
        {
            Debug.Log("Ataqie");
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
			LM.camRef.FocusCameraOnCharacter(gameObject);
			//Focus camera
		}
		else if (clicked > 2 || Time.time - clickTime > 1)
		{
			clicked = 0;
		}

    }

    //Función que guarda todo lo que ocurre cuando se selecciona un personaje. Esta función sirve para no repetir codigo y además para poder llamarla desde el Level Manager.
    public virtual void SelectedFunctionality()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy != null && LM.selectedEnemy != GetComponent<EnemyUnit>())
            {
                LM.HideEnemyHover(LM.selectedEnemy);
                //Llamo a LevelManager para desactivar hover
                if (LM.selectedCharacter != null)
                {
                    LM.selectedCharacter.HideDamageIcons(LM.selectedCharacter);
                }
                LM.HideHover(LM.selectedEnemy);
                LM.selectedEnemy.HealthBarOn_Off(false);
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

                if (myPortrait != null)
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
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && !GameManager.Instance.isGamePaused)
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
                    LM.CalculatePreviousActionPlayer(LM.selectedCharacter, this);
                   
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                    LM.CheckIfHoverShouldAppear(this);
                    HealthBarOn_Off(true);
                }
            }
            else if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                if (!isDead)
                {
                    HealthBarOn_Off(true);

                    if (myPortrait != null)
                        myPortrait.HighlightMyself();

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

                    if (myPortrait != null)
                        myPortrait.HighlightMyself();

                    //Cambio el color del personaje
                    SelectedColor();
                }
            }
        }
    }

    //Creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public virtual void OnHoverEnterFunctionality()
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

		//LM.UIM.ShowCharacterInfo(unitInfo, this); 
		HealthBarOn_Off(true);
        //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

        if (myPortrait != null)
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
                sombraHoverUnit.SetActive(false);

                if (myPortrait != null)
                myPortrait.UnHighlightMyself();
            }
        }
    }

    //Al igual que con enter creo una función con todo lo que tiene que ocurrir el hover para que también se pueda usar en el hover del retrato.
    public virtual void OnHoverExitFunctionality()
    {
        LM.HideEnemyHover(this);
        sombraHoverUnit.SetActive(false);

        if (LM.selectedCharacter != null && LM.selectedCharacter.sombraHoverUnit != null)
        {
            LM.selectedCharacter.sombraHoverUnit.SetActive(false);

            if (LM.selectedCharacter.tilesInEnemyHover.Count > 0)
            {
                for (int i = 0; i < LM.selectedCharacter.tilesInEnemyHover.Count; i++)
                {
                    LM.selectedCharacter.tilesInEnemyHover[i].ColorDesAttack();

                    if (LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile != null)
                    {
                        LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.ResetColor();
                        LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.sombraHoverUnit.SetActive(false);
                    }
                }
            }

            Debug.Log("aa");
            HealthBarOn_Off(false);
            LM.selectedCharacter.healthBar.SetActive(false);
            LM.selectedCharacter.HideAttackEffect(this);
            LM.selectedCharacter.tilesInEnemyHover.Clear();
        }

        if(LM.selectedCharacter != null) 
        {
            if (LM.selectedCharacter.tilesInEnemyHover.Count > 0)
            {
                for (int i = 0; i < LM.selectedCharacter.tilesInEnemyHover.Count; i++)
                {
                    LM.selectedCharacter.tilesInEnemyHover[i].ColorDesAttack();

                    if (LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile != null)
                    {
                        LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.ResetColor();
                        LM.selectedCharacter.tilesInEnemyHover[i].unitOnTile.sombraHoverUnit.SetActive(false);
                    }
                }

                LM.selectedCharacter.healthBar.SetActive(false);
                LM.selectedCharacter.HideAttackEffect(this);
                LM.selectedCharacter.tilesInEnemyHover.Clear();
            }
        }
        
        if (LM.selectedEnemy == null)
        {
            if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                sombraHoverUnit.SetActive(false);

                ResetColor();

                if (myPortrait != null)
                    myPortrait.UnHighlightMyself();

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

            LM.selectedCharacter.HideDamageIcons(this);
            myCurrentTile.ColorDesAttack();
            previsualizeAttackIcon.SetActive(false);          
        }
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (LM.selectedCharacter != null)
		{
			//LM.UIM.ShowUnitInfo(LM.selectedCharacter.unitGeneralInfo, LM.selectedCharacter);
        }

        else
        {
            ResetColor();
            LM.HideHover(this);
            HealthBarOn_Off(false);
        }

        //Quito el healthbar de los objetivos a los que puedo atacar al salir del hover
        //Aunque lo desactivo en el hover exit, se activan en el CheckUnits en vez de en el hover enter
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].HealthBarOn_Off(false);
        }

        if(myPortrait != null)
        {
            myPortrait.UnHighlightMyself();
        }

        Knight knightRef = FindObjectOfType<Knight>();
        if (knightRef != null)
        {

            knightRef.HideAttackEffect(this);
        }

        if (LM.selectedCharacter != null)
        {
            LM.selectedCharacter.healthBar.SetActive(false);
        }


        myCurrentTile.ColorDesAttack();
    }

    public virtual void SelectedColor()
    {
        unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
    }

    #endregion

    #region DARK_LORD_POSSESION

    EnemyUnit[] allEnemiesScript;

    public void StartPosesion()
    {
        if (GetComponent<DarkLord>() && GetComponent<DarkLord>() != this)
        {
           
        }

        GetComponent<DarkLord>().enabled = true;
        GetComponent<DarkLord>().amITheOriginalDarkLord = false;

        gameObject.name = "Poseido";

        arrowEnemyIndicator.SetActive(false);
        myCurrentEnemyState = enemyState.Waiting;

        #region copia FinishActions sin pasar de turno
        LM.HideEnemyHover(GetComponent<EnemyUnit>());
        hasMoved = false;
        hasAttacked = false;
        myCurrentEnemyState = enemyState.Waiting;

        //Me aseguro de que el tiempo de movimiento vuelve a la normalidad por si le ha dado a acelerar
        currentTimeForMovement = timeMovementAnimation;

        Debug.Log(6);
        currentTimeWaitingBeforeStarting = timeWaitingBeforeStarting;
        currentTimeWaitinBeforeMovement = timeWaitingBeforeMovement;
        currentTimeWaitinBeforeAttacking = timeWaitingBeforeAttacking;
        currentTimeWaitingBeforeEnding = timeWaitingBeforeEnding;

        if (myPortrait != null)
        {
            myPortrait.UnHighlightMyself();
        }

        #endregion

        LM.NextEnemyInList();

        Debug.Log("d");
        LM.enemiesOnTheBoard.Remove(this);
        this.enabled = false;
        GetComponent<DarkLord>().InitializeAfterPosesion(currentHealth);

        return;

      
    }

    #endregion

    #region DAMAGE

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        //ES LO MISMO PERO SIN LA INSTANCIACIÓN DE PARTICULAS. EN EL FUTURO HACER QUE LAS PARTÍCULAS VAYAN POR EVENTOS DE ANIMACIÓN

        CalculateDamage(unitToDealDamage);
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        currentHealth -= damageReceived;

        Debug.Log("Soy " + gameObject.name + "y me han hecho " + damageReceived + " de daño");
        Debug.Log("Mi vida actual es " + currentHealth);

        myAnimator.SetTrigger("Damage");

        if (currentHealth <= 0)
        {
            if (unitAttacker != null)
            {
                //Logro balista mata enemigo
                if (unitAttacker.GetComponent<EnBalista>())
                {
                    GameManager.Instance.UnlockAchievement("");
                }

                //Logro gigante mata enemigo
                else if (unitAttacker.GetComponent<EnGiant>())
                {
                    GameManager.Instance.UnlockAchievement("");
                }

                //Logro matar enemigo en ventaja altura
                if (unitAttacker.myCurrentTile.height > myCurrentTile.height)
                {
                    GameManager.Instance.UnlockAchievement("");
                }

                //Logro matar enemigo en desventaja altura
                if (unitAttacker.myCurrentTile.height < myCurrentTile.height)
                {
                    GameManager.Instance.UnlockAchievement("");
                }
            }

            Die();
        }

        if (bossPortrait != null)
        {
            bossPortrait.RefreshHealth();
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

        if (exclamationIcon != null)
        {
            exclamationIcon.SetActive(false);
        }

        GetComponent<Collider>().enabled = false;

        //Aviso de que el enemigo está muerto
        isDead = true;

        //Estas dos llamadas tienen que ir despues del bool de isdead = true
        LM.UIM.SetEnemyOrder();

        //Contador de enemigos para logro
        GameManager.Instance.EnemyKilled();

        //No uso FinishMyActions porque no me interesa que pase turno, sólo que se quede en waiting por si acaso se muere en su turno.
        myCurrentEnemyState = enemyState.Waiting;
    }

    #endregion

    #region CHECKS

    //Esta función es el equivalente al chequeo de objetivos del jugador.Charger y balista usan versiones diferentes por eso el virtual. Es distinta de la del player y en principio no se puede reutilizar la misma debido a estas diferencias.
    public virtual void CheckCharactersInLine(bool _shouldWarnTilesForBalistaColoring, IndividualTiles _referenceTile)
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

    public override void UndoAttack(AttackCommand lastAttack)
    {
        //Resetear el material
        ResetColor();

        //Actualizar hud
        UIM.RefreshHealth();
        UIM.RefreshTokens();

        #region Rotation

        if (lastAttack.objPreviousRotation == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), 0);
            currentFacingDirection = FacingDirection.North;
        }

        else if (lastAttack.objPreviousRotation == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), 0);
            currentFacingDirection = FacingDirection.South;
        }

        else if (lastAttack.objPreviousRotation == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), 0);
            currentFacingDirection = FacingDirection.East;
        }

        else if (lastAttack.objPreviousRotation == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), 0);
            currentFacingDirection = FacingDirection.West;
        }
        #endregion

        //Mover de tile
        transform.DOMove(lastAttack.objPreviousTile.transform.position, 0);
        UpdateInformationAfterMovement(lastAttack.objPreviousTile);

        //Vida
        currentHealth = lastAttack.objPreviousHealth;

        currentArmor = lastAttack.objArmor;

        isStunned = lastAttack.objIsStunned;

        isMarked = lastAttack.objIsMarked;
        numberOfMarks = lastAttack.objnumberOfMarks;

        //Faltan añadir bufos y debufos
    }

    public void ExecuteAnimationAttack()
    {
        //Debug.Log("Corruitna ataque");
        StartCoroutine("AnimationAttack");
    }

    IEnumerator AnimationAttack()
    {
        //Debug.Log("waiting");

        yield return new WaitForSeconds(currentTimeWaitinBeforeAttacking);
        myAnimator.SetTrigger("Attack");
        Instantiate(attackParticle, unitModel.transform.position, unitModel.transform.rotation);

        myCurrentEnemyState = enemyState.Ended;
        //Esta ultima linea sustituye a los:
        ///else
        ///{
        ///    myCurrentEnemyState = enemyState.Ended;
        ///}
        //Actualmetne el summoner,skeleton, watcher no llaman a ExecuteAnimationAttack por lo que si se cambia habrá que arreglar que llamen a enemystate.ended.
        //El problema básicamnet es que aunque aqui se llama a una corrutina para esperar a la animación, el codigo en el propio enemigo sigue funcionando,
        //por lo que es como si no hubiese habido ninguna pausa realmente.
    }
    
    public void EnemyIsGoingToBeAlerted()
    {
        //Aparece exclamación
        if (exclamationIcon != null)
        {
            exclamationIcon.SetActive(true);
        }

        else
        {
            exclamationIcon.SetActive(true);
        }

        //Quito particulas dormido
        if (sleepParticle != null)
        {
            sleepParticle.SetActive(false);
        }
        //El retrato se cambia solo con el UImanager en SetEnemyOrder

        //Updateo el bool
        isGoingToBeAlertedOnEnemyTurn = true;
    }

    //Esto lo uso para poder desactivar las exclamaciones del goblin tier 2 pero sin quitarsela a los enemigos que ya tenían la exclamación por si solos
    public void ShowHideExclamation(bool _show)
    {
        if (_show)
        {
            exclamationIcon.SetActive(true);
        }

        else if (!isGoingToBeAlertedOnEnemyTurn)
        {
            exclamationIcon.SetActive(false);
        }
    }

    public void ShowHideInterrogationBoss(bool _show)
    {
        interrogationParticle.SetActive(_show);
    }

    public void DesAlertEnemy()
    {
        //Desaparece exclamación
        if (exclamationIcon != null)
        {
            exclamationIcon.SetActive(false);
        }
        else
        {
            exclamationIcon.SetActive(false);
        }

        isGoingToBeAlertedOnEnemyTurn = false;
    }

    //Sirve para pintar el rango de ataque de los personajes en naranja
    public virtual void CheckTilesInRange(IndividualTiles _referenceTile, FacingDirection _referenceDirection)
    {
        currentTilesInRangeForAttack.Clear();

        if (_referenceDirection == FacingDirection.North)
        {
            if (attackRange <= _referenceTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineUp[i].isEmpty && !_referenceTile.tilesInLineUp[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineUp[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineUp[i]);
                }

                if (_referenceTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineUp[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        else if (_referenceDirection == FacingDirection.South)
        {
            if (attackRange <= _referenceTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineDown[i].isEmpty && !_referenceTile.tilesInLineDown[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineDown[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineDown[i]);
                }

                if (_referenceTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineDown[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        else if (_referenceDirection == FacingDirection.East)
        {
            if (attackRange <= _referenceTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineRight[i].isEmpty && !_referenceTile.tilesInLineRight[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineRight[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineRight[i]);
                }

                if (_referenceTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineRight[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        else if (_referenceDirection == FacingDirection.West)
        {
            if (attackRange <= _referenceTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineLeft[i].isEmpty && !_referenceTile.tilesInLineLeft[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineLeft[i]);
                }

                if (_referenceTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }
    }

    public void ClearRange()
    {
        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorDesAttack();
        }

        currentTilesInRangeForAttack.Clear();
    }


    protected Vector3 rotationChosenAfterMovement;
    [HideInInspector]
    public FacingDirection facingDirectionAfterMovement;

    //Esta función se usa para saber la dirección en la que va a acabar el personaje al acabar de moverse
    //Principalmente es una función para poder usarla en el levelmanager al hacer hover sobre el enemigo y que use la dirección para llamar a la funcion CalculateDamagePreviousAttack() o pintar tiles de rango;
    //El bool solo se usa cuando se llama durante el turno enemigo
    public FacingDirection SpecialCheckRotation(IndividualTiles _tileToComparePosition, bool _DoAll)
    {
        if (currentUnitsAvailableToAttack.Count > 0)
        {
            //Arriba o abajo
            if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX == _tileToComparePosition.tileX)
            {
                //Arriba
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tileZ > _tileToComparePosition.tileZ)
                {
                    if (_DoAll)
                    {
                        for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                        {
                            pathToObjective.Add(_tileToComparePosition.tilesInLineUp[i]);
                        }
                    }

                    //Roto al charger
                    rotationChosenAfterMovement = new Vector3(0, 0, 0);
                    facingDirectionAfterMovement = FacingDirection.North;
                }
                //Abajo
                else
                {
                    if (_DoAll)
                    {
                        for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                        {
                            pathToObjective.Add(_tileToComparePosition.tilesInLineDown[i]);
                        }
                    }


                    //Roto al charger
                    rotationChosenAfterMovement = new Vector3(0, 180, 0);
                    facingDirectionAfterMovement = FacingDirection.South;
                }
            }
            //Izquierda o derecha
            else
            {

                //Derecha
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX > _tileToComparePosition.tileX)
                {
                    if (_DoAll)
                    {
                        for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                        {
                            pathToObjective.Add(_tileToComparePosition.tilesInLineRight[i]);
                        }
                    }


                    //Roto al charger
                    rotationChosenAfterMovement = new Vector3(0, 90, 0);
                    facingDirectionAfterMovement = FacingDirection.East;
                }
                //Izquierda
                else
                {
                    if (_DoAll)
                    {

                        for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                        {
                            pathToObjective.Add(_tileToComparePosition.tilesInLineLeft[i]);
                        }
                    }


                    //Roto al charger
                    rotationChosenAfterMovement = new Vector3(0, -90, 0);
                    facingDirectionAfterMovement = FacingDirection.West;
                }
            }
        }

        return facingDirectionAfterMovement; 
    }

    #region GOBLIN_SHARED_FUNCTIONALITY

    //Función de movimiento que se llama al cambiar al state Moving.
    //Acordarse de que cada enemigo debe actualizar al moverse los tiles (vacíar el tile anterior y setear el nuevo tile y la unidad del nuevo tile)
    public virtual void MoveUnit()
    {
        //ESTA ES LA BASE DEL GOBLIN Y QUE SE REUTILIZA PARA OTROS ENEMIGOS:
        //Gigante,
        //
        limitantNumberOfTilesToMove = 0;
        movementParticle.SetActive(true);
        ShowActionPathFinding(false);

        //Como el path guarda el tile en el que esta el enemigo yel tile en el que esta el personaje del jugador resto 2.
        //Si esta resta se pasa del número de unidades que me puedo mover entonces solo voy a recorrer el número de tiles máximo que puedo llegar.
        if (pathToObjective.Count - 2 > movementUds)
        {
            limitantNumberOfTilesToMove = movementUds;
        }

        //Si esta resta por el contrario es menor o igual a movementUds significa que me voy mover el máximo o menos tiles.
        else
        {
            limitantNumberOfTilesToMove = pathToObjective.Count - 2;
        }

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(myCurrentTile, pathToObjective[pathToObjective.Count - 1], true);

        myCurrentEnemyState = enemyState.Waiting;

        //Actualizo info de los tiles
        UpdateInformationAfterMovement(pathToObjective[limitantNumberOfTilesToMove]);

        StartCoroutine("MovingUnitAnimation");
    }

    //Corrutina de movimiento
    //De los enemigos que se mueven los que no la usan son el charger y la balista
    protected IEnumerator MovingUnitAnimation()
    {
        //Animación de movimiento
        //Es -1 ya que no me interesa que se mueva hasta el tile en el que está la otra unidad
        for (int j = 1; j <= limitantNumberOfTilesToMove; j++)
        {
            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = pathToObjective[j].transform.position;  //new Vector3(pathToObjective[j].transform.position.x, pathToObjective[j].transform.position.y, pathToObjective[j].transform.position.z);

            //Muevo y roto a la unidad
            transform.DOMove(currentTileVectorToMove, currentTimeForMovement);
            unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation, AxisConstraint.Y);

            //Espera entre casillas
            yield return new WaitForSeconds(currentTimeForMovement);
        }

        hasMoved = true;

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(myCurrentTile, pathToObjective[pathToObjective.Count - 1], true);
        myCurrentEnemyState = enemyState.Searching;

        movementParticle.SetActive(false);

        HideActionPathfinding();
        //ShowActionPathFinding(false);
    }

    protected Vector3 tempVector3Rotation;
    protected FacingDirection tempFacingDirection;

    //Decidir rotación al moverse por los tiles.
    public FacingDirection CheckTileDirection(IndividualTiles referenceTile, IndividualTiles tileToCheck, bool _shouldRotateToo)
    {
        //Arriba o abajo
        if (tileToCheck.tileX == referenceTile.tileX)
        {
            //Arriba
            if (tileToCheck.tileZ > referenceTile.tileZ)
            {
                tempVector3Rotation = new Vector3(0, 0, 0);
                tempFacingDirection = FacingDirection.North;
            }
            //Abajo
            else
            {
                tempVector3Rotation = new Vector3(0, 180, 0);
                tempFacingDirection = FacingDirection.South;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileToCheck.tileX > referenceTile.tileX)
            {
                tempVector3Rotation = new Vector3(0, 90, 0);
                tempFacingDirection = FacingDirection.East;
            }
            //Izquierda
            else
            {
                tempVector3Rotation = new Vector3(0, -90, 0);
                tempFacingDirection = FacingDirection.West;
            }
        }

        if (_shouldRotateToo)
        {
            unitModel.transform.DORotate(tempVector3Rotation, timeDurationRotation);
            currentFacingDirection = tempFacingDirection;
        }

        return tempFacingDirection;
    }

    //Decidir rotación al terminar de moverse para atacar
    protected void RotateLogic(FacingDirection newDirection)
    {
        //Roto al gigante
        if (newDirection == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.North;
        }

        else if (newDirection == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.South;
        }

        else if (newDirection == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.East;
        }

        else if (newDirection == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.West;
        }
    }

    public virtual void Attack()
    {
        //Cada enemigo realiza su propio ataque

        if (currentUnitsAvailableToAttack.Count > 0)
        {
            Debug.Log("AQUI");

            ///Este se usaba antes y calculaba mal el daño. COMPROBAR SI EL OTRO LO CALCULA BIEN SIEMPRE
            //CalculateDamage(currentUnitsAvailableToAttack[0]);

            CalculateDamagePreviousAttack(currentUnitsAvailableToAttack[0], GetComponent<UnitBase>(), myCurrentTile, currentFacingDirection);
            

            currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
        }
    }

    #endregion
}
