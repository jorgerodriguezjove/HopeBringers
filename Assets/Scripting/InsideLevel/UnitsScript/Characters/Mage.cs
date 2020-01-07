using System.Collections;
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

    public bool crossAttack;

    public bool electricityAttack;
    public int timeElectricityAttackExpands;
    [HideInInspector]
    public List<UnitBase> unitsAttacked;




    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        Instantiate(chargingParticle, gameObject.transform.position, chargingParticle.transform.rotation);

        Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

        if (crossAttack)
        {

            //Animación de ataque 
            //HAY QUE HACER UNA PARA EL ATAQUE EN CRUZ O PARTÍCULAS
            //myAnimator.SetTrigger("Attack");

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
        else if (electricityAttack)
        {
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
        CalculateDamage(unitToDealDamage);
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
}
