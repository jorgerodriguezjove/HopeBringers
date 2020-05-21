using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    public TextAsset DEBUG_DIALOG;

    [SerializeField]
    //Offset negativo para determinar la altura de las flechas
    private float offsetHeightArrow;

    [Header("SELECCIÓN DE UNIDADES")]
    [SerializeField]
    private List<Transform> initialCharacterDataPosition = new List<Transform>();
    [SerializeField]
    private GameObject characterSelectionBox;

    [Header("CONDICIONES DE DERROTA")]
    [SerializeField]
    private int turnLimit;
    //Int que lleva la cuenta del turno actual
    private int currentTurn = 0;

    [SerializeField]
    bool isObjectiveKillSpecificEnemies;

    [SerializeField]
    bool isObjectiveWaitForTurnLimit;

    [SerializeField]
    List<UnitBase> enemiesNecessaryToWin = new List<UnitBase>();

    [Header("INTERACCIÓN CON UNIDADES")]

    //Personaje actualmente seleccionado.
    [HideInInspector]
    public PlayerUnit selectedCharacter;

    //Enemigo actualmente seleccionado.
    [HideInInspector]
    public EnemyUnit selectedEnemy;

    //Tiles que actualmente están disponibles para el movimiento de la unidad seleccionada
    [HideInInspector]
    public List<IndividualTiles> tilesAvailableForMovement = new List<IndividualTiles>();

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    float timeForMovementAnimation = 0.2f;

    //Posición a la que tiene que moverse la unidad actualmente
    private Vector3 currentTileVectorToMove;

    //Tile al que se va a mover tras rotar
    [HideInInspector]
    public IndividualTiles tileToMoveAfterRotate;

    //Tiles que los enemigos pueden moverse, que se muestran al hacer hover o clickar en los enemigos
    [HideInInspector]
    public List<IndividualTiles> tilesAvailableForMovementEnemies = new List<IndividualTiles>();

    //Tiles que los enemigos pueden moverse, que se muestran al hacer hover o clickar en los enemigos
    [HideInInspector]
    public List<IndividualTiles> tilesAvailableForRangeEnemies = new List<IndividualTiles>();

    //Int que guarda el número de objetivos que tiene para atacar la unidad actual. Se usa únicamente en la función SelectUnitToAttack para marcar el índice de un for y que no de error si se deselecciona la unidad actual.
    private int enemiesNumber;

    //Tiles que tienen efecto de daño
    //[HideInInspector]
    public List<DamageTile> damageTilesInBoard = new List<DamageTile>();

    private List<UnitBase> unitsToEnableCollider = new List<UnitBase>();

    [Header("TURNOS Y FASES")]

    //Bool que sirve para que se pueda probar un nivel sin necesidad de haber elegido personajes antes
    [SerializeField]
    public bool FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa;

    //Lista con unidades que no han sido colocadas en escena todavía y que estan en formato lista
    List<GameObject> unitsWithoutPositionFromGameManager = new List<GameObject>();

    //Lista con los tiles disponibles para colocar personajes. Me sirve para limpiar el color de los tiles al terminar.
    [HideInInspector]
    public List<IndividualTiles> tilesForCharacterPlacement = new List<IndividualTiles>();

    //Lista con los tiles disponibles para colocar personajes. Me sirve para limpiar el color de los tiles al terminar.
    [HideInInspector]
    public List<IndividualTiles> tilesForFinishingPlayers = new List<IndividualTiles>();

    //Cada unidad se encarga desde su script de incluirse en la lista
    //Lista con todas las unidades del jugador en el tablero
    //[HideInInspector]
    public List<PlayerUnit> charactersOnTheBoard;
    
    //Lista con todas las unidades enemigas en el tablero
    [SerializeField]
    public List<EnemyUnit> enemiesOnTheBoard;

    //Contador que controla a que unidad le toca. Sirve cómo indice para la lista de enemigos.
    //QUITAR EL SERIALIZED
    [SerializeField]
    public int counterForEnemiesOrder;

    //Enum que indica si es la fase del jugador o del enemigo.
    [HideInInspector]
    public enum LevelState { Initializing ,PlayerPhase, ProcessingPlayerActions, EnemyPhase, ProcessingEnemiesActions};

    [HideInInspector]
    public LevelState currentLevelState { get; private set; }

    [Header("REFERENCIAS")]

    //Referencia al Tile Manager
    [HideInInspector]
    public TileManager TM;
	[HideInInspector]
    public UIManager UIM;
    [HideInInspector]
    public NewCameraController camRef;
    [HideInInspector]
    public MapGenerator MapGenRef;

    //Referencia momentanea para el playtesting
    [SerializeField]
    private GameObject defeatPanel;

    [SerializeField]
    Camera hud3DCamera;
    [SerializeField]
    LayerMask hud3D;

    //Lo añado al LevelManager porque quiero que solo exista un count global para todos los personajes
    //int con el honor acumulador
    public int honorCount;

	[Header("TUTORIALES")]

	[SerializeField]
	public bool tutorialLevel1;
	[SerializeField]
	public bool tutorialLevel2;
	[SerializeField]
	public bool tutorialLevel3;
	[SerializeField]
	public bool tutorialLevel4;

	[SerializeField]
	public GameObject tutorialGameObject;

    #endregion

    #region INIT


    //IMPORTANTE QUE SEA START Y NO AWAKE
    //EL ENEMYUNIT Y EL PLAYERUNIT TIENEN QUE INICIALIZARSE EN EL AWAKE ANTES DE PODER HACER ESTO
    //TANTO EL Start COMO StartGameplayAfterDialog TIENEN UN ORDEN MUY CONCRETO QUE NO SE PUEDE CAMBIAR A LA LIGERA POR EL ORDEN DE INICIALIZACIÓN
    /// <summary>
    /// Tenía aqui apuntado esto antes de hacer el cambio de quitar los awakes y eso:
    /// //Si no el dragón no puede buscar sus tiles a través del tilemanager cuando empieza el nivel.
    /// Revisar si el dragón sigue funcionando
    /// </summary>
    private void Start()
    {
        //Se inicializan los managers de este nivel
        TM = FindObjectOfType<TileManager>();
        UIM = FindObjectOfType<UIManager>();
        camRef = FindObjectOfType<NewCameraController>();
        MapGenRef = FindObjectOfType<MapGenerator>();

        ///DEBUG!!!!
        ///ESTA IF SIRVE SI EN EDITOR SE HA ACTIVADO EL BOOL PARA JUGAR SIN PASAR POR EL LEVEL MANAGER
        ///Hace lo mismo que el StartGameplayAfterDialog pero sin colocar a las unidades
        ///DEBUG!!!!

        if (FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa)
        {
            //Si es random preparo los obstáculos ANTES de crear el grid
            if (MapGenRef != null)
            {
                MapGenRef.LookAndSortSpots();
                MapGenRef.CreateObstacle();
            }

            TM.CreateGrid();

            characterSelectionBox.SetActive(false);

            //Si es random creo los enemigos una vez creado el grid
            if (MapGenRef != null)
            {
                MapGenRef.SetEnemyProbablity();
                MapGenRef.CreateEnemies();
            }

            for (int i = 0; i < FindObjectsOfType<PlayerUnit>().Length; i++)
            {
                charactersOnTheBoard.Add(FindObjectsOfType<PlayerUnit>()[i]);
                charactersOnTheBoard[i].InitializeUnitOnTile();
            }

            Monk monk = FindObjectOfType<Monk>();
            for (int i = 0; i < charactersOnTheBoard.Count; i++)
            {
                charactersOnTheBoard[i].pjMonkUnitReference = monk;
            }

            for (int i = 0; i < enemiesOnTheBoard.Count; i++)
            {
                enemiesOnTheBoard[i].InitializeUnitOnTile();

                if (enemiesOnTheBoard[i].GetComponent<Crystal>())
                {
                    enemiesOnTheBoard.RemoveAt(i);
                    i--;
                }
            }
            UIM.InitializeUI();
            currentLevelState = LevelState.PlayerPhase;
            camRef.SetCameraMovable(true, true);
        }

        //Se resetea la lista de comandos por si acaso tenía algún elemento dentro
        FindObjectOfType<CommandInvoker>().ResetCommandList();
    }

    //Una vez ha terminado el diálogo, el GameManager avisa a esta función de que comience el nivel
    public void StartGameplayAfterDialog()
    {

        //if (FindObjectOfType<MechaBoss>() || FindObjectOfType<DarkLord>() || FindObjectOfType<BossMultTile>())
        //{
        //    SoundManager.Instance.PlayMusic(AppMusic.BOSS_MUSIC);
        //                  SoundManager.Instance.StopMusic();

        //}
        //else
        //{
        //    SoundManager.Instance.PlayMusic(AppMusic.COMBAT_MUSIC);
        //            SoundManager.Instance.StopMusic();
        //}

        hud3DCamera.cullingMask = hud3D;

        //Si es random preparo los obstáculos ANTES de crear el grid
        if (MapGenRef != null)
        {
            MapGenRef.LookAndSortSpots();
            MapGenRef.CreateObstacle();
        }

        //Se crea el grid
        TM.CreateGrid();

        //Si es random creo los enemigos una vez creado el grid
        if (MapGenRef != null)
        {
            MapGenRef.SetEnemyProbablity();
            MapGenRef.CreateEnemies();
        }

        //Esto lo hago por si hay personajes puestos ya en el nivel porque se desbloquean en dicho nivel
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>(); 
        for (int i = 0; i < players.Length; i++)
        {
            if (!charactersOnTheBoard.Contains(players[i]))
            {
                charactersOnTheBoard.Add(players[i]);
            }
        }

        Monk monk = FindObjectOfType<Monk>();
        for (int i = 0; i < charactersOnTheBoard.Count; i++)
        {
            charactersOnTheBoard[i].pjMonkUnitReference = monk;
        }

        //Se inicializan todas las unidades
        for (int i = 0; i < charactersOnTheBoard.Count; i++)
        {
            charactersOnTheBoard[i].InitializeUnitOnTile();
        }

        for (int i = 0; i < enemiesOnTheBoard.Count; i++)
        {
            if (i > 0)
            {
                enemiesOnTheBoard[i].InitializeUnitOnTile();

                if (enemiesOnTheBoard[i].GetComponent<Crystal>())
                {
                    enemiesOnTheBoard.RemoveAt(i);
                    i--;
                }

                if (enemiesOnTheBoard[i].GetComponent<DarkLord>() && !enemiesOnTheBoard[i].GetComponent<DarkLord>().amITheOriginalDarkLord)
                {
                    enemiesOnTheBoard.RemoveAt(i);
                    i--;
                }
            }
        }

        //Se activa la cámara
        camRef.SetCameraMovable(true, true);

        //Comienza la fase de colocación de unidades
        //El Hud se inicializa al terminar la colocación de unidades
        InitializeCharacters();
    }

    //Crea a los personajes del jugador correspondientes
    private void InitializeCharacters()
    {
        //Aparece la caja
        UIM.ActivateHudUnitPlacement();

        //Turnos durante colocación de unidades
        UIM.UpdateTurnNumber(1, turnLimit);

        //Se instancian en los transform los personajes desbloqueados (GameManager necesita saberlos)
        for (int i = 0; i < GameManager.Instance.characterDataForCurrentLevel.Count; i++)
        {
            GameObject unitInstantiated = Instantiate(GameManager.Instance.characterDataForCurrentLevel[i].GetComponent<CharacterData>().myUnit.gameObject, initialCharacterDataPosition[i]);
            unitInstantiated.GetComponent<PlayerUnit>().insideGameInfoObject.SetActive(false);
            unitInstantiated.transform.position = initialCharacterDataPosition[i].position;

            //Guardo posición inicial en caja
            unitInstantiated.GetComponent<PlayerUnit>().initialPosInBox = initialCharacterDataPosition[i];

            unitInstantiated.transform.localScale = Vector3.one;

            unitInstantiated.GetComponent<UnitBase>().InitializeHealth();
        }
    }

    //Ordeno la lista de personajes del jugador y la lista de enemigos
    //Cuando muere un enemigo, también se llama aquí
    private void UpdateUnitsOrder()
    {
        if (charactersOnTheBoard.Count > 0)
        {
            charactersOnTheBoard.Sort(delegate (PlayerUnit a, PlayerUnit b)
            {
                return (b.GetComponent<PlayerUnit>().speed).CompareTo(a.GetComponent<PlayerUnit>().speed);

            });
        }

        if (enemiesOnTheBoard.Count > 0)
        {
            for (int i = 0; i < enemiesOnTheBoard.Count; i++)
            {
                //&& currentLevelState != LevelState.ProcessingPlayerActions esta para que al moverse y actualizar estado de los enemigos no los destruya y funcione el undo.
                if (enemiesOnTheBoard[i].GetComponent<EnemyUnit>().isDead && currentLevelState != LevelState.ProcessingPlayerActions)
                {
                    EnemyUnit deadEnemy = enemiesOnTheBoard[i];
                    enemiesOnTheBoard.Remove(deadEnemy);
                    Destroy(deadEnemy.gameObject);
                    counterForEnemiesOrder--;
                    i--;
                }
            }

            //Ordenar por despierto
            enemiesOnTheBoard.Sort(delegate (EnemyUnit a, EnemyUnit b)
            {
                return (b.GetComponent<EnemyUnit>().haveIBeenAlerted).CompareTo(a.GetComponent<EnemyUnit>().haveIBeenAlerted);

            });

            //Ordenar por velocidad
            enemiesOnTheBoard.Sort(delegate (EnemyUnit a, EnemyUnit b)
            {
                return (b.GetComponent<EnemyUnit>().speed).CompareTo(a.GetComponent<EnemyUnit>().speed);

            });

            //Ordenar por alerta
            enemiesOnTheBoard.Sort(delegate (EnemyUnit a, EnemyUnit b)
            {
                return (b.GetComponent<EnemyUnit>().isGoingToBeAlertedOnEnemyTurn).CompareTo(a.GetComponent<EnemyUnit>().isGoingToBeAlertedOnEnemyTurn);

            });
        }

		UIM.SetEnemyOrder();
	}

    #endregion

    #region UNIT_INTERACTION

    [SerializeField]
    public PlayerUnit currentCharacterPlacing;
    [SerializeField]
    List<PlayerUnit> charactersAlreadyPlaced = new List<PlayerUnit>();

        #region HOVER

    //Muestra el rango de movimiento de una unidad aliada al hacer hover en ella.
    public void ShowUnitHover(int movementUds, PlayerUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            //hoverUnit.HealthBarOn_Off(true);
            //hoverUnit.GetComponent<PlayerHealthBar>().ReloadHealth();
            hoverUnit.myCurrentTile.ColorCurrentTileHover();

            //Pinto tiles de movimiento
            //Importante tienen que ir antes de pintar el rango de ataque
            if (hoverUnit.hasMoved == false)
            {
                tilesAvailableForMovement = new List<IndividualTiles>(TM.CalculateAvailableTilesForHover(hoverUnit.myCurrentTile, hoverUnit));

                for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                {
                    tilesAvailableForMovement[i].ColorMovement();
                }
            }

            //Chequeo y pinto el rango de ataque
            hoverUnit.CheckUnitsAndTilesInRangeToAttack(false);
        }
    }

    //Hago desaparecer el hover
    public void HideHover(UnitBase enemyToCheck)
    {
        //Ocultar hover
        enemyToCheck.DisableCanvasHover();
    }

    //Esconde el rango de movimiento de una unidad aliada al quitar el ratón de ella. !!Cuidado con función de arriba que se llama parecido (HideHover)!!
    public void HideUnitHover(PlayerUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            hoverUnit.HealthBarOn_Off(false);
            UIM.TooltipDefault();
            hoverUnit.myCurrentTile.ColorDeselect();

            if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
            {
                for (int i = 0; i < hoverUnit.currentUnitsAvailableToAttack.Count; i++)
                {
                    if (hoverUnit.currentUnitsAvailableToAttack[i] != null)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[i].ResetColor();
                        hoverUnit.currentUnitsAvailableToAttack[i].myCurrentTile.ColorDesCharge();
                        hoverUnit.currentUnitsAvailableToAttack[i].previsualizeAttackIcon.SetActive(false);
                        hoverUnit.currentUnitsAvailableToAttack[i].DisableCanvasHover();
                    }
                }
            }

            if (hoverUnit.currentTilesInRangeForAttack.Count > 0)
            {
                for (int i = 0; i < hoverUnit.currentTilesInRangeForAttack.Count; i++)
                {
                    hoverUnit.currentTilesInRangeForAttack[i].ColorDesCharge();
                }
            }


            if (hoverUnit.hasMoved == false)
            {
                for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                {
                    tilesAvailableForMovement[i].ColorDeselect();
                }
                tilesAvailableForMovement.Clear();
            }
        }
    }

    //Compruebo si el enemigo sobre el que está haciendo hover el jugador está disponible para atacar o no.
    public void CheckIfHoverShouldAppear(UnitBase enemyToCheck)
    {
        if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
        {
            enemiesNumber = selectedCharacter.currentUnitsAvailableToAttack.Count;

            for (int i = 0; i < enemiesNumber; i++)
            {
                if (enemyToCheck == selectedCharacter.currentUnitsAvailableToAttack[i])
                {
                    //Muestro hover avisando a Selected Character
                    selectedCharacter.ShowHover(enemyToCheck);
                }
            }
        }
    }

    //showActionRangeInsteadOfMovement sirve para que se pinte naranaja el rango de acción del enemigo al estar dormido
    public void ShowEnemyHover(int movementUds, bool showActionRangeInsteadOfMovement, EnemyUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            if (hoverUnit.GetComponent<EnBalista>())
            {
                if (!hoverUnit.GetComponent<EnBalista>().isAttackPrepared)
                {
                    hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine(false, hoverUnit.myCurrentTile);

                    //Dibuja el ataque que va a preparar si las unidades se quedan ahí
                    if (hoverUnit.GetComponent<EnBalista>().currentUnitsAvailableToAttack.Count > 0)
                    {
                        hoverUnit.GetComponent<EnBalista>().particleChargingAttack.SetActive(true);
                        hoverUnit.GetComponent<EnBalista>().FeedbackTilesToCharge(true);
                    }

                    //Dibuja el próximo movimiento si no tiene a ningún jugador en su línea antes de moverse
                    else 
                    {
                        IndividualTiles tileToMove = hoverUnit.GetComponent<EnBalista>().GetTileToMove();

                        if (tileToMove != null)
                        {
                            hoverUnit.sombraHoverUnit.SetActive(true);
                            Vector3 positionToSpawn = new Vector3(tileToMove.transform.position.x, tileToMove.transform.position.y + 0.3f, tileToMove.transform.position.z);
                            hoverUnit.sombraHoverUnit.transform.position = positionToSpawn;
                            tileToMove.ColorMovement();

                            hoverUnit.myLineRenderer.enabled = true;
                            hoverUnit.myLineRenderer.positionCount = 2;
                            Vector3 positionToSpawnLineRenderer = new Vector3(hoverUnit.myCurrentTile.transform.position.x, hoverUnit.myCurrentTile.transform.position.y + 0.3f, hoverUnit.myCurrentTile.transform.position.z);
                            hoverUnit.myLineRenderer.SetPosition(0, positionToSpawnLineRenderer);
                            hoverUnit.myLineRenderer.SetPosition(1, positionToSpawn);
                        }

                        //Compruebo si después de moverse tiene objetivos que atacar
                        if (tileToMove != null)
                        {
                            hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine(false, tileToMove);
                        }

                        //Dibuja el ataque que va a preparar si las unidades se quedan ahí
                        if (hoverUnit.GetComponent<EnBalista>().currentUnitsAvailableToAttack.Count > 0)
                        {
                            hoverUnit.GetComponent<EnBalista>().FeedbackTilesToCharge(true);
                        }
                    }
                }

                else
                {
                    hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine(false, hoverUnit.myCurrentTile);

                    //Marco las unidades disponibles para atacar de color rojo
                    for (int i = 0; i < hoverUnit.currentUnitsAvailableToAttack.Count; i++)
                    {
                        hoverUnit.CalculateDamagePreviousAttack(hoverUnit.currentUnitsAvailableToAttack[i],hoverUnit, hoverUnit.myCurrentTile,hoverUnit.currentFacingDirection);
                        //hoverUnit.CalculateDamage(hoverUnit.currentUnitsAvailableToAttack[i]);

                        //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                        if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                        }

                        hoverUnit.currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                        hoverUnit.currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
                    }
                }
            }

            else if (hoverUnit.GetComponent<EnCharger>())
            {
                hoverUnit.GetComponent<EnCharger>().CheckCharactersInLine(false, null);

                //Muestro la acción que va a realizar el enemigo 
                hoverUnit.ShowActionPathFinding(true);

                //Dibuja el ataque que va a preparar si las unidades se quedan ahí
                if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count > 0 && hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack[0] != null)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(true);

                    //Calculo el daño que le hace al llegar al tile anterior
                    if (hoverUnit.pathToObjective.Count > 0)
                    {
                        hoverUnit.CalculateDamagePreviousAttack(hoverUnit.currentUnitsAvailableToAttack[0], hoverUnit, hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1], hoverUnit.GetComponent<EnCharger>().SpecialCheckRotation(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1], false));
                        hoverUnit.currentUnitsAvailableToAttack[0].CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1]);
                    }

                    //Si ni siquiera es > 0 entonces es que no se mueve del tile
                    else
                    {
                        hoverUnit.CalculateDamagePreviousAttack(hoverUnit.currentUnitsAvailableToAttack[0], hoverUnit, hoverUnit.myCurrentTile, hoverUnit.GetComponent<EnCharger>().SpecialCheckRotation(hoverUnit.myCurrentTile, false));
                        hoverUnit.currentUnitsAvailableToAttack[0].CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.myCurrentTile);
                    }

                    //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                    if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                    }

                    hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                    hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);


                    if (hoverUnit.pathToObjective.Count > 0)
                    {
                        hoverUnit.sombraHoverUnit.SetActive(true);
                        hoverUnit.sombraHoverUnit.transform.position = hoverUnit.GetComponent<EnCharger>().pathToObjective[hoverUnit.GetComponent<EnCharger>().pathToObjective.Count - 1].transform.position;
                        hoverUnit.SearchingObjectivesToAttackShowActionPathFinding();
                    }

                    else
                    {
                        hoverUnit.sombraHoverUnit.SetActive(true);
                        hoverUnit.sombraHoverUnit.transform.position = hoverUnit.myCurrentTile.transform.position;
                        hoverUnit.SearchingObjectivesToAttackShowActionPathFinding();
                    }

                    if (hoverUnit.myTierLevel == EnemyUnit.TierLevel.Level2)
                    {
                        hoverUnit.GetComponent<EnCharger>().InstantiateIconsFire();
                    }

                    hoverUnit.GetComponent<EnCharger>().ShowPushResult();

                }

                else if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count == 0)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(true);
                }
            }

            else if (hoverUnit.GetComponent<BossMultTile>() || hoverUnit.GetComponent<DarkLord>() || hoverUnit.GetComponent<MechaBoss>())
            {
                hoverUnit.ShowHideInterrogationBoss(true);
            }

            else if (hoverUnit.GetComponent<EnWatcher>())
            {
                for (int i = 0; i < hoverUnit.unitsInRange.Count; i++)
                {
                    if (hoverUnit.unitsInRange[i].GetComponent<PlayerUnit>())
                    {
                        hoverUnit.SetBuffDebuffIcon(-1, hoverUnit.unitsInRange[i], true);                      
                        hoverUnit.SetMovementIcon(-1, hoverUnit.unitsInRange[i], true);
                        
                    }
                    ////El -1 es para que aparezca bien el número en el icono pero luego a nivel logico no se reste
                    //hoverUnit.unitsInRange[i].ShowHideFear(true, hoverUnit.GetComponent<EnWatcher>().turnDurationDebuffs-1);
                    ////Pongo -1 para que no pinte nada
                    //hoverUnit.unitsInRange[i].ColorAvailableToBeAttackedAndNumberDamage(-1);
                }

                //Rango de acción
                tilesAvailableForRangeEnemies = TM.CheckAvailableTilesForEnemyAction(hoverUnit.rangeOfAction, hoverUnit);

                //Rango despertar y miedo
                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorActionRange();
                }
            }

            else if (hoverUnit.GetComponent<EnSummoner>())
            {
                if (!hoverUnit.haveIBeenAlerted)
                {
                    //Rango de acción
                    tilesAvailableForRangeEnemies = TM.CheckAvailableTilesForEnemyAction(hoverUnit.rangeOfAction, hoverUnit);

                    //Rango despertar y miedo
                    for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                    {
                        tilesAvailableForRangeEnemies[i].ColorActionRange();
                    }
                }

                hoverUnit.GetComponent<EnSummoner>().HideShowFeedbackSpawnPosition(true);
            }

            else if (hoverUnit.GetComponent<EnGrabber>())
            {
                hoverUnit.GetComponent<EnGrabber>().CheckUnitToAttack(hoverUnit.myCurrentTile, hoverUnit.currentFacingDirection);

                if (hoverUnit.currentUnitsAvailableToAttack.Count == 0)
                {
                    hoverUnit.ShowActionPathFinding(true);

                    //Tengo que hacer que esta comprobación se haga ahora desde el tile en el que va a acabar
                    hoverUnit.GetComponent<EnGrabber>().CheckUnitToAttack(hoverUnit.shadowTile, hoverUnit.facingDirectionAfterMovement);

                    if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
                    {
                        hoverUnit.GetComponent<EnGrabber>().ShowGrabShadow(hoverUnit.shadowTile, hoverUnit.SpecialCheckRotation(hoverUnit.shadowTile, false));
                        hoverUnit.GetComponent<EnGrabber>().CalculateDamageForEnemiesGrabbedAfterMovement();
                    }
                }

                else
                {
                    Debug.Log("No me muevo pero si ataco");
                    hoverUnit.GetComponent<EnGrabber>().ShowGrabShadow(hoverUnit.myCurrentTile, hoverUnit.currentFacingDirection);
                }

                //¡¡MUY IMPORTANTE!! hoverUnit.ShowActionPathFinding(true); TIENE QUE IR ANTES QUE ESTOS ifs
                if (!hoverUnit.haveIBeenAlerted)
                {
                    //Rango de acción sólo se pinta si no ha sido alertado
                    tilesAvailableForRangeEnemies = TM.CheckAvailableTilesForEnemyAction(hoverUnit.rangeOfAction, hoverUnit);
                }

                tilesAvailableForMovementEnemies = new List<IndividualTiles>(TM.CalculateAvailableTilesForHover(hoverUnit.myCurrentTile, hoverUnit));

                for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
                {
                    tilesAvailableForMovementEnemies[i].ColorMovement();
                }

                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorActionRange();
                }

                hoverUnit.GetComponent<EnGrabber>().CheckUnitToAttack(hoverUnit.myCurrentTile, hoverUnit.currentFacingDirection);

                if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
                {
                    if (hoverUnit.pathToObjective.Count > 0)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1]);
                    }

                    //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                    if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                    }

                    hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                    hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);

                }

                //Pinto rango de acción desde el enemigo si no se mueve.
                //Si por el contrario se mueve, pinto el rango desde la función ShowActionPathFinding
                else
                {
                    hoverUnit.GetComponent<EnGrabber>().CheckUnitToAttack(hoverUnit.shadowTile, hoverUnit.facingDirectionAfterMovement);
                    if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
                    {
                        //Líneas para comprobar si está atacando al Decoy y tiene que hacer la función
                        if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>())
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().ShowAttackEffect(hoverUnit);
                        }

                        if (hoverUnit.pathToObjective.Count > 0)
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1]);
                        }

                        hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                        hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);

                    }

                    else
                    {
                        hoverUnit.CheckTilesInRange(hoverUnit.myCurrentTile, hoverUnit.currentFacingDirection);
                    }  
                }

                //Una vez pintado los tiles naranjas de rango se pinta el tile rojo al que va atacar
                hoverUnit.ColorAttackTile();
            }

            //Goblin, gigante, esqueleto, duelista
            else
            {
                if (hoverUnit.GetComponent<EnGoblin>() && hoverUnit.GetComponent<EnGoblin>().myTierLevel == EnemyUnit.TierLevel.Level2)
                {
                    //Hacer que aparezca el icono de miedo o de rotación en la cabeza del player que va a ser atacado.
                    for (int i = 0; i < hoverUnit.unitsInRange.Count; i++)
                    {
                        if (hoverUnit.unitsInRange[i].GetComponent<EnemyUnit>())
                        {
                            hoverUnit.unitsInRange[i].GetComponent<EnemyUnit>().ShowHideExclamation(true);
                        }
                    }
                }

                //Muestro la acción que va a realizar el enemigo 
                hoverUnit.ShowActionPathFinding(true);

                //¡¡MUY IMPORTANTE!! hoverUnit.ShowActionPathFinding(true); TIENE QUE IR ANTES QUE ESTOS ifs
                if (!hoverUnit.haveIBeenAlerted)
                {
                    //Rango de acción sólo se pinta si no ha sido alertado
                    tilesAvailableForRangeEnemies = TM.CheckAvailableTilesForEnemyAction(hoverUnit.rangeOfAction, hoverUnit);
                }

                tilesAvailableForMovementEnemies = new List<IndividualTiles>(TM.CalculateAvailableTilesForHover(hoverUnit.myCurrentTile, hoverUnit));

                for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
                {
                    tilesAvailableForMovementEnemies[i].ColorMovement();
                }

                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorActionRange();
                }

                if (hoverUnit.currentUnitsAvailableToAttack.Count > 0 && hoverUnit.currentUnitsAvailableToAttack[0] != null)
                {
                    //Pinto sombra
                    if (hoverUnit.pathToObjective.Count >0 && hoverUnit.pathToObjective[0] != null)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1]);
                    }

                    if (hoverUnit.GetComponent<EnDuelist>())
                    {
                        hoverUnit.GetComponent<EnDuelist>().ShowArrowsOnEnemies(true);

                        if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<Knight>())
                        {
                            //0 daño porque le gira antes de atacar
                            hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(0);
                            hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);
                        }

                        //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                        else if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                        }

                        else
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                            hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);
                        }
                    }              
                    
                    else
                    {
                         //Aplico los mismos efectos a las unidades laterales del objetivo si el enemigo es un gigante
                        if (hoverUnit.GetComponent<EnGiant>())
                        {
                            if (hoverUnit.GetComponent<EnGiant>().myTierLevel == EnemyUnit.TierLevel.Level2)
                            {
                                hoverUnit.SetStunIcon(hoverUnit.currentUnitsAvailableToAttack[0], true, true);
                            }

                            hoverUnit.GetComponent<EnGiant>().SaveLateralUnitsForNumberAttackInLevelManager();

                            //Solo hago esto si al lado del tile donde se va a mover hay una unidad a la que atacar.
                            if (hoverUnit.myCurrentTile.neighbours.Contains(hoverUnit.currentUnitsAvailableToAttack[0].myCurrentTile) ||
                                hoverUnit.shadowTile.neighbours.Contains(hoverUnit.currentUnitsAvailableToAttack[0].myCurrentTile))
                            {
                                //Pintar de rojo y mostrar daño

                                //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                                if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                                {
                                    hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                                }

                                hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                                hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);

                                for (int i = 0; i < hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective.Count; i++)
                                {
                                    if (hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile != null)
                                    {
                                        UnitBase tempLateralUnitGiant = hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile;

                                        hoverUnit.GetComponent<EnGiant>().CalculateDamagePreviousAttackLateralEnemies(tempLateralUnitGiant);

                                        tempLateralUnitGiant.CalculateDirectionOfAttackReceivedToShowShield(hoverUnit.pathToObjective[hoverUnit.pathToObjective.Count - 1]);

                                        //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                                        if (tempLateralUnitGiant.GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                                        {
                                            tempLateralUnitGiant.GetComponent<MageDecoy>().ShowBombAttackEffect();
                                        }

                                        tempLateralUnitGiant.ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                                        tempLateralUnitGiant.HealthBarOn_Off(true);

                                        if (hoverUnit.GetComponent<EnGiant>().myTierLevel == EnemyUnit.TierLevel.Level2)
                                        {
                                            hoverUnit.SetStunIcon(hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile, true, true);
                                        }
                                    }
                                }
                            }
                        }

                        //Goblin/Esquelto y lobo
                        else
                        {
                            //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función y que enseñe la bomba
                            if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>() && hoverUnit.damageWithMultipliersApplied > 0)
                            {
                                hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>().ShowBombAttackEffect();
                            }

                            hoverUnit.currentUnitsAvailableToAttack[0].ColorAvailableToBeAttackedAndNumberDamage(hoverUnit.damageWithMultipliersApplied);
                            hoverUnit.currentUnitsAvailableToAttack[0].HealthBarOn_Off(true);
                        }
                    }
                }

                //Pinto rango de acción desde el enemigo si no se mueve.
                //Si por el contrario se mueve, pinto el rango desde la función ShowActionPathFinding
                else
                {
                    hoverUnit.CheckTilesInRange(hoverUnit.myCurrentTile, hoverUnit.currentFacingDirection);
                }

                //Una vez pintado los tiles naranjas de rango se pinta el tile rojo al que va atacar
                hoverUnit.ColorAttackTile();
            }
        }
    }

    public void HideEnemyHover(EnemyUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            if (hoverUnit.GetComponent<EnBalista>())
            {
                hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine(true, hoverUnit.myCurrentTile);

                if (hoverUnit.GetComponent<EnBalista>().currentUnitsAvailableToAttack.Count > 0 && hoverUnit.GetComponent<EnBalista>().isAttackPrepared == false)
                {
                    hoverUnit.GetComponent<EnBalista>().particleChargingAttack.SetActive(hoverUnit.GetComponent<EnBalista>().isAttackPrepared);
                    //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función
                    if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>())
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().HideAttackEffect(hoverUnit);
                    }
                    hoverUnit.GetComponent<EnBalista>().FeedbackTilesToAttack(false);
                }

                else if (hoverUnit.GetComponent<EnBalista>().isAttackPrepared == false)
                {
                    IndividualTiles tileToMove = hoverUnit.GetComponent<EnBalista>().GetTileToMove();
                    if (tileToMove != null)
                    {
                        hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine(true, tileToMove);

                        hoverUnit.sombraHoverUnit.SetActive(false);
                        tileToMove.ColorDeselect();
                        hoverUnit.myLineRenderer.enabled = false;

                        if (hoverUnit.currentUnitsAvailableToAttack.Count > 0 && hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>())
                        {
                            hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().HideAttackEffect(hoverUnit);
                        }
                        hoverUnit.GetComponent<EnBalista>().FeedbackTilesToAttack(false);
                    }
                }

                //Desmarco las unidades disponibles para atacar
                for (int i = 0; i < hoverUnit.currentUnitsAvailableToAttack.Count; i++)
                {
                    hoverUnit.currentUnitsAvailableToAttack[i].DisableCanvasHover();
                    hoverUnit.currentUnitsAvailableToAttack[i].ResetColor();
                    hoverUnit.currentUnitsAvailableToAttack[i].HealthBarOn_Off(false);
                }

            }

            else if (hoverUnit.GetComponent<EnCharger>())
            {
                if (hoverUnit.myTierLevel == EnemyUnit.TierLevel.Level2)
                {
                    hoverUnit.GetComponent<EnCharger>().RemoveIconsFire();
                }

                hoverUnit.sombraHoverUnit.SetActive(false);
                hoverUnit.GetComponent<EnCharger>().CheckCharactersInLine(false, null);
                if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count > 0)
                {
                    //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función
                    if (hoverUnit.currentUnitsAvailableToAttack.Count > 0 && hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>())
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().HideAttackEffect(hoverUnit);
                    }
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(false);
                }
                else if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count == 0)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(false);

                }


                //Desmarco las unidades disponibles para atacar
                for (int i = 0; i < hoverUnit.currentUnitsAvailableToAttack.Count; i++)
                {
                    hoverUnit.currentUnitsAvailableToAttack[i].DisableCanvasHover();
                    hoverUnit.currentUnitsAvailableToAttack[i].ResetColor();
                    hoverUnit.currentUnitsAvailableToAttack[i].HealthBarOn_Off(false);
                }

                hoverUnit.GetComponent<EnCharger>().HideShowResult();

            }

            else if (hoverUnit.GetComponent<EnWatcher>())
            {
                //Hacer que aparezca el icono de miedo o de rotación en la cabeza del player que va a ser atacado.
                for (int i = 0; i < hoverUnit.unitsInRange.Count; i++)
                {      
                    if (hoverUnit.unitsInRange[i].GetComponent<PlayerUnit>())
                    {
                        if(currentLevelState == LevelState.ProcessingPlayerActions)
                        {
                            hoverUnit.SetBuffDebuffIcon(0, hoverUnit.unitsInRange[i], true);
                            hoverUnit.SetMovementIcon(0, hoverUnit.unitsInRange[i], true);
                        }
                       
                    }
                    //hoverUnit.unitsInRange[i].ShowHideFear(false, hoverUnit.unitsInRange[i].turnsWithFear);
                    //hoverUnit.unitsInRange[i].ResetColor();
                    //hoverUnit.unitsInRange[i].DisableCanvasHover();
                }

                //Rango despertar y miedo
                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorDeselect();
                }
                tilesAvailableForRangeEnemies.Clear();
            }

            else if (hoverUnit.GetComponent<EnSummoner>())
            {
                //Rango despertar y miedo
                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorDeselect();
                }
                tilesAvailableForRangeEnemies.Clear();

                hoverUnit.GetComponent<EnSummoner>().HideShowFeedbackSpawnPosition(false);
            }

            else if (hoverUnit.GetComponent<BossMultTile>() || hoverUnit.GetComponent<DarkLord>() || hoverUnit.GetComponent<MechaBoss>())
            {
                hoverUnit.ShowHideInterrogationBoss(false);
            }

            //Goblin, gigante, boss y demás
            else
            {
                if (hoverUnit.GetComponent<EnGoblin>() && hoverUnit.GetComponent<EnGoblin>().myTierLevel == EnemyUnit.TierLevel.Level2)
                {
                    //Hacer que aparezca el icono de miedo o de rotación en la cabeza del player que va a ser atacado.
                    for (int i = 0; i < hoverUnit.unitsInRange.Count; i++)
                    {
                        if (hoverUnit.unitsInRange[i].GetComponent<EnemyUnit>())
                        {
                            hoverUnit.unitsInRange[i].GetComponent<EnemyUnit>().ShowHideExclamation(false);
                        }
                    }
                }

                if (hoverUnit.currentUnitsAvailableToAttack.Count > 0 && hoverUnit.currentUnitsAvailableToAttack[0] != null)
                {
                    //Líneas para comprobar si el está atacando al Decoy y tiene que hacer la función
                    if (hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<MageDecoy>())
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().HideAttackEffect(hoverUnit);
                    }
                    hoverUnit.currentUnitsAvailableToAttack[0].myCurrentTile.ColorDesAttack();
                }

                hoverUnit.HideActionPathfinding();

                for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
                {
                    tilesAvailableForMovementEnemies[i].ColorDeselect();
                }
                tilesAvailableForMovementEnemies.Clear();

                for (int i = 0; i < tilesAvailableForRangeEnemies.Count; i++)
                {
                    tilesAvailableForRangeEnemies[i].ColorDeselect();
                }
                tilesAvailableForRangeEnemies.Clear();

                if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
                {
                    if (hoverUnit.currentUnitsAvailableToAttack[0] != null)
                    {
                        hoverUnit.currentUnitsAvailableToAttack[0].ResetColor();
                        hoverUnit.currentUnitsAvailableToAttack[0].previsualizeAttackIcon.SetActive(false);
                        hoverUnit.currentUnitsAvailableToAttack[0].DisableCanvasHover();
                    }
                }

                if (hoverUnit.GetComponent<EnDuelist>())
                {
                    hoverUnit.GetComponent<EnDuelist>().ShowArrowsOnEnemies(false);
                }

                else if (hoverUnit.GetComponent<EnGrabber>())
                {
                    hoverUnit.GetComponent<EnGrabber>().HideGrabShadow();
                }

                //Aplico los mismos efectos a las unidades laterales del objetivo si el enemigo es un gigante
                else if (hoverUnit.GetComponent<EnGiant>())
                {  
                    if (hoverUnit.currentUnitsAvailableToAttack.Count > 0)
                    {
                        hoverUnit.SetStunIcon(hoverUnit.currentUnitsAvailableToAttack[0], true, false);
                    }

                    for (int i = 0; i < hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective.Count; i++)
                    {
                        if (hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile != null)
                        {
                            UnitBase tempLateralUnitGiant = hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile;

                            tempLateralUnitGiant.ResetColor();
                            tempLateralUnitGiant.previsualizeAttackIcon.SetActive(false);
                            tempLateralUnitGiant.DisableCanvasHover();

                            if (tempLateralUnitGiant.GetComponent<Knight>())
                            {
                                tempLateralUnitGiant.GetComponent<PlayerUnit>().ShowHideFullShield(false);
                                tempLateralUnitGiant.GetComponent<PlayerUnit>().ShowHidePartialShield(false);
                            }
                        }

                        hoverUnit.SetStunIcon(hoverUnit.GetComponent<EnGiant>().tempLateralTilesToFutureObjective[i].unitOnTile, true, false);
                    }
                }

                hoverUnit.ClearRange();
            }

            if (hoverUnit.currentUnitsAvailableToAttack.Count > 0 && hoverUnit.currentUnitsAvailableToAttack[0] != null)
            {
                hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().ShowHideFullShield(false);
                hoverUnit.currentUnitsAvailableToAttack[0].GetComponent<PlayerUnit>().ShowHidePartialShield(false);
            }
        }
    }

   public void CalculatePreviousActionPlayer(PlayerUnit playerToDo, UnitBase unitToDO)
   {
       playerToDo.ShowAttackEffect(unitToDO);
   }

    #endregion

    #region SELECT_&_DESELECT

    //Al clickar sobre una unidad del jugador se llama a esta función
    public void SelectUnit(int movementUds, PlayerUnit clickedUnit)
    {
        //Si es el comienzo del nivel y estoy recolocando las unidades
        if (currentLevelState == LevelState.Initializing && !clickedUnit.characterStartedOnTheLevel)
        {
            if (charactersAlreadyPlaced.Count <= GameManager.Instance.maxUnitsInThisLevel)
            {
                //Si clicko en unidad sin tile es que esta en la caja
                if (clickedUnit.GetComponent<PlayerUnit>().myCurrentTile == null)
                {
                    //Si no tenia ninguna otra unidad seleccionada simplemente selecciono la unidad clickada
                    if (currentCharacterPlacing == null)
                    {
                        Debug.Log("First caja");
                        currentCharacterPlacing = clickedUnit;
                    }

                    //Si ya tenia una unidad clickada y esta tiene tile, significa que quiero intercambiar una unidad de tablero con una de la caja
                    else if (currentCharacterPlacing.myCurrentTile != null)
                    {
                        Debug.Log("Sustitute caja");
                        SustituteUnitsOnPlacementPhase(currentCharacterPlacing.myCurrentTile, clickedUnit);
                    }

                    //Si no el personaje clickado estaba en la caja y lo único que hago es cambiar la selección.
                    else
                    {
                        Debug.Log("Second caja");
                        currentCharacterPlacing.ResetColor();
                        currentCharacterPlacing = clickedUnit;
                    }
                }

                //Si clicko una unidad del tablero
                else
                {
                    if (currentCharacterPlacing == null)
                    {
                        Debug.Log("First tablero");
                        currentCharacterPlacing = clickedUnit;
                    }

                    //Intercambio unidad entre tablero.
                    else if (currentCharacterPlacing.myCurrentTile != null)
                    {
                        Debug.Log("Sustitute manual tablero");
                        SustituteUnitsOnPlacementPhase(clickedUnit.myCurrentTile, clickedUnit);
                    }

                    //Intercambio con unidad en caja
                    else
                    {
                        Debug.Log("SUSTITUTE Con personaje tablero");
                        SustituteUnitsOnPlacementPhase(clickedUnit.myCurrentTile, clickedUnit);
                    }
                }

                Debug.Log("---------");
            }
        }

        //Si es el turno del player compruebo si puedo hacer algo con la unidad.
        if (currentLevelState == LevelState.ProcessingPlayerActions && !GameManager.Instance.isGamePaused)
        {
            //Si no hay unidad seleccionada significa que está seleccionando una unidad
            if (selectedCharacter == null)
            {
				//Si no se ha movido significa que la puedo mover y doy feedback de sus casillas de movimiento
				if (!clickedUnit.hasAttacked && !clickedUnit.hasMoved)
                {
                    if (selectedEnemy != null)
                    {
                        DeselectEnemy();
                    }

                    //Desactivo el botón de pasar turno cuando selecciona la unidad
                    //Está función no se puede llamar fuera del if para que afecte a ambos casos porque entonces también se cambia al pulsar en una unidad que ya ha atacado.
                    UIM.ActivateDeActivateEndButton();
					UIM.TooltipMove();
					
                    selectedCharacter = clickedUnit;

                    selectedCharacter.myCurrentTile.ColorCurrentTileHover();
                    
					//selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
                    selectedCharacter.SelectedColor();

                    ///Esta línea comentada estaba antes para calcular los tiles pero daba problemas y no cogía a veces todos los tiles. En principio como al hacer hover ya los calculo una vez y lo hace bien,
                    ///uso esos para todos los calculos de pintar,mover etc.
                    /////tilesAvailableForMovement = new List<IndividualTiles>(TM.OptimizedCheckAvailableTilesForMovement(clickedUnit.movementUds, clickedUnit, true));
                   
                    //He puesto esta linea porque si tienes un pj seleccionado al hacer hover sobre otro no se pintan los tiles por lo que si lo seleccionas no aparece ninguno. En principio esta función
                    //Calcuala bien los tiles y no se quedan sin despintar
                    tilesAvailableForMovement = new List<IndividualTiles>(TM.CalculateAvailableTilesForHover(clickedUnit.myCurrentTile, clickedUnit));

                    Debug.Log("Seleccionar personaje: " + selectedCharacter + " " + tilesAvailableForMovement.Count);

                    for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                    {
                        tilesAvailableForMovement[i].ColorMovement();
                    }

                    selectedCharacter.CheckUnitsAndTilesInRangeToAttack(false);

					if (selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
					{
						UIM.TooltipMoveorAttack();
                    }

					SoundManager.Instance.PlaySound(AppSounds.PLAYER_SELECTION);
                }

                //Si se ha movido pero no ha atacado, entonces le doy el feedback de ataque.
                else if (!clickedUnit.hasAttacked)
                {
                    if (selectedEnemy != null)
                    {
                        DeselectEnemy();
                    }

                    //Desactivo el botón de pasar turno cuando selecciona la unidad
                    UIM.ActivateDeActivateEndButton();

                    selectedCharacter = clickedUnit;
                    
                    
					//selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
					/*UIM.ShowCharacterInfo(selectedCharacter.unitInfo, selectedCharacter);*/ /*Legacy Code*/
					selectedCharacter.SelectedColor();

                    selectedCharacter.CheckUnitsAndTilesInRangeToAttack(false);

                    //He añadido este if para que el rogue no pueda atacar dos veces a la misma unidad usando una de sus habilidades
                    if (selectedCharacter.GetComponent<Rogue>())
                    {

                        for (int i = 0; i < selectedCharacter.GetComponent<Rogue>().unitsAttacked.Count; i++)
                        {
                            selectedCharacter.currentUnitsAvailableToAttack.Remove(selectedCharacter.GetComponent<Rogue>().unitsAttacked[i]);
                            selectedCharacter.GetComponent<Rogue>().unitsAttacked[i].ResetColor();
                        }
                    }

                    
                    if (selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
					{
						//UIM.TooltipAttack();
					}
					else
					{
						//UIM.TooltipNoAttackable();
					}

                    SoundManager.Instance.PlaySound(AppSounds.PLAYER_SELECTION);
                }
			}

            //Si ya hay una seleccionada compruebo si puedo atacar a la unidad
            else
            {
                if (selectedEnemy != null)
                {
                    DeselectEnemy();   
                }

                if (!selectedCharacter.isMovingorRotating && !selectedCharacter.GetComponent<BossMultTile>())
                {
                    SelectUnitToAttack(clickedUnit);
                }
            }
        }
    }

    public void DeSelectUnit()
    {
        if (selectedCharacter != null && !GameManager.Instance.isGamePaused)
        {
            if (selectedCharacter.isMovingorRotating)
            {
                //Hacer que desaparezcan los botones de rotación
                selectedCharacter.isMovingorRotating = false;
                selectedCharacter.canvasWithRotationArrows.gameObject.SetActive(false);
            }
           
            selectedCharacter.HealthBarOn_Off(false);
            selectedCharacter.previsualizeAttackIcon.SetActive(false);
            selectedCharacter.canvasHover.SetActive(false);
            selectedCharacter.notAttackX.SetActive(false);
            selectedCharacter.myPanelPortrait.GetComponent<Portraits>().UnHighlightPortrait();
            selectedCharacter.myPanelPortrait.GetComponent<Portraits>().isClicked = false;
            UIM.TooltipDefault();

            if (selectedCharacter.GetComponent<Samurai>())
            {
                selectedCharacter.GetComponent<Samurai>().lonelyBox.SetActive(false);
            }

            //Desmarco las unidades disponibles para atacar
            for (int i = 0; i < selectedCharacter.currentUnitsAvailableToAttack.Count; i++)
            {
                if (selectedCharacter.currentUnitsAvailableToAttack[i] != null)
                {
                    selectedCharacter.currentUnitsAvailableToAttack[i].ResetColor();
                    selectedCharacter.currentUnitsAvailableToAttack[i].myCurrentTile.ColorDesAttack();
                    selectedCharacter.currentUnitsAvailableToAttack[i].previsualizeAttackIcon.SetActive(false);
                    selectedCharacter.currentUnitsAvailableToAttack[i].DisableCanvasHover();
                    selectedCharacter.currentUnitsAvailableToAttack[i].HealthBarOn_Off(false);
                    selectedCharacter.currentUnitsAvailableToAttack[i].notAttackX.SetActive(false);
                }
            }

            //Desmarco los tiles indicando rango de ataque
            if (selectedCharacter.currentTilesInRangeForAttack.Count > 0)
            {
                for (int i = 0; i < selectedCharacter.currentTilesInRangeForAttack.Count; i++)
                {
                    selectedCharacter.currentTilesInRangeForAttack[i].ColorDeselect();
                }
            }

            Debug.Log("(Antes descolorear) Deseleccionar unidad " + selectedCharacter + " " + tilesAvailableForMovement.Count);

            //Si no se ha movido lo deselecciono.
            for (int i = 0; i < tilesAvailableForMovement.Count; i++)
            {
                tilesAvailableForMovement[i].ColorDeselect();
            }

            Debug.Log("(Descolorear) Deseleccionar unidad " + selectedCharacter + " " + tilesAvailableForMovement.Count);

            //Activo el botón de end turn para que no le de mientras la unidad siga seleccionada
            UIM.ActivateDeActivateEndButton();
            selectedCharacter.HideDamageIcons(selectedCharacter);
            
            tilesAvailableForMovement.Clear();
            selectedCharacter.ResetColor();
            selectedCharacter.myCurrentTile.ColorDeselect();

            if(selectedCharacter.currentUnitsAvailableToAttack.Count > 0 && selectedCharacter.currentUnitsAvailableToAttack[0] != null)
            {
                if (!selectedCharacter.currentUnitsAvailableToAttack[0].isMarked)
                {
                    selectedCharacter.currentUnitsAvailableToAttack[0].monkMark.SetActive(false);
                    selectedCharacter.currentUnitsAvailableToAttack[0].monkMarkUpgrade.SetActive(false);                   
                }

            }

            //Estas líneas las añado para comprobar si el halo de la valquiria tiene que salir
            Valkyrie valkyrieRef = FindObjectOfType<Valkyrie>();
            if (valkyrieRef != null)
            {
                valkyrieRef.ChangePositionIconFeedback(false, valkyrieRef);
                //Desmarco las unidades disponibles para atacar
                for (int i = 0; i < valkyrieRef.currentUnitsAvailableToAttack.Count; i++)
                {
                    if (selectedCharacter.currentUnitsAvailableToAttack[i] != null)
                    {
                        valkyrieRef.ChangePositionIconFeedback(false, valkyrieRef.currentUnitsAvailableToAttack[i]);
                    }
                }
                valkyrieRef.CheckValkyrieHalo();
            }

            selectedCharacter = null;

            //Reactivo el collider de las unidades que se les ha quitado al seleccionar tile para rotar.
            for (int j = 0; j < unitsToEnableCollider.Count; j++)
            {
                unitsToEnableCollider[j].EnableUnableCollider(true);
            }

            unitsToEnableCollider.Clear();

          
        }

        else if(selectedEnemy !=null)
        {
            DeselectEnemy();
        }
    }

    public void SelectEnemy(string _unitInfo, EnemyUnit _enemySelected)
    {
        if (!GameManager.Instance.isGamePaused)
        {
            DeselectEnemy();

            selectedEnemy = _enemySelected;

            CheckIfHoverShouldAppear(_enemySelected);

            _enemySelected.SelectedFunctionality();
            UIM.MoveScrollToEnemy(_enemySelected);
        }
	}

    //Función que se llama al clickar sobre un enemigo o sobre un aliado si ya tengo seleccionado un personaje
    public void SelectUnitToAttack(UnitBase clickedUnit)
    {
        if (selectedCharacter != null && !clickedUnit.GetComponent<BossMultTile>())
        {
            if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
            {
                if (selectedCharacter.currentUnitsAvailableToAttack.Contains(clickedUnit) && !GetComponent<BossMultTile>())
                {
                    selectedCharacter.Attack(clickedUnit);
                }

                //enemiesNumber = selectedCharacter.currentUnitsAvailableToAttack.Count;
                //
                //Compruebo si está en la lista de posibles targets
                //for (int i = 0; i < enemiesNumber; i++)
                //{
                //    if (selectedCharacter != null && !selectedCharacter.isMovingorRotating && !selectedCharacter.hasAttacked)
                //    {
                //        if (clickedUnit == selectedCharacter.currentUnitsAvailableToAttack[i])
                //        {


                //            UnitBase obj = selectedCharacter.currentUnitsAvailableToAttack[i];

                //            ICommand command = new AttackCommand(obj.currentFacingDirection, selectedCharacter.currentFacingDirection,
                //                                                 obj.myCurrentTile, selectedCharacter.myCurrentTile,
                //                                                 obj.currentHealth, selectedCharacter.currentHealth,
                //                                                 selectedCharacter, obj,
                //                                                 selectedCharacter.currentArmor, obj.currentArmor,
                //                                                 selectedCharacter.isStunned, obj.isStunned,
                //                                                 selectedCharacter.isMarked, obj.isMarked, selectedCharacter.numberOfMarks, obj.numberOfMarks);
                //            CommandInvoker.AddCommand(command);
                //            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                //            UIM.CheckActionsAvaliable();
                //            return;
                //        }
                //    }
                //}
            }

            //Si llega hasta aqui significa que la unidad seleccionada no formaba parte de las unidades a las que puede atacar.
            //Compruebo si es un player y de ser así lo selecciono
            //PREGUNTAR SI AQUÍ FALTA UN ELSE (al atacar y matar con el ninja sale el su número y la espada)
            else
            {
                if (clickedUnit.GetComponent<PlayerUnit>() && !clickedUnit.GetComponent<MageDecoy>())
                {
                    DeSelectUnit();
                    SelectUnit(clickedUnit.movementUds, clickedUnit.GetComponent<PlayerUnit>());
                }

                else if (clickedUnit.GetComponent<EnemyUnit>())
                {
                    DeSelectUnit();
                    SelectEnemy(clickedUnit.unitGeneralInfo, clickedUnit.GetComponent<EnemyUnit>());
                }
            }
        }
    }

    public void DeselectEnemy()
    {
        if (selectedEnemy != null)
        {
            selectedEnemy.HealthBarOn_Off(false);
            selectedEnemy.ResetColor();

            if (selectedEnemy.myPortrait != null)
                selectedEnemy.myPortrait.UnHighlightMyself();

            HideEnemyHover(selectedEnemy);

            UIM.TooltipDefault();
            selectedEnemy = null;
        }
    }

    #endregion

    #region TILE_&_PLACEMENT

    //Decido si muevo a la unidad, si tengo que colocarla por primera vez o si no hago nada
    public void TileClicked(IndividualTiles tileToMove)
    {
        //Si es el comienzo del nivel
        if (currentLevelState == LevelState.Initializing)
        {
            if (currentCharacterPlacing != null && charactersAlreadyPlaced.Count < GameManager.Instance.maxUnitsInThisLevel)
            {
                //Coloco el personaje si el tile está pensado para colocar personajes y está vacío
                if (tileToMove.isAvailableForCharacterColocation && tileToMove.unitOnTile == null)
                {
                    //Le coloco en el tile
                    if (!charactersAlreadyPlaced.Contains(currentCharacterPlacing))
                    {
                        charactersAlreadyPlaced.Add(currentCharacterPlacing);
                    }
                    currentCharacterPlacing.transform.position = tileToMove.transform.position;
                    currentCharacterPlacing.GetComponent<PlayerUnit>().UpdateInformationAfterMovement(tileToMove);

                    //Le quito el padre y le pongo en la escala y rotación correcta
                    currentCharacterPlacing.transform.parent = null;
                    currentCharacterPlacing.transform.localScale = Vector3.one;
                    currentCharacterPlacing.transform.rotation = Quaternion.identity;

                    //Variable a null
                    currentCharacterPlacing = null;

                    UIM.UpdateUnitsPlaced(charactersAlreadyPlaced.Count);
                }

                if (charactersAlreadyPlaced.Count == GameManager.Instance.maxUnitsInThisLevel)
                {
                    //Aparece el botón
                    UIM.finishUnitPlacement.SetActive(true);
                }
            }
        }

        else if (!GameManager.Instance.isGamePaused)
        {
            //Movimiento de la unidad
            if (selectedCharacter != null && !selectedCharacter.hasAttacked && !selectedCharacter.hasMoved)
            {

                SoundManager.Instance.PlaySound(AppSounds.TILECLICK);

                ///Esta línea comentada estaba antes para calcular los tiles pero daba problemas y no cogía a veces todos los tiles. En principio como al hacer hover ya los calculo una vez y lo hace bien,
                ///uso esos para todos los calculos de pintar,mover etc.
                //tilesAvailableForMovement = new List<IndividualTiles>(TM.OptimizedCheckAvailableTilesForMovement(selectedCharacter.movementUds, selectedCharacter, true));

                //He puesto esta linea porque si tienes un pj seleccionado al hacer hover sobre otro no se pintan los tiles por lo que si lo seleccionas no aparece ninguno. En principio esta función
                //Calcuala bien los tiles y no se quedan sin despintar
                tilesAvailableForMovement = new List<IndividualTiles>(TM.CalculateAvailableTilesForHover(selectedCharacter.myCurrentTile, selectedCharacter));

                //Reactivo el collider de las unidades que se les ha quitado antes. Esto es por si hace click en otro tile sin haber rotado.
                for (int j = 0; j < unitsToEnableCollider.Count; j++)
                {
                    unitsToEnableCollider[j].EnableUnableCollider(true);
                }

                unitsToEnableCollider.Clear();

                //Añado la unidad actual a las unidades a las que quitar el collider.
                selectedCharacter.EnableUnableCollider(false);
                unitsToEnableCollider.Add(selectedCharacter);

                //Desactivo collider de unidades que rodean al tile clickado
                for (int j = 0; j < tileToMove.surroundingNeighbours.Count; j++)
                {
                    if (tileToMove.surroundingNeighbours[j].unitOnTile != null)
                    {
                        tileToMove.surroundingNeighbours[j].unitOnTile.EnableUnableCollider(false);
                        unitsToEnableCollider.Add(tileToMove.surroundingNeighbours[j].unitOnTile);
                    }
                }

                for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                {
                    if (tileToMove == tilesAvailableForMovement[i] || tileToMove == selectedCharacter.myCurrentTile)
                    {
                        //Calculo el path de la unidad
                        TM.CalculatePathForMovementCost(tileToMove.tileX, tileToMove.tileZ, false);
                        selectedCharacter.myCurrentTile.ColorDeselect();

                        //Aviso a la unidad de que se tiene que mover
                        if (selectedCharacter != null)
                        {
                            tileToMoveAfterRotate = tileToMove;
                             
                            //Hacer que aparezcan los botones de rotación
                            selectedCharacter.isMovingorRotating = true;
                            selectedCharacter.canvasWithRotationArrows.gameObject.SetActive(true);

                            #region DEPRECATED_ALTURA_FLECHAS_ROTACION
                            //offsetHeightArrow = 0.5f;
                            //offsetHeightArrowForEnemies = 0.5f;

                            ////Vemos si tiene enemigos o tiles más altos cerca para que las flechas no se tapen
                            //for (int j = 0; j < tileToMove.neighbours.Count; ++j)
                            //{
                            //    if (tileToMove.neighbours[j].isObstacle 
                            //            || tileToMove.neighbours[j].isEmpty 
                            //            || tileToMove.neighbours[j].noTilesInThisColumn)
                            //    {
                            //        offsetHeightArrow += 0.5f;

                            //     }

                            //    if (tileToMove.neighbours[j].height > tileToMove.height)
                            //        {
                            //        offsetHeightArrow += 0.5f;


                            //        }

                            //    if (tileToMove.neighboursOcuppied >= 1)
                            //    {
                            //        offsetHeightArrowForEnemies+= 0.5f;
                            //    }


                            //}

                            //offsetHeightArrow += offsetHeightArrowForEnemies;

                            #endregion

                            //Coloco las flechas de rotación
                            Vector3 positionToSpawn = new Vector3(tileToMove.transform.position.x, tileToMove.transform.position.y + offsetHeightArrow, tileToMove.transform.position.z);
                            selectedCharacter.canvasWithRotationArrows.gameObject.transform.position = positionToSpawn;
							UIM.TooltipRotate();

							//Desmarco las unidades que antes estaban disponibles para ser atacadas
							if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0 && tileToMove != selectedCharacter.myCurrentTile)
                            {
                                for (int j = 0; j < selectedCharacter.currentTilesInRangeForAttack.Count; j++)
                                {
                                    selectedCharacter.currentTilesInRangeForAttack[j].ColorDeselect();
                                }

                                for (int j = 0; j < selectedCharacter.currentUnitsAvailableToAttack.Count; j++)
                                {
                                    if (selectedCharacter.currentUnitsAvailableToAttack[j] != null)
                                    {
                                        selectedCharacter.currentUnitsAvailableToAttack[j].ResetColor();
                                        selectedCharacter.currentUnitsAvailableToAttack[j].previsualizeAttackIcon.SetActive(false);
                                        selectedCharacter.currentUnitsAvailableToAttack[j].DisableCanvasHover();
                                    }
                                }
                            }

                        }
                    }  
                }
            }
        }
    }

    //Si hago click en una unidad en la caja y luego a otra en escena, devuelve la de escena a la caja y coloca la clickada en el tile en el que estaba la anterior.
    private void SustituteUnitsOnPlacementPhase(IndividualTiles _tileToMove, PlayerUnit _playerClicked)
    {
        //Basicamente estos dos ifs hacen lo mismo pero a la inversa. Uno colocael personaje clickado en el tablero y devuelve el ya seleccionado a la caja y el otro lo contrario.

        //Personaje ya seleccionado anteriormente estaba en tablero y es el que vuelve a la caja

        if (_playerClicked.myCurrentTile != null && currentCharacterPlacing.myCurrentTile != null)
        {
            IndividualTiles otherUnitTile = currentCharacterPlacing.myCurrentTile;

            //Quito unidad anteriormente seleccionada de su tile
            currentCharacterPlacing.myCurrentTile.unitOnTile = null;
            currentCharacterPlacing.myCurrentTile.WarnInmediateNeighbours();
            currentCharacterPlacing.myCurrentTile = null;

            //La muevo a al nuevo tile
            currentCharacterPlacing.transform.position = _tileToMove.transform.position;
            currentCharacterPlacing.transform.rotation = Quaternion.identity;
            currentCharacterPlacing.UpdateInformationAfterMovement(_tileToMove);

            currentCharacterPlacing.ResetColor();

            //Quito unidad anteriormente seleccionada de su tile
            _playerClicked.myCurrentTile.unitOnTile = null;
            _playerClicked.myCurrentTile.WarnInmediateNeighbours();
            _playerClicked.myCurrentTile = null;

            //La muevo a al nuevo tile
            _playerClicked.transform.position = otherUnitTile.transform.position;
            _playerClicked.transform.rotation = Quaternion.identity;
            _playerClicked.UpdateInformationAfterMovement(otherUnitTile);

            //Variable a null
            currentCharacterPlacing = null;
        }

        else
        {
            if (currentCharacterPlacing.myCurrentTile != null)
            {
                //QUITAR UNIDAD EN TABLERO
                //Quitar de la listas de unidades en el tablero
                charactersAlreadyPlaced.Remove(currentCharacterPlacing);

                //Quitar tile y avisar
                currentCharacterPlacing.myCurrentTile.unitOnTile = null;
                currentCharacterPlacing.myCurrentTile.WarnInmediateNeighbours();
                currentCharacterPlacing.myCurrentTile = null;

                //Devolver a la caja, padre y tamaño
                currentCharacterPlacing.transform.parent = currentCharacterPlacing.initialPosInBox;
                currentCharacterPlacing.transform.localScale = Vector3.one;
                currentCharacterPlacing.transform.localPosition = Vector3.zero;
                currentCharacterPlacing.transform.localRotation = Quaternion.identity;

                currentCharacterPlacing.ResetColor();

                //PONER UNIDAD EN TABLERO

                //Le coloco en el tile
                charactersAlreadyPlaced.Add(_playerClicked);
                _playerClicked.transform.position = _tileToMove.transform.position;
                _playerClicked.UpdateInformationAfterMovement(_tileToMove);

                //Le pongo en la escala correcta
                _playerClicked.transform.parent = null;
                _playerClicked.transform.localScale = Vector3.one;
                _playerClicked.transform.rotation = Quaternion.identity;

                //Variable a null
                currentCharacterPlacing = null;
            }

            //Personaje clickado es el que esta en el tablero y es el que vuelve a la caja
            else
            {
                //QUITAR UNIDAD EN TABLERO
                //Quitar de la listas de unidades en el tablero
                charactersAlreadyPlaced.Remove(_playerClicked);

                //Quitar tile y avisar
                _playerClicked.myCurrentTile.unitOnTile = null;
                _playerClicked.myCurrentTile.WarnInmediateNeighbours();
                _playerClicked.myCurrentTile = null;

                //Devolver a la caja, padre y tamaño
                _playerClicked.transform.parent = _playerClicked.initialPosInBox;
                _playerClicked.transform.localScale = Vector3.one;
                _playerClicked.transform.localPosition = Vector3.zero;
                _playerClicked.transform.localRotation= Quaternion.identity;

                //PONER UNIDAD EN TABLERO

                //Le coloco en el tile
                charactersAlreadyPlaced.Add(currentCharacterPlacing);
                currentCharacterPlacing.transform.position = _tileToMove.transform.position;
                currentCharacterPlacing.GetComponent<PlayerUnit>().UpdateInformationAfterMovement(_tileToMove);

                //Le pongo en la escala correcta
                currentCharacterPlacing.transform.parent = null;
                currentCharacterPlacing.transform.localScale = Vector3.one;
                currentCharacterPlacing.transform.rotation = Quaternion.identity;

                //Variable a null
                currentCharacterPlacing = null;
            }

        }
    }

        #endregion

    //Cuando el jugador elige la rotación de la unidad se avisa para que reaparezca el botón de end turn.
    public void UnitHasFinishedMovementAndRotation()
    {
        //Al terminar de moverse se despintan los tiles 
        for (int j = 0; j < tilesAvailableForMovement.Count; j++)
        {
            tilesAvailableForMovement[j].ColorDeselect();
        }
        tilesAvailableForMovement.Clear();


        if (selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
        {
            UIM.TooltipAttack();
        }
        else
        {
            UIM.TooltipNoAttackable();
        }

        //Desmarco los tiles indicando rango de ataque
        if (selectedCharacter.currentTilesInRangeForAttack.Count > 0)
        {
            for (int i = 0; i < selectedCharacter.currentTilesInRangeForAttack.Count; i++)
            {
                selectedCharacter.currentTilesInRangeForAttack[i].ColorDeselect();
            }
        }

        for (int j = 0; j < unitsToEnableCollider.Count; j++)
        {
            unitsToEnableCollider[j].EnableUnableCollider(true);
        }
    }

    #endregion

    //Función intermediaria que sirve para calcular un área en forma de rombo
    public List<IndividualTiles> CalculateRhombusArea(IndividualTiles _rhombusCenter, int _radius)
    {
       return TM.GetRhombusTiles(_rhombusCenter, _radius);
    }

    public void SkipEnemyAnimation()
    {
        enemiesOnTheBoard[counterForEnemiesOrder].SkipAnimation();
    }

    #region TURN_STATE

    public void ChangePhase()
    {
        FindObjectOfType<CommandInvoker>().ResetCommandList();

        currentLevelState = LevelState.EnemyPhase;
    }

    IEnumerator ChangePhaseWait()
    {
        if(currentLevelState == LevelState.EnemyPhase)
        {
            //Aparece cartel
            UIM.EnemyTurnBanner(true);
            SoundManager.Instance.PlaySound(AppSounds.ENEMYTURN);

            //Pausa
            yield return new WaitForSeconds(UIM.bannerTime);
            
            //Comienza turno enemigo
            UIM.EnemyTurnBanner(false);
            BeginEnemyPhase();
        }

        else if (currentLevelState == LevelState.PlayerPhase)
        {
            //Aparece cartel
            UIM.PlayerTurnBanner(true);
            SoundManager.Instance.PlaySound(AppSounds.PLAYERTURN);

            //Pausa
            yield return new WaitForSeconds(UIM.bannerTime);

            //Comienza turno player
            UIM.PlayerTurnBanner(false);
			if (tutorialLevel2 || tutorialLevel3 || tutorialLevel4)
			{
				tutorialGameObject.SetActive(true);
			}

			BeginPlayerPhase();
        }
    }

    private void BeginPlayerPhase()
    {
        //POR SI ACASO, reactivo los colliders de los personajes que se les desactivo antes.
        for (int j = 0; j < unitsToEnableCollider.Count; j++)
        {
            unitsToEnableCollider[j].EnableUnableCollider(true);
        }

        //Recoloco la lista de enemigos donde estaba al inicio.
        UIM.ResetScrollPosition();

        //Hago desaparecer el botón de fast forward y aparecer el de undo
        UIM.HideShowEnemyUi(false);

        camRef.SetCameraMovable(true,true);

        //Quito los booleanos de los tiles de daño para que puedan hace daño el próximo turno.
        for (int i = 0; i < damageTilesInBoard.Count; i++)
        {
            damageTilesInBoard[i].damageDone = false;

            if (damageTilesInBoard[i].GetComponent<SmokeTile>())
            {
                damageTilesInBoard[i].GetComponent<SmokeTile>().tileCounter--;
                if (damageTilesInBoard[i].GetComponent<SmokeTile>().tileCounter <= 0)
                {
                    Destroy(damageTilesInBoard[i]);
                }
            }
           
        }

        //Me aseguro que no quedan tiles en la lista de tiles para moverse.
        tilesAvailableForMovement.Clear();
        UpdateUnitsOrder();

        if (currentTurn > 0)
        {
            //Aparece cartel con turno del player
            UIM.RotateButtonStartPhase();
			UIM.ResetActionsAvaliable();

            //Resetear todas las variables tipo bool y demás de los players
            for (int i = 0; i < charactersOnTheBoard.Count; i++)
            {
                        
                if (charactersOnTheBoard[i] != null && charactersOnTheBoard[i].GetComponent<Berserker>() != null)
                {
                   charactersOnTheBoard[i].GetComponent<Berserker>().RageChecker();                        
                }

                charactersOnTheBoard[i].ResetUnitState();
                CheckIfGameOver();
            }

            //Reaparecer el botón de endbutton
            UIM.ActivateDeActivateEndButton();

            Debug.Log(currentTurn);
            currentTurn++;
            Debug.Log(currentTurn);
            UIM.UpdateTurnNumber(currentTurn, turnLimit);
            selectedCharacter = null;
            currentLevelState = LevelState.ProcessingPlayerActions;

            CheckIfGameOver();
        }

        else
        {
            currentTurn = 1;
            UIM.UpdateTurnNumber(currentTurn, turnLimit);
            AlertEnemiesOfPlayerMovement();
        }
    }

    //Se llama desde el UI Manager al pulsar el botón de end turn 
    private void BeginEnemyPhase()
    {
        //Hago aparecer el botón de fast forward y desaparecer el de undo
        UIM.HideShowEnemyUi(true);

        UpdateUnitsOrder();

        if (enemiesOnTheBoard.Count > 0)
        {
            //Desaparece botón de end turn
            UIM.ActivateDeActivateEndButton();

            //Aparece cartel con turno del enemigo
            //Me aseguro de que el jugador no puede interactuar con sus pjs
            //Actualizo el número de unidades en el tablero (Lo hago aquí en vez de  al morir la unidad para que no se cambie el orden en medio del turno enemigo)

            counterForEnemiesOrder = 0;

            if (!enemiesOnTheBoard[counterForEnemiesOrder].isDead && (enemiesOnTheBoard[counterForEnemiesOrder].haveIBeenAlerted || enemiesOnTheBoard[counterForEnemiesOrder].isGoingToBeAlertedOnEnemyTurn))
            {
                //Focus en enemigo si está despierto
                camRef.SetCameraMovable(false, true);
                camRef.LockCameraOnEnemy(enemiesOnTheBoard[counterForEnemiesOrder].gameObject);

                //Turn Start
                enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
            }

            else
            {
                counterForEnemiesOrder = 0;
                currentLevelState = LevelState.PlayerPhase;
            }
        }
    }

    //Cuando el enemigo acaba sus acciones avisa al LM para que la siguiente unidad haga sus acciones.
    public void NextEnemyInList()
    {
        if (counterForEnemiesOrder >= enemiesOnTheBoard.Count-1)
        {
            counterForEnemiesOrder = 0;
            currentLevelState = LevelState.PlayerPhase;
        }

        else
        {
            counterForEnemiesOrder++;

            if (!enemiesOnTheBoard[counterForEnemiesOrder].isDead)
            {

                if (enemiesOnTheBoard[counterForEnemiesOrder].haveIBeenAlerted || enemiesOnTheBoard[counterForEnemiesOrder].isGoingToBeAlertedOnEnemyTurn)
                {
                    //Bajo la lista de scroll
                    UIM.ScrollUpOnce();

                    //Focus en enemigo si está despierto
                    camRef.SetCameraMovable(false, true);
                    camRef.LockCameraOnEnemy(enemiesOnTheBoard[counterForEnemiesOrder].gameObject);

                    //Empieza el turno del siguiente enemigo
                    enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
                }

                else
                {
                    counterForEnemiesOrder = 0;
                    currentLevelState = LevelState.PlayerPhase;
                }
            }

            else
            {
                counterForEnemiesOrder++;
                NextEnemyInList();

                if (counterForEnemiesOrder >30)
                {
                    Debug.LogError("Custom error para evitar loop recursivo");
                    Debug.Break();
                }
            }
        }
    }

    //Función que se llama cuando se termina de colocar unidades
    public void FinishPlacingUnits()
    {
        if (FindObjectOfType<MechaBoss>() || FindObjectOfType<DarkLord>() || FindObjectOfType<BossMultTile>())
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayMusic(AppMusic.BOSS_MUSIC);
        }
        else
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayMusic(AppMusic.COMBAT_MUSIC);
        }

        for (int i = 0; i < tilesForCharacterPlacement.Count; i++)
        {
            tilesForCharacterPlacement[i].ColorDeselect();
        }

        characterSelectionBox.SetActive(false);

        for (int i = 0; i < charactersAlreadyPlaced.Count; i++)
        {
            //Añado las unidades que han quedado al final colocadas en la lista de unidades en el tablero.
            charactersOnTheBoard.Add(charactersAlreadyPlaced[i]);
            charactersAlreadyPlaced[i].insideGameInfoObject.SetActive(true);
        }

        for (int i = 0; i < GameManager.Instance.characterDataForCurrentLevel.Count; i++)
        {
            GameManager.Instance.characterDataForCurrentLevel[i].UpdateMyUnitStatsForTheLevel();
        }

        currentLevelState = LevelState.PlayerPhase;

        UIM.InitializeUI();

        //Reordeno las unidades y también llamo al UIManager para que actualice el orden
        UpdateUnitsOrder();

        counterForEnemiesOrder = 0;

        UIM.finishUnitPlacement.SetActive(false);
    }

    //Función que llaman el gigante y el goblin para determinar la distancia hasta los enmigos
    public List<UnitBase> CheckEnemyPathfinding(EnemyUnit enemyUnitToCheck)
    {
        return TM.OnlyCheckClosestPathToPlayer(enemyUnitToCheck);
        //return TM.checkAvailableCharactersForAttack(range, enemyUnitToCheck.GetComponent<EnemyUnit>());
    }

    int xpTurns;
    int xpCharacters;

    int totalXp;

    public void CheckIfGameOver()
    {
        //Derrota
        if (charactersOnTheBoard.Count == 0 || (!isObjectiveWaitForTurnLimit && currentTurn > turnLimit))
        {
            Debug.Log("Game Over");
            defeatPanel.SetActive(true);
            UIM.optionsButton.SetActive(false);
            GameManager.Instance.LevelLost();

            return;
        }

        //Calculo xp por turnos
        xpTurns = (turnLimit - currentTurn) * GameManager.Instance.xpPerTurnThisLevel;

        if (xpTurns < 0)
        {
            xpTurns = 0;
        }

        //Calculo xp por characters
        xpCharacters = charactersOnTheBoard.Count * GameManager.Instance.xpPerCharacterThisLevel;

        if (xpCharacters < 0)
        {
            xpCharacters = 0;
        }


        totalXp = xpCharacters + xpTurns + GameManager.Instance.possibleXpToGainIfCurrentLevelIsWon;

        if (isObjectiveWaitForTurnLimit && currentTurn > turnLimit)
        {
            Debug.Log("Victory");
            UIM.HideGameHud();
            GameManager.Instance.VictoryAchieved(totalXp);
        }

        //Victoria
        else if (isObjectiveKillSpecificEnemies)
        {
            for (int i = 0; i < enemiesNecessaryToWin.Count; i++)
            {
                if (!enemiesNecessaryToWin[i].isDead)
                {
                    return;
                }
            }

            Debug.Log("Victory by killing specific enemies");
            UIM.HideGameHud();
            GameManager.Instance.VictoryAchieved(totalXp);
        }

        else if (enemiesOnTheBoard.Count == 0 ||
            enemiesOnTheBoard.Count == 1 && enemiesOnTheBoard[0].isDead)
        {
            Debug.Log("Victory");
            UIM.HideGameHud();
            GameManager.Instance.VictoryAchieved(totalXp);
        }
    }

    public void VictoryScreen()
    {
        UIM.Victory(GameManager.Instance.possibleXpToGainIfCurrentLevelIsWon, xpCharacters, xpTurns);
    }

    private void Update()
    {
        switch (currentLevelState)
        {
            case (LevelState.PlayerPhase):
                StartCoroutine("ChangePhaseWait");
                currentLevelState = LevelState.ProcessingPlayerActions;
                break;

            case (LevelState.ProcessingPlayerActions):
                break;

            case (LevelState.EnemyPhase):
                StartCoroutine("ChangePhaseWait");
                currentLevelState = LevelState.ProcessingEnemiesActions;
                break;

            case (LevelState.ProcessingEnemiesActions):
                break;
        }

        //INPUT
        if (Input.GetMouseButtonDown(1))
        {
            DeSelectUnit();
        }

        //if (selectedCharacter != null)
        //{
        //    Debug.Log(selectedCharacter + " " + tilesAvailableForMovement.Count);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    //RaycastHit hit;
        //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    //if (Physics.Raycast(ray, out hit, 100.0f))
        //    //{
        //    //    Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
        //    //}
        //}
    }
    #endregion

    public void InstaWin(bool _shouldEndTurn)
    {
        for (int i = 0; i < enemiesOnTheBoard.Count; i++)
        {
            enemiesOnTheBoard[i].ReceiveDamage(999, null);
        }

        if (_shouldEndTurn)
        {
            UIM.EndTurn();
        }       
    }

    //Para ver si todas las unidades del jugador estan en un tile de acabar la partida
    public bool CheckIfFinishingTilesReached()
    {
        for (int i = 0; i < charactersOnTheBoard.Count; i++)
        {   
            if (!charactersOnTheBoard[i].myCurrentTile.isFinishingTile)
            {
                if (charactersOnTheBoard[i].GetComponent<MageDecoy>())
                {
                    continue;
                }

                return false;
            }
        }

        return true;
    }

    //Esta función esta en el level manager porque tengo que pasarsela manualmente al botón de cerrar el tutorial y si estuviese en el gamemanager no podría pasar la referencia desde el editor.
    public void UnPauseGame()
    {
        GameManager.Instance.isGamePaused = false;
    }

    //Usar lista para pasar al player los enemigos que se han activado y desactivar al hacer undo
    //public List<EnemyUnit> enemiesAlertedByCharacter = new List<EnemyUnit>();
    public void AlertEnemiesOfPlayerMovement()
    {
        for (int i = 0; i < enemiesOnTheBoard.Count; i++)
        {
            //Si esta muerto, ya esta despierto o va a ser alertado no hace falta que compruebe nada.
            if (!enemiesOnTheBoard[i].isDead && !enemiesOnTheBoard[i].haveIBeenAlerted && !enemiesOnTheBoard[i].isGoingToBeAlertedOnEnemyTurn)
            {
                //Balista y charger tienen check diferente
                if (enemiesOnTheBoard[i].GetComponent<EnBalista>())
                {
                    //Comprobar si tiene que ser false o true
                    enemiesOnTheBoard[i].CheckCharactersInLine(false, enemiesOnTheBoard[i].myCurrentTile);
                }

                else if (enemiesOnTheBoard[i].GetComponent<EnCharger>())
                {
                    enemiesOnTheBoard[i].CheckCharactersInLine(false, null);
                }

                //Resto de enemigos
                else
                {
                    enemiesOnTheBoard[i].SearchingObjectivesToAttackShowActionPathFinding();
                }

                if (enemiesOnTheBoard[i].currentUnitsAvailableToAttack.Count > 0)
                {
                    enemiesOnTheBoard[i].EnemyIsGoingToBeAlerted();
                    continue;
                }
            }
        }

        //Al acabar for updateo lista de enemigos
        UpdateUnitsOrder();
    }

    public void ClickSound()
    {
        SoundManager.Instance.PlaySound(AppSounds.BUTTONCLICK);
    }
}

