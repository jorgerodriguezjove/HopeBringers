using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class UnitBase : MonoBehaviour
{
    #region VARIABLES

    [Header("BASE")]

    //Variable que se usará para ordenar a las unidades
    [SerializeField]
    public int speed;

    //Vida máxima que tiene cada unidad
    [SerializeField]
    public int maxHealth;

    //Uds movimiento máximas de la unidad.
    [SerializeField]
    public int movementUds;

    [HideInInspector]
    //Este int lo pongo para saber el primer número y que así el tier 2 del Watcher no esté restando a los current movementsUds
    public int fMovementUds;

    [Header("DAMAGE")]

    //Daño de la unidad
    [SerializeField]
    public int baseDamage;

    [SerializeField]
    public int attackRange;

    //Daño cuándo ataca por la espalda
    [SerializeField]
    public float bonusDamageBackAttack;

    //Daño cuándo ataca con más altura
    [SerializeField]
    public float bonusDamageMoreHeight;

    //Daño cuándo ataca con menos altura
    [SerializeField]
    public float penalizatorDamageLessHeight;

    //Máxima diferencia de altura para atacar
    [SerializeField]
    protected float maxHeightDifferenceToAttack;

    //Máxima diferencia de altura para moverse
    [SerializeField]
    public float maxHeightDifferenceToMove;

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByPush;

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByFall;

    [Header("MODEL & MATERIAL")]

    //Modelo de la unidad. TIENE QUE ESTAR SERIALIZADO
    [SerializeField]
    public GameObject unitModel;

    //Modelo de la unidad dónde se guarda el material
    [SerializeField]
    protected GameObject unitMaterialModel;

    //Material inicial y al ser seleccionado
    protected Material initMaterial;

    [Header("ANIMATION TIME")]

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    [SerializeField]
    protected float timePushAnimation;

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    [SerializeField]
    protected float timeMovementAnimation;

    //Tiempo que tarda en rotar a la unidad.
    [SerializeField]
    protected float timeDurationRotation;

    //Tiempo que pasa antes de ocultarse la barra de vida después de recibir daño.
    [SerializeField]
    float timeToWaitBeforeHidingHealthbar;

    //DAMAGE

    //Variable en la que guardo el daño a realizar
    [HideInInspector]
    public float damageWithMultipliersApplied;

    //AÑADO ESTA VARIABLE PARA QUE SEPA SI PUEDE ATACAR A LA UNIDAD DEL SIGUIENTE TILE
    [HideInInspector]
    protected float previousTileHeight;

    //Bool que comprueba si el enemigo ha muerto para quitarlo de la lista de enemigos al final del turno.
    [HideInInspector]
    public bool isDead = false;

    //Variable que guarda el número más pequeño al comparar el rango del personaje con el número de tiles disponibles para atacar.
    protected int rangeVSTilesInLineLimitant;

    [Header("PARTICLES")]

    [SerializeField]
    protected GameObject movementParticle;
    [SerializeField]
    protected GameObject deathParticle;
    [SerializeField]
    protected GameObject attackParticle;
    [SerializeField]
    protected GameObject criticAttackParticle;


    [Header("FEEDBACK")]

    //Objeto general con todo lo que aparece en el hover
    [SerializeField]
    public GameObject healthBar;

    //Bool que indica si el healthbar puede desaparecer o no
    protected bool shouldLockHealthBar;

    //Referncia al token de vida que se instancia
    [SerializeField]
    private GameObject lifeTokenPref;

    //Objeto donde van a aparecer los puntos de vida
    [SerializeField]
    public GameObject lifeContainer;

    //Lista con los tokens de vida del jugador
    [HideInInspector]
    private List<GameObject> lifeTokensListInSceneHealthBar = new List<GameObject>();

    [SerializeField]
    protected Material AvailableToBeAttackedColor;

    [SerializeField]
    protected Material AvailableToBeHealedColor;
       
    //Referencia al gameobject que actua como hover de los enemigos.
    //La cambio aquí para que el playerunit también lo use.
    [SerializeField]
    public GameObject sombraHoverUnit;

    //Se usa para el ataque del samurai y el ataque del berserker
    [SerializeField]
    public TextMeshProUGUI timesRepeatNumber;

    //Este canvas sirve para mostrar temas de vida al hacer hover en el caso del enemigo y en el caso del player (no está implementado) sirve para mostrar barra de vida.
    [SerializeField]
    public GameObject canvasHover;

    [Header("HEALTHBAR_ICON")]

    [SerializeField]
    public GameObject backStabIcon;

    [SerializeField]
    public GameObject upToDownDamageIcon, downToUpDamageIcon, buffIcon, debuffIcon, movementBuffIcon, movementDebuffIcon, stunnedIcon;

    public TextMeshProUGUI buffIconText, debuffIconText, movementBuffIconText, movementDebuffIconText;

    [Header("HOVER_PARTICLE")]

    [SerializeField]
    public GameObject hoverBuffIcon;

    [SerializeField]
    public GameObject hoverDebuffIcon, hoverMovementBuffIcon, hoverMovementDebuffIcon, hoverStunnedIcon, hoverImpactIcon;

    //Se usa para indicar las marcas del monk
    public GameObject monkMark;
    public GameObject monkMarkUpgrade;

    //Icono que aparece encima para dar feedback de que se puede intercambiar con el decoy u otras unidades en el caso de la valquiria
    //Lo pongo en unitbase porque si no hay funciones que no van
    [SerializeField]
    public GameObject changePositionIcon;

    //Este icono lo utilizo para poner la espada encima de los posibles enemigos. 
    [SerializeField]
    public GameObject previsualizeAttackIcon;

    //Se una para indicar que el samurai no puede atacar a una unidad
    [SerializeField]
    public GameObject notAttackX;

    //ANIMATIONS

    //Animator
    protected Animator myAnimator;

    [Header("DEBUG ONLY")]

    [SerializeField]
    public int currentHealth;

    [SerializeField]
    public int currentArmor;

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [SerializeField]
    public bool hasMoved = false;
    [SerializeField]
    public bool hasAttacked = false;

    //Tile en el que está el personaje actualmente. Se setea desde el editor.
    [SerializeField]
    public IndividualTiles myCurrentTile;

    //Enum con las cuatro posibles direcciones en las que puede estar mirando una unidad.
    [HideInInspector]
    public enum FacingDirection { North, East, South, West }

    //Dirección actual. ESTÁ EN SERIALIZEFIELD PARA PROBARLO.
    [SerializeField]
    public FacingDirection currentFacingDirection;

    //MARCAS Y BUFF/DEBUFF/STUN

    //Una vez que el feedback esté implementado, hay que esconderlo en el inspector
    //Bool que indica si está marcado o no 
    [SerializeField]
    public bool isMarked;
    //Int para poder utilizar la mejora de la activa 2 del monk
    [SerializeField]
    public int numberOfMarks;

    //Una vez que el feedback esté implementado, hay que esconderlo en el inspector
    //Bool que indica si está stuneado o no 
    [SerializeField]
    public bool isStunned;
    //Añado esto por si los stuns se puede acumular
    [SerializeField]
    public int turnStunned;

    //Bool para poder ocultar a las unidades
    [SerializeField]
    public bool isHidden;

    //Daño para añadir buff  (tambien lo usamos para los debuff)
    [SerializeField]
    public int buffbonusStateDamage;

    [SerializeField]
    //Turnos que el buff o debuff tiene que estar aplicado
    public int turnsWithBuffOrDebuff;

    [SerializeField]
    //Turnos que el buff o debuff de movimiento tiene que estar aplicado
    public int turnsWithMovementBuffOrDebuff;

    //TILES

    //Posición a la que tiene que moverse la unidad actualmente
    //La cambio a public para que el LevelManager pueda acceder
    [HideInInspector]
    public Vector3 currentTileVectorToMove;

    //Lista de posibles unidades a las que atacar
    [HideInInspector]
    public List<IndividualTiles> currentTilesInRangeForAttack;

    [Header("INFO (ÚLTIMA VARIABLE UNITBASE)")]
	[SerializeField]
	public Sprite characterImage;

	[SerializeField]
	public Sprite tooltipImage;

	[SerializeField]
	public string unitName;

	[SerializeField]
	protected Image inGamePortrait;

    [SerializeField]
    [@TextAreaAttribute(15, 20)]
    public string unitGeneralInfo;

    #endregion

    //El level manager llama a esta función sustituyendo al start
    public virtual void InitializeUnitOnTile()
    {
        FindAndSetFirstTile();
        myCurrentTile.unitOnTile = this;
        myCurrentTile.WarnInmediateNeighbours();

        InitializeHealth();
    }

    //Esta función la usan todos los personajes al moverse para reajustar la información del tile actual y del nuevo al que se van a mover.
    //El override se hace en el dragón ya que tiene que actualizar más tiles.
    public virtual void UpdateInformationAfterMovement(IndividualTiles newTile)
    {
        //Este if está porque la función también se usa al colocar las unidades al principio del nivel y ahí no hay tile
        if (myCurrentTile != null && myCurrentTile.unitOnTile == this)
        {
            myCurrentTile.unitOnTile = null;

            //Aviso al tile en el que estaba de que avise a la balista si le toca
            if (GetComponent<PlayerUnit>())
            {
                myCurrentTile.WarnBalista(GetComponent<PlayerUnit>());
            }

        }
        newTile.unitOnTile = this;

        //A pesar de que es el mismo if que antes, no pueden ir juntos porque tiene que ir en este orden concreto.
        if (myCurrentTile != null)
        {
            myCurrentTile.WarnInmediateNeighbours();
        }
            
        myCurrentTile = newTile;
        myCurrentTile.WarnInmediateNeighbours();

        //Aviso al tile que me he movido de que avise a la balista si le toca
        if (GetComponent<PlayerUnit>())
        {
            myCurrentTile.WarnBalista(GetComponent<PlayerUnit>());
        }
    }

    #region DAMAGE_&_DIE

    //Calcula PERO NO aplico el daño a la unidad elegida
    public virtual void CalculateDamage(UnitBase unitToDealDamage)
    {
        //Reseteo la variable de daño a realizar
        damageWithMultipliersApplied = baseDamage;

        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > myCurrentTile.height)
        {
            damageWithMultipliersApplied -= penalizatorDamageLessHeight;
        }

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < myCurrentTile.height)
        {
            damageWithMultipliersApplied += bonusDamageMoreHeight;
        }

        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
        {
            if (unitToDealDamage.GetComponent<EnDuelist>()
                && unitToDealDamage.GetComponent<EnDuelist>().hasTier2)
            {
                
                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.South;
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.North;
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                   
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.West;
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.East;
                }
            }

            else
            {
                //Ataque por la espalda
                damageWithMultipliersApplied += bonusDamageBackAttack;
            }
        }
        damageWithMultipliersApplied += buffbonusStateDamage;
    }

    //Prueba para calcular damages en el hover
    public virtual void CalculateDamagePreviousAttack(UnitBase unitToDealDamage, UnitBase unitAttacking, IndividualTiles tileAttack, FacingDirection endFacingDirection)
    {
        //Reseteo la variable de daño a realizar
        unitAttacking.damageWithMultipliersApplied = unitAttacking.baseDamage;

        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > tileAttack.height)
        {
            unitAttacking.damageWithMultipliersApplied -= unitAttacking.penalizatorDamageLessHeight;
        }

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < tileAttack.height)
        {
            unitAttacking.damageWithMultipliersApplied += unitAttacking.bonusDamageMoreHeight;
        }

        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == endFacingDirection)
        {
            //Ataque por la espalda
            unitAttacking.damageWithMultipliersApplied += unitAttacking.bonusDamageBackAttack;
        }

        unitAttacking.damageWithMultipliersApplied += unitAttacking.buffbonusStateDamage;

        //Estas líneas las añado para comprobar si el caballero tiene que defender
        Knight knightDef = FindObjectOfType<Knight>();

        if (knightDef != null && knightDef.isBlockingNeighbours)
        {
            unitToDealDamage.GetComponent<PlayerUnit>().CheckIfKnightIsDefending(knightDef, endFacingDirection);
            unitAttacking.damageWithMultipliersApplied -= knightDef.shieldDef;

            if (knightDef.shieldDef > 0)
            {
                //Escudo full
                if (knightDef.isBlockingNeighboursFull)
                {
                    unitToDealDamage.GetComponent<PlayerUnit>().ShowHideFullShield(true);
                }

                //Escudo parcial
                else if (knightDef.isBlockingNeighbours)
                {
                    unitToDealDamage.GetComponent<PlayerUnit>().ShowHidePartialShield(true);
                }
            }
        }

        #region Knight_Blocks

        //FrontBlock
        if (unitToDealDamage != null && unitToDealDamage.GetComponent<Knight>() && (
               endFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
            || endFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
            || endFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
            || endFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East))
        {
            Debug.Log("Front Block");
            //Icono escudo total
            unitToDealDamage.GetComponent<Knight>().ShowHideFullShield(true);
            unitAttacking.damageWithMultipliersApplied = 0;
        }

        //Lateralblock
        else if (unitToDealDamage != null && unitToDealDamage.GetComponent<Knight>() && unitToDealDamage.GetComponent<Knight>().lateralBlock && 
          ((unitToDealDamage.currentFacingDirection == FacingDirection.North || unitToDealDamage.currentFacingDirection == FacingDirection.South &&  endFacingDirection == FacingDirection.East  || endFacingDirection == FacingDirection.West) 
         ||(unitToDealDamage.currentFacingDirection == FacingDirection.East  || unitToDealDamage.currentFacingDirection == FacingDirection.West  &&  endFacingDirection == FacingDirection.North || endFacingDirection == FacingDirection.South)))
        {
            Debug.Log("Lateral Block");
            //Icono escudo parcial
            unitToDealDamage.GetComponent<Knight>().ShowHidePartialShield(true);
            unitAttacking.damageWithMultipliersApplied -= unitToDealDamage.GetComponent<Knight>().damageLateralBlocked;
        }

        //Backblock
        else if (unitToDealDamage != null && unitToDealDamage.GetComponent<Knight>() && unitToDealDamage.GetComponent<Knight>().backBlock && (
                endFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.North
             || endFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.East
             || endFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.South
             || endFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.West))
        {
            Debug.Log("Back Block");
            //Icono escudo parcial
            unitToDealDamage.GetComponent<Knight>().ShowHidePartialShield(true);
            unitAttacking.damageWithMultipliersApplied -= unitToDealDamage.GetComponent<Knight>().damageBackBlocked;
        }

        #endregion

        Debug.Log("CalculateDamagePreviousAttack");
    }

    //Aplico el daño a la unidad elegida
    protected virtual void DoDamage(UnitBase unitToDealDamage)
    {
        CalculateDamage(unitToDealDamage);
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);

        //Si ataco por la espalda instancio la partícula de ataque crítico
        if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
        {
            Instantiate(criticAttackParticle, unitModel.transform.position, unitModel.transform.rotation);
           
        }

        //Si no, instancio la partícula normal
        else
        {
            Instantiate(attackParticle, unitModel.transform.position, unitModel.transform.rotation);
        }


        //Logro hacer 0 daño
        if (GetComponent<PlayerUnit>() && damageWithMultipliersApplied == 0)
        {
            GameManager.Instance.UnlockAchievement(AppAchievements.ACHV_MISS);
        }
    }

    //Función para recibir daño
    public virtual void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Cada unidad se resta vida con esta función.
        //Lo pongo en unit base para que sea genérico entre unidades y no tener que hacer la comprobación todo el rato.
        HealthBarOn_Off(true);
        shouldLockHealthBar = true;
        RefreshHealth(false);

        hoverImpactIcon.SetActive(false);

        StartCoroutine("WaitBeforeHiding");
    }

    public virtual void Die()
    {
        //Cada unidad hace lo propio al morir
    }

    //Esta función se usa para mostrar el escudo encima de los personajes. Puede aparecer al hacer hover sobre enemigo y mostrar ataque o justo en la animación de ataque.
    public void CalculateDirectionOfAttackReceivedToShowShield(IndividualTiles tileWhereEnemyFinishMovement)
    {
        if (GetComponent<Knight>())
        {
            //GetComponent<Knight>().shieldBlockAllDamage.SetActive(true);
        }
    }

    #endregion

    #region PUSH
    //Esta función solo calcula el tile en el que acaba la unidad empujada. SIRVE PARA MOSTRAR EFECTO DE ATAQUE CABALLERO.
    public IndividualTiles CalculatePushLogic(int numberOfTilesMoved, List<IndividualTiles> tilesToCheckForCollision, int attackersDamageByPush, int attackersDamageByFall)
    {
        if (!isDead)
        {
            

            if (tilesToCheckForCollision.Count <= 1)
            {
                Debug.Log("borde");
                //No calculo nada

                Debug.Log(gameObject.name);
                if (tilesToCheckForCollision.Count > 0)
                {
                    tilesToCheckForCollision[0].unitOnTile.hoverImpactIcon.SetActive(true);
                }
                return null;
            }

            //Si hay tiles en la lista me empujan contra tiles que no son bordes 
            else
            {
                for (int i = 1; i <= numberOfTilesMoved; i++)
                {
                    //El tile al que empujo está más alto (pared)
                    if (tilesToCheckForCollision[i].height > myCurrentTile.height)
                    {
                        Debug.Log("pared");
                        if (tilesToCheckForCollision.Count > 0)
                        {
                            tilesToCheckForCollision[0].unitOnTile.hoverImpactIcon.SetActive(true);
                        }
                        return tilesToCheckForCollision[i - 1];
                        
                    }

                    //El tile al que empujo está más bajo (caída)
                    else if (Mathf.Abs(tilesToCheckForCollision[i].height - myCurrentTile.height) > 1)
                    {
                        Debug.Log("caída");

                        //Compruebo si hay otra unidad
                        if (tilesToCheckForCollision[i].unitOnTile != null)
                        {
                            if (tilesToCheckForCollision.Count > 0)
                            {
                                if (tilesToCheckForCollision[0].unitOnTile != null)
                                {
                                    tilesToCheckForCollision[0].unitOnTile.hoverImpactIcon.SetActive(true);
                                }
                                if (tilesToCheckForCollision[i].unitOnTile != null)
                                {
                                    tilesToCheckForCollision[i].unitOnTile.hoverImpactIcon.SetActive(true);

                                }
                               

                              
                            }

                            if (tilesToCheckForCollision[i].unitOnTile.currentHealth > currentHealth)
                            {
                                //Muere la unidad que cae
                            }

                            else
                            {
                                //Muere la unidad de abajo
                            }
                        }

                        else
                        {
                            //Caída sin más
                        }

                        return tilesToCheckForCollision[i];
                    }

                    //Si la altura del tile al que empujo y la mía son iguales compruebo si el tile está vacío, es un obstáculo o tiene una unidad.
                    else
                    {
                        //Es tile vacío u obstáculo
                        if (tilesToCheckForCollision[i].isEmpty || tilesToCheckForCollision[i].isObstacle)
                        {
                            Debug.Log("vacío");

                            return tilesToCheckForCollision[i - 1];
                        }

                        //Es tile con unidad
                        else if (tilesToCheckForCollision[i].unitOnTile != null)
                        {
                            Debug.Log("otra unidad");
                            Debug.Log(gameObject.name);

                            if (tilesToCheckForCollision[0].unitOnTile != null)
                            {
                                tilesToCheckForCollision[0].unitOnTile.hoverImpactIcon.SetActive(true);
                            }
                            
                            tilesToCheckForCollision[i].unitOnTile.hoverImpactIcon.SetActive(true);

                            
                          
                            return tilesToCheckForCollision[i - 1];
                        }
                    }
                }

                //Si sale del for entonces es que todos los tiles que tiene que comprobar son normales y simplemente lo muevo al último tile

                if (numberOfTilesMoved > 0)
                {
                    return tilesToCheckForCollision[numberOfTilesMoved];
                }
            }
        }

        return null;
    }

    //Función genérica que sirve para calcular a que tile debe ser empujada una unidad
    //La función pide tanto el daño por caída como el daño de empujón de la unidad atacante ya que pueden existir mejoras que modifiquen estos valores.
    public void ExecutePush(int numberOfTilesMoved, List<IndividualTiles> tilesToCheckForCollision, int attackersDamageByPush, int attackersDamageByFall)
    {
        Debug.Log("Empuje");

        for (int j = 0; j < tilesToCheckForCollision.Count; j++)
        {
            if (tilesToCheckForCollision[j].unitOnTile != null)
            {
                tilesToCheckForCollision[j].unitOnTile.hoverImpactIcon.SetActive(false);
            }
        }

        //Si no hay tiles en la lista me han empujado contra un borde
        //Tiene que ser menor o igual que 1 en vez de 0 porque para empujar a una unidad contra el borde, la unidad que empuja siempre va a necesitar 1 tile para atacar (que es donde está la unidad a la que voy a atacar)

        if (!isDead)
        {
            if (tilesToCheckForCollision.Count <= 1)
            {
                Debug.Log("borde");

                //Recibo daño 
                ReceiveDamage(attackersDamageByPush, null);

                if (currentHealth <= 0)
                {
                    healthBar.SetActive(false);
                }
                //Hago animación de rebote??
            }

            //Si hay tiles en la lista me empujan contra tiles que no son bordes 
            else
            {
                for (int i = 1; i <= numberOfTilesMoved; i++)
                {
                    //El tile al que empujo está más alto (pared)
                    if (tilesToCheckForCollision[i].height > myCurrentTile.height)
                    {
                        Debug.Log("pared");
                        //Recibo daño 
                        ReceiveDamage(attackersDamageByPush, null);

                        //Desplazo a la unidad
                        MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                        if(currentHealth <= 0)
                        {
                            healthBar.SetActive(false);
                        }
                        return;
                    }

                    //El tile al que empujo está más bajo (caída)
                    else if (Mathf.Abs(tilesToCheckForCollision[i].height - myCurrentTile.height) > 1)
                    {
                        Debug.Log("caída");

                        //Compruebo la altura de la que lo tiro??

                        //Compruebo si hay otra unidad
                        if (tilesToCheckForCollision[i].unitOnTile != null)
                        {
                            ReceiveDamage(attackersDamageByFall, null);
                            tilesToCheckForCollision[i].unitOnTile.ReceiveDamage(attackersDamageByPush, null);

                            if (tilesToCheckForCollision[i].unitOnTile.currentHealth > currentHealth)
                            {
                                //Muere la unidad que cae
                                Die();
                                if (currentHealth <= 0)
                                {
                                    healthBar.SetActive(false);
                                }
                            }

                            else
                            {
                                //Muere la unidad de abajo
                                tilesToCheckForCollision[i].unitOnTile.Die();
                                if (tilesToCheckForCollision[i].unitOnTile.currentHealth <= 0)
                                {
                                    tilesToCheckForCollision[i].unitOnTile.healthBar.SetActive(false);
                                }
                            }
                        }

                        else
                        {
                            ReceiveDamage(attackersDamageByFall, null);
                        }

                        //Que pasa si hay un obstáculo en el tile de abajo?

                        MoveToTilePushed(tilesToCheckForCollision[i]);

                        return;
                    }

                    //Si la altura del tile al que empujo y la mía son iguales compruebo si el tile está vacío, es un obstáculo o tiene una unidad.
                    else
                    {
                        //Es tile vacío u obstáculo
                        if (tilesToCheckForCollision[i].isEmpty || tilesToCheckForCollision[i].isObstacle)
                        {
                            Debug.Log("vacío");
                            //Recibo daño 
                            ReceiveDamage(attackersDamageByPush, null);

                            // Desplazo a la unidad
                            MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                            //Animación de rebote??

                            return;
                        }

                        //Es tile con unidad
                        else if (tilesToCheckForCollision[i].unitOnTile != null)
                        {
                            Debug.Log("otra unidad");
                            //Recibo daño 
                            ReceiveDamage(attackersDamageByPush, null);

                            //Hago daño a la otra unidad
                            tilesToCheckForCollision[i].unitOnTile.ReceiveDamage(attackersDamageByPush, null);

                            //Desplazo a la unidad
                            MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                            //Animación de rebote??

                            return;
                        }
                    }
                }

                //Si sale del for entonces es que todos los tiles que tiene que comprobar son normales y simplemente lo muevo al último tile

                SoundManager.Instance.PlaySound(AppSounds.COLLISION);

                if (numberOfTilesMoved > 0)
                {
                    //Desplazo a la unidad
                    MoveToTilePushed(tilesToCheckForCollision[numberOfTilesMoved]);
                    Debug.Log(tilesToCheckForCollision[numberOfTilesMoved]);
                }
            }
        }
    }

    //Función que ejecuta el movimiento del push
    //Es virtual para que la balista pueda despintar y pintar los nuevos tiles
    public virtual void MoveToTilePushed(IndividualTiles newTile)
    {
        //Mover al nuevo tile
        currentTileVectorToMove = newTile.transform.position;

        transform.DOMove(currentTileVectorToMove, timePushAnimation);

        //Si no ha muerto tras el choque, actualizo la info
        if (!isDead)
        {
            //Aviso a los tiles del cambio de posición
            UpdateInformationAfterMovement(newTile);
        }  
    }

    #endregion

    #region COLORS

    //Cambiar a color inicial
    public virtual void ResetColor()
    {
        if (!isDead)
        {
                unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;  
        }
        
    }

    //Cambiar a color que indica que puede ser atacado
    public virtual void ColorAvailableToBeAttackedAndNumberDamage(float damageCalculated)
    {
        if (!isDead)
        {
            if (unitMaterialModel != null && unitMaterialModel.GetComponent<SkinnedMeshRenderer>())
            {
                unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = AvailableToBeAttackedColor;
            }

            if (damageCalculated >= 0)
            {
                if (previsualizeAttackIcon != null)
                {
                    previsualizeAttackIcon.SetActive(true);
                }
               
                EnableCanvasHover(damageCalculated);
            }
        }        
    }

   
    public virtual void ColorAvailableToBeHealed()
    {
        if (!isDead)
        {
            unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = AvailableToBeHealedColor;

        }
    }

    #endregion

    #region UI_HOVER

    public virtual void EnableCanvasHover(float damageReceived)
    {
        
        canvasHover.SetActive(true);
        canvasHover.GetComponent<CanvasHover>().damageNumber.SetText( "-" + damageReceived.ToString());
        canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32 (180, 0, 0, 255);
    }

    public void DisableCanvasHover()
    {
        canvasHover.SetActive(false);
        previsualizeAttackIcon.SetActive(false);
    }

    //El segundo bool sirve para hacer que el healthbar no desaparezca si se quita el ratón
	public virtual void HealthBarOn_Off(bool isOn)
	{
        if (!isDead)
        {
            if (shouldLockHealthBar && isOn)
            {
                healthBar.SetActive(isOn);
            }
            else if (!shouldLockHealthBar)
            {
                healthBar.SetActive(isOn);
            }
        } 
    }

    //Función que instancia los puntos de vida en la barra de vida
    public void InitializeHealth()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject lifeTokInScene = Instantiate(lifeTokenPref, lifeContainer.transform);
            lifeTokensListInSceneHealthBar.Add(lifeTokInScene);
        }
    }

    //Función que se encarga de actualizar la vida del personaje.
    public void RefreshHealth(bool undoHealthDamage)
    {
        //for (int i = 0; i < maxHealth; i++)
        //{
        //    if (i < currentHealth)
        //    {
        //        if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
        //        {
        //            lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().ResetToken();
        //        }
        //    }
        //    else
        //    {
        //        if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
        //        {
        //            lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().FlipToken();
        //        }
        //    }
        //}
        
       for (int i = 0; i < maxHealth; i++)
       {
           if (i < currentHealth)
           {
               if (i < currentArmor)
               {
                   if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
                   {

                        lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().ArmoredToken();
                   }
               }
               else
               {
                   if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
                   {
                        lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().ResetToken();
                   }
               }
           }
           else
           {
               if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
               {
                    lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().FlipToken();
               }
           }
       }
        


        //Recorro la lista de tokens empezando por el final. 
        //El -1 en el count es porque la lista empieza en el 0 y por tanto es demasiado grande
        //Sin embargo tengo que sumarle 1 en la i porque si no la current health al principio no entra
        //for (int i = lifeTokensListInSceneHealthBar.Count - 1; i + 1 > currentHealth; i--)
        //{
        //    if (i >= 0)
        //    {
        //        if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
        //        {
        //            if (undoHealthDamage)
        //            {
        //                lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().ResetToken();
        //            }

        //            else if (!lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().haveIFlipped)
        //            {
        //                lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().FlipToken();
        //            }
        //        }
        //    }
        //}
    }

    IEnumerator WaitBeforeHiding()
    {
        yield return new WaitForSeconds(timeToWaitBeforeHidingHealthbar);
        shouldLockHealthBar = false;
        HealthBarOn_Off(false);
    }


    #endregion

    #region DETECT_TILE

    RaycastHit hit;

    //Es virtual para que en el Player pueda hacer la comprobación de si el nivel viene del selector de niveles o no.
    protected virtual void FindAndSetFirstTile()
    {
        Debug.DrawRay(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), transform.TransformDirection(Vector3.down), Color.yellow, 20f);
        
        if (Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), transform.TransformDirection(Vector3.down), out hit))
        {
            Debug.Log(hit.collider.gameObject);
            myCurrentTile = hit.collider.gameObject.GetComponent<IndividualTiles>();            
            myCurrentTile.unitOnTile = GetComponent<UnitBase>();
        }
    }

    #endregion

    //Es virtual porque el mago tiene que añadir la funcionalidad de quitar el decoy
    public virtual void UndoMove(IndividualTiles tileToMoveBack, FacingDirection rotationToTurnBack, bool shouldResetMovement)
    {
        #region Rotation

        if (rotationToTurnBack == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), 0);
            currentFacingDirection = FacingDirection.North;
        }

        else if (rotationToTurnBack == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), 0);
            currentFacingDirection = FacingDirection.South;
        }

        else if (rotationToTurnBack == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), 0);
            currentFacingDirection = FacingDirection.East;
        }

        else if (rotationToTurnBack == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), 0);
            currentFacingDirection = FacingDirection.West;
        }
        #endregion

        //Mover de tile
        transform.DOMove(tileToMoveBack.transform.position, 0);
        UpdateInformationAfterMovement(tileToMoveBack);
       
    }

    //public virtual void UndoAttack(int previousHealth)
    //{
    //    currentHealth = previousHealth;

    //}

    //Esta función resetea los valores de las unidades al darle a undo.
    //La necesitan tanto pjs como enemigos
    public virtual void UndoAttack(AttackCommand lastAttack, bool _isThisUnitTheAttacker)
    {
        //Todas las variables se tienen que setear en el override del player y el obj, ya que cada uno tiene la suya.
    }

    public virtual void StunUnit(UnitBase unitToStun)
    {
        
        unitToStun.isStunned = true;
        unitToStun.turnStunned = 1;
        SetStunIcon(unitToStun,false, true);
    }

    public virtual void SetStunIcon(UnitBase _unitToStun, bool onHover, bool hasToShow)
    {
        if (_unitToStun != null)
        {
            if (onHover)
            {
                if (hasToShow)
                {
                    _unitToStun.hoverStunnedIcon.SetActive(true);
                }

                else
                {
                    _unitToStun.hoverStunnedIcon.SetActive(false);
                }
            }

            else
            {
                if (hasToShow)
                {
                    _unitToStun.stunnedIcon.SetActive(true);
                    _unitToStun.hoverStunnedIcon.SetActive(false);
                }
                else
                {
                    _unitToStun.stunnedIcon.SetActive(false);
                    _unitToStun.hoverStunnedIcon.SetActive(false);
                }
            }
        }             
    }

    public virtual void ApplyBuffOrDebuffDamage(UnitBase unitToApply, int damageAdded, int turnsAdded)
    {
        if (unitToApply.GetComponent<Druid>())
        {
            unitToApply.GetComponent<Druid>().healedLife += unitToApply.GetComponent<Druid>().buffHeal;
            unitToApply.turnsWithBuffOrDebuff = turnsAdded;
        }

        else
        {
            unitToApply.buffbonusStateDamage = damageAdded;
            unitToApply.turnsWithBuffOrDebuff = turnsAdded;
        }

        SetBuffDebuffIcon(damageAdded, unitToApply, false);
    }

    public void SetBuffDebuffIcon(int numToCheck, UnitBase unitToApply, bool isOnHover)
    {
        if (numToCheck > 0)
        {
            if (isOnHover)
            {
                unitToApply.hoverBuffIcon.SetActive(true);
                unitToApply.hoverDebuffIcon.SetActive(false);
            }
            else
            {
                unitToApply.buffIcon.SetActive(true);
                unitToApply.buffIconText.text = unitToApply.turnsWithBuffOrDebuff.ToString();
                unitToApply.debuffIcon.SetActive(false);
            }           
        }
        else if(numToCheck < 0)
        {
            if (isOnHover)
            {
                unitToApply.hoverBuffIcon.SetActive(false);
                unitToApply.hoverDebuffIcon.SetActive(true);
            }
            else
            {                
                unitToApply.buffIcon.SetActive(false);
                unitToApply.debuffIcon.SetActive(true);
                unitToApply.debuffIconText.text = unitToApply.turnsWithBuffOrDebuff.ToString();
            }           
        }
        else
        {
            unitToApply.hoverBuffIcon.SetActive(false);
            unitToApply.hoverDebuffIcon.SetActive(false);
            unitToApply.buffIcon.SetActive(false);
            unitToApply.debuffIcon.SetActive(false);
        }
    }

    public virtual void ApplyBuffOrDebuffMovement(UnitBase unitToApply, int movementAddedOrRemoved, int turnsAdded)
    {

        unitToApply.movementUds += movementAddedOrRemoved;
        unitToApply.turnsWithMovementBuffOrDebuff = turnsAdded;


        SetMovementIcon(movementAddedOrRemoved, unitToApply, false);
    }

    public void SetMovementIcon(int numToCheck, UnitBase unitToApply, bool isOnHover)
    {
        if (numToCheck > 0)
        {
            if (isOnHover)
            {
                unitToApply.hoverMovementBuffIcon.SetActive(true);
                unitToApply.hoverMovementDebuffIcon.SetActive(false);

            }
            else
            {
                unitToApply.movementBuffIcon.SetActive(true);
                unitToApply.movementBuffIconText.text = unitToApply.turnsWithBuffOrDebuff.ToString();
                unitToApply.movementDebuffIcon.SetActive(false);
            }



        }
        else if (numToCheck < 0)
        {
            if (isOnHover)
            {
                unitToApply.hoverMovementBuffIcon.SetActive(false);
                unitToApply.hoverMovementDebuffIcon.SetActive(true);

            }
            else
            {
                unitToApply.movementBuffIcon.SetActive(false);
                unitToApply.movementDebuffIcon.SetActive(true);
                unitToApply.movementDebuffIconText.text = unitToApply.turnsWithBuffOrDebuff.ToString();

            }

        }
        else
        {
            unitToApply.hoverMovementBuffIcon.SetActive(false);
            unitToApply.hoverMovementDebuffIcon.SetActive(false);
            unitToApply.movementBuffIcon.SetActive(false);
            unitToApply.movementDebuffIcon.SetActive(false);
        }

    }

    public void QuitMarks()
    {
        isMarked = false;
        monkMark.SetActive(false);
        monkMarkUpgrade.SetActive(false);

    }

    public virtual void SetShadowRotation(UnitBase unitToSet, IndividualTiles unitToCheckPos, IndividualTiles otherUnitToCheck)
    {
        //if (unitToSet.currentFacingDirection == FacingDirection.North)
        //{
        //    unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
           
        //}

        //else if (unitToSet.currentFacingDirection == FacingDirection.South)
        //{
        //    unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
        //}

        //else if (unitToSet.currentFacingDirection == FacingDirection.East)
        //{

        //    unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
        //}

        //else if (unitToSet.currentFacingDirection == FacingDirection.West)
        //{
        //    unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
        //}

        if (unitToCheckPos.tileX == otherUnitToCheck.tileX)
        {
            //Arriba
            if (unitToCheckPos.tileZ > otherUnitToCheck.tileZ)
            {
                unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
            }
            //Abajo
            else
            {
                unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (unitToCheckPos.tileX > otherUnitToCheck.tileX)
            {
                unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
            }
            //Izquierda
            else
            {
                unitToSet.sombraHoverUnit.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
            }
        }
    }

    public void EnableUnableCollider(bool _shouldEnableCollider)
    {
        GetComponent<Collider>().enabled = _shouldEnableCollider;
    }

}
