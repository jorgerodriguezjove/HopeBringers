using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    //Offset negativo para determinar la altura de las flechas
    private float offsetHeightArrow;

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

    //Int que guarda el número de objetivos que tiene para atacar la unidad actual. Se usa únicamente en la función SelectUnitToAttack para marcar el índice de un for y que no de error si se deselecciona la unidad actual.
    private int enemiesNumber;

    //Tiles que tienen efecto de daño
    //[HideInInspector]
    public List<DamageTile> damageTilesInBoard = new List<DamageTile>();

    [Header("UNDO ACTION")]

    [Header("TURNOS Y FASES")]

    //Bool que sirve para que se pueda probar un nivel sin necesidad de haber elegido personajes antes
    [SerializeField]
    public bool FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa;

    //Lista con unidades que no han sido colocadas en escena todavía y que estan en formato lista
    List<GameObject> unitsWithoutPositionFromGameManager = new List<GameObject>();

    //Las unidades sin colocar se pasan a un stack para que al quitar una unidad del tablero se coloque la primera
    Stack<GameObject> unitsWithoutPosition = new Stack<GameObject>();

    //Lista con los tiles disponibles para colocar personajes. Me sirve para limpiar el color de los tiles al terminar.
    [HideInInspector]
    public List<IndividualTiles> tilesForCharacterPlacement = new List<IndividualTiles>();

    //Int que lleva la cuenta del turno actual
    private int currentTurn = 0;

    //Cada unidad se encarga desde su script de incluirse en la lista
    //Lista con todas las unidades del jugador en el tablero
    //[HideInInspector]
    public List<PlayerUnit> charactersOnTheBoard;
    
    //Lista con todas las unidades enemigas en el tablero
    [SerializeField]
    public List<EnemyUnit> enemiesOnTheBoard;

    //Contador que controla a que unidad le toca. Sirve cómo indice para la lista de enemigos.
    [HideInInspector]
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

    //Referencia momentanea para el playtesting
    [SerializeField]
    private GameObject victoryPanel;

    //Referencia momentanea para el playtesting
    [SerializeField]
    private GameObject defeatPanel;

    #endregion

    #region INIT

    //Crea a los personajes del jugador correspondientes
    private void InitializeCharacters()
    { 
        for (int i = 0; i < GameManager.Instance.unitsForCurrentLevel.Count; i++)
        {
            GameObject unitInstantiated = Instantiate(GameManager.Instance.unitsForCurrentLevel[i].gameObject);
            unitInstantiated.SetActive(false);
            unitsWithoutPosition.Push(unitInstantiated);
        }   
    }

    //IMPORTANTE QUE ESTAS DOS REFERENCIAS VAYAN EN EL AWAKE
    //Si no el dragón no puede buscar sus tiles a través del tilemanager cuando empieza el nivel.
    private void Awake()
    {
        TM = FindObjectOfType<TileManager>();
        UIM = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        FindObjectOfType<CommandInvoker>().ResetCommandList();

        //Crea a los jugadores seleccionados para el nivel.
        if (FuncionarSinHaberSeleccionadoPersonajesEnEscenaMapa)
        {
            UIM.InitializeUI();
            currentLevelState = LevelState.PlayerPhase;
        }
        else
        {
            InitializeCharacters();
        }        

        //La inicialización real ocurren en la función de Tile clicked cuando detecta que se han colocado todas las unidades.
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
                if (enemiesOnTheBoard[i].GetComponent<EnemyUnit>().isDead)
                {
                    EnemyUnit deadEnemy = enemiesOnTheBoard[i];
                    enemiesOnTheBoard.Remove(deadEnemy);
                    Destroy(deadEnemy.gameObject);
                    counterForEnemiesOrder--;
                    i--;
                }
            }

            enemiesOnTheBoard.Sort(delegate (EnemyUnit a, EnemyUnit b)
            {
                return (b.GetComponent<EnemyUnit>().speed).CompareTo(a.GetComponent<EnemyUnit>().speed);

            });
        }



		UIM.SetEnemyOrder();
	}

    #endregion

    #region UNIT_INTERACTION

    //Al clickar sobre una unidad del jugador se llama a esta función
    public void SelectUnit(int movementUds, PlayerUnit clickedUnit)
    {
        //Si es el comienzo del nivel y estoy recolocando las unidades
        if (currentLevelState == LevelState.Initializing)
        {
            //Quitar personaje del tablero y añadirlo a la lista de nuevo
            clickedUnit.gameObject.SetActive(false);
            unitsWithoutPosition.Push(clickedUnit.gameObject);
            clickedUnit.myCurrentTile.unitOnTile = null;
            clickedUnit.myCurrentTile.WarnInmediateNeighbours();
            clickedUnit.myCurrentTile = null;
        }

        //Si es el turno del player compruebo si puedo hacer algo con la unidad.
        if (currentLevelState == LevelState.ProcessingPlayerActions)
        {
          
            //Si no hay unidad seleccionada significa que está seleccionando una unidad
            if (selectedCharacter == null )
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
                    selectedCharacter.HealthBarOn_Off(true);
					//selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
                    selectedCharacter.SelectedColor();
                    UIM.ShowUnitInfo(selectedCharacter.unitGeneralInfo, selectedCharacter);


                    //This
                    //UIM.ShowCharacterInfo(selectedCharacter.unitInfo, selectedCharacter); F Code

                    tilesAvailableForMovement = TM.OptimizedCheckAvailableTilesForMovement(movementUds, clickedUnit);
                    for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                    {
                        tilesAvailableForMovement[i].ColorSelect();
                    }

                    selectedCharacter.CheckUnitsInRangeToAttack();
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
                    
                    selectedCharacter.HealthBarOn_Off(true);
					//selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
					/*UIM.ShowCharacterInfo(selectedCharacter.unitInfo, selectedCharacter);*/ /*Legacy Code*/
					selectedCharacter.SelectedColor();

                    selectedCharacter.CheckUnitsInRangeToAttack();

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
						UIM.TooltipAttack();
					}
					else
					{
						UIM.TooltipNoAttackable();
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
                if (!selectedCharacter.isMovingorRotating)
                {
                    SelectUnitToAttack(clickedUnit);

                }
            }
        }
    }

    //Función que se llama al clickar sobre un enemigo o sobre un aliado si ya tengo seleccionado un personaje
    public void SelectUnitToAttack(UnitBase clickedUnit)
    {
        if (selectedCharacter != null )
        {
            if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
            {
                enemiesNumber = selectedCharacter.currentUnitsAvailableToAttack.Count;

                //Compruebo si está en la lista de posibles targets
                for (int i = 0; i < enemiesNumber; i++)
                {
                    if (selectedCharacter != null && !selectedCharacter.isMovingorRotating && !selectedCharacter.hasAttacked)
                    {
                        if (clickedUnit == selectedCharacter.currentUnitsAvailableToAttack[i])
                        {
                            ICommand command = new AttackCommand(selectedCharacter.currentUnitsAvailableToAttack[i].currentFacingDirection, selectedCharacter.currentFacingDirection, selectedCharacter.currentUnitsAvailableToAttack[i].myCurrentTile, selectedCharacter.myCurrentTile, selectedCharacter.currentUnitsAvailableToAttack[i].currentHealth, selectedCharacter.currentHealth, selectedCharacter, selectedCharacter.currentUnitsAvailableToAttack[i]);
                            CommandInvoker.AddCommand(command);
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
							UIM.CheckActionsAvaliable();
                            return;
                        }
                    }
                }
            }

            //Si llega hasta aqui significa que la unidad seleccionada no formaba parte de las unidades a las que puede atacar.
            //Compruebo si es un player y de ser así lo selecciono

            if (clickedUnit.GetComponent<PlayerUnit>() && !clickedUnit.GetComponent<MageDecoy>())
            {
                DeSelectUnit();
                SelectUnit(clickedUnit.movementUds, clickedUnit.GetComponent<PlayerUnit>());
                UIM.ShowUnitInfo(clickedUnit.unitGeneralInfo, clickedUnit);
            }

            else if (clickedUnit.GetComponent<EnemyUnit>())
            {
                DeSelectUnit();
                SelectEnemy(clickedUnit.unitGeneralInfo ,clickedUnit.GetComponent<EnemyUnit>());
            }
        }
    }

    public void DeSelectUnit()
    {
        if (selectedCharacter != null)
        {
            if (selectedCharacter.isMovingorRotating)
            {
                //Hacer que desaparezcan los botones de rotación
                selectedCharacter.isMovingorRotating = false;
                selectedCharacter.canvasWithRotationArrows.gameObject.SetActive(false);

            }

			selectedCharacter.HealthBarOn_Off(false);
            selectedCharacter.myPanelPortrait.GetComponent<Portraits>().UnHighlightPortrait();
            selectedCharacter.myPanelPortrait.GetComponent<Portraits>().isClicked = false;
            UIM.TooltipDefault();

            //Desmarco las unidades disponibles para atacar
            for (int i = 0; i < selectedCharacter.currentUnitsAvailableToAttack.Count; i++)
            {
                if (selectedCharacter.currentUnitsAvailableToAttack[i] != null)
                {
                    selectedCharacter.currentUnitsAvailableToAttack[i].ResetColor();
                }
            }

            //Si no se ha movido lo deselecciono.
            for (int i = 0; i < tilesAvailableForMovement.Count; i++)
            {
                tilesAvailableForMovement[i].ColorDeselect();
            }

            //Activo el botón de end turn para que no le de mientras la unidad siga seleccionada
            UIM.ActivateDeActivateEndButton();
            UIM.HideUnitInfo("");
            selectedCharacter.HideDamageIcons();
            tilesAvailableForMovement.Clear();
            selectedCharacter.ResetColor();
            selectedCharacter.myCurrentTile.ColorDeselect();
            selectedCharacter = null;
        }
        else if(selectedEnemy !=null)
        {

            DeselectEnemy();
        }
    }

    //public void UndoMove()
    //{
    //    //ESTO HAY QUE CAMBIARLO PARA QUE GUARDE TANTO LA UNIDAD CÓMO EL TILE EN EL QUE ESTABA (QUIZÁS USAR UN DICCIONARIO)

    //    if (selectedCharacter != null && !selectedCharacter.isMovingorRotating)
    //    {
    //        //Si el personaje ya se ha movido lo vuelvo a poner donde estaba.
    //        if (selectedCharacter.hasMoved)
    //        {
    //            selectedCharacter.gameObject.transform.position = new Vector3(previousCharacterTile.transform.position.x, previousCharacterTile.transform.position.y + 1, previousCharacterTile.transform.position.z);

    //            selectedCharacter.myCurrentTile = previousCharacterTile;

    //            selectedCharacter.hasMoved = false;

    //            tilesAvailableForMovement = TM.OptimizedCheckAvailableTilesForMovement(selectedCharacter.movementUds, selectedCharacter);
    //            for (int i = 0; i < tilesAvailableForMovement.Count; i++)
    //            {
    //                tilesAvailableForMovement[i].ColorSelect();
    //            }
    //        }
    //    }

    //    else if (previousCharacterTile != null)
    //    {

    //    }
    //}

    public void SelectEnemy(string _unitInfo, EnemyUnit _enemySelected)
    {
        DeselectEnemy();
   
        selectedEnemy = _enemySelected;
        CheckIfHoverShouldAppear(_enemySelected);
        UIM.ShowUnitInfo(_unitInfo, _enemySelected);

        _enemySelected.SelectedFunctionality();
		UIM.MoveScrollToEnemy(_enemySelected);
	}

    //Decido si muevo a la unidad, si tengo que colocarla por primera vez o si no hago nada
    public void TileClicked(IndividualTiles tileToMove)
    {
        //Si es el comienzo del nivel
        if (currentLevelState == LevelState.Initializing)
        {
            //Si hay unidades por colocar todavía
            if (unitsWithoutPosition.Count > 0)
            {
                //Si el tile clickado está disponible para colocar unidades.
                if (tileToMove.isAvailableForCharacterColocation && tileToMove.unitOnTile == null)
                {
                    //Colocar a la unidad
                    //Peek sirve para tomar una referencia a la primera unidad del Stack sin retirarla
                    unitsWithoutPosition.Peek().SetActive(true);
                    unitsWithoutPosition.Peek().transform.position = tileToMove.transform.position;
                    unitsWithoutPosition.Peek().GetComponent<PlayerUnit>().UpdateInformationAfterMovement(tileToMove);

                    //Pop es basicamente quitar el primer elemento del Stack
                    unitsWithoutPosition.Pop();

                    if (unitsWithoutPosition.Count <= 0)
                    {
                        for (int i = 0; i < tilesForCharacterPlacement.Count; i++)
                        {
                            tilesForCharacterPlacement[i].ColorDeselect();
                        }

                        //AQUÍ COMIENZA LA PARTIDA COMO TAL
                        FinishPlacingUnits();
                    }
                }
            }
        }

        else
        {
            //Movimiento de la unidad
            if (selectedCharacter != null && !selectedCharacter.hasAttacked)
            {
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

                            //Desmarco las unidades que antes estaban disponibles para ser atacadas
                            if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0 && tileToMove != selectedCharacter.myCurrentTile)
                            {
                                for (int j = 0; j < selectedCharacter.currentUnitsAvailableToAttack.Count; j++)
                                {
                                    if (selectedCharacter.currentUnitsAvailableToAttack[j] != null)
                                    {
                                        selectedCharacter.currentUnitsAvailableToAttack[j].ResetColor();
                                    }
                                }
                            }
                        }
                    }
                   
                }
            }
        }
    }

    //Cuando el jugador elige la rotación de la unidad se avisa para que reaparezca el botón de end turn.
    public void UnitHasFinishedMovementAndRotation()
    {
        //UIM.ActivateDeActivateEndButton();+

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

                    //Cambiar icono del cursor
                }
            }
        }
    }

    //Hago desaparecer el hover
    public void HideHover(UnitBase enemyToCheck)
    {
        //Ocultar hover
        enemyToCheck.DisableCanvasHover();
    }

    //Muestra el rango de movimiento de una unidad aliada al hacer hover en ella.
    public void ShowUnitHover(int movementUds, PlayerUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            hoverUnit.HealthBarOn_Off(true);
            //hoverUnit.GetComponent<PlayerHealthBar>().ReloadHealth();
            hoverUnit.myCurrentTile.ColorCurrentTileHover();
            UIM.ShowUnitInfo(hoverUnit.unitGeneralInfo, hoverUnit);

            if (hoverUnit.hasMoved == false)
            {
                tilesAvailableForMovement = TM.OptimizedCheckAvailableTilesForMovement(movementUds, hoverUnit);
                for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                {
                    tilesAvailableForMovement[i].ColorSelect();
                }
            }
        }
    }

    //Esconde el rango de movimiento de una unidad aliada al quitar el ratón de ella. !!Cuidado con función de arriba que se llama parecido (HideHover)!!
    public void HideUnitHover(PlayerUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            hoverUnit.HealthBarOn_Off(false);
            UIM.HideUnitInfo("");
            UIM.TooltipDefault();
            hoverUnit.myCurrentTile.ColorDeselect();

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

    public void ShowUnitInfo()
    {
        if (selectedCharacter != null)
        {
            UIM.ShowUnitInfo(selectedCharacter.unitGeneralInfo, selectedCharacter);
        }
        else if (selectedEnemy != null)
        {
            UIM.ShowUnitInfo(selectedEnemy.unitGeneralInfo, selectedEnemy);
        }
    }

    public void HideUnitInfo()
    {
        UIM.HideUnitInfo("");
    }

    //showActionRangeInsteadOfMovement sirve para que se pinte naranaja el rango de acción del enemigo al estar dormido
    public void ShowEnemyHover(int movementUds, bool showActionRangeInsteadOfMovement, EnemyUnit hoverUnit)
    {
        if (selectedCharacter == null)
        {
            if (hoverUnit.GetComponent<EnBalista>())
            {
                hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine();
                //Dibuja el ataque que va a preparar si las unidades se quedan ahí
                if (hoverUnit.GetComponent<EnBalista>().currentUnitsAvailableToAttack.Count > 0)
                {
                    hoverUnit.GetComponent<EnBalista>().FeedbackTilesToCharge(true);
                }
                //Dibuja el próximo movimiento si no tiene a ningún jugador en su línea
                else if (hoverUnit.GetComponent<EnBalista>().isAttackPrepared == false)
                {
                   
                    IndividualTiles tileToMove = hoverUnit.GetComponent<EnBalista>().GetTileToMove();
                    
                    if (tileToMove != null)
                    {
                        hoverUnit.shaderHover.SetActive(true);
                        Vector3 positionToSpawn = new Vector3(tileToMove.transform.position.x, tileToMove.transform.position.y + 0.3f, tileToMove.transform.position.z);
                        hoverUnit.shaderHover.transform.position = positionToSpawn;
                        tileToMove.ColorSelect();

                        hoverUnit.myLineRenderer.enabled = true;
                        hoverUnit.myLineRenderer.positionCount = 2;
                        Vector3 positionToSpawnLineRenderer = new Vector3(hoverUnit.myCurrentTile.transform.position.x, hoverUnit.myCurrentTile.transform.position.y + 0.3f, hoverUnit.myCurrentTile.transform.position.z);
                        hoverUnit.myLineRenderer.SetPosition(0, positionToSpawnLineRenderer);
                        hoverUnit.myLineRenderer.SetPosition(1, positionToSpawn);
                    }


                }               
            }

            else if (hoverUnit.GetComponent<EnCharger>())

            {
                hoverUnit.GetComponent<EnCharger>().CheckCharactersInLine();
                
                //Dibuja el ataque que va a preparar si las unidades se quedan ahí
                if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count > 0)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(true);

                    if (hoverUnit.GetComponent<EnCharger>().pathToObjective.Count > 0)
                    {

                        hoverUnit.shaderHover.SetActive(true);
                        hoverUnit.shaderHover.transform.position = hoverUnit.GetComponent<EnCharger>().pathToObjective[hoverUnit.GetComponent<EnCharger>().pathToObjective.Count - 1].transform.position;
                        hoverUnit.SearchingObjectivesToAttackShowActionPathFinding();
                    }
                }
                else if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count == 0)
                {
                        hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(true);

                }
            }

            //Goblin, gigante, boss y demás
            else
            {
                //Muestro la acción que va a realizar el enemigo 
                hoverUnit.ShowActionPathFinding(true);

                //¡¡MUY IMPORTANTE!! hoverUnit.ShowActionPathFinding(true); TIENE QUE IR ANTES QUE ESTOS IFS
                if (hoverUnit.haveIBeenAlerted)
                {
                    tilesAvailableForMovementEnemies = TM.OptimizedCheckAvailableTilesForMovement(movementUds, hoverUnit);
                }

                else
                {
                    tilesAvailableForMovementEnemies = TM.CheckAvailableTilesForEnemyAction(movementUds, hoverUnit);
                }

                for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
                {
                    if (showActionRangeInsteadOfMovement)
                    {
                        if (tilesAvailableForMovementEnemies[i].isObstacle
                           || (tilesAvailableForMovementEnemies[i].isEmpty))
                        {
                            continue;
                        }
                        tilesAvailableForMovementEnemies[i].ColorActionRange();
                    }

                    else
                    {
                        if (tilesAvailableForMovementEnemies[i].isObstacle
                           || (tilesAvailableForMovementEnemies[i].isEmpty))
                        {
                            continue;
                        }
                        tilesAvailableForMovementEnemies[i].ColorSelect();
                    }
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
                hoverUnit.GetComponent<EnBalista>().CheckCharactersInLine();
                
                if (hoverUnit.GetComponent<EnBalista>().currentUnitsAvailableToAttack.Count > 0 && hoverUnit.GetComponent<EnBalista>().isAttackPrepared == false)
                {
                    hoverUnit.GetComponent<EnBalista>().FeedbackTilesToAttack(false);
                }
                
                else if (hoverUnit.GetComponent<EnBalista>().isAttackPrepared == false)
                {
                    IndividualTiles tileToMove = hoverUnit.GetComponent<EnBalista>().GetTileToMove();
                    if (tileToMove != null)
                    {
                        hoverUnit.shaderHover.SetActive(false);
                        tileToMove.ColorDeselect();
                        hoverUnit.myLineRenderer.enabled = false;
                    }

                }
            }
            else if (hoverUnit.GetComponent<EnCharger>())

            {
                hoverUnit.shaderHover.SetActive(false);
                hoverUnit.GetComponent<EnCharger>().CheckCharactersInLine();
                if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count > 0)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(false);
                }
                else if (hoverUnit.GetComponent<EnCharger>().currentUnitsAvailableToAttack.Count == 0)
                {
                    hoverUnit.GetComponent<EnCharger>().FeedbackTilesToAttack(false);

                }

            }

            //Goblin, gigante, boss y demás
            else 
            {
                hoverUnit.HideActionPathfinding();

                for (int i = 0; i < tilesAvailableForMovementEnemies.Count; i++)
                {
                    tilesAvailableForMovementEnemies[i].ColorDeselect();
                }
                tilesAvailableForMovementEnemies.Clear();
            }

        }
    }

    public void DeselectEnemy()
    {
        if (selectedEnemy != null)
        {
            selectedEnemy.HealthBarOn_Off(false);
            selectedEnemy.ResetColor();
            selectedEnemy.myPortrait.UnHighlightMyself();
            HideEnemyHover(selectedEnemy);

       
            UIM.HideUnitInfo("");
            
            
            UIM.TooltipDefault();
            selectedEnemy = null;
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

    //Cambia de fase. Si era la fase del player ahora es la del enemigo y viceversa
    //Se llama desde el UI Manager al pulsar el botón de end turn 
    public void ChangePhase()
    {
        FindObjectOfType<CommandInvoker>().ResetCommandList();

        currentLevelState = LevelState.EnemyPhase;
    }

    private void BeginPlayerPhase()
    {
        //Recoloco la lista de enemigos donde estaba al inicio.
        UIM.ResetScrollPosition();

        //Hago desaparecer el botón de fast forward y aparecer el de undo
        UIM.HideShowEnemyUi(false);

        //Quito los booleanos de los tiles de daño para que puedan hace daño el próximo turno.
        for (int i = 0; i < damageTilesInBoard.Count; i++)
        {
            damageTilesInBoard[i].damageDone = false;
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
                charactersOnTheBoard[i].ResetUnitState();

                if (charactersOnTheBoard[i].GetComponent<Berserker>())
                {
                    charactersOnTheBoard[i].GetComponent<Berserker>().RageChecker();
                }
            }

            //Reaparecer el botón de endbutton
            UIM.ActivateDeActivateEndButton();

            currentTurn++;
            selectedCharacter = null;
            currentLevelState = LevelState.ProcessingPlayerActions;
        }

        else
        {
            currentTurn = 1;
        }
    }

    private void BeginEnemyPhase()
    {
        //Hago aparecer el botón de fast forward y desaparecer el de undo
        UIM.HideShowEnemyUi(true);

        //Compruebro si los tiles de daño tienen que hacer daño. Lo añado aquí porque si no salía un bug al morir una unidad mediante un tile de daño, ya que la ordenaba y luego la intentaba buscar.
        for (int i = 0; i < damageTilesInBoard.Count; i++)
        {
            damageTilesInBoard[i].CheckHasToDoDamage();
        } 

        UpdateUnitsOrder();

        if (enemiesOnTheBoard.Count > 0)
        {
           
            //Desaparece botón de end turn
            UIM.ActivateDeActivateEndButton();

            //Aparece cartel con turno del enemigo
            //Me aseguro de que el jugador no puede interactuar con sus pjs
            //Actualizo el número de unidades en el tablero (Lo hago aquí en vez de  al morir la unidad para que no se cambie el orden en medio del turno enemigo)

            counterForEnemiesOrder = 0;
            enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
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

            //Bajo la lista de scroll
            UIM.ScrollUpOnce();

            //Empieza el turno enemigo
            enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
        }
    }

    //Función que se llama cuando se termina de colocar unidades
    private void FinishPlacingUnits()
    {
        for (int i = 0; i < GameManager.Instance.characterDataForCurrentLevel.Count; i++)
        {
            GameManager.Instance.characterDataForCurrentLevel[i].UpdateMyUnitStatsForTheLevel();
        }

        currentLevelState = LevelState.PlayerPhase;

        UIM.InitializeUI();

        //Reordeno las unidades y también llamo al UIManager para que actualice el orden
        UpdateUnitsOrder();

        counterForEnemiesOrder = 0;
    }

    //Función que llaman el gigante y el goblin para determinar la distancia hasta los enmigos
    public List<UnitBase> CheckEnemyPathfinding(EnemyUnit enemyUnitToCheck)
    {
        return TM.OnlyCheckClosestPathToPlayer(enemyUnitToCheck);
        //return TM.checkAvailableCharactersForAttack(range, enemyUnitToCheck.GetComponent<EnemyUnit>());
    }

    public void CheckIfGameOver()
    {
        if (enemiesOnTheBoard.Count == 0 ||
            enemiesOnTheBoard.Count == 1 && enemiesOnTheBoard[0].isDead)
        {
            Debug.Log("Victory");
            victoryPanel.SetActive(true);
            UIM.optionsButton.SetActive(false);
            GameManager.Instance.VictoryAchieved();
        }

        if (charactersOnTheBoard.Count == 0)
        {
            Debug.Log("Game Over");
            defeatPanel.SetActive(true);
            UIM.optionsButton.SetActive(false);
            GameManager.Instance.LevelLost();
        }
    }

    private void Update()
    {
        switch (currentLevelState)
        {
            case (LevelState.PlayerPhase):
                BeginPlayerPhase();
                currentLevelState = LevelState.ProcessingPlayerActions;
                break;

            case (LevelState.ProcessingPlayerActions):
                break;

            case (LevelState.EnemyPhase):
                BeginEnemyPhase();
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
    }
    #endregion
}

