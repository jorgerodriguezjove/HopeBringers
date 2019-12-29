using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Portraits : MonoBehaviour
{
	#region VARIABLES

	[HideInInspector]
	public PlayerUnit assignedPlayer;
	[HideInInspector]
	private UIManager UIM;
    //Añadido para hacer comprobaciones de turnos
    [HideInInspector]
    private LevelManager LM;

    //Barra de vida y valor de la barra del personaje
    [SerializeField]
    public TextMeshProUGUI healthValue;
    [SerializeField]
    public Slider healthBar;

    //Padre donde se van a instanciar los tokens de vida
    [SerializeField]
    private GameObject lifeContainer;

    //Prefab de los tokens de vida
    [SerializeField]
    private GameObject lifeTokenPref;

    //Lista con los tokens de vida del jugador
    [HideInInspector]
    private List<GameObject> lifeTokensList = new List<GameObject>();

    //Los tokens son listas por si en el futuro hay personajes que necesitan más tokens. (De ser así habría que hacer más cambios)
    [SerializeField]
    public List<GameObject> attackTokens;
    [SerializeField]
    public List<GameObject> movementTokens;

	[HideInInspector]
	private Sprite selectedImage;
	[HideInInspector]
	private Sprite initImage;
    //Con este panel es más fácil cambiar el color que se quiere desde el editor, en vez de estar haciendo un nuevo sprite cada vez.
    [SerializeField]
    public GameObject selectedPanel;

	//Gameobject Image dónde va el sprite del personaje
	[SerializeField]
    public Image characterPortrait;

    //Bool que indica a los retratos si están clickados
    public bool isClicked;

    #endregion

    #region INIT

    private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();

        //Añadido para hacer comprobaciones de turnos
        LM = FindObjectOfType<LevelManager>();

        //Se desactiva para que el UImanager active únicamente los necesarios en función del número de personajes.
        gameObject.SetActive(false);
		//initImage = GetComponent<Image>().sprite;
	}

    private void Start()
    {
        RefreshHealth();
        RefreshSprites();
        RefreshTokens();
    }

    #endregion
    

    //A las funciones de hover y click se llama gracias al event triggger que tienen en el componente (mirar editor vamos)
    #region INTERACTION

    public void AssignClickerPlayer()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            UIM.PortraitCharacterSelect(assignedPlayer);
            isClicked = true;
        }
	}

	public void Highlight()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            UIM.HighlightCharacter(assignedPlayer);

            if (LM.selectedCharacter == null && LM.selectedEnemy == null)
            {
                ShowCharacterImageFromPortrait();
                UIM.LM.ShowUnitHover(assignedPlayer.movementUds, assignedPlayer);

            }
            selectedPanel.SetActive(true);
        }
    }

	public void Unhighlight()
	{
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedCharacter == null && LM.selectedEnemy == null)
            {
               
                UIM.LM.HideUnitHover(assignedPlayer);
            }

            if (LM.selectedCharacter != assignedPlayer)
            {
                UIM.UnHighlightCharacter(assignedPlayer);
            }
               

            if (isClicked == false)
            {
               

                if (LM.selectedCharacter != assignedPlayer)
                {
                    assignedPlayer.ResetColor();
                    selectedPanel.SetActive(false);
                }
        
            }
        }
    }

	public void HighlightPortrait()
	{
        //GetComponent<Image>().sprite = selectedImage;
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            selectedPanel.SetActive(true);
        }
    }
	public void UnHighlightPortrait()
	{
        //GetComponent<Image>().sprite = initImage;

        //if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        //{
        //    selectedPanel.SetActive(false);
        //}
        if (LM.selectedCharacter != this)
        {
            selectedPanel.SetActive(false);
        }
            
    }

	public void ShowCharacterImageFromPortrait()
	{
		if (UIM.LM.selectedCharacter == null)
		{
			UIM.ShowUnitInfo(assignedPlayer.unitInfo, assignedPlayer);
		}
	}

	public void OnMouseEnter()
	{
		Highlight();
		
	}

	private void OnMouseExit()
	{
		Unhighlight();
	}

	private void OnMouseDown()
	{
		AssignClickerPlayer();
	}

	#endregion

	#region REFRESH

	//Función que inicializa la vida de los personajes
	public void InitializeHealth()
    {
        for (int i = 0; i < assignedPlayer.maxHealth; i++)
        {
            GameObject lifeTok = Instantiate(lifeTokenPref, lifeContainer.transform);
            lifeTokensList.Add(lifeTok);
            
        }
    }

    //Función que se encarga de actualizar la vida del personaje.
    public void RefreshHealth()
    {
        //Recorro la lista de tokens empezando por el final. 
        //El -1 en el count es porque la lista empieza en el 0 y por tanto es demasiado grande
        //Sin embargo tengo que sumarle 1 en la i porque si no la current health al principio no entra
        for (int i = lifeTokensList.Count - 1; i+1 > assignedPlayer.currentHealth ; i--)
        {
            if (lifeTokensList[i].GetComponent<LifeToken>())
            {
                if (!lifeTokensList[i].GetComponent<LifeToken>().haveIFlipped)
                {
                    lifeTokensList[i].GetComponent<LifeToken>().FlipToken();
                }
            }
        }

        //Código antiguo con la barra de vida
        #region OldCode
        //healthBar.maxValue = assignedPlayer.maxHealth;
        //healthBar.value = assignedPlayer.currentHealth;
        //healthValue.text = assignedPlayer.currentHealth + "/" + assignedPlayer.maxHealth;
        #endregion
    }

    //Función que se encarga de actualizar el estado de los tokens de movimiento y ataque.
    public void RefreshTokens()
    {
        for (int i = 0; i < attackTokens.Count; i++)
        {
            //Añado este if para que compruebe si es un decoy o no.
            if (!assignedPlayer.GetComponent<MageDecoy>())
            {
                //attackTokens[i].SetActive(!assignedPlayer.hasAttacked);
                if (!assignedPlayer.hasAttacked)
                {
                    attackTokens[i].GetComponent<Animator>().Play("TokenReset");
                }
                else
                {
                    attackTokens[i].GetComponent<Animator>().Play("TokenFlip");
                }
            }
        }
        for (int i = 0; i < movementTokens.Count; i++)
        {
            //Añado este if para que compruebe si es un decoy o no.
            if (!assignedPlayer.GetComponent<MageDecoy>())
            {
                //movementTokens[i].SetActive(!assignedPlayer.hasMoved);
                if (!assignedPlayer.hasMoved)
                {
                    movementTokens[i].GetComponent<Animator>().Play("TokenReset");
                }
                else
                {
                    movementTokens[i].GetComponent<Animator>().Play("TokenFlip");
                }
            }
		}
    }

    public void RefreshSprites()
    {
        characterPortrait.sprite = assignedPlayer.portraitImage;
    }

    #endregion

}
