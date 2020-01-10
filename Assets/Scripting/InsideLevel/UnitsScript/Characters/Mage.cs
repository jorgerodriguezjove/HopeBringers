﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : PlayerUnit
{
    #region VARIABLES

    [Header("SPECIAL VARIABLES FOR CHARACTER")]

    [SerializeField]
    protected GameObject chargingParticle;

    //Prefab del mage decoy
    [SerializeField]
    protected GameObject mageDecoyRefAsset;

    //Lista con decoys que tiene este mago.
    [SerializeField]
    private List<GameObject> myDecoys = new List<GameObject>();

    //Número máximo de decoys que se pueden instanciar
    [SerializeField]
    private int maxDecoys;


    [Header("MEJORAS DE PERSONAJE")]

    public bool crossAreaAttack;

    public bool lightningChain;
    public int timeElectricityAttackExpands;
    [HideInInspector]
    public List<UnitBase> unitsAttacked;

    //Este bool sirve para decidir si el ataque en concreto hace daño por la espalda o no
    [HideInInspector]
    public bool backDamageOff;



    #endregion

    public void SetSpecificStats(bool _lightningChain1, bool _crossAreaAttack1)
    {
        lightningChain = _lightningChain1;
        crossAreaAttack = _crossAreaAttack1;
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        Instantiate(chargingParticle, gameObject.transform.position, chargingParticle.transform.rotation);

        Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

        if (crossAreaAttack)
        {

            //Animación de ataque 
            //HAY QUE HACER UNA PARA EL ATAQUE EN CRUZ O PARTÍCULAS
            //myAnimator.SetTrigger("Attack");

            backDamageOff = true;
            //Hago daño
            DoDamage(unitToAttack);

            //Hago daño a las unidades adyacentes
            for (int i = 0; i < unitToAttack.myCurrentTile.neighbours.Count; ++i)
            {
                if (unitToAttack.myCurrentTile.neighbours[i].unitOnTile != null)
                {
                    DoDamage(unitToAttack.myCurrentTile.neighbours[i].unitOnTile);
                }


            }

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
        else if (lightningChain)
        {
            backDamageOff = true;
            //Hago daño
            DoDamage(unitToAttack);
            unitsAttacked.Add(unitToAttack);

           
            for (int j = 0; j < unitsAttacked.Count; j++)
            {
                
                if (timeElectricityAttackExpands > 0)
                {
                    timeElectricityAttackExpands--;
                    for (int k = 0; k < unitsAttacked[j].myCurrentTile.neighbours.Count; ++k)
                    {
                       
                        if (unitsAttacked[j].myCurrentTile.neighbours[k].unitOnTile != null && !unitsAttacked.Contains(unitsAttacked[j].myCurrentTile.neighbours[k].unitOnTile))
                        {

                            DoDamage(unitsAttacked[j].myCurrentTile.neighbours[k].unitOnTile);
                            unitsAttacked.Add(unitsAttacked[j].myCurrentTile.neighbours[k].unitOnTile);
                        }
                    }

                   
                }

            }
               
                
            
            unitsAttacked.Clear();

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
        else
        {
            //Hago daño
            DoDamage(unitToAttack);

            SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

    }
        
    //Override especial del mago para que no instancie la partícula de ataque
    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        if (!backDamageOff)
        {
            CalculateDamage(unitToDealDamage);
        }
       
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
    }

    #region MOVEMENT

    //El LevelManager avisa a la unidad de que debe moverse.
    //Esta función tiene que ser override para que el mago pueda instanciar decoys.
    public override void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        //Compruebo la dirección en la que se mueve para girar a la unidad
        //   CheckTileDirection(tileToMove);
        hasMoved = true;
        movementTokenInGame.SetActive(false);
        //Refresco los tokens para reflejar el movimiento
        UIM.RefreshTokens();
        myCurrentPath = pathReceived;

       if (tileToMove != LM.selectedCharacter.myCurrentTile)
        {
            //Compruebo si tengo que instanciar decoy
            CheckDecoy();

        }



        StartCoroutine("MovingUnitAnimation");

        UpdateInformationAfterMovement(tileToMove);
    }

    public void CheckDecoy()
    {
        if (myDecoys.Count < maxDecoys)
        {
            //Instancio el decoy
            InstantiateDecoy();
        }

        else
        {
            //Destruyo al decoy anterior
            GameObject decoyToDestroy = myDecoys[0];
            Destroy(decoyToDestroy);
            myDecoys.Remove(decoyToDestroy);

            //Instancio el decoy
            InstantiateDecoy();
        }
    }

    public void InstantiateDecoy()
    {
        GameObject decoyToInstantiate = Instantiate(mageDecoyRefAsset, transform.position, transform.rotation);
        myDecoys.Add(decoyToInstantiate);
    }

    #endregion

    #region CHECKS
    //Hago override a esta función para que pueda atravesar unidades al atacar.
    public override void CheckUnitsInRangeToAttack()
    {
        currentUnitsAvailableToAttack.Clear();

        if (currentFacingDirection == FacingDirection.North)
        {
            if (attackRange <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South)
        {
            if (attackRange <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East)
        {
            if (attackRange <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                   
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West)
        {
            if (attackRange <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    
                }
            }

        }

        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttacked();
        }


    }
    #endregion
}
