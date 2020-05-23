using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UITableManager : MonoBehaviour
{
    #region VARIABLES

    //GameObjects que guardan la UI de cada pantalla
    [SerializeField]
    private GameObject mapUI;
    [SerializeField]
    private GameObject selectionUI;
    [SerializeField]
    private GameObject progresionUI;
    [SerializeField]
    private GameObject upgradesUI;

    [Header("MAP")]
    //Referencia al texto en la esquina superior izquierda donde se muestra el nombre del nivel
    [SerializeField]
    private TextMeshProUGUI levelNameInfoText;

    [Header("SELECTION")]

    //Referencia al texto de título
    [SerializeField]
    private TextMeshProUGUI titleTextRef;

    //Referencia al texto de descripción
    [SerializeField]
    private TextMeshProUGUI descriptionTextRef;

    [Header("PROGRESION")]

    [SerializeField]
    private TextMeshProUGUI currentCharacterPowerLevelText;

    [SerializeField]
    private TextMeshProUGUI currentTotalXpText;

    //Referencia al objeto dónde aparece el árbol de habilidades del personaje.
    [SerializeField]
    private GameObject rightPageProgresionBook;

    //Referencia al árbol de habilidades actualmente en pantalla.
    private GameObject currentSkillTreeObj;

    //Panel donde se coloca la unidad para mejorarla
    [SerializeField]
    public GameObject panelForUnitUpgrade;

    //Referencias de las listas de ids del personaje y la lista de upgrades del skill tree.
    private List<int> ids;
    private List<UpgradeNode> upgrades;

    //Confirmación compra mejora
    [SerializeField]
    private GameObject confirmateUpgrade;

    //Aviso bloqueará mejora del otro árbol
    [SerializeField]
    private GameObject warningBlockUpgrade;

    [SerializeField]
    private float timeToHidePanel;
    [SerializeField]
    private GameObject timedPanel;

    [SerializeField]
    private GameObject textNotEnoughXp;
    [SerializeField]
    private GameObject textAlreadyBought;

    private GameObject currentText;

    [Header("OPTIONS")]

    //Resolución
    [SerializeField]
    TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    //Fulsscreen
    [SerializeField]
    Toggle toggleFullsCreen;

    //Música y sonido
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sFXSlider;

    //Calidad
    [SerializeField]
    TMP_Dropdown qualityDropdown;

    [Header("CRÉDITOS")]
    [SerializeField]
    private GameObject credits1Panel;
    [SerializeField]
    private GameObject credits2Panel;

    [Header("REFERENCIAS")]
    [SerializeField]
    private TableManager TM;

    [HideInInspector]
    public UpgradeNode lastUpgradeClicked;
    #endregion

    #region INIT

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            //Si no hay resolución guardada se pone por defecto en 1920 x 1080
            if (!PlayerPrefs.HasKey(AppPlayerPrefKeys.RESOLUTION) && resolutions[i].width == 1920 && resolutions[i].height == 1080)
            {
                PlayerPrefs.SetInt(AppPlayerPrefKeys.RESOLUTION, i);
            }
        }

        resolutionDropdown.AddOptions(options);
        SetResolution(PlayerPrefs.GetInt(AppPlayerPrefKeys.RESOLUTION));
        resolutionDropdown.value = PlayerPrefs.GetInt(AppPlayerPrefKeys.RESOLUTION);
        resolutionDropdown.RefreshShownValue();

        //Si no hay ajuste guardado se pone por defecto en fullscreen
        if (!PlayerPrefs.HasKey(AppPlayerPrefKeys.FULLSCREEN))
        {
            SetFullScreen(true);
        }

        else
        {
            if (PlayerPrefs.GetInt(AppPlayerPrefKeys.FULLSCREEN) == 0)
            {
                SetFullScreen(false);
            }
            else
            {
                SetFullScreen(true);
            }
        }

        //Valores iniciales calidad
        if (PlayerPrefs.HasKey(AppPlayerPrefKeys.QUALITY_LEVEL))
        {
            SetQuality(5);
        }

        else
        {
            SetQuality(PlayerPrefs.GetInt(AppPlayerPrefKeys.QUALITY_LEVEL));
        }

        //Valores iniciales de slider
        musicSlider.value = Mathf.Clamp(SoundManager.Instance.MusicVolume, 0, 1);
        sFXSlider.value = Mathf.Clamp(SoundManager.Instance.SfxVolume, 0, 1);

    }

    #endregion

    #region MAP
    public void ShowInfoOnLevelClick(string levelName)
    {
        levelNameInfoText.SetText(levelName);
    }

    public void MoveToSelectionUI()
    {
        TM.MoveToSelection();
        SetLevelBookInfo(TM.lastLevelClicked);

        //Desactivar mapa
        mapUI.SetActive(false);

        //Activar Selection
        selectionUI.SetActive(true);
    }

    public void MoveToProgresionUI()
    {
        TM.MoveToProgresion();

        //FALTA MOSTRAR EL POWER LEVEL DEL PERSONAJE QUE PROBABLEMENTE NO VAYA NI SI QUIERA AQUI
        //currentCharacterPowerLevelText.SetText(character)

        //Setear en libro xpTotal
        currentTotalXpText.SetText(GameManager.Instance.currentExp.ToString());

        //Desactivar mapa
        mapUI.SetActive(false);

        //Activar Progresion
        progresionUI.SetActive(true);
    }

    public void MoveToUpgradesUI(CharacterData _unitClicked)
    {
        SetCharacterUpgradeBookInfo(_unitClicked);

        UpdateProgresionBook(_unitClicked);

        //Desactivar progresion
        progresionUI.SetActive(false);

        //Activar mejoras
        upgradesUI.SetActive(true);
    }

    public void BackToProgresion()
    {
        TM.BackToProgresion();

        currentCharacterPowerLevelText.SetText("---");

        //Desactivar upgrades
        upgradesUI.SetActive(false);

        //Activar progresion
        progresionUI.SetActive(true);

        //Hago desaparecer el árbol de habilidades
        ResetCharacterUpbradeBookInfo();
    }

    #endregion

    #region CHARACTER_SELECTION

    public void SetLevelBookInfo(LevelNode _levelClicked)
    {
        //Setear textos del nivel
        titleTextRef.SetText(_levelClicked.LevelTitle);
        descriptionTextRef.SetText(_levelClicked.descriptionText);
    }

    public void BackToMapUI()
    {
        //Movimientos de cámara y seteos de variables
        TM.BackToMap();

        //Activo UI Mapa
        mapUI.SetActive(true);

        //Desactivo UI Selección y progresión
        selectionUI.SetActive(false);
        progresionUI.SetActive(false);
    }

    #endregion

    #region PROGRESION

    public void SetCharacterUpgradeBookInfo(CharacterData unitClicked)
    {
        if (currentSkillTreeObj != null)
        {
            currentSkillTreeObj.SetActive(false);
            //Destroy(currentSkillTreeObj);
        }

        //Instancio árbol de habilidades
        unitClicked.skillTreePrefab.SetActive(true);
        currentSkillTreeObj = unitClicked.skillTreePrefab; //  Instantiate(unitClicked.skillTreePrefab, rightPageProgresionBook.transform);

        upgrades = currentSkillTreeObj.GetComponent<SkillTree>().allUpgradesInTree;
        ids = unitClicked.idSkillsBought;

        //Updatear upgrades
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].GetComponent<UpgradeNode>().TM = TM;
            upgrades[i].GetComponent<UpgradeNode>().myUnit = unitClicked;
            upgrades[i].GetComponent<UpgradeNode>().idUpgrade = i;
            upgrades[i].GetComponent<UpgradeNode>().UpdateIconAndDescription();
        }

        currentSkillTreeObj.GetComponent<SkillTree>().UpdateActiveAndPasive(unitClicked);

        //Por cada mejora compruebo si existe en ids compradas un valor en la lista que coincida el id
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (ids.Contains(upgrades[i].idUpgrade))
            {
                upgrades[i].UpgradeBought();
            }
        }

        UpdateUpgradesBlocked();
    }

    public void UpdateUpgradesBlocked()
    {
        SkillTree currentSkillTree = currentSkillTreeObj.GetComponent<SkillTree>();

        //Bloquear ramas activas
        if (currentSkillTree.active1Upgrades[0].isBought)
        {
            //Me aseguro de que rama 1 no está bloqueada
            currentSkillTree.active1Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            //Bloqueo la rama 2
            currentSkillTree.active2Upgrades[0].ShowHideFeedbackBlockedBranch(true);

            //Desbloqueo mejora 2
            currentSkillTree.active1Upgrades[1].isInteractuable = true;

        }
        else if (currentSkillTree.active2Upgrades[0].isBought)
        {
            //Me aseguro de que rama 2 no está bloqueada
            currentSkillTree.active2Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            //Bloqueo la rama 1
            currentSkillTree.active1Upgrades[0].ShowHideFeedbackBlockedBranch(true);

            //Desbloqueo mejora 2
            currentSkillTree.active2Upgrades[1].isInteractuable = true;
        }

        //Si ninguna ha sido comprada
        else
        {
            //Me aseguro de que rama 1 y 2 no está bloqueada
            currentSkillTree.active1Upgrades[0].ShowHideFeedbackBlockedBranch(false);
            currentSkillTree.active2Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            currentSkillTree.active1Upgrades[1].ShowFeedbackBlockedUpgrade();
            currentSkillTree.active2Upgrades[1].ShowFeedbackBlockedUpgrade();
        }

        //Bloquear ramas pasivas
        if (currentSkillTree.pasive1Upgrades[0].isBought)
        {
            //Me aseguro de que rama 1 no está bloqueada
            currentSkillTree.pasive1Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            //Bloqueo la rama 2
            currentSkillTree.pasive2Upgrades[0].ShowHideFeedbackBlockedBranch(true);

            //Desbloqueo mejora 2
            currentSkillTree.pasive1Upgrades[1].isInteractuable = true;
        }

        else if (currentSkillTree.pasive2Upgrades[0].isBought)
        {
            //Me aseguro de que rama 2 no está bloqueada
            currentSkillTree.pasive2Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            //Bloqueo la rama 1
            currentSkillTree.pasive1Upgrades[0].ShowHideFeedbackBlockedBranch(true);

            //Desbloqueo mejora 2
            currentSkillTree.pasive2Upgrades[1].isInteractuable = true;
        }

        else
        {
            //Me aseguro de que rama 1 no está bloqueada
            currentSkillTree.pasive1Upgrades[0].ShowHideFeedbackBlockedBranch(false);
            currentSkillTree.pasive2Upgrades[0].ShowHideFeedbackBlockedBranch(false);

            currentSkillTree.pasive1Upgrades[1].ShowFeedbackBlockedUpgrade();
            currentSkillTree.pasive2Upgrades[1].ShowFeedbackBlockedUpgrade();
        }
    }

    public void UpdateProgresionBook(CharacterData _unit)
    {
        currentCharacterPowerLevelText.SetText(_unit.unitPowerLevel.ToString());
        currentTotalXpText.SetText(GameManager.Instance.currentExp.ToString());
    }

    public void ResetCharacterUpbradeBookInfo()
    {
        if (currentSkillTreeObj != null)
        {
            currentSkillTreeObj.SetActive(false);
            //Destroy(currentSkillTreeObj);
        }
    }

    #endregion

    #region SCENE_FUNCTIONS

    //Al pulsar el botón de ready se carga la escena
    public void SceneToLoad()
    {
        GameManager.Instance.CheckStartLevel(TM.currentClickedSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

    #region LEVELS

    LevelNode[] debugAllLevelsInMapCheat;
    [SerializeField]
    CharacterData[] debugAllCharacterData;

    //Cheats
    public void UnlockAllLevelsAndCharacters()
    {
        debugAllLevelsInMapCheat = FindObjectsOfType<LevelNode>();
        for (int i = 0; i < debugAllLevelsInMapCheat.Length; i++)
        {
            debugAllLevelsInMapCheat[i].UnlockThisLevel();
        }

        debugAllCharacterData = FindObjectsOfType<CharacterData>();
        for (int i = 0; i < debugAllCharacterData.Length; i++)
        {
            Debug.Log("Has desbloqueado a un nuevo pj");

            GameManager.Instance.newCharacterToUnlock = debugAllCharacterData[i].gameObject;
            TM.UnlockNewCharacter();
        }
    }

    #endregion

    #region MAIN_MENU

    [SerializeField]
    GameObject confirmationPanel;

    [SerializeField]
    GameObject playCanvas;

    [SerializeField]
    GameObject optionsCanvas;

    [SerializeField]
    GameObject credits;

    [SerializeField]
    GameObject continueButton;

    [SerializeField]
    GameObject confirmationDeleteFile;

    [SerializeField]
    GameObject confirmationDeleteFileLAST;

    //Botón de exit en libro lo activa y botón de no en confirmar lo desactiva
    public void ConfirmateExit(bool _shouldActivate)
    {
        //Activar o desactivar panel
        confirmationPanel.SetActive(_shouldActivate);
    }

    public void PlayButton()
    {
        playCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        credits.SetActive(false);

        //Guardo el valor del sonido dejado al mover el slider
        SoundManager.Instance.MusicVolumeSave = musicSlider.value;
        SoundManager.Instance.SfxVolumeSave = sFXSlider.value;

        ShowHideContinueButton();
    }

    public void OptionsButton()
    {
        optionsCanvas.SetActive(true);
        playCanvas.SetActive(false);
        credits.SetActive(false);
    }

    public void CreditsButton()
    {
        credits.SetActive(true);
        optionsCanvas.SetActive(false);
        playCanvas.SetActive(false);
    }

    public void BackToMenu()
    {
        TM.MoveToMainMenu();

        //Guardar la partida-+
    }

    #region PLAY_GAME

     public void NewGameButton()
     {
        if (GameManager.Instance.CheckIfSaveFileExists())
        {
            //Aviso borrar partida
            confirmationDeleteFile.SetActive(true);
        }

        else
        {
            GameManager.Instance.SaveGame();

            //Desbloqueo primer logro
            GameManager.Instance.UnlockAchievement(AppAchievements.ACHV_BEGIN);

            //Mover cámara
            TM.BackToMap();
        }
     }

    //Hacer aparecer o desaparecer botón de continue
    public void ShowHideContinueButton()
    {
        if (GameManager.Instance.CheckIfSaveFileExists())
        {
            continueButton.SetActive(true);
        }

        else
        {
            continueButton.SetActive(false);
        }
    }

    public void ConfirmateDelet(bool _firstWarning)
    {
        if (_firstWarning)
        {
            confirmationDeleteFile.SetActive(false);
            confirmationDeleteFileLAST.SetActive(true);
        }

        else
        {
            confirmationDeleteFile.SetActive(false);
            confirmationDeleteFileLAST.SetActive(false);

            GameManager.Instance.ReseteSaveFile();
            GameManager.Instance.SaveGame();

            TM.BackToMap();
        }
    }

    public void CancelDelete()
    {
        confirmationDeleteFile.SetActive(false);
        confirmationDeleteFileLAST.SetActive(false);
    }
     
     public void ContinueButton()
     {
        //Cargar partida
        GameManager.Instance.LoadGame();

        //Actualizar??

        //Cargar partida y mover cámara
        TM.BackToMap();
     }

    #endregion

    #region OPTIONS

    public void SetResolution(int _resolutionIndex)
    {
        //Resolution resolution = resolutions[_resolutionIndex];
        //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        //PlayerPrefs.SetInt(AppPlayerPrefKeys.RESOLUTION, _resolutionIndex);
    }


    public void SetFullScreen (bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
        toggleFullsCreen.isOn = _isFullScreen;

        if (_isFullScreen)
        {
            PlayerPrefs.SetInt(AppPlayerPrefKeys.FULLSCREEN, 1);
        }
        else
        {
            PlayerPrefs.SetInt(AppPlayerPrefKeys.FULLSCREEN, 0);
        }
       
    }

    public void OnMusicValueChanged()
    {
        SoundManager.Instance.MusicVolume = musicSlider.value;
    }

    public void OnSfxValueChanged()
    {
        SoundManager.Instance.SfxVolume = sFXSlider.value;
    }

    public void SetQuality (int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
        qualityDropdown.value = _qualityIndex;
        qualityDropdown.RefreshShownValue();

        PlayerPrefs.SetInt(AppPlayerPrefKeys.QUALITY_LEVEL, _qualityIndex);
    }

    public void ResetValues()
    {
        //Reseteo resolución
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == 1920 && resolutions[i].height == 1080)
            {
                SetResolution(i);
                resolutionDropdown.value = PlayerPrefs.GetInt(AppPlayerPrefKeys.RESOLUTION);
                resolutionDropdown.RefreshShownValue();
                break;
            }
        }

        //FullScreen
        SetFullScreen(true);

        //Volumen
        SoundManager.Instance.MusicVolume = 0.5f;
        musicSlider.value = 0.5f; 
        SoundManager.Instance.SfxVolume = 0.5f;
        sFXSlider.value = 0.5f;

        //Calidad
        SetQuality(5);

    }

    #endregion

    #endregion

    #region UPGRADES

    public void ConfirmateUpgrade(bool _showConfirmation, UpgradeNode _upgradeClicked)
    {
        confirmateUpgrade.SetActive(_showConfirmation);

        //Aviso rama se bloqueará
        if (currentSkillTreeObj.GetComponent<SkillTree>().firstUpgradesInTree.Contains(_upgradeClicked))
        {
            warningBlockUpgrade.SetActive(true);
        }

        else
        {
            warningBlockUpgrade.SetActive(false);
        }

        lastUpgradeClicked = _upgradeClicked;
    }

    public void YesBuyUpgrade()
    {
        SoundManager.Instance.PlaySound(AppSounds.BUYABILITIES);

        FindObjectOfType<TableManager>().BuyUpgrade(lastUpgradeClicked);
        confirmateUpgrade.SetActive(false);


        SkillTree currentSkillTree = currentSkillTreeObj.GetComponent<SkillTree>();
        Instantiate(TM.vfxLevelUp, currentSkillTree.active1Upgrades[0].myUnit.gameObject.transform);

        GameManager.Instance.SaveGame();
    }

    public void NoBuyUpgrade()
    {
        confirmateUpgrade.SetActive(false);
    }

    public void NotEnoughXp()
    {
        currentText = textNotEnoughXp;
        StartCoroutine("ShowHidePanelXp");
    }

    public void AlreadyBought()
    {
        currentText = textAlreadyBought;
        StartCoroutine("ShowHidePanelXp");
    }

    IEnumerator ShowHidePanelXp()
    {
        timedPanel.SetActive(true);
        currentText.SetActive(true);

        yield return new WaitForSeconds(timeToHidePanel);

        currentText.SetActive(false);
        timedPanel.SetActive(false);
    }

    #endregion

    #region HIDE_SHOW_UI

    public void ShowMapUI()
    {
        mapUI.SetActive(true);
        selectionUI.SetActive(false);
        progresionUI.SetActive(false);
        upgradesUI.SetActive(false);

    }

    public void ShowLevelInfoUI()
    {
        mapUI.SetActive(false);
        selectionUI.SetActive(true);
        progresionUI.SetActive(false);
        upgradesUI.SetActive(false);

    }

    public void ShowProgresionUI()
    {
        mapUI.SetActive(false);
        selectionUI.SetActive(false);
        progresionUI.SetActive(true);
        upgradesUI.SetActive(false);

    }

    public void ShowUpgradesUI()
    {
        mapUI.SetActive(false);
        selectionUI.SetActive(false);
        progresionUI.SetActive(false);
        upgradesUI.SetActive(true);
    }

    public void HideAllUI()
    {
        mapUI.SetActive(false);
        selectionUI.SetActive(false);
        progresionUI.SetActive(false);
        upgradesUI.SetActive(false);
    }

    #endregion

    public void DebugFreeXp()
    {
        GameManager.Instance.currentExp = 9000;
    }

    public void ClickSound()
    {
        SoundManager.Instance.PlaySound(AppSounds.BUTTONCLICK);
    }

    public void ChangeCreditsPage(bool isInPage1)
    {
        if (isInPage1) 
        {
            credits1Panel.SetActive(false);
            credits2Panel.SetActive(true);
            isInPage1 = false;
        }
        else
        {
            credits1Panel.SetActive(true);
            credits2Panel.SetActive(false);
            isInPage1 = true;
        }      
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Guardo el valor del sonido dejado al mover el slider
            SoundManager.Instance.MusicVolumeSave = musicSlider.value;
            SoundManager.Instance.SfxVolumeSave = sFXSlider.value;
        }
    }
}
