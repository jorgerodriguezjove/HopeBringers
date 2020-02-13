using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    static Queue<ICommand> commandBuffer;

    static List<ICommand> commandHistory;
    static int counter;

    [Header("REFERENCES")]
    private LevelManager LM;

    //Unidad que estaba seleccionada cuando le he dado a undo move
    PlayerUnit playerSelectedWhileClickingUndo;

    private void Awake()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();

        LM = FindObjectOfType<LevelManager>();
    }

    public static void AddCommand(ICommand _command)
    {
        while (commandHistory.Count > counter)
        {
            commandHistory.RemoveAt(counter);
        }

        commandBuffer.Enqueue(_command);
    }

    private void Update()
    {
        if (commandBuffer.Count > 0)
        {
            ICommand c = commandBuffer.Dequeue();
            c.Execute();

            commandHistory.Add(c);
            counter++;
            Debug.Log("Command History length: " + commandHistory.Count);
        }
    }

    public void UndoAction()
    {
        playerSelectedWhileClickingUndo = LM.selectedCharacter;

       
        if (playerSelectedWhileClickingUndo != null && !playerSelectedWhileClickingUndo.isMovingorRotating || playerSelectedWhileClickingUndo == null)
        {
            if (counter > 0)
            {
                counter--;
                commandHistory[counter].Undo();
                LM.DeSelectUnit();
            }
        }

        playerSelectedWhileClickingUndo = null;
    }

    //Resetear el undo
    public void ResetCommandList()
    {
        commandBuffer.Clear();
        commandHistory.Clear();
        counter = 0;
    }

}
