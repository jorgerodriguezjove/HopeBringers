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

    //Array con los paneles que se pueden activar para colocar las unidades en un nivel
    [SerializeField]
    public GameObject[] panelsForUnitColocation = new GameObject[4];

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

    [SerializeField]
    private GameObject confirmateUpgrade;

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
    Resolution[] resolutions;
    [SerializeField]
    TMP_Dropdown resolutionDropdown;

    //Fulsscreen
    [SerializeField]
    Toggle toggleFullsCreen;

    //Música y sonido
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sFXSlider;

    //Calidad
    [SerializeField]
    TMP_Dropdown qualityDropdown;

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

        for (int i = 0; i < _levelClicked.maxNumberOfUnits; i++)
        {
            panelsForUnitColocation[i].SetActive(true);
        }
    }

    public void BackToMapUI()
    {
        //Desactivo todos los paneles de unidad
        for (int i = 0; i < panelsForUnitColocation.Length; i++)
        {
            panelsForUnitColocation[i].SetActive(false);
        }

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
            Destroy(currentSkillTreeObj);
        }

        //Instancio árbol de habilidades
        currentSkillTreeObj = Instantiate(unitClicked.skillTreePrefab, rightPageProgresionBook.transform);

        upgrades = currentSkillTreeObj.GetComponent<SkillTree>().allUpgradesInTree;
        ids = unitClicked.idSkillsBought;
        Debug.Log(unitClicked.idSkillsBought.Count);

        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].GetComponent<UpgradeNode>().TM = TM;
            upgrades[i].GetComponent<UpgradeNode>().myUnit = unitClicked;
            upgrades[i].GetComponent<UpgradeNode>().idUpgrade = i;
        }

        Debug.Log(ids.Count);

        //Por cada mejora compruebo si existe en ids compradas un valor en la lista que coincida el id
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (ids.Contains(upgrades[i].idUpgrade))
            {
                upgrades[i].UpgradeBought();
            }
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
            Destroy(currentSkillTreeObj);
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

    LevelNode[] allLevelsInMapCheat;

    //Cheats
    public void UnlockAllLevels()
    {
        allLevelsInMapCheat = FindObjectsOfType<LevelNode>();
        for (int i = 0; i < allLevelsInMapCheat.Length; i++)
        {
            allLevelsInMapCheat[i].UnlockThisLevel();
        }
    }

    #region MAIN_MENU

    [SerializeField]
    GameObject confirmationPanel;

    [SerializeField]
    GameObject playCanvas;

    [SerializeField]
    GameObject optionsCanvas;

    [SerializeField]
    GameObject credits;


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
        //Comprobar si hay ya una partida guardada y activar aviso

        //Desbloqueo primer logro
        GameManager.Instance.UnlockAchievement(AppAchievements.ACHV_BEGIN);

        //Else crear guardado y mover cámara
        TM.BackToMap();
     }
     
     public void ContinueButton()
     {
        //Cargar partida y mover cámara
        TM.BackToMap();
     }

    #endregion

    #region OPTIONS

    public void SetResolution(int _resolutionIndex)
    {
        Resolution resolution = resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt(AppPlayerPrefKeys.RESOLUTION, _resolutionIndex);
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

    public void ConfirmateUpgrade(bool _showConfirmation, UpgradeNode _upgradeClicked)
    {
        confirmateUpgrade.SetActive(_showConfirmation);
        lastUpgradeClicked = _upgradeClicked;
    }

    public void YesBuyUpgrade()
    {
        FindObjectOfType<TableManager>().BuyUpgrade(lastUpgradeClicked);
        confirmateUpgrade.SetActive(false);
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


}
