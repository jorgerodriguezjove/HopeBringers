using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands
{ 
    
}

public class MoveCommand : ICommand
{
    UnitBase.FacingDirection newRotation;
    UnitBase.FacingDirection previousRotation;
    IndividualTiles previousTile;
    IndividualTiles tileToMove;
    List<IndividualTiles> currentPath;
    UnitBase pj;

    public MoveCommand(UnitBase.FacingDirection newRotation, UnitBase.FacingDirection previousRotation, IndividualTiles previousTile, IndividualTiles tileToMove, List<IndividualTiles> currentPath, UnitBase pj)
    {
        this.newRotation = newRotation;
        this.previousRotation = previousRotation;
        this.previousTile = previousTile;
        this.tileToMove = tileToMove;
        this.currentPath = currentPath;
        this.pj = pj;
    }

    public void Execute()
    {
        //En level manager no mueve al personaje si no que llama a esto
        //Mover el personaje (posición inicial y PJ) aqui basicamente llamar a función de movimiento una vez level manager ha dado visto bueno. 
        //Rotación, color y token más adelante

        GameObject.FindObjectOfType<LevelManager>().selectedCharacter.RotateUnitFromButton(newRotation, tileToMove, currentPath);
    }

    public void Undo()
    {
        pj.GetComponent<PlayerUnit>().UndoMove(previousTile, previousRotation);
    }
}
