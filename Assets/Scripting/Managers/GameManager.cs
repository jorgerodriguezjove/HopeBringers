using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    #region VARIABLES

    [Header("GAMEFLOW_CONTROLLER")]
    //Este bool avisa al level manager de que el nivel ha sido cargado y el diálogo ha terminado
    public bool canGameplayLevelStart = false;

    [Header("VARIBLES NIVEL SELECCIONADO")]
    //Lista de character data que se tienen que cargar en el nivel
    [SerializeField]
    public List<CharacterData> characterDataForCurrentLevel = new List<CharacterData>();

    //Bool que indica si al cargar el level selection debería desbloquear un nuevo personaje
    [HideInInspector]
    public GameObject newCharacterToUnlock;

    //Referencia al nodo del nivel que ha sido empezado
    public int currentLevelNode;
    //Experiencia que obtiene el jugador si completa el nivel
    public int possibleXpToGainIfCurrentLevelIsWon;

    [SerializeField]
    public int maxUnitsInThisLevel;

    [Header("DIÁLOGOS")]
    [HideInInspector]
    public TextAsset currentLevelStartDialog;
    [HideInInspector]
    public TextAsset currentLevelEndDialog;

    string dialogInitializer = "reactions.InReac";

    //HACER QUE ESTA VARIABLE NO SE PUEDA SETEAR DESDE OTROS SCRIPTS
    [HideInInspector]
    public bool _isFirstTimeLoadingGame = true;

    [Header("GAME PROGRESS")]
    //Experiencia actual. 
    [SerializeField]
    public int currentExp;

    //Lista que va a guardar todos los objetos que tengan el componente Character Data
    CharacterData[] oldCharacterDataList;

    //Array con todos los niveles del juego.
    [HideInInspector]
    public LevelNode[] allLevelNodes;

    //Lista con los ids de los niveles completados
    [HideInInspector]
    public List<int> levelIDsUnlocked = new List<int>();

    [Header("REFERENCIAS")]

    //Variable que avisa al InkManager de que se ha inicializado
    public bool dialogTime;

    //Referencia al InkManRef
    private InkManager inkManRef;
    private DialogManager dialogManRef;

    private LevelManager LM;

    #endregion

    #region INIT

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
    public void VictoryAchieved()
    {
        Debug.Log("Experiencia al ganar el nivel: " + possibleXpToGainIfCurrentLevelIsWon);

        levelIDsUnlocked.Add(currentLevelNode);

        currentExp += possibleXpToGainIfCurrentLevelIsWon;
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

        //Comienza el juego
        LM = FindObjectOfType<LevelManager>();
        LM.StartGameplayAfterDialog();
    }

    public void StartDialog(bool _isStartDialog) /*string dialogToReproduce, string NPCName , string NPCstartAudio, string NPCfinalAudio, List<string> NPCAudio, Sprite portraitNPC)*/
    {
        dialogTime = true;

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

}
