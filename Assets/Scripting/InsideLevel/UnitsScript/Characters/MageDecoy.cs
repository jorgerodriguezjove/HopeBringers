using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageDecoy : Mage
{

  
    #region INIT

    private void Awake()
    {
        FindAndSetFirstTile();
        myCurrentTile.unitOnTile = this;
        myCurrentTile.WarnInmediateNeighbours();

        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = FindObjectOfType<LevelManager>();
        //LM.characthersOnTheBoard.Add(this);
        //Referencia al UIM 
        UIM = FindObjectOfType<UIManager>();
        //Aviso al tile en el que empiezo que soy su unidad.


        ///SETEAR EL TILE AL COLOCAR A LA UNIDAD EN UNO DE LOS TILES DISPONIBLES PARA COLOCARLAS
        // myCurrentTile.unitOnTile = this;
        // myCurrentTile.WarnInmediateNeighbours();

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        currentHealth = maxHealth;
    }

    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    //Es virtual para el decoy del mago.
    protected override void OnMouseDown()
    {
       
    }

    //Es virtual para el decoy del mago.
    protected override void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy == null)
            {
                if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                }
                if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                   
                }

                if (!hasAttacked)
                {
                  
                    SelectedColor();
                    LM.ShowUnitHover(movementUds, this);
                }
            }
        }
    }

    //Es virtual para el decoy del mago.
    protected override void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (LM.selectedCharacter == null)
        {
            LM.HideUnitHover(this);          
            ResetColor();
        }
        else if (LM.selectedCharacter == this)
        {
            return;
        }
        else if (LM.selectedCharacter != this.gameObject)
        {
            LM.HideUnitHover(this);        
            ResetColor();

        }



    }

    #endregion

}
