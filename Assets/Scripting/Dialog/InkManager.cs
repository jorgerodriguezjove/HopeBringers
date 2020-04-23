using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;

public class InkManager : MonoBehaviour
{
    public TextAsset debugDialog;

    [Header("PERSONAJES")]
    [SerializeField]
    List<NpcSCO> allCharactersInGame = new List<NpcSCO>();

    //La 0 es fuera de escena y 1,2,3,4 son las que se ven en cámara
    [SerializeField]
    public List<Transform> allCharacterPositionsInDialog = new List<Transform>();

    //Archivos de text
    [HideInInspector]
    public TextAsset inkJSONAsset;
    [HideInInspector]
    public Story story;

    [Header("REFERENCIAS")]
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
    public struct structCharacters
    {
        public Color                colorStruct;
        public CharacterInDialog    characterInstantiated;
    }
    structCharacters currStruct;
    Dictionary<string, structCharacters> nameAndModelDict = new Dictionary<string, structCharacters>();
    //Esta lista simplemente la uso para mover a todos los personajes al acabar el dialogo
    List<CharacterInDialog> listWithOnlyCharacters = new List<CharacterInDialog>();

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
        //Inicializo todos los structs con el retrato, color y nombre del personaje
        InitNPCVariables();

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

                        for (int i = 0; i < listWithOnlyCharacters.Count; i++)
                        {
                            listWithOnlyCharacters[i].DeactivateCharacters();

                        }

                        GameManager.Instance.EndDialog();

                        Destroy(text.gameObject);
                        text = Instantiate(textPrefabTeleType, textBox.transform);

                        //Vacio el diccionario
                        nameAndModelDict.Clear();


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
        //Se crean dos variables string que van a guardar el texto que queda a la izquierda y a la derecha después de la división
        string subjectID = "";
        string content = "";
        
        //Compruebo si es un cambio de personaje
        if (TrySplitContentBySearchString(rawContent, "%", ref subjectID, ref content))
        {
            Debug.Log(subjectID);
            Debug.Log(content);
            Debug.Log(int.Parse(content));

            nameAndModelDict[subjectID].characterInstantiated.MoveToPosition(int.Parse(content));

            //Esto habrá que hacer que en vez de devolver "" que salte a la siguiente línea
            //O quizas está bien para que se vea como entra/sale el personaje
            return "";
        }

        //Esto es lo que permite saber quien esta hablando al poner "nombre: " en el inkle
        else
        {
            //Se llama a la funcion que divide el contenido. Si no encuentra ninguna división devuelve el texto sin cambios
            if (!TrySplitContentBySearchString(rawContent, ": ", ref subjectID, ref content)) return rawContent;
            Debug.Log(subjectID);
            ChangeSpeaker(subjectID);
            return content;
        }
    }

    //Divide una línea de texto en 2. Recibe el texto normal, lo que tiene que buscar como divisor (por ej ": ") y una referencia a 2 variables string para guardar
    //el contenido que queda a la izquierda y otra a la derecha
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
    public void InitNPCVariables()
    {
        for (int i = 0; i < allCharactersInGame.Count; i++)
        {
            //Instancio el personaje, le añado el componentte y le seteo las posiciones
            GameObject pjInst = Instantiate(allCharactersInGame[i].characterModel, allCharacterPositionsInDialog[0]);
            pjInst.AddComponent<CharacterInDialog>();
            pjInst.GetComponent<CharacterInDialog>().positionsToMove = allCharacterPositionsInDialog;

            //Seteo los valores del struct y de la lista solo con los charactersDialog
            currStruct.characterInstantiated = pjInst.GetComponent <CharacterInDialog>();
            listWithOnlyCharacters.Add(pjInst.GetComponent<CharacterInDialog>());
            currStruct.colorStruct = allCharactersInGame[i].colorTextNPC;

            //Si no está repetido en el diccionario le añado
            if (!nameAndModelDict.ContainsKey(allCharactersInGame[i].nameNPC))
            {
                nameAndModelDict.Add(allCharactersInGame[i].nameNPC, currStruct);
            }
        }
    }

    string previousSpeaker;

    //Cambia el retrato en función de la persona que está hablando.
    private void ChangeSpeaker(string speaker)
    {
        portraitLocation.GetComponent<Image>().sprite = portraitPlayer;
        text.color = Color.white; //Prueba a hacer que el texto del player sea blanco

        for (int i = 0; i < nameAndModelDict.Count; i++)
        {
            if (nameAndModelDict.ContainsKey(speaker))
            {
                if (previousSpeaker != null)
                {
                    nameAndModelDict[previousSpeaker].characterInstantiated.DesHighlightSpeaker();
                }

                //Encontrar clase modelo en escena y decirle que se resalte y que se oculte el anterior
                Debug.Log(speaker);
                Debug.Log(nameAndModelDict[speaker]);
                Debug.Log(nameAndModelDict[speaker].characterInstantiated);

                nameAndModelDict[speaker].characterInstantiated.HighlightSpeaker();
                text.color = nameAndModelDict[speaker].colorStruct;

                previousSpeaker = speaker;
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
