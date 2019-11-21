using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    #region VARIABLES

    [Header("Level Logic")]
    //Número de unidades que requiere el nivel
    [SerializeField]
    public int maxNumberOfUnits;

    ///BUSCAR FORMA MEJOR DE PASAR EL NIVEL QUE CON STRINGS. 
    //Escena asociada al nivel
    [SerializeField]
    public string sceneName;

    //Bool que indica si el nivel ha sido desbloqueado
    [SerializeField]
    public bool isBlocked;

    [Header("Relationed levels")]

    //Niveles que están conectados a este nivel. En el futuro servirá para el movimiento de la ficha
    [SerializeField]
    public List<LevelNode> surroundingLevels;

    //Niveles que se desbloquean al completarse este nivel
    [SerializeField]
    public List<LevelNode> unlockableLevels;

    [Header("Level Info Text")]

    //Título del nivel
    [SerializeField]
    public string LevelTitle;
    //Cápitulo al que corresponde
    [SerializeField]
    public string chapter;
    //Decripción del nivel
    [SerializeField]
    public string descriptionText;

    [Header("Referencias")]
    [SerializeField]
    private TableManager TM;

    #endregion

    #region INIT

    #endregion

    //Al pulsar sobre el nivel
    private void OnMouseDown()
    {
        //Avisar al TM de que se ha pulsado un nivel
        if (!isBlocked)
        {
            TM.MoveToCharacterSelectionForLevel(GetComponent<LevelNode>());
        }
    }



}
