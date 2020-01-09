using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNode : MonoBehaviour
{
    #region VARIABLES

    [Header("LÓGICA")]
    //Número de unidades que requiere el nivel
    //IMPORTANTE, SI AUMENTAMOS EL NÚMERO DE UNIDADES POR ENCIMA DE 4 HAY QUE CAMBIAR:
    //En Ui table Manager el tamaño del array de panelsForUnitColocation
    //En escena hay que ajustar el grid que contiene los paneles para que entren todos los paneles en el libro
    [Range(1,4)]
    [SerializeField]
    public int maxNumberOfUnits;

    [SerializeField]
    public int idLevel;

    [SerializeField]
    public int xpGained;

    ///BUSCAR FORMA MEJOR DE PASAR EL NIVEL QUE CON STRINGS. 
    //Escena asociada al nivel
    [SerializeField]
    public string sceneName;

    //Bool que indica si el nivel ha sido desbloqueado
    [SerializeField]
    public bool isUnlocked;

    [Header("NIVELES RELACIONADOS")]

    //Niveles que están conectados a este nivel. En el futuro servirá para el movimiento de la ficha
    [SerializeField]
    public List<LevelNode> surroundingLevels;

    //Niveles que se desbloquean al completarse este nivel
    [SerializeField]
    public List<LevelNode> unlockableLevels;

    [Header("LEVEL INFO TEXT")]

    //Título del nivel
    [SerializeField]
    public string LevelTitle;
    //Decripción del nivel
    [SerializeField]
    public string descriptionText;

	[Header("MATERIALES")]

	//Materiales para indicar el estado del nivel. Verde = completado. Rojo = Desbloqueado pero sin completar. Negro = Bloqueado
	//[SerializeField]
	//Material unlockedLevel;
	//[SerializeField]
	//Material completedLevel;
	//[SerializeField]
	//Material blockedLevel;
	[SerializeField]
	Sprite unlockedLevel;
	[SerializeField]
	Sprite completedLevel;
	[SerializeField]
	GameObject dottedLinePath;

    [Header("REFERENCIAS")]
    [SerializeField]
    private TableManager TM;

    #endregion

    #region INIT

    #endregion

	#region INTERACTION
	
	//Llamar en click desde el event trigger
	public void SelectLevel()
	{
		//Avisar al TM de que se ha pulsado un nivel
		if (isUnlocked)
		{
			TM.OnLevelClicked(GetComponent<LevelNode>());
			GameManager.Instance.currentLevelNode = idLevel;
            GameManager.Instance.possibleXpToGainIfCurrentLevelIsWon = xpGained;
		}
	}

	#endregion

	#region FEEDBACK

	//Función que desbloquea este nivel
	public void UnlockThisLevel()
    {
        isUnlocked = true;
        GetComponent<Image>().sprite = unlockedLevel;
		GetComponent<Image>().enabled = true;
		if (dottedLinePath != null)
		{
			dottedLinePath.SetActive(true);
		}
    }

    //Aviso a los niveles conectados que tienen que desbloquearse.
    public void UnlockConnectedLevels()
    {
		GetComponent<Image>().sprite = completedLevel;

		for (int i = 0; i <unlockableLevels.Count ; i++)
        {
            unlockableLevels[i].UnlockThisLevel();
        }
    }


    #endregion
}
