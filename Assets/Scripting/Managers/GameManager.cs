using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : PersistentSingleton<GameManager>
{
    #region VARIABLES

    [Header("GAMEFLOW_CONTROLLER")]
    //Este bool avisa al level manager de que el nivel ha sido cargado y el diálogo ha terminado
    public bool canGameplayLevelStart = false;

    //Todos los personajes del juego esten o no desbloqueados
    [SerializeField]
    CharacterData[] allCharacters;

    [Header("VARIBLES NIVEL SELECCIONADO")]
    //Lista de character data que se tienen que cargar en el nivel
    [SerializeField]
    public List<CharacterData> characterDataForCurrentLevel = new List<CharacterData>();

    //Bool que indica si al cargar el level selection debería desbloquear un nuevo personaje
    [SerializeField]
    public GameObject newCharacterToUnlock;

    //Referencia al nodo del nivel que ha sido empezado
    public int currentLevelNode;

    //Experiencia que obtiene el jugador si completa el nivel
    public int possibleXpToGainIfCurrentLevelIsWon;

    //Experiencia que obtiene el jugador si completa el nivel
    public int xpPerTurnThisLevel;

    //Experiencia que obtiene el jugador si completa el nivel
    public int xpPerCharacterThisLevel;

    [SerializeField]
    public int maxUnitsInThisLevel;

    [Header("DIÁLOGOS")]
    [HideInInspector]
    public TextAsset currentLevelStartDialog;
    [HideInInspector]
    public TextAsset currentLevelEndDialog;

    string dialogInitializer = "reactions.InReac";

    //Bool para saber si al acabar el diálogo saco la caja de colocación de unidades o no
    private bool isCurrentDialogStart = true;

    //HACER QUE ESTA VARIABLE NO SE PUEDA SETEAR DESDE OTROS SCRIPTS
    [HideInInspector]
    public bool _isFirstTimeLoadingGame = true;

    [Header("GAME PROGRESS")]
    //Experiencia actual. 
    [SerializeField]
    public int currentExp;

    //Array con todos los niveles del juego.
    [HideInInspector]
    public LevelNode[] allLevelNodes;

    //Lista con los ids de los niveles completados
    [HideInInspector]
    public List<int> levelIDsUnlocked = new List<int>();

    [Header("REFERENCIAS")]

    //Lista que va a guardar todos los objetos que tengan el componente Character Data
    CharacterData[] oldCharacterDataList;

    //Variable que avisa al InkManager de que se ha inicializado
    public bool dialogTime;

    //Referencia al InkManRef
    private InkManager inkManRef;
    private DialogManager dialogManRef;

    private LevelManager LM;



    public bool isGamePaused
    {
        get
        {
            return _isGamePaused;
        }

        set
        {
            _isGamePaused = value;
            FindObjectOfType<NewCameraController>().StopResumeCameraCompletely(!value);
        }
    }

    [SerializeField]
    private bool _isGamePaused;
    #endregion

    #region INIT

    private void Start()
    {
        gameObject.name = "GameManager";

        allCharacters = FindObjectsOfType<CharacterData>();

        LoadGame();
    }

    //Añado la función a la carga de escenas
    private void OnEnable()
    {
        SceneManager.sceneLoaded += RemoveOldCharacterData;
        SceneManager.sceneLoaded += UpdateLevelStates;
        SceneManager.sceneLoaded += WaitForLevelEndChargingToStartDialog;
    }

    #endregion

    #region ON_SCENE_MAP_LOADED

    public void UpdateLevelStates(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == AppScenes.MAP_SCENE)
        {
            allLevelNodes = FindObjectsOfType<LevelNode>();

            for (int i = 0; i < allLevelNodes.Length; i++)
            {
                for (int j = 0; j < levelIDsUnlocked.Count; j++)
                {
                    if (allLevelNodes[i].idLevel == levelIDsUnlocked[j])
                    {
                        allLevelNodes[i].UnlockThisLevel();
                        allLevelNodes[i].UnlockConnectedLevels();
                        break;
                    }
                }
            }
        }
    }

    //Al cargar la escena de mapa borro los personajes desactualizados
    public void RemoveOldCharacterData(Scene scene, LoadSceneMode mode)
    {
        oldCharacterDataList = FindObjectsOfType<CharacterData>();

        //FALTA DESTRUIR LOS QUE YA EXISTEN EN LA ESCENA
        for (int i = 0; i < oldCharacterDataList.Length; i++)
        {
            if (!oldCharacterDataList[i].initialized)
            {
                Destroy(oldCharacterDataList[i].gameObject);
            }
        }
    }

    #endregion

    #region LEVEL_START

    //Cargar el nivel
    public void CheckStartLevel(string _levelName)
    {
        SceneManager.LoadScene(_levelName, LoadSceneMode.Single);
    }

    private void WaitForLevelEndChargingToStartDialog(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != AppScenes.MAP_SCENE)
        {
            inkManRef = FindObjectOfType<InkManager>();
            dialogManRef = FindObjectOfType<DialogManager>();

            StartDialog(true);
        }
    }

    #endregion

    #region LEVEL_END

    //Al completar un nivel el levelManager avisa de que en la escena de mapa va a tener que desbloquear niveles.
    public void VictoryAchieved(int _totalXp)
    {
        Debug.Log("Experiencia al ganar el nivel: " + possibleXpToGainIfCurrentLevelIsWon);

        levelIDsUnlocked.Add(currentLevelNode);

        currentExp += _totalXp;

        if (currentLevelEndDialog != null)
        {
            StartDialog(false);
        }

        else
        {
            LM.VictoryScreen();
        }
    }

    public void LevelLost()
    {
        newCharacterToUnlock = null;
    }

    #endregion

    #region DIALOG

    public void EndDialog()
    {
        //Termino el diálogo
        dialogManRef.CloseDialogWindow();
        dialogTime = false;
        Debug.Log("dialog ended");
        //SoundManager.Instance.PlaySound(AppSounds.ENDDIALOG_SFX);


        if (isCurrentDialogStart)
        {
            //Comienza el juego
            LM = FindObjectOfType<LevelManager>();
            LM.StartGameplayAfterDialog();
        }

        else
        {
            //Avisar de que salga ventana de victoria
            LM.VictoryScreen();
        }


    }

    public void StartDialog(bool _isStartDialog) /*string dialogToReproduce, string NPCName , string NPCstartAudio, string NPCfinalAudio, List<string> NPCAudio, Sprite portraitNPC)*/
    {
        dialogTime = true;
        isCurrentDialogStart = _isStartDialog;

        //Si es startDialog cargo el dialogo de start y si no cargo el de end
        if (_isStartDialog)
        {
            inkManRef.inkJSONAsset = currentLevelStartDialog;
        }

        else
        {
            inkManRef.inkJSONAsset = currentLevelEndDialog;
        }

        inkManRef.StartStory();


        inkManRef.story.ChoosePathString(dialogInitializer);
        inkManRef.RefreshView();

        //dialogManRef.SetVariables(dialogToReproduce, portraitNPC);
        dialogManRef.OpenDialogWindow();
        ///SoundManager.Instance.PlaySound(AppSounds.ENTERDIALOG_SFX);
        ///npcEndsound = npcData.finalAudio;
    }

    #endregion

    #region SAVE_&_LOAD

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("File Saved");
        Debug.Log("-----");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            //Aqui se hace lo contrario que en el CreateSaveGameObject. Se toman las variables del save y se aplican esos valores a las variables que se usan en código
            currentExp = save.s_currentXp;

            print(Application.persistentDataPath);

            characterDataForCurrentLevel.Clear();

            for (int i = 0; i < allCharacters.Length; i++)
            {
                if (save.s_charactersUnlocked.Contains(allCharacters[i].idCharacter))
                {
                    characterDataForCurrentLevel.Add(allCharacters[i]);
                    allCharacters[i].isCharacterUnlocked = true;
                    allCharacters[i].HideShowMeshCharacterData(true);
                }
            }


            allLevelNodes = FindObjectsOfType<LevelNode>();
            for (int i = 0; i < allLevelNodes.Length; i++)
            {
                if (save.s_levelIDsUnlocked.Contains(allLevelNodes[i].idLevel))
                {
                    allLevelNodes[i].UnlockConnectedLevels();
                    levelIDsUnlocked.Add(allLevelNodes[i].idLevel);
                }
            }

            #region Characters

            KnightData _knight = FindObjectOfType<KnightData>();

            _knight.unitPowerLevel = save.s_KnightPowerLevel;
            if (_knight.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_KnightSkillsIds.Count; i++)
                {
                    _knight.idSkillsBought.Add(save.s_KnightSkillsIds[i]);
                }
            }

            RogueData _rogue = FindObjectOfType<RogueData>();

            _rogue.unitPowerLevel = save.s_RoguePowerLevel;
            if (_rogue.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_RogueSkillsIds.Count; i++)
                {
                    _rogue.idSkillsBought.Add(save.s_RogueSkillsIds[i]);
                }
            }

            MageData _mage = FindObjectOfType<MageData>();

            _mage.unitPowerLevel = save.s_MagePowerLevel;
            if (_mage.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_MageSkillsIds.Count; i++)
                {
                    _mage.idSkillsBought.Add(save.s_MageSkillsIds[i]);
                }
            }

            BerserkerData _berserker = FindObjectOfType<BerserkerData>();

            _berserker.unitPowerLevel = save.s_BerserkerPowerLevel;
            if (_berserker.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_BerserkerSkillsIds.Count; i++)
                {
                    _berserker.idSkillsBought.Add(save.s_BerserkerSkillsIds[i]);
                }
            }

            ValkyrieData _valkyrie = FindObjectOfType<ValkyrieData>();

            _valkyrie.unitPowerLevel = save.s_ValkyriePowerLevel;
            if (_valkyrie.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_ValkyrieSkillsIds.Count; i++)
                {
                    _valkyrie.idSkillsBought.Add(save.s_ValkyrieSkillsIds[i]);
                }
            }

            DruidData _druid = FindObjectOfType<DruidData>();

            _druid.unitPowerLevel = save.s_DruidPowerLevel;
            if (_druid.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_DruidSkillsIds.Count; i++)
                {
                    _druid.idSkillsBought.Add(save.s_DruidSkillsIds[i]);
                }
            }

            MonkData _monk = FindObjectOfType<MonkData>();

            _monk.unitPowerLevel = save.s_MonkPowerLevel;
            if (_monk.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_MonkSkillsIds.Count; i++)
                {
                    _monk.idSkillsBought.Add(save.s_MonkSkillsIds[i]);
                }
            }

            SamuraiData _samurai = FindObjectOfType<SamuraiData>();

            _samurai.unitPowerLevel = save.s_SamuraiPowerLevel;
            if (_samurai.isCharacterUnlocked)
            {
                for (int i = 0; i < save.s_SamuraiSkillsIds.Count; i++)
                {
                    _samurai.idSkillsBought.Add(save.s_SamuraiSkillsIds[i]);
                }
            }

            #endregion

            Debug.Log("File Load");
            Debug.Log("-----");
        }

        else
        {
            Debug.LogError("IGNORE CUSTOM ERROR: Se ha intentado cargar y no existe archivo de guardado");

            SaveGame();
        }
    }

    //Esta función se usa únicamente en el SaveGame. La he separado para que quede más claro la información que se guarda en cada archivo
    private Save CreateSaveGameObject()
    {
        //Construyo archivo de guardado e inicializo todas las variables en el save
        Save save = new Save();

        save.s_currentXp = currentExp;

        for (int i = 0; i < characterDataForCurrentLevel.Count; i++)
        {
            save.s_charactersUnlocked.Add(characterDataForCurrentLevel[i].idCharacter);
        }

        for (int i = 0; i < levelIDsUnlocked.Count; i++)
        {
            save.s_levelIDsUnlocked.Add(levelIDsUnlocked[i]);
        }


        #region Characters
        KnightData _knight = FindObjectOfType<KnightData>();

        save.s_KnightPowerLevel = _knight.unitPowerLevel;
        if (_knight.isCharacterUnlocked)
        {
            for (int i = 0; i < _knight.idSkillsBought.Count; i++)
            {
                save.s_KnightSkillsIds.Add(_knight.idSkillsBought[i]);
            }
        }

        RogueData _rogue = FindObjectOfType<RogueData>();

        save.s_RoguePowerLevel = _rogue.unitPowerLevel;
        if (_rogue.isCharacterUnlocked)
        {
            for (int i = 0; i < _rogue.idSkillsBought.Count; i++)
            {
                save.s_RogueSkillsIds.Add(_rogue.idSkillsBought[i]);
            }
        }

        MageData _mage = FindObjectOfType<MageData>();

        save.s_MagePowerLevel = _mage.unitPowerLevel;
        if (_mage.isCharacterUnlocked)
        {
            for (int i = 0; i < _mage.idSkillsBought.Count; i++)
            {
                save.s_MageSkillsIds.Add(_mage.idSkillsBought[i]);
            }
        }

        BerserkerData _berserker = FindObjectOfType<BerserkerData>();

        save.s_BerserkerPowerLevel = _berserker.unitPowerLevel;
        if (_berserker.isCharacterUnlocked)
        {
            for (int i = 0; i < _berserker.idSkillsBought.Count; i++)
            {
                save.s_BerserkerSkillsIds.Add(_berserker.idSkillsBought[i]);
            }
        }

        ValkyrieData _valkyrie = FindObjectOfType<ValkyrieData>();

        save.s_ValkyriePowerLevel = _valkyrie.unitPowerLevel;
        if (_valkyrie.isCharacterUnlocked)
        {
            for (int i = 0; i < _valkyrie.idSkillsBought.Count; i++)
            {
                save.s_ValkyrieSkillsIds.Add(_valkyrie.idSkillsBought[i]);
            }
        }

        DruidData _druid = FindObjectOfType<DruidData>();

        save.s_DruidPowerLevel = _druid.unitPowerLevel;
        if (_druid.isCharacterUnlocked)
        {
            for (int i = 0; i < _druid.idSkillsBought.Count; i++)
            {
                save.s_DruidSkillsIds.Add(_druid.idSkillsBought[i]);
            }
        }

        MonkData _monk = FindObjectOfType<MonkData>();

        save.s_MonkPowerLevel = _monk.unitPowerLevel;
        if (_monk.isCharacterUnlocked)
        {
            for (int i = 0; i < _monk.idSkillsBought.Count; i++)
            {
                save.s_MonkSkillsIds.Add(_monk.idSkillsBought[i]);
            }
        }

        SamuraiData _samurai = FindObjectOfType<SamuraiData>();

        save.s_SamuraiPowerLevel = _samurai.unitPowerLevel;
        if (_samurai.isCharacterUnlocked)
        {
            for (int i = 0; i < _samurai.idSkillsBought.Count; i++)
            {
                save.s_SamuraiSkillsIds.Add(_samurai.idSkillsBought[i]);
            }
        }

        #endregion

        return save;
    }

    //DEBUG SAVE LOAD
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    private void ResetSaveFile()
    {

    }

    #endregion
}
