using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    #region VARIABLES

    [Header("PERSONAJES")]
    //Referencia al personaje que esta siendo mejorado actualemente
    private CharacterData currentCharacterUpgrading;

    [SerializeField]
    private List<CharacterData> initialCharacters = new List<CharacterData>();

    [Header("CÁMARAS")]
    //Cámara del mapa
    [SerializeField]
    private GameObject mainMenuCamera;
    //Cámara del mapa
    [SerializeField]
    private GameObject mapCamera;
    //Cámara de la selección de unidades
    [SerializeField]
    private GameObject selectCamera;
    //Cámara de la progresión de unidades
    [SerializeField]
    private GameObject progresionCamera;
    [SerializeField]
    private GameObject upgradesCamera;

   
    [Header("LEVEL")]
    //Objeto que se mueve entre los niveles e indica el nivel actual.
    [SerializeField]
    private GameObject levelIndicator;

    //Referencia al nivel clickado. 
    [HideInInspector]
    public LevelNode lastLevelClicked;

    //Nombre del nivel actual que hay que cargar si el jugador le da a ready
    [HideInInspector]
    public string currentClickedSceneName;

    [SerializeField]
    List<LevelNode> allLevelNodesInGame = new List<LevelNode>();

    [Header("REFERENCIAS")]
    [SerializeField]
    private UITableManager UITM;

    //Referencia al primer nivel para que se setee al principio
    [SerializeField]
    private LevelNode level1;

    [Header("VFX ")]
    [SerializeField]
    public GameObject vfxLevelUp;

    #endregion

    #region INIT

    private void Start()
    {
        //La primera vez que se abre el juego añado las unidades iniciales.
        if (GameManager.Instance._isFirstTimeLoadingGame)
        {
            for (int i = 0; i < initialCharacters.Count; i++)
            {
                GameManager.Instance.characterDataForCurrentLevel.Add(initialCharacters[i].GetComponent<CharacterData>());
                GameManager.Instance._isFirstTimeLoadingGame = false;
            }

            //Activo la cámara del libro
        }

        else
        {
            //Activo la cámara del mapa
            BackToMap();
        }

        //Devuelvo las figuras a la caja y reseteo las listas de unidades
        ResetCharactersToBox();

        //Si he completado el nivel y hay un personaje que desbloquear lo desbloqueo.
        //TIENE QUE IR ANTES QUE level1.SelectLevel!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (GameManager.Instance.newCharacterToUnlock != null)
        {
            UnlockNewCharacter();
        }

        //Al cargar el nivel de mapa se deja predeterminado el nivel 1 seleccionado
        if (GameManager.Instance.currentLevelNodeID == 0)
        {
            level1.SelectLevel();
        }
        else
        {
            for (int i = 0; i < allLevelNodesInGame.Count; i++)
            {
                if (allLevelNodesInGame[i].idLevel == GameManager.Instance.currentLevelNodeID)
                {
                    allLevelNodesInGame[i].SelectLevel();
                }
            }

			//Tutorial mejoras (es 1 o 2)
			if(GameManager.Instance.levelIDsUnlocked.Count == 2)
			{
				//Saltar el moñeco de las mejoras
			}
        }
    }

    #endregion

    #region CAMERA_MOVEMENT

    //Al seleccionar un nivel se setea todo para que aparezca la parte de selección de unidades
    public void OnLevelClicked(LevelNode levelClicked, int _idLevel, int _xpToWin, int _xpPerTurn, int _xpPerCharacter, int _maxUnits, TextAsset _startDialog, TextAsset _endDialog, bool _isInterlude, string _sceneInterlude, TextAsset _interludeDialog)
    {
        //Se mueve el indicador del nivel
        levelIndicator.transform.position = new Vector3(levelClicked.transform.position.x, levelIndicator.transform.position.y , levelClicked.transform.position.z);

        //Cargo la información en el GameManager
        GameManager.Instance.currentLevelNodeID = _idLevel;
        GameManager.Instance.possibleXpToGainIfCurrentLevelIsWon = _xpToWin;
        GameManager.Instance.xpPerTurnThisLevel = _xpPerTurn;
        GameManager.Instance.xpPerCharacterThisLevel = _xpPerCharacter;
        GameManager.Instance.maxUnitsInThisLevel = _maxUnits;
        GameManager.Instance.currentLevelStartDialog = _startDialog;
        GameManager.Instance.currentLevelEndDialog = _endDialog;
        GameManager.Instance.isInterlude = _isInterlude;
        GameManager.Instance.interludeSceneName = _sceneInterlude;
        GameManager.Instance.interludeDialog = _interludeDialog;

        //Personaje a desbloquear
        if (levelClicked.newCharacterToUnlock  != null)
        {
            GameManager.Instance.newCharacterToUnlock = levelClicked.newCharacterToUnlock;
        }

        else
        {
            GameManager.Instance.newCharacterToUnlock = null;
        }

        Debug.Log(levelClicked.newCharacterToUnlock);
        Debug.Log("Xp to win in this level " + _xpToWin);

        //El UI Manager se encarga de actualizar la info del nivel
        UITM.ShowInfoOnLevelClick(levelClicked.LevelTitle);

        //Aparece el botón para entrar en la selección de nivel (quizás no es tanto que aparezca como simplmente guardar la informacion para cuando le de)
        lastLevelClicked = levelClicked;
    }

    public void MoveToMainMenu()
    {
        mainMenuCamera.SetActive(true);
        mapCamera.SetActive(false);

        GameManager.Instance.SaveGame();

        UITM.ShowHideContinueButton();
        UITM.HideAllUI();
    }

    public void MoveToSelection()
    {
        mapCamera.SetActive(false);
        selectCamera.SetActive(true);

        currentClickedSceneName = lastLevelClicked.sceneName;

        UITM.ShowLevelInfoUI();
    }

    public void MoveToProgresion()
    {
        mapCamera.SetActive(false);
        progresionCamera.SetActive(true);

        UITM.ShowProgresionUI();
    }

    public void MoveToUpgrades()
    {
        //Botón de atrás ahora cambia

        progresionCamera.SetActive(false);
        upgradesCamera.SetActive(true);

        UITM.ShowUpgradesUI();
    }
    
    public void BackToProgresion()
    {
        upgradesCamera.SetActive(false);
        progresionCamera.SetActive(true);
        currentCharacterUpgrading.transform.position = currentCharacterUpgrading.initialPosition;

        UITM.ShowProgresionUI();
    }

    public void BackToMap()
    {
        ///EN EL FUTURO MOVER LA FICHA AQUÍ y cambiar el nombre del capítulo arriba a la izquierda y poner confirmación del nivel

        //Movimiento de cámara
        mapCamera.SetActive(true);
        mainMenuCamera.SetActive(false);
        selectCamera.SetActive(false);
        progresionCamera.SetActive(false);

        //Reseteo personajes seleccionados
        ResetCharactersToBox();

        UITM.ShowMapUI();
    }

    public void ResetCharactersToBox()
    {
        currentCharacterUpgrading = null;
    }

    #endregion

    //Al hacer click sobre el personaje aparece sus árboles de habilidad
    public void OnClickCharacter(CharacterData _unitClicked)
    {
        if (progresionCamera.activeSelf)
        {
            MoveToUpgrades();
            _unitClicked.transform.position = UITM.panelForUnitUpgrade.transform.position;
            UITM.MoveToUpgradesUI(_unitClicked);
            currentCharacterUpgrading = _unitClicked;
        }
    }

    public void BuyUpgrade(UpgradeNode upgradeClicked)
    {
        //Comprobar si tengo exp suficiente
        if (currentCharacterUpgrading.unitPowerLevel <= GameManager.Instance.currentExp)
        {
            //Gastar Exp
            GameManager.Instance.currentExp -= currentCharacterUpgrading.unitPowerLevel;

            //Avisar al nodo de mejora de que ha sido comprado  y Desbloquear los siguientes nodos
            upgradeClicked.UpgradeBought();

            //Aumentar power level del personaje
            //Añadir id a la lista del personaje
            currentCharacterUpgrading.UpgradeAcquired(upgradeClicked.upgradeCost, upgradeClicked.idUpgrade);

            UITM.UpdateProgresionBook(currentCharacterUpgrading);

            //Comprobar logros
            GameManager.Instance.CheckUpgradeAchievements();
        }

        else
        {
            //¿Dar feedback de que no hay suficiente exp?
            Debug.Log("No hay suficiente xp");
        }
    }

    //Desbloquear nuevo personaje
    public void UnlockNewCharacter()
    {
        Debug.Log("Unlock");

        if (!GameManager.Instance.newCharacterToUnlock.GetComponent<CharacterData>().isCharacterUnlocked)
        {
            Debug.Log("Has desbloqueado a un nuevo pj");

            //Activo la ficha dentro de la caja
            GameManager.Instance.newCharacterToUnlock.GetComponent<CharacterData>().isCharacterUnlocked = true;
            GameManager.Instance.newCharacterToUnlock.GetComponent<CharacterData>().HideShowMeshCharacterData(true);
            GameManager.Instance.characterDataForCurrentLevel.Add(GameManager.Instance.newCharacterToUnlock.GetComponent<CharacterData>());
        }

    }

}
