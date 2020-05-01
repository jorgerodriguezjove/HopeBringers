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

    UnitBase lastUndoPlayer;
    UnitBase NextCommandPlayer;

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
            LM.UIM.CheckActionsAvaliable();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
                Debug.Log("Command History count: " + commandHistory.Count);

                //Reduzco el counter
                counter--;

                //Guardo el player del siguiente comando
                lastUndoPlayer = commandHistory[counter].Player();

                //Deshago el comando
                commandHistory[counter].Undo();

                Debug.Log("Primer undo: " + commandHistory[counter]);

                ////Compruebo si la siguiente acción está realizada por el mismo personaje
                while (counter > 0)
                {
                    NextCommandPlayer = commandHistory[counter - 1].Player();

                    if (NextCommandPlayer == lastUndoPlayer && !commandHistory[counter - 1].CheckIfMoveCommand())
                    {
                        Debug.Log("Siguiente Undo: " + commandHistory[counter]);

                        //Repito las tres primeras lineas deshaciendo esta acción
                        counter--;
                        lastUndoPlayer = commandHistory[counter].Player();
                        commandHistory[counter].Undo();
                    }

                    //Si siguen quedando acciones pero ya no se cumple que coincida el personaje sago del loop.
                    else
                    {
                        break;
                    }
                }

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
