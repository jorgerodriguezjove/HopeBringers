using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnSummoner : EnemyUnit
{
    //El número máximo de unidades que se pueden invocar
    public int maxUnitsSummoned;
    //El número actual de unidades invocadas
    public int currentUnitsSummoned;

    //Prefab que el summoner va a invocar
    public GameObject skeletonPrefab;

    //Posición en vector para instanciar el prefab del enemigo.
    public Vector3 posToSpawn;
  
    //Número de bufo que aplica a las unidades
    public int enemyBuff;

    [SerializeField]
    private GameObject spawnFeedback;

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

        if (myTierLevel == TierLevel.Level2)
        {
            for (int i = 0; i < unitsInRange.Count; i++)
            {

                if (unitsInRange[i].GetComponent<EnemyUnit>())
                {
                    ApplyBuffOrDebuffDamage(unitsInRange[i], enemyBuff, 3);
                    
                }
            }
        }
        
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

        if (currentUnitsSummoned < maxUnitsSummoned)
        {
            DetermineSpawnPosition(true);

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

    public void DetermineSpawnPosition(bool _shouldSpawn)
    {
        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            if (myCurrentTile.neighbours[i].unitOnTile == null)
            {
                Debug.Log("MaxPasado2");
                posToSpawn = new Vector3(myCurrentTile.neighbours[i].transform.position.x, myCurrentTile.neighbours[i].transform.position.y, myCurrentTile.neighbours[i].transform.position.z);
                
                if (_shouldSpawn)
                {
                    Instantiate(skeletonPrefab, posToSpawn, myCurrentTile.neighbours[i].transform.rotation);
                    skeletonPrefab.GetComponent<EnSkeleton>().UpdateInformationAfterMovement(myCurrentTile.neighbours[i]);
                    skeletonPrefab.GetComponent<EnSkeleton>().whoIsMySummoner = this;
                    currentUnitsSummoned++;
                    break;
                }

                break;
            }
        }
    }

    public void HideShowFeedbackSpawnPosition(bool _shouldShow)
    {
        if (_shouldShow)
        {
            DetermineSpawnPosition(false);
            spawnFeedback.SetActive(true);
            posToSpawn += new Vector3(0, 0.5f, 0);
            spawnFeedback.transform.position = posToSpawn;
        }

        else
        {
            spawnFeedback.SetActive(false);
        }
       
    }
}

