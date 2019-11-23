using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    #region VARIABLES

    [Header("Level Logic")]
    //Número de unidades que requiere el nivel
    [Range(1,4)]
    [SerializeField]
    public int maxNumberOfUnits;

    [SerializeField]
    public int idLevel;

    ///BUSCAR FORMA MEJOR DE PASAR EL NIVEL QUE CON STRINGS. 
    //Escena asociada al nivel
    [SerializeField]
    public string sceneName;

    //Bool que indica si el nivel ha sido desbloqueado
    [SerializeField]
    public bool isUnlocked;

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

    [Header("Materiales")]

    //Materiales para indicar el estado del nivel. Verde = completado. Rojo = Desbloqueado pero sin completar. Negro = Bloqueado
    [SerializeField]
    Material unlockedLevel;
    [SerializeField]
    Material completedLevel;
    [SerializeField]
    Material blockedLevel;

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
        if (isUnlocked)
        {
            TM.OnLevelClicked(GetComponent<LevelNode>());
            GameManager.Instance.currentLevelNode = idLevel;
        }
    }

    #region FEEDBACK

    //Función que desbloquea este nivel
    public void UnlockThisLevel()
    {
        isUnlocked = true;
        GetComponent<MeshRenderer>().material = unlockedLevel;
    }

    //Aviso a los niveles conectados que tienen que desbloquearse.
    public void UnlockConnectedLevels()
    {
        GetComponent<MeshRenderer>().material = completedLevel;

        for (int i = 0; i <unlockableLevels.Count ; i++)
        {
            unlockableLevels[i].UnlockThisLevel();
        }
    }


    #endregion
}
