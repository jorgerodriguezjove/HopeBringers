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
        pj.UndoMove(previousTile, previousRotation, true);
    }

    public UnitBase Player()
    {
        return pj;
    }

    public bool CheckIfMoveCommand()
    {
        return true;
    }
}

public class AttackCommand : ICommand
{
    UnitBase.FacingDirection enemyPreviousRotation;
    UnitBase.FacingDirection pjPreviousRotation;
    IndividualTiles enemyPreviousTile;
    IndividualTiles pjPreviousTile;
    int enemyPreviousHealth;
    int pjPreviousHealth;
    UnitBase pj;
    UnitBase enemy;

    public AttackCommand(UnitBase.FacingDirection _enemypreviousRotation, UnitBase.FacingDirection _pjpreviousRotation, IndividualTiles _enemyPreviousTile, IndividualTiles _pjPreviousTile, int _enemyPreviousHealth, int _pjPreviousHealth, UnitBase _pj, UnitBase _enemy)
    {
        enemyPreviousRotation = _enemypreviousRotation;
        pjPreviousRotation = _pjpreviousRotation;
        enemyPreviousTile = _enemyPreviousTile;
        pjPreviousTile = _pjPreviousTile;
        enemyPreviousHealth = _enemyPreviousHealth;
        pjPreviousHealth = _pjPreviousHealth;
        pj = _pj;
        enemy = _enemy;
    }

    public void Execute()
    {
        pj.GetComponent<PlayerUnit>().Attack(enemy);
    }

    public void Undo()
    {
        //El enemigo usa tanto undo attack como undomove.
        //Restaurar vida del enemigo
        enemy.UndoAttack(enemyPreviousHealth);
        //Mover y rotar al enemigo usando el undo movement
        enemy.UndoMove(enemyPreviousTile, enemyPreviousRotation, false);

        //El Pj sin embargo no puede usar el undoMove porque si no se resetea la capacidad de moverse y el decoy del mago.
        //El UndoAttack a parte de la vida, en el caso del player tambíén se encarga de moverlo

        pj.UndoMove(pjPreviousTile, pjPreviousRotation, false);
        pj.UndoAttack(pjPreviousHealth);
    }

    public UnitBase Player()
    {
        return pj;
    }

    public bool CheckIfMoveCommand()
    {
        return false;
    }

}
