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


    //AÑADIDO
    //Barra de vida y valor de la barra del personaje
    [SerializeField]
    public TextMeshProUGUI healthValue;
    [SerializeField]
    public Slider healthBar;

    //Los tokens son listas por si en el futuro hay personajes que necesitan más tokens. (De ser así habría que hacer más cambios)
    [SerializeField]
    public List<GameObject> attackTokens;
    [SerializeField]
    public List<GameObject> movementTokens;

    //Gameobject Image dónde va el sprite del personaje
    [SerializeField]
    public Image characterPortrait;

	[SerializeField]
	public Material highlightMaterial;
	[HideInInspector]
	public Material nonHighlightMaterial;

    #endregion

    #region INIT

    private void Awake()
	{
		UIM = FindObjectOfType<UIManager>();
        //Se desactiva para que el UImanager active únicamente los necesarios en función del número de personajes.
        gameObject.SetActive(false);
		nonHighlightMaterial = GetComponent<Image>().material;
	}

    private void Start()
    {
        RefreshHealth();
        RefreshSprites();
        RefreshTokens();
    }

    #endregion

    #region INTERACTION

    public void AssignClickerPlayer()
	{
		UIM.PortraitCharacterSelect(assignedPlayer);
	}

	public void Highlight()
	{
		UIM.HighlightCharacter(assignedPlayer);
	}

	public void Unhighlight()
	{
		UIM.UnHighlightCharacter(assignedPlayer);
	}

	public void HighlightPortrait()
	{
		GetComponent<Image>().material = highlightMaterial;
	}
	public void UnHighlightPortrait()
	{
		GetComponent<Image>().material = nonHighlightMaterial;
	}

    #endregion

    #region REFRESH

    public void RefreshHealth()
    {
        healthBar.maxValue = assignedPlayer.maxHealth;
        healthBar.value = assignedPlayer.currentHealth;
        healthValue.text = assignedPlayer.currentHealth + "/" + assignedPlayer.maxHealth;
    }

    public void RefreshTokens()
    {
        for (int i = 0; i < attackTokens.Count; i++)
        {
            attackTokens[i].SetActive(!assignedPlayer.hasAttacked);
        }
        for (int i = 0; i < movementTokens.Count; i++)
        {
            movementTokens[i].SetActive(!assignedPlayer.hasMoved);
        }
    }

    public void RefreshSprites()
    {
        characterPortrait.sprite = assignedPlayer.portraitImage;
    }

    #endregion

}
