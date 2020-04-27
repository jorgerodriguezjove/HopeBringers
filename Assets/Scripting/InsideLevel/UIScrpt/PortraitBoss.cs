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

        if (FindObjectOfType<MechaBoss>())
        {
            assignedBoss = FindObjectOfType<MechaBoss>();
        }

        else if (FindObjectOfType<DarkLord>())
        {
            assignedBoss = FindObjectOfType<DarkLord>();
        }

        else if (FindObjectOfType<BossMultTile>())
        {
            assignedBoss = FindObjectOfType<BossMultTile>();
        }

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

    #endregion

    //A las funciones de hover y click se llama gracias al event triggger que tienen en el componente (mirar editor vamos)
    #region INTERACTION

    //public void AssignClickerPlayer()
    //{
    //    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
    //    {
    //        UIM.PortraitCharacterSelect(assignedPlayer);
    //        LM.camRef.FocusCameraOnCharacter(assignedPlayer.gameObject);
    //        isClicked = true;
    //    }
    //}

    //public void Highlight()
    //{
    //    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
    //    {
    //        UIM.HighlightCharacter(assignedPlayer);

    //        if (LM.selectedCharacter == null && LM.selectedEnemy == null)
    //        {
    //            ShowCharacterImageFromPortrait();
    //            UIM.LM.ShowUnitHover(assignedPlayer.movementUds, assignedPlayer);

    //        }
    //        selectedPanel.SetActive(true);
    //    }
    //}

    //public void Unhighlight()
    //{
    //    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
    //    {
    //        if (LM.selectedCharacter == null && LM.selectedEnemy == null)
    //        {

    //            UIM.LM.HideUnitHover(assignedPlayer);
    //        }

    //        if (LM.selectedCharacter != assignedPlayer)
    //        {
    //            UIM.UnHighlightCharacter(assignedPlayer);
    //        }


    //        if (isClicked == false)
    //        {


    //            if (LM.selectedCharacter != assignedPlayer)
    //            {
    //                assignedPlayer.ResetColor();
    //                selectedPanel.SetActive(false);
    //            }

    //        }
    //    }
    //}


    //public void HighlightPortrait()
    //{
    //    //GetComponent<Image>().sprite = selectedImage;
    //    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
    //    {
    //        selectedPanel.SetActive(true);
    //    }
    //}
    //public void UnHighlightPortrait()
    //{
    //    //GetComponent<Image>().sprite = initImage;

    //    //if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
    //    //{
    //    //    selectedPanel.SetActive(false);
    //    //}
    //    if (LM.selectedCharacter != this)
    //    {
    //        selectedPanel.SetActive(false);
    //    }

    //}

    //public void ShowCharacterImageFromPortrait()
    //{
    //    if (UIM.LM.selectedCharacter == null)
    //    {
    //        UIM.ShowUnitInfo(assignedPlayer.unitGeneralInfo, assignedPlayer);
    //    }
    //}

    //public void OnMouseEnter()
    //{
    //    Highlight();

    //}

    //private void OnMouseExit()
    //{
    //    Unhighlight();
    //}

    //private void OnMouseDown()
    //{
    //    AssignClickerPlayer();
    //}

    #endregion

    #region REFRESH

    //Función que inicializa la vida del boss
    public void InitializeHealth()
    {
        for (int i = 0; i < assignedBoss.maxHealth; i++)
        {
            GameObject lifeTok = Instantiate(lifeTokenPref, lifeContainer.transform);
            lifeTokensList.Add(lifeTok);
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

    ////Función que se encarga de actualizar el estado de los tokens de movimiento y ataque.
    //public void RefreshTokens()
    //{
    //    for (int i = 0; i < attackTokens.Count; i++)
    //    {
    //        //Añado este if para que compruebe si es un decoy o no.
    //        if (assignedBoss != null)
    //        {
    //            //attackTokens[i].SetActive(!assignedPlayer.hasAttacked);
    //            if (!assignedBoss.hasAttacked)
    //            {
    //                attackTokens[i].GetComponent<Animator>().Play("TokenReset");
    //            }
    //            else
    //            {
    //                attackTokens[i].GetComponent<Animator>().Play("TokenFlip");
    //            }
    //        }
    //    }
    //    for (int i = 0; i < movementTokens.Count; i++)
    //    {
    //        //Añado este if para que compruebe si es un decoy o no.
    //        if (assignedBoss != null)
    //        {
    //            //movementTokens[i].SetActive(!assignedPlayer.hasMoved);
    //            if (!assignedBoss.hasMoved)
    //            {
    //                movementTokens[i].GetComponent<Animator>().Play("TokenReset");
    //            }
    //            else
    //            {
    //                movementTokens[i].GetComponent<Animator>().Play("TokenFlip");
    //            }
    //        }
    //    }
    //}

    public void RefreshSprites()
    {
        characterPortrait.sprite = assignedBoss.characterImage;
    }

    #endregion

}
