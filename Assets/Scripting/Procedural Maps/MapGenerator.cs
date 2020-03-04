using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region VARIABLES

    [Header("ENEMIES")]

    //Número mínimo de enemigos en el nivel
    [SerializeField]
    private int minEnemyNumber;

    //Lista con los spawns enemigos
    List<RandomTile> enemySpawns = new List<RandomTile>();

    //Lista que aparece en el editor con la configuración de los enemigos
    [SerializeField]
    private List<EnemyConfig> enemyConfiguration = new List<EnemyConfig>();

    //SET PROBABILITY

    //Lista con los valores entre 1 y 100 donde se separa cada enemigo. guarda los resultados de hacer 100 -probabilidad, probabilidad del anterior - probabilidad...
    List<int> enemyPercentageDivisors = new List<int>();
    //Int temporal donde guardo los resultados antes de meterlos en la lista
    int tempInt;

    //Guardo el número de enemigos que voy a crear
    int numberOfEnemiesInThisInstance;

    //CREATE ENEMIES

    //Valor aleatorio que se compara con el rango de cada enemigo para ver cual instancia
    int enemyRandomValue;

    //Valor aleatorio que determina en que spawn se instancia el enemigo que se ha decidido
    int randomSpawnEnemy;

    //Guardo la probabilidad del enemigo que ha sido borrado por llegar al máximo para repartirla entre el resto de enemigos restantes
    int probabilityOfEnemyDeleted;

    //ENEMIGOS
    [System.Serializable]
    public class EnemyConfig
    {
        public GameObject enemyPref;
        [Range(1, 100)]
        public int spawnProbability;
        public int maxEnemiesOfThisType;

        //Enemigos instanciados de este tipo
        [HideInInspector]
        public int currentEnemiesOfThisType;
    }

    [Header("OBSTACLES")]

    //Probabilidad de que aparezca un obstáculo en cada spawn.
    [Range(1,100)]
    [SerializeField]
    private int eachTileObstacleProbability;

    //Probabilidad de que dos tiles juntos decidan juntarse para crear un obstáculo de 2.
    [Range(1,100)]
    [SerializeField]
    private int probabilityOf2tilesObstacle;

    //Lista que aparece en el editor con la configuración de los enemigos
    [SerializeField]
    private List<ObstacleConfig> oneTileObstacleConfig = new List<ObstacleConfig>(1);

    //Lista que aparece en el editor con la configuración de los enemigos
    [SerializeField]
    private List<ObstacleConfig> twoTileObstacleConfig = new List<ObstacleConfig>(2);

    ////Lista que aparece en el editor con la configuración de los enemigos
    //[SerializeField]
    //private List<ObstacleConfig> threeTileObstacleConfig = new List<ObstacleConfig>(3);

    //Lista con los spawns enemigos
    List<RandomTile> obstacleSpawns = new List<RandomTile>();

    [System.Serializable]
    public class ObstacleConfig
    {
        public GameObject obstacleAsset;
        private int obstacleTileSize = 1;

        public ObstacleConfig(int obstacleTileSize)
        {
            this.obstacleTileSize = obstacleTileSize;
        }

        //[Range(1, 100)]
        //public int spawnProbability;
        //public int maxEnemiesOfThisType;

        ////Enemigos instanciados de este tipo
        //[HideInInspector]
        //public int currentEnemiesOfThisType;
    }

    #endregion

    //"Start" que se llama desde el awake del level manager
    public void Init()
    {
        LookAndSortSpots();
        //SetEnemyProbablity();
        //CreateEnemies();

        CreateObstacle();
    }

    public void LookAndSortSpots()
    {
        foreach (RandomTile spot in FindObjectsOfType<RandomTile>())
        {
            if (spot.GetComponent<RandomTile>().thisTileType == RandomTile.tileType.Enemy)
            {
                enemySpawns.Add(spot);
            }

            if (spot.GetComponent<RandomTile>().thisTileType == RandomTile.tileType.Obstacle)
            {
                obstacleSpawns.Add(spot);
            }

            //if (spot.GetComponent<RandomTile>().thisTileType == RandomTile.tileType.Height)
            //{

            //}
        }
    }


    #region ENEMIGOS

    GameObject enemyInstantiated;

    //Convierto la probabilidad de los enemigos en un rango para poder usar el random al crearlos.
    public void SetEnemyProbablity()
    {
        //Importante limpiar la lista al principio
        enemyPercentageDivisors.Clear();

        for (int i = 0; i < enemyConfiguration.Count; i++)
        {
            if (i == 0)
            {
                tempInt = 100 - enemyConfiguration[i].spawnProbability;
            }

            else
            {
                tempInt -= enemyConfiguration[i].spawnProbability;
            }

            enemyPercentageDivisors.Add(tempInt);
        }

        numberOfEnemiesInThisInstance = Random.Range(minEnemyNumber, enemySpawns.Count+1);
    }

    //Creo los enemigos
    public void CreateEnemies()
    {
        for (int i = 0; i <= numberOfEnemiesInThisInstance; i++)
        {
            enemyRandomValue = Random.Range(1, 100);

            for (int j = 0; j < enemyConfiguration.Count; j++)
            {
                if (enemyRandomValue > enemyPercentageDivisors[j])
                {
                    if (j == 0 && enemyRandomValue <= 100 || j != 0 && enemyRandomValue <= enemyPercentageDivisors[j-1])
                    {
                        //Instanciar enemigo en tile random
                        randomSpawnEnemy = Random.Range(0, enemySpawns.Count);

                        enemyInstantiated = Instantiate(enemyConfiguration[j].enemyPref, enemySpawns[randomSpawnEnemy].transform.position, enemyConfiguration[j].enemyPref.transform.rotation);

                        //Quitar tile de lista de spawns enemigos
                        enemySpawns.RemoveAt(randomSpawnEnemy);

                        enemyConfiguration[j].currentEnemiesOfThisType++;

                        //Si se ha llegado al max de ese tipo de enemigo le quito de la lista y vuelvo a llamar a SetEnemyProbability
                        if (enemyConfiguration[j].currentEnemiesOfThisType >= enemyConfiguration[j].maxEnemiesOfThisType)
                        {
                            probabilityOfEnemyDeleted = enemyConfiguration[j].spawnProbability;
                            enemyConfiguration.RemoveAt(j);
                            RedestributeEnemyProbabilities();
                            SetEnemyProbablity();
                        }

                        break;
                    }
                }
            }
        }
    }

    //Al eliminar un enemigo de la lista divido su porcentaje de spawn entre el número de enemigos restantes y equitativamente reparto las probabilidades entre ellos.
    public void RedestributeEnemyProbabilities()
    {
        if (enemyConfiguration.Count != 0)
        {
            probabilityOfEnemyDeleted /= enemyConfiguration.Count;

            for (int i = 0; i < enemyConfiguration.Count; i++)
            {
                enemyConfiguration[i].spawnProbability += probabilityOfEnemyDeleted;
            }
        }   
    }

    #endregion

    #region OBSTÁCULOS

    //Número aleatorio para decidir si instancio el obstáculo
    int obstacleRandomNumber;

    //Spawn checkeandose actualmente
    RandomTile currentObstacleChecking;

    List<RandomTile> nearbySpawns = new List<RandomTile>();

    //Spawn adyacente al que se esta checkeando y que se va a usar para spawnear obstaculo de 2 tiles
    RandomTile currentsecondObstacleChecking;

    GameObject obstacleInstantiated;
    LayerMask obstacleLayer;

    public void CreateObstacle()
    {
        //Setear layer?? o viene con obstáculo??

        //Este for solo suma en i si no instancia ningún obstáculo
        for (int i = 0; i < obstacleSpawns.Count;)
        {
            obstacleRandomNumber = Random.Range(1, 100);

            //Limpio la lista de spawns cercanos
            nearbySpawns.Clear();

            if (obstacleRandomNumber <= eachTileObstacleProbability)
            {
                currentObstacleChecking = obstacleSpawns[Random.Range(0, obstacleSpawns.Count)];
                if (currentObstacleChecking.isObstacle)
                {
                    obstacleLayer = LayerMask.NameToLayer("Obstacle");
                }

                else
                {
                    obstacleLayer = LayerMask.NameToLayer("NoTileHere");
                }

                //Buscar tiles obstáculos cercanos
                //1.25 porque es la mitad entre 1 tile y 1 tile y medio

                for (int j = 0; j < obstacleSpawns.Count; j++)
                {
                    if (obstacleSpawns[j] != currentObstacleChecking)
                    {
                        if (Vector3.Distance(obstacleSpawns[j].transform.position, currentObstacleChecking.transform.position) <= 1.25f)
                        {   
                            nearbySpawns.Add(obstacleSpawns[j]);
                        }
                    }
                }

                //Si hay un spawn cerca miro la posibilidad de instanciar un obstáculo que ocupe 2
                if (nearbySpawns.Count > 0)
                {
                    //Si supera el random la probabilidad instancio un obstáculo de 2.
                    if (Random.Range(1,100) <= probabilityOf2tilesObstacle)
                    {
                        currentsecondObstacleChecking = nearbySpawns[Random.Range(0, nearbySpawns.Count)];

                        //Instancio el obstáculo
                        obstacleInstantiated = Instantiate(twoTileObstacleConfig[Random.Range(0, twoTileObstacleConfig.Count)].obstacleAsset, (currentObstacleChecking.transform.position + currentsecondObstacleChecking.transform.position) / 2, Quaternion.identity);

                        //Recoloco la rotación del obstáculo 
                        if (currentObstacleChecking.transform.position.z != currentsecondObstacleChecking.transform.position.z)
                        {
                            obstacleInstantiated.transform.Rotate(new Vector3(0,90,0));
                        }

                        obstacleSpawns.Remove(currentObstacleChecking);
                        obstacleSpawns.Remove(currentsecondObstacleChecking);
                    }

                    //Si no sale mismo codigo que en else de abajo. Instancio obstáculo de 1
                    else
                    {
                        //Si no hay obstáculos cercanos instancio un obstáculo de 1 tile
                        obstacleInstantiated = Instantiate(oneTileObstacleConfig[Random.Range(0, oneTileObstacleConfig.Count)].obstacleAsset, currentObstacleChecking.transform.position, Quaternion.identity);

                        //Elimino el spawn de obstáculos ocupado de la lista
                        obstacleSpawns.Remove(currentObstacleChecking);
                    }
                }

                //Si no hay spawns cercano instancio obstáculo de 1.
                else
                {
                    //Si no hay obstáculos cercanos instancio un obstáculo de 1 tile
                    obstacleInstantiated = Instantiate(oneTileObstacleConfig[Random.Range(0, oneTileObstacleConfig.Count)].obstacleAsset, currentObstacleChecking.transform.position, Quaternion.identity);

                    //Elimino el spawn de obstáculos ocupado de la lista
                    obstacleSpawns.Remove(currentObstacleChecking);
                }

                obstacleInstantiated.AddComponent<BoxCollider>();
                obstacleInstantiated.GetComponent<BoxCollider>().size = new Vector3(0.9f,0.9f,0.9f);
                obstacleInstantiated.layer = obstacleLayer;
            }

            else
            {
                i++;
                continue;
            }
        }
    }

    #endregion

}
