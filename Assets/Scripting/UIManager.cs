using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //REFERENCIAS
    private LevelManager LM;

    [SerializeField]
    private Button endTurnButton;

    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
    }

    #region END_TURN

    //Se llama desde el botón de finalizar turno
    public void EndTurn()
    {
        LM.ChangePhase();
    }

    //Función que activa o desactiva el botón de pasar turno en función de si es la fase del player o del enemigo
    public void ActivateDeActivateEndButton()
    {
        endTurnButton.interactable = !endTurnButton.interactable;
    }

    #endregion


    //Se llama desde el botón de finalizar turno
    public void UndoMove()
    {
        LM.UndoMove();
    }



}
