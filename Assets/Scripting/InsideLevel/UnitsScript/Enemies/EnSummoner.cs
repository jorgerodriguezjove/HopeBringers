using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnSummoner : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurrentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    //Lista que guarda los enmeigos y personajes que están dentro del rango de alerta del personaje (ya sea para comprobar personajes o alertar a enemigos)
    [HideInInspector]
    private List<UnitBase> unitsInRange = new List<UnitBase>();

    //El número máximo de unidades que se pueden invocar
    public int maxUnitsSummoned;
    //El número actual de unidades invocadas
    public int currentUnitsSummoned;

    //Prefab que el summoner va a invocar
    public GameObject skeletonPrefab;

    public Vector3 posToSpawn;

    public override void SearchingObjectivesToAttack()
    {
        myCurrentObjective = null;
        myCurrentObjectiveTile = null;
        pathToObjective.Clear();

        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

            //Comprobar las unidades que hay en mi rango de acción
            unitsInRange = LM.TM.GetAllUnitsInRangeWithoutPathfinding(rangeOfAction, GetComponent<UnitBase>());

            //Si hay personajes del jugador en mi rango de acción paso a attacking donde me alerto y hago mi accion
            for (int i = 0; i < unitsInRange.Count; i++)
            {
                if (unitsInRange[i].GetComponent<PlayerUnit>())
                {
                    myCurrentEnemyState = enemyState.Attacking;
                    return;
                }
            }

            //Si llega hasta aqui significa que no había personajes en rango y termina
            myCurrentEnemyState = enemyState.Ended;
         
    }

    public override void Attack()
    {
        hasAttacked = true;

        if (currentUnitsSummoned <= maxUnitsSummoned)
        {
            
            for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
            {
                if (myCurrentTile.neighbours[i].unitOnTile == null)
                {
                    Debug.Log("MaxPasado2");
                    posToSpawn = new Vector3(myCurrentTile.neighbours[i].transform.position.x, myCurrentTile.neighbours[i].transform.position.y , myCurrentTile.neighbours[i].transform.position.z );
                    
                    Instantiate(skeletonPrefab, posToSpawn, myCurrentTile.neighbours[i].transform.rotation);
                    skeletonPrefab.GetComponent<EnSkeleton>().UpdateInformationAfterMovement(myCurrentTile.neighbours[i]);
                    skeletonPrefab.GetComponent<EnSkeleton>().whoIsMySummoner = this;                 
                    currentUnitsSummoned++;
                    break;
                }
            }


            if (!hasAttacked)
            {
                myCurrentEnemyState = enemyState.Searching;
            }

            else
            {
                myCurrentEnemyState = enemyState.Ended;
            }
        }
      
        else
        {

            myCurrentEnemyState = enemyState.Ended;

        }

       
    }


}

