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
        LM.characthersOnTheBoard.Add(this);
        //Referencia al UIM 
        UIM = FindObjectOfType<UIManager>();
        //Aviso al tile en el que empiezo que soy su unidad.


        ///SETEAR EL TILE AL COLOCAR A LA UNIDAD EN UNO DE LOS TILES DISPONIBLES PARA COLOCARLAS
        // myCurrentTile.unitOnTile = this;
        // myCurrentTile.WarnInmediateNeighbours();

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        movementParticle.SetActive(false);

        currentHealth = maxHealth;

    }

    #endregion
}
