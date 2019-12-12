using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : PlayerUnit
{
    #region VARIABLES

    [Header("SPECIAL VARIABLES FOR CHARACTER")]

    [SerializeField]
    protected GameObject chargingParticle;

    [SerializeField]
    protected GameObject mageDecoy;




    //Lista con decoys que tiene este mago.
    [SerializeField]
    private List<GameObject> myDecoys;

    //Número máximo de decoys que se pueden instanciar
    [SerializeField]
    private int maxDecoys;




    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        Instantiate(chargingParticle, gameObject.transform.position, chargingParticle.transform.rotation);

        Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

        //Hago daño
        DoDamage(unitToAttack);

        SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);

        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
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

        if (myDecoys.Count < maxDecoys)
        {
           
            //Instancio el decoy
            Instantiate(mageDecoy, transform.position, transform.rotation);
            myDecoys.Add(mageDecoy);
        }
        else
        {
            //myDecoys[0].Destroy(gameObject);
            myDecoys.Remove(myDecoys[0]);

            //Instancio el decoy
            Instantiate(mageDecoy, transform.position, transform.rotation);
            myDecoys.Add(mageDecoy);
        }
        
       
        StartCoroutine("MovingUnitAnimation");

        

        UpdateInformationAfterMovement(tileToMove);
    }
    #endregion
}
