using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITableManager : MonoBehaviour
{
    #region VARIABLES

    [Header("UI Referencias")]

    //Referencia al texto de título
    [SerializeField]
    private TextMeshProUGUI titleTextRef;

    //Referencia al texto de descripción
    [SerializeField]
    private TextMeshProUGUI descriptionTextRef;

    //Referencia al panel dónde se instancian los huecos para colocar a los personajes
    [SerializeField]
    private GameObject panelForUnitColocation;



    #endregion

    #region INIT

    #endregion

    #region CHARACTER_SELECTION

    public void SetBookInfo(LevelNode levelClicked)
    {
        //Setear textos del nivel
        titleTextRef.SetText(levelClicked.LevelTitle);
        descriptionTextRef.SetText(levelClicked.descriptionText);

        //Crear número adecuado de huecos para personaje
    }



    #endregion
}
