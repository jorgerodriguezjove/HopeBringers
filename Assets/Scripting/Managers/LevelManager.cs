using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region VARIABLES

    [Header("ANIMACIÓN INICIAL")]

    private GameObject[] tilesInScene;

    WaitForSeconds waitFallingTiles = new WaitForSeconds(0.05f);

    [Header("INTERACCIÓN CON UNIDADES")]

    //Personaje actualmente seleccionado.
    [HideInInspector]
    public PlayerUnit selectedCharacter;

    //Tiles que actualmente están dispoibles para el movimiento de la unidad seleccionada
    [HideInInspector]
    public List<IndividualTiles> tilesAvailableForMovement = new List<IndividualTiles>();

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    float timeForMovementAnimation = 0.2f;

    //Posición a la que tiene que moverse la unidad actualmente
    private Vector3 currentTileVectorToMove;

    //Int que guarda el número de objetivos que tiene para atacar la unidad actual. Se usa únicamente en la función SelectUnitToAttack para marcar el índice de un for y que no de error si se deselecciona la unidad actual.
    private int enemiesNumber;

    [Header("TURNOS Y FASES")]

    //Int que lleva la cuenta del turno actual
    private int currentTurn = 0;

    //Cada unidad se encarga desde su script de incluirse en la lista
    //Lista con todas las unidades del jugador en el tablero
    [HideInInspector]
    public List<PlayerUnit> characthersOnTheBoard;
    [HideInInspector]
    //Lista con todas las unidades enemigas en el tablero
    public List<EnemyUnit> enemiesOnTheBoard;

    //Contador que controla a que unidad le toca. Sirve cómo indice para la lista de enemigos.
    private int counterForEnemiesOrder ;

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

    #endregion

    #region INIT

    private void Start()
    {
        TM = FindObjectOfType<TileManager>();
        UIM = FindObjectOfType<UIManager>();

        //Reordeno las unidades y también llamo al UIManager para que actualice el orden
        UpdateUnitsOrder();

        //Comienza el nivel con el turno del jugador
        currentLevelState = LevelState.PlayerPhase;

        counterForEnemiesOrder = 0;

        //StartFallAnimation();

    }

    //Función que se llama para que caigan los tiles y los personajes
    private void StartFallAnimation()
    {
        //Accedo a la lista de tiles del TM. En principio como esto va en el Start no debería haber problema.
        //Aumento su posición en Y y después reproduzco la animación de cada Tile

        // tilesInScene = new GameObject[TM.tilesInScene.Length];
        tilesInScene = TM.tilesInScene;

        Debug.Log(tilesInScene[0]);

        StartCoroutine("FallingAnimation");
    }

    IEnumerator FallingAnimation()
    {
        //Caen los tiles
        for (int i = 0; i < tilesInScene.Length; i++)
        {
            tilesInScene[i].GetComponent<IndividualTiles>().FallAnimation();
            yield return waitFallingTiles;
        }

        currentLevelState = LevelState.PlayerPhase;

        ////Se hace lo mismo con las figuras
        //for (int i = 0; i < fakeFigurasList.Count; i++)
        //{
        //    fakeFigurasList[i].SetActive(true);
        //    yield return waitFallingTiles;
        //}
    }

    //Ordeno la lista de personajes del jugador y la lista de enemigos
    //Cuando muere un enemigo, también se llama aquí
    private void UpdateUnitsOrder()
    {
        if (characthersOnTheBoard.Count > 0)
        {
            characthersOnTheBoard.Sort(delegate (PlayerUnit a, PlayerUnit b)
            {
                return (b.GetComponent<PlayerUnit>().speed).CompareTo(a.GetComponent<PlayerUnit>().speed);

            });
        }

        if (enemiesOnTheBoard.Count > 0)
        {
            for (int i = 0; i < enemiesOnTheBoard.Count-1; i++)
            {
                if(enemiesOnTheBoard[i].GetComponent<EnemyUnit>().isDead)
                {
                    EnemyUnit deadEnemy = enemiesOnTheBoard[i];
                    enemiesOnTheBoard.Remove(deadEnemy);
                    Destroy(deadEnemy.gameObject);
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
        //Si es el turno del player compruebo si puedo hacer algo con la unidad.
        if (currentLevelState == LevelState.ProcessingPlayerActions)
        {
            //Si no hay unidad seleccionada significa que está seleccionando una unidad
            if (selectedCharacter == null)
            {
				//Si no se ha movido significa que la puedo mover y doy feedback de sus casillas de movimiento
				if (!clickedUnit.hasAttacked && !clickedUnit.hasMoved)
                {
                    //Desactivo el botón de pasar turno cuando selecciona la unidad
                    //Está función no se puede llamar fuera del if para que afecte a ambos casos porque entonces también se cambia al pulsar en una unidad que ya ha atacado.
                    UIM.ActivateDeActivateEndButton();
					UIM.TooltipMove();
					

                    selectedCharacter = clickedUnit;
					selectedCharacter.HealthBarOn_Off(true);
					selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
					UIM.ShowCharacterInfo(selectedCharacter.unitInfo);
					selectedCharacter.SelectedColor();

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
                    //Desactivo el botón de pasar turno cuando selecciona la unidad
                    UIM.ActivateDeActivateEndButton();

                    selectedCharacter = clickedUnit;
					selectedCharacter.HealthBarOn_Off(true);
					selectedCharacter.GetComponent<PlayerHealthBar>().ReloadHealth();
					UIM.ShowCharacterInfo(selectedCharacter.unitInfo);
					selectedCharacter.SelectedColor();

                    selectedCharacter.CheckUnitsInRangeToAttack();

					if (selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
					{
						UIM.TooltipAttack();
					}
					else
					{
						UIM.TooltipNoAttackable();
					}

				}
			}

            //Si ya hay una seleccionada compruebo si puedo atacar a la unidad
            else 
            {
                SelectUnitToAttack(clickedUnit);
            }
        }
    }

    //Función que se llama al clickar sobre un enemigo o sobre un aliado si ya tengo seleccionado un personaje
    public void SelectUnitToAttack(UnitBase clickedUnit)
    {
        if (selectedCharacter != null && !selectedCharacter.isMovingorRotating)
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
                            selectedCharacter.Attack(clickedUnit);
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                            return;
                        }
                    }
                }
            }

            //Si llega hasta aqui significa que la unidad seleccionada no formaba parte de las unidades a las que puede atacar.
            //Compruebo si es un player y de ser así lo selecciono

            if (clickedUnit.GetComponent<PlayerUnit>())
            {
                DeSelectUnit();
                SelectUnit(clickedUnit.movementUds, clickedUnit.GetComponent<PlayerUnit>());
            }
        }
    }

    public void DeSelectUnit()
    {
        if (selectedCharacter != null && !selectedCharacter.isMovingorRotating)
        {
			selectedCharacter.HealthBarOn_Off(false);
			UIM.HideCharacterInfo("");
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
            tilesAvailableForMovement.Clear();
            selectedCharacter.ResetColor();
            selectedCharacter = null;
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

    public void MoveUnit(IndividualTiles tileToMove)
    {
        //Desmarco las unidades que antes estaban disponibles para ser atacadas
        if (selectedCharacter != null && !selectedCharacter.hasAttacked)
        {
            if (selectedCharacter != null && selectedCharacter.currentUnitsAvailableToAttack.Count > 0)
            {
                for (int i = 0; i < selectedCharacter.currentUnitsAvailableToAttack.Count; i++)
                {
                    if (selectedCharacter.currentUnitsAvailableToAttack[i] != null)
                    {
                        selectedCharacter.currentUnitsAvailableToAttack[i].ResetColor();
                    }
                }
            }

            for (int i = 0; i < tilesAvailableForMovement.Count; i++)
            {
                if (tileToMove == tilesAvailableForMovement[i])
                {
                    //Calculo el path de la unidad
                    TM.CalculatePathForMovementCost(tileToMove.tileX, tileToMove.tileZ);

                    //Al terminar de moverse se deseleccionan los tiles
                    for (int j = 0; j < tilesAvailableForMovement.Count; j++)
                    {
                        tilesAvailableForMovement[j].ColorDeselect();
                    }
                    tilesAvailableForMovement.Clear();

                    //Aviso a la unidad de que se tiene que mover
                    if (selectedCharacter != null)
                    {
                        selectedCharacter.MoveToTile(tileToMove, TM.currentPath);
                    }
                }
            }
        }
    }

    //Cuando el jugador elige la rotación de la unidad se avisa para que reaparezca el botón de end turn.
    public void UnitHasFinishedMovementAndRotation()
    {
		//UIM.ActivateDeActivateEndButton();+
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


    #endregion

    #region TURN_STATE

    //Cambia de fase. Si era la fase del player ahora es la del enemigo y viceversa
    //Se llama desde el UI Manager al pulsar el botón de end turn 
    public void ChangePhase()
    {
        currentLevelState = LevelState.EnemyPhase;
    }

    private void BeginPlayerPhase()
    {
        //Me aseguro que no quedan tiles en la lista de tiles para moverse.
        tilesAvailableForMovement.Clear();
        UpdateUnitsOrder();

        if (currentTurn > 0)
        {
			//Aparece cartel con turno del player
			UIM.RotateButtonStartPhase();
			//Resetear todas las variables tipo bool y demás de los players
			for (int i = 0; i < characthersOnTheBoard.Count; i++)
            {
                characthersOnTheBoard[i].ResetUnitState();
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
        UpdateUnitsOrder();

        //Desaparece botón de end turn
        UIM.ActivateDeActivateEndButton();

        //Aparece cartel con turno del enemigo
        //Me aseguro de que el jugador no puede interactuar con sus pjs
        //Actualizo el número de unidades en el tablero (Lo hago aquí en vez de  al morir la unidad para que no se cambie el orden en medio del turno enemigo)

        counterForEnemiesOrder = 0;
        enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
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
            enemiesOnTheBoard[counterForEnemiesOrder].MyTurnStart();
        }
    }

    //Función que llaman el gigante y el goblin para determinar la distancia hasta los enmigos
    public List<UnitBase> CheckEnemyPathfinding(int range, GameObject enemyUnitToCheck)
    {
        return TM.checkAvailableCharactersForAttack(range, enemyUnitToCheck.GetComponent<EnemyUnit>());
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

