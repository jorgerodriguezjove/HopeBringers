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

    //PREGUNTAR A MARIO
    //public int pj_damageBuff;
    //public int pj_damageDebuff;
    //public int pj_movementBuff;
    //public int pj_movementDebuff;

    //public int obj_damageBuff;
    //public int obj_damageDebuff;
    //public int obj_movementBuff;
    //public int obj_movementDebuff;

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
    public bool isInParry;
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
                         bool _pjIsMarked, bool _objIsMarked, int _pjnumberOfMarks, int _objnumberOfMarks)
                         //,int pj_damageBuff, int pj_damageDebuff, int pj_movementBuff, int pj_movementDebuff,
                         //int obj_damageBuff, int obj_damageDebuff, int obj_movementBuff, int obj_movementDebuff)
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

            //TODO LO QUE FALTA AQUI HAY QUE PONERLO TAMBIEN EN LA FUNCIÓN UNDOATTACK DE CADA PERSONAJE

            //Añadir en ninja referencia a tiles de humo que instancia

            //for (int i = 0; i < refPj.smoke; i++)
            //{
            //    smokeTiles.Add(refPj.smoke[0]);
            //}
            
            //Poner aqui daño que gana ninja al matar enemigos

            //ninjaBonusDamage = refPj. ;
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

        else if (pj.GetComponent<Berserker>())
        {
            Berserker refPj = pj.GetComponent<Berserker>();

            isInRage = refPj.isInRage;
            rageTurnsLeft = refPj.turnsLeftToRageOff;
        }

        else if (pj.GetComponent<Samurai>())
        {
            Samurai refPj = pj.GetComponent<Samurai>();

            //isInParry = refPj.; //Quitar el aprry
            //honor = LM.HONOR; Hay que hacer que el samurai también guarde el honor o algo porque no puedo acceder a LM
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

        #endregion
    }

    public void Execute()
    {
        //pj.GetComponent<PlayerUnit>().Attack(obj);
    }

    public void Undo()
    {
        obj.UndoAttack(this);   
        pj.UndoAttack(this);

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



