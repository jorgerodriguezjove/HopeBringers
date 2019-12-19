using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UITableManager : MonoBehaviour
{
    #region VARIABLES
    
    [Header("UI REFERENCIAS")]

    //Referencia al texto de título
    [SerializeField]
    private TextMeshProUGUI titleTextRef;

    //Referencia al texto de descripción
    [SerializeField]
    private TextMeshProUGUI descriptionTextRef;

    //Referencia al panel dónde se instancian los huecos para colocar a los personajes
    [SerializeField]
    private GameObject panelForUnitColocation;

    //Referencia al objeto dónde aparece el árbol de habilidades del personaje.
    [SerializeField]
    private GameObject rightPageProgresionBook;

    [Header("PROGRESION")]

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

    #region CHARACTER_SELECTION

    public void SetLevelBookInfo(LevelNode levelClicked)
    {
        //Setear textos del nivel
        titleTextRef.SetText(levelClicked.LevelTitle);
        descriptionTextRef.SetText(levelClicked.descriptionText);

        //Crear número adecuado de huecos para personaje
    }

    public void SetCharacterUpgradeBookInfo(CharacterData unitClicked)
    {
        if (currentSkillTreeObj != null)
        {
            Destroy(currentSkillTreeObj);
        }

        //Instancio árbol de habilidades
        currentSkillTreeObj = Instantiate(unitClicked.skillTreePrefab,rightPageProgresionBook.transform);

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
