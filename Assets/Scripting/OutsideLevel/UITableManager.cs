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

    [Header("REFERENCIAS")]
    [SerializeField]
    private TableManager TM;
    #endregion

    #region INIT

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

            if (ids.Count == 0)
            {
                upgrades[i].GetComponent<UpgradeNode>().idUpgrade = i;
            }
        }

        //Si las ids no son 0 entonces es que no es la primera vez que se setea el árbol y no tengo que volver a setear los ids
        if (ids.Count != 0)
        {
            for (int j = 0; j < ids.Count; j++)
            {
                if (upgrades[j].idUpgrade == ids[j])
                {

                    upgrades[j].UpgradeBought();
                    Debug.Log(upgrades[j].name);

                    //¿Este break esta bien?
                    break;
                }
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


}
