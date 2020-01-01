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

    //SIN HACER TODAVIA
    //Referencia al panel dónde se instancian los huecos para colocar a los personajes
    //[SerializeField]
    //private GameObject panelForUnitColocation;

    [Header("PROGRESION")]

    //Referencia al objeto dónde aparece el árbol de habilidades del personaje.
    [SerializeField]
    private GameObject rightPageProgresionBook;

    //Referencia al árbol de habilidades actualmente en pantalla.
    private GameObject currentSkillTreeObj;

    //Referencias de las listas de ids del personaje y la lista de upgrades del skill tree.
    private List<int> ids;
    private List<UpgradeNode> ups;

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
       
        //Desactivar mapa
        mapUI.SetActive(false);

        //Activar Progresion
        progresionUI.SetActive(true);
    }

    public void MoveToUpgradesUI(CharacterData _unitClicked)
    {
        SetCharacterUpgradeBookInfo(_unitClicked);

        //Desactivar progresion
        progresionUI.SetActive(false);

        //Activar mejoras
        upgradesUI.SetActive(true);
    }

    public void BackToProgresion()
    {
        TM.BackToProgresion();

        //Desactivar upgrades
        upgradesUI.SetActive(false);

        //Activar progresion
        progresionUI.SetActive(true);

        //Hago desaparecer el árbol de habilidades
        ResetCharacterUpbradeBookInfo();
    }

    #endregion


    #region CHARACTER_SELECTION

    public void SetLevelBookInfo(LevelNode levelClicked)
    {
        //Setear textos del nivel
        titleTextRef.SetText(levelClicked.LevelTitle);
        descriptionTextRef.SetText(levelClicked.descriptionText);

        //Crear número adecuado de huecos para personaje
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
            Destroy(currentSkillTreeObj);
        }

        //Instancio árbol de habilidades
        currentSkillTreeObj = Instantiate(unitClicked.skillTreePrefab, rightPageProgresionBook.transform);

        ups = currentSkillTreeObj.GetComponent<SkillTree>().allUpgradesInTree;
        ids = unitClicked.idSkillsBought;

        for (int i = 0; i < ups.Count; i++)
        {
            ups[i].GetComponent<UpgradeNode>().TM = TM;
            ups[i].GetComponent<UpgradeNode>().myUnit = unitClicked;

            for (int j = 0; j < ids.Count; j++)
            {
                if (ups[i].idUpgrade == ids[j])
                {
                    ups[i].UpgradeBought();

                    //¿Este break esta bien?
                    break;
                }
            }
        }
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
        SceneManager.LoadScene(TM.currentClickedSceneName, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

}
