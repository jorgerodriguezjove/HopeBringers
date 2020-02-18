using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] GameObject  myBackgroundInCanvas;
    [SerializeField] Image       myPortraitInCanvas;
    [SerializeField] Text        myTextInCanvas;


    #endregion

    #region OPEN/CLOSE

    //Está separado el seteo de variables de abrir el dialogo porque puede ser el jugador el que empiece hablando.
    public void SetVariables(string textoDeEjemplo, Sprite portraitNPC)
    {
        myPortraitInCanvas.GetComponent<Image>().sprite = portraitNPC;
        //myTextInCanvas.GetComponent<Text>().text = textoDeEjemplo;
    }

    public void OpenDialogWindow()
    {
        myBackgroundInCanvas.SetActive(true);

        ///GameManager.Instance.MakeAllButtonsNOInteractable();
        ///

        //Setear input de mando para que pueda avanzar el texto.
    }

    public void CloseDialogWindow()
    {
        myBackgroundInCanvas.SetActive(false);
        ///GameManager.Instance.MakeAllButtonsYESInteractable();
        ///

        //No hace falta setear input, de eso ya se encarga el Gamemanager.
    }

    #endregion


}
