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

    //PREGUNTAR A MARIO
    //int damageBuff;
    //int damageDebuff;
    //int movementBuff;
    //int movementDebuff;


    public MoveCommand(UnitBase.FacingDirection newRotation, UnitBase.FacingDirection previousRotation,
                       IndividualTiles previousTile, IndividualTiles tileToMove, 
                       List<IndividualTiles> currentPath, UnitBase pj)
                       //int _currentDamageBuff, int _currentDamageDebuff, int _currentMovementBuff, int _currentMovementDebuff)
    {
        this.newRotation = newRotation;
        this.previousRotation = previousRotation;

        this.previousTile = previousTile;
        this.tileToMove = tileToMove;

        this.currentPath = currentPath;
        this.pj = pj;

        //this.damageBuff = _currentDamageBuff;
        //this.damageDebuff = _currentDamageDebuff;
        //this.movementBuff = _currentMovementBuff;
        //this.movementDebuff = _currentMovementDebuff;
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
    public UnitBase.FacingDirection objPreviousRotation;
    public UnitBase.FacingDirection pjPreviousRotation;
    
    public IndividualTiles objPreviousTile;
    public IndividualTiles pjPreviousTile;
    
    public int objPreviousHealth;
    public int pjPreviousHealth;
    
    public UnitBase pj;
    public UnitBase obj;
    
    public int pjArmor;
    public int objArmor;
    
    public bool pjIsStunned;
    public bool objIsStunned;
     
    public bool pjIsMarked;
    public bool objIsMarked;
    public int  pjnumberOfMarks;
    public int  objnumberOfMarks;

    public bool pjHasMoved;
    public bool objHasMoved;
    public bool pjHasAttacked;
    public bool objHasAttacked;

    //Bufos y debufos
    public int pj_damageBuffDebuff;
    public int pj_turnsDamageBuffDebuff;

    public int obj_damageBuffDebuff;
    public int obj_turnsDamageBuffDebuff;

    public int pj_movementBuffDebuff;
    public int pj_turnsMovementBuffDebuff;

    public int obj_movementBuffDebuff;
    public int obj_turnsMovementBuffDebuff;


    #region VARIABLES ESPECÍFICAS PJ

    //Ninja
    public List<UnitBase> unitsAttacked = new List<UnitBase>();
    public int ninjaExtraTurns;
    public int ninjaExtraJumps;
    public List<GameObject> smokeTiles = new List<GameObject>();
    public int ninjaBonusDamage;

    //Mago
    public GameObject oldDecoy;
    public GameObject newDecoy; //Esto da problemas seguro, mirar quizas en el execute que quite el actual en vez de guardarlo antes.
    public bool hasMovedWithDecoy; //Quizás es poner simplemente que se ha movido

    //Berserker
    public bool isInRage;
    public int rageTurnsLeft;

    //Samurai
    public UnitBase unitToParry;
    public int honor; //Assegurar que no jode nada del bufo al resetearlo

    //Druid
    GameObject healTileInstantiated;
    GameObject damageTileReplaced;

    //Valk
    public bool hasInterchanged; //No me acuerdo como iba el intercambio pero habrá que hacer algo como para el decoy.

    //thiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiis
    public bool isAlone; //ESTO EN REALIDAD IRIA EN EL MOVIMIENTO, COMPROBAR QUE FUNCIONA (HACER LO MISMO CON EL CABALLERO Y SI SE PONE A PROTEGER A LOS LADOS
    #endregion

    public AttackCommand(UnitBase.FacingDirection _enemypreviousRotation, UnitBase.FacingDirection _pjpreviousRotation,
                         IndividualTiles _enemyPreviousTile, IndividualTiles _pjPreviousTile, 
                         int _enemyPreviousHealth, int _pjPreviousHealth, 
                         UnitBase _pj, UnitBase _enemy,
                         int _pjArmor, int _objArmor,
                         bool _pjIsStunned, bool _objIsStunned,
                         bool _pjIsMarked, bool _objIsMarked, int _pjnumberOfMarks, int _objnumberOfMarks,
                         bool _pjHasMoved, bool _objHasMoved, bool _pjHasAttacked, bool _objHasAttacked,
                         int _pjDamageBuffDebuff, int _objDamageBuffDebuff, int _pjMovementBuffDebuff, int _objMovementBuffDebuff,
                         int _pjTurnsDamageBuffDebuff, int _objTurnsDamageBuffDebuff, int _pjTurnsMovementBuffDebuff, int _objTurnsMovementBuffDebuff)
    {
        objPreviousRotation = _enemypreviousRotation;
        pjPreviousRotation = _pjpreviousRotation;

        objPreviousTile = _enemyPreviousTile;
        pjPreviousTile = _pjPreviousTile;

        objPreviousHealth = _enemyPreviousHealth;
        pjPreviousHealth = _pjPreviousHealth;

        pj = _pj;
        obj = _enemy;

        pjArmor = _pjArmor;
        objArmor = _objArmor;

        pjIsStunned = _pjIsStunned;
        objIsStunned = _objIsStunned;

        pjIsMarked = _pjIsMarked;
        objIsMarked = _objIsMarked;
        pjnumberOfMarks = _pjnumberOfMarks;
        objnumberOfMarks = _objnumberOfMarks;

        pjHasMoved = _pjHasMoved;
        objHasMoved = _objHasMoved;
        pjHasAttacked = _pjHasAttacked;
        objHasAttacked = _objHasAttacked;

        pj_damageBuffDebuff = _pjDamageBuffDebuff;
        pj_turnsDamageBuffDebuff = _pjTurnsDamageBuffDebuff;

        obj_damageBuffDebuff = _objDamageBuffDebuff;
        obj_turnsDamageBuffDebuff = _objTurnsDamageBuffDebuff;

        pj_movementBuffDebuff = _pjMovementBuffDebuff;
        pj_turnsMovementBuffDebuff = _pjTurnsMovementBuffDebuff;

        obj_movementBuffDebuff = _objMovementBuffDebuff;
        obj_turnsMovementBuffDebuff = _objTurnsMovementBuffDebuff;

        #region Specific

        if (pj.GetComponent<Rogue>())
        {
            Rogue refPj = pj.GetComponent<Rogue>();

            unitsAttacked.Clear();

            for (int i = 0; i < refPj.unitsAttacked.Count; i++)
            {
                unitsAttacked.Add(refPj.unitsAttacked[i]);
            }

            ninjaExtraTurns = refPj.extraTurnCount;
            ninjaExtraJumps = refPj.unitsCanJump;

            smokeTiles.Clear();
            for (int i = 0; i < refPj.realBombsSpawned.Count; i++)
            {
                smokeTiles.Add(refPj.realBombsSpawned[i]);
            }

            ninjaBonusDamage = refPj.baseDamage;
        }

        else if (pj.GetComponent<Mage>())
        {
            Mage refPj = pj.GetComponent<Mage>();

            if (refPj.myDecoys.Count >0)
            {
                oldDecoy = refPj.myDecoys[0];
            }
            
            //newDecoy; //Esto da problemas seguro, mirar quizas en el execute que quite el actual en vez de guardarlo antes.


            //hasMovedWithDecoy = refPj.hasMoved; //Quizás es poner simplemente si ha movido
        }

        else if (pj.GetComponent<Samurai>())
        {
            Samurai refPj = pj.GetComponent<Samurai>();

            unitToParry = refPj.unitToParry; 
            honor = pj.GetComponent<Samurai>().currentHonor; 
        }

        else if (pj.GetComponent<Druid>())
        {
            Druid refPj = pj.GetComponent<Druid>();

            //healTileInstantiated = ; Poner variables en el druida que guarden estos tiles.
            //damageTileReplaced =;

          //El druida parece que afecta a movementUDs ¿Esto esta bien? ¿o debería ser un bufo?

        }

        else if (pj.GetComponent<Valkyrie>())
        {
            Valkyrie refPj = pj.GetComponent<Valkyrie>();

            //hasInterchanged = refPj.hasMoved; Igual que el mago con el decoy.
        }

        if (obj.GetComponent<Berserker>())
        {
            Berserker refPj = obj.GetComponent<Berserker>();

            isInRage = refPj.isInRage;
            rageTurnsLeft = refPj.turnsLeftToRageOff;
        }

        #endregion
    }

    public void Execute()
    {
        //pj.GetComponent<PlayerUnit>().Attack(obj);
    }

    public void Undo()
    {
        obj.UndoAttack(this, false);   
        pj.UndoAttack(this, true);

        //En las funciones que he llamado falta setear los bufos de daño y movimiento
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



