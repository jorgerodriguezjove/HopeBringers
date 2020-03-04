using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;

public class InkManager : MonoBehaviour
{
    [SerializeField]
    List<NpcSCO> allCharactersInGame = new List<NpcSCO>();

    //Archivos de text
    [HideInInspector]
    public TextAsset inkJSONAsset;
    [HideInInspector]
    public Story story;

    //Caja dónde aparece el texto
    [SerializeField]
    private GameObject textBox;
    [SerializeField]
    private GameObject textBoxButtons;
    [SerializeField]
    private TextMeshProUGUI text; //Texto donde se escribe
    [SerializeField]
    private TextMeshProUGUI textPrefabTeleType;
    //private Text text;

    //Retratos de los personajes
    [SerializeField]
    private Image portraitLocation; //Hueco dónde aparece el sprite que toque.
    [SerializeField]
    private Sprite portraitPlayer;  //El retrato del protagonista se pone desde el editor.

    //Struct y Diccionario para tener diálogos con más de un NPC
    public struct valuePortraitAndColor
    {
        public Sprite portraitStruct;
        public Color  colorStruct;
    }
    valuePortraitAndColor structPortraitColor;
    Dictionary<string, valuePortraitAndColor> nameAndPortrait = new Dictionary<string, valuePortraitAndColor>();

    // UI Prefabs
    [SerializeField]
    private Button buttonPrefab; //Se asigna el prefab del botón desde el editor

    //Input delay diálogo
    //float waitSeconds = 1.2f;
    //float timerDialog;
    private bool waitingOnInput; //Bool que indica si el jugador puede avanzar en la línea

    //Flecha y x de avanzar o cerrar diálogo
    [SerializeField]
    GameObject triangleUI, xUI; //Se asigna los iconos desde el editor.

    //Referencia al botón que se crea cuando aparecen las elecciones. Está por la maldita corrutina.
    Button currentButtonCreated;
    Button firstButtonCreated; //Primer botón del diálogo para seleccionarlo.


    [SerializeField]
    Color newHighlightColorButton;

    public void StartStory()
    {
        HideChoices();
        story = new Story(inkJSONAsset.text);

        RefreshView();
    }

    void Update()
    {
        if (GameManager.Instance.dialogTime)
        {
            if (text.GetComponent<TeleType>().visibleCount >= text.GetComponent<TeleType>().totalVisibleCharacters)
            {
                if (story.canContinue || (!story.canContinue && story.currentChoices.Count > 0))
                {
                    triangleUI.SetActive(true);
                }
            }

            if (!waitingOnInput && Input.GetMouseButtonDown(0))
            {
                if (text.GetComponent<TeleType>().visibleCount >= text.GetComponent<TeleType>().totalVisibleCharacters)
                {
                    text.GetComponent<TeleType>().ResetVisibleLetters(); //Nada más cambiar de línea se ocultan las letras
                    triangleUI.SetActive(false);

                    if (story.canContinue)
                    {
                        RefreshView();

                        ///SoundManager.Instance.PlaySound(AppSounds.PASARDIALOG_SFX);
                        ///

                        if (!story.canContinue && story.currentChoices.Count <= 0)
                        {
                            //Cambia la flecha por una x en el último diálogo.
                            xUI.SetActive(true);
                            triangleUI.SetActive(false);
                        }

                    }
                    else if (story.currentChoices.Count > 0)
                    {
                        RefreshView();
                        triangleUI.SetActive(false);
                        xUI.SetActive(false);
                    }
                    else
                    {
                        //triangleUI.SetActive(true);
                        xUI.SetActive(false);
                        text.GetComponent<TeleType>().ResetVisibleLetters();

                        GameManager.Instance.EndDialog();

                        Destroy(text.gameObject);
                        text = Instantiate(textPrefabTeleType, textBox.transform);

                        //Vacio el diccionario
                        nameAndPortrait.Clear();


                        ///GameManager.Instance.playerScriptRef.currentMode = PlayerController.PlayerMode.DEFAULT;
                        ///HACER QUE SE CARGUE EL JUEGO UNA VEZ SE HA ACABADO EL DIÁLOGO O SI ES DIÁLOGO DE FINAL DE NIVEL QUE SE ACABE EL NIVEL
                    }
                }
                else  //Si todavía no han salido todas las letras al meter el input se muestran.
                {
                    text.GetComponent<TeleType>().ForceAllLettersToBeVisible();
                    if (story.canContinue || (!story.canContinue && story.currentChoices.Count > 0))
                    {
                        triangleUI.SetActive(true);
                    }

                }
            }
        }
    }

    public void RefreshView()
    {
        if (story.canContinue)
        {
            HideChoices();
            text.enabled = true;
            textBoxButtons.SetActive(false);
            text.gameObject.SetActive(true);
            
            string rawText = story.Continue().Trim();

            text.text = ParseContent(rawText);
        }
        else if (story.currentChoices.Count > 0)
        {
            HideChoices();
            text.enabled = false;
            textBoxButtons.SetActive(true);
            ChangeSpeaker("Player");
           
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                currentButtonCreated = CreateChoiceView(choice.text.Trim());

                // Tell the button what to do when we press it
                currentButtonCreated.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);

                });

                if (i == 0)
                {
                    firstButtonCreated = currentButtonCreated;
                }
                StartCoroutine("WaitbeforeButtonSelect");
                
            }
            waitingOnInput = true;
        }
    }

    public string ParseContent(string rawContent)
    {
        string subjectID = "";
        string content = "";
        if (!TrySplitContentBySearchString(rawContent, ": ", ref subjectID, ref content)) return rawContent;
        ChangeSpeaker(subjectID);
        return content;
       
    }

    public bool TrySplitContentBySearchString(string rawContent, string searchString, ref string left, ref string right)
    {
        int firstSpecialCharacterIndex = rawContent.IndexOf(searchString);
        if (firstSpecialCharacterIndex == -1) return false;

        left = rawContent.Substring(0, firstSpecialCharacterIndex).Trim();
        right = rawContent.Substring(firstSpecialCharacterIndex + searchString.Length, rawContent.Length - firstSpecialCharacterIndex - searchString.Length).Trim();
        return true;
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        Debug.Log("booton");
        story.ChooseChoiceIndex(choice.index);
        text.GetComponent<TeleType>().ResetVisibleLetters();
        RefreshView(); //Al no refrescarlo aqui, se refresca solo con el input y aparece la línea de diálogo al pulsar el botón.
        waitingOnInput = false;
    }

    //Se llama desde el GameManager y setea la variable con el sprite del NPC.
    public void InitNPCVariables(Sprite NPCPortrait, string NPCName, Color textColor)
    {
        structPortraitColor.portraitStruct = NPCPortrait;
        structPortraitColor.colorStruct = textColor;

        if (!nameAndPortrait.ContainsKey(NPCName))
        {
            nameAndPortrait.Add(NPCName, structPortraitColor);
        }
    }

    //Cambia el retrato en función de la persona que está hablando.
    private void ChangeSpeaker(string speaker)
    {
        portraitLocation.GetComponent<Image>().sprite = portraitPlayer;
        text.color = Color.white; //Prueba a hacer que el texto del player sea blanco

        for (int i = 0; i < nameAndPortrait.Count; i++)
        {
            if (nameAndPortrait.ContainsKey(speaker))
            {
                portraitLocation.GetComponent<Image>().sprite = nameAndPortrait[speaker].portraitStruct;
                text.color = nameAndPortrait[speaker].colorStruct;
            }
        }
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(textBoxButtons.transform, false);

        //Cambiar color del botón al instanciarlo
        var colors = choice.GetComponent<Button>().colors;
        colors.highlightedColor = newHighlightColorButton;
        choice.GetComponent<Button>().colors = colors;

        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.SetText(text);

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }
   
    //Antes erá el removeChildren. Lo he cambiado para que sólo borre los botones.
    //Importante que no borre el objeto de texto dónde aparece el texto.
    void HideChoices()
    {
        int childCount = textBoxButtons.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            if (textBoxButtons.transform.GetChild(i).gameObject.GetComponent<Button>())
            {
                GameObject.Destroy(textBoxButtons.transform.GetChild(i).gameObject);
            }

            if (textBoxButtons.transform.GetChild(i).gameObject.GetComponent<Text>())
            {
                textBoxButtons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WaitbeforeButtonSelect()
    {
        yield return new WaitForSeconds(1.5f);
        if (firstButtonCreated != null)
        {
            firstButtonCreated.Select();
        }
    }



  
}
