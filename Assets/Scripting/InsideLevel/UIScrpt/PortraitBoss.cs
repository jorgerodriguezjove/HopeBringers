using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PortraitBoss : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    public EnemyUnit assignedBoss;

    [HideInInspector]
    public int numberOfAttackTokens;

    [Header("TOKENS VIDA")]
    //Uso este int para poder resetear los tokens 
    private int activatedTokens;

    //Padre donde se van a instanciar los tokens de vida
    [SerializeField]
    private GameObject lifeContainer;

    //Prefab de los tokens de vida
    [SerializeField]
    private GameObject lifeTokenPref;

    //Lista con los tokens de vida del jugador
    [HideInInspector]
    public List<GameObject> lifeTokensList = new List<GameObject>();

    [Header("TOKENS MOV Y ATQ")]
    //Los tokens son listas por si en el futuro hay personajes que necesitan más tokens. (De ser así habría que hacer más cambios)
    [SerializeField]
    public List<GameObject> attackTokens;
    [SerializeField]
    public List<GameObject> movementTokens;

    [Header("RETRATO Y PANEL")]
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

    [Header("REFERENCIAS")]
    [HideInInspector]
    private UIManager UIM;
    //Añadido para hacer comprobaciones de turnos
    [HideInInspector]
    private LevelManager LM;
    #endregion

    #region INIT

    private void OnEnable()
    {
        Debug.Log("Enabled");

        UIM = FindObjectOfType<UIManager>();

        //Añadido para hacer comprobaciones de turnos
        LM = FindObjectOfType<LevelManager>();

        //Se desactiva para que el UImanager active únicamente los necesarios en función del número de personajes.
        gameObject.SetActive(true);
        //initImage = GetComponent<Image>().sprite;

        AssignCurrentBoss();

        if (assignedBoss != null)
        {
            numberOfAttackTokens = assignedBoss.numberOfAttackTokens;

            InitializeHealth();
            InitializeAttackTokens();

            RefreshHealth();
            activatedTokens = assignedBoss.maxHealth;

            RefreshSprites();
            //RefreshTokens();
        }

        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AssignCurrentBoss()
    {
        if (FindObjectOfType<MechaBoss>())
        {
            assignedBoss = FindObjectOfType<MechaBoss>();
            assignedBoss.bossPortrait = this;
        }

        else if (FindObjectOfType<DarkLord>())
        {
            assignedBoss = FindObjectOfType<DarkLord>();
            assignedBoss.bossPortrait = this;
        }

        else if (FindObjectOfType<BossMultTile>())
        {
            assignedBoss = FindObjectOfType<BossMultTile>();
            assignedBoss.bossPortrait = this;
        }
    }

    #endregion

    //A las funciones de hover y click se llama gracias al event triggger que tienen en el componente (mirar editor vamos)
    #region INTERACTION

    public void ClickPortrait()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            //UIM.PortraitCharacterSelect(assignedBoss);
            LM.camRef.FocusCameraOnCharacter(assignedBoss.gameObject);
            isClicked = true;
        }
    }

    public void Highlight()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            //UIM.HighlightCharacter(assignedBoss);

            if (LM.selectedCharacter == null && LM.selectedEnemy == null)
            {
                //ShowCharacterImageFromPortrait();
                //UIM.LM.ShowUnitHover(assignedBoss.movementUds, assignedBoss);
            }

            Debug.Log("owo");
            selectedPanel.SetActive(true);
        }
    }

    public void Unhighlight()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedCharacter == null && LM.selectedEnemy == null)
            {

                //UIM.LM.HideUnitHover(assignedBoss);
            }

            if (LM.selectedCharacter != assignedBoss)
            {
                //UIM.UnHighlightCharacter(assignedBoss);
            }

            if (isClicked == false)
            {
                if (LM.selectedCharacter != assignedBoss)
                {
                    assignedBoss.ResetColor();
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

        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            selectedPanel.SetActive(false);
        }

        if (LM.selectedCharacter != this)
        {
            selectedPanel.SetActive(false);
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
        ClickPortrait();
    }

    #endregion

    #region REFRESH

    //Función que inicializa la vida del boss
    public void InitializeHealth()
    {
        for (int i = 0; i < assignedBoss.maxHealth; i++)
        {
            if (lifeTokensList.Count < assignedBoss.maxHealth)
            {
                GameObject lifeTok = Instantiate(lifeTokenPref, lifeContainer.transform);
                lifeTokensList.Add(lifeTok);
            }   
        }
    }

    public void InitializeAttackTokens()
    {
        //Activo los tokesn según el número de ataques del boss
        for (int i = 0; i < numberOfAttackTokens; i++)
        {
            attackTokens[i].SetActive(true);
        }
    }

    //Función que se encarga de actualizar la vida del personaje.
    public void RefreshHealth()
    {
        Debug.Log("refresh");
        if (assignedBoss != null)
        {
            for (int i = 0; i < assignedBoss.maxHealth; i++)
            {
                if (i < assignedBoss.currentHealth)
                {
                    if (i < assignedBoss.currentArmor)
                    {
                        if (lifeTokensList[i].GetComponent<LifeToken>())
                        {
                            lifeTokensList[i].GetComponent<LifeToken>().ArmoredToken();
                            Debug.Log("armor");
                        }
                    }

                    else
                    {
                        if (lifeTokensList[i].GetComponent<LifeToken>())
                        {
                            lifeTokensList[i].GetComponent<LifeToken>().ResetToken();
                            Debug.Log("reset");
                        }
                    }
                }

                else
                {
                    if (lifeTokensList[i].GetComponent<LifeToken>())
                    {
                        lifeTokensList[i].GetComponent<LifeToken>().FlipToken();
                        Debug.Log("flip"); 
                    }
                }
            }
        }
    }

    int attackCounter;

    //Flipea el siguiente token de ataque
    public void FlipAttackTokens()
    {
        if (attackCounter <= numberOfAttackTokens) 
        {
            attackTokens[attackCounter].GetComponent<Animator>().Play("TokenFlip");
            attackCounter++;
        }
        
    }

    //Flipea el token de movimiento
    public void FlipMovementToken()
    {
        movementTokens[0].GetComponent<Animator>().Play("TokenFlip");
    }

    //Resetea los tokens
    public void RefreshAllTokens()
    {
        for (int i = 0; i < attackTokens.Count; i++)
        {
            if (assignedBoss != null)
            {
                attackTokens[i].GetComponent<Animator>().Play("TokenReset");
            }
        }
        for (int i = 0; i < movementTokens.Count; i++)
        {
            if (assignedBoss != null)
            {
                movementTokens[i].GetComponent<Animator>().Play("TokenReset");
            }
        }

        attackCounter = 0;
    }

    public void RefreshSprites()
    {
        characterPortrait.sprite = assignedBoss.characterImage;
    }

    #endregion

}
