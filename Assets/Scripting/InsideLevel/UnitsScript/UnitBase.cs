using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UnitBase : MonoBehaviour
{
    #region VARIABLES

    [Header("STATS GENÉRICOS")]

    //Variable que se usará para ordenar a las unidades
    [SerializeField]
    public int speed;

    //Vida máxima que tiene cada unidad
    [SerializeField]
    public int maxHealth;

	[HideInInspector]
    public int currentHealth;

    //Uds movimiento máximas de la unidad.
    [SerializeField]
    public int movementUds;

    [SerializeField]
    public int attackRange;

    //Una vez que el feedback esté implementado, hay que esconderlo en el inspector
    //Bool que indica si está marcado o no 
    public bool isMarked;

    [Header("DAMAGE")]

    //Daño de la unidad
    [SerializeField]
    public int baseDamage;

    //Daño cuándo ataca por la espalda
    [SerializeField]
    public float bonusDamageBackAttack;

    //Daño cuándo ataca con más altura
    [SerializeField]
    public float bonusDamageMoreHeight;

    //Daño cuándo ataca con menos altura
    [SerializeField]
    public float penalizatorDamageLessHeight;

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByPush;

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByFall;

    //Daño para añadir buff o debuff
    public int bonusStateDamage;

    [Header("LOGIC")]

    //Modelo de la unidad. TIENE QUE ESTAR SERIALIZADO
    [SerializeField]
    public GameObject unitModel;

    //Modelo de la unidad dónde se guarda el material
    [SerializeField]
    protected GameObject unitMaterialModel;

    //Tile en el que está el personaje actualmente. Se setea desde el editor.
    [SerializeField]
    public IndividualTiles myCurrentTile;

    //Enum con las cuatro posibles direcciones en las que puede estar mirando una unidad.
    [HideInInspector]
    public enum FacingDirection { North, East, South, West }

    //Dirección actual. ESTÁ EN SERIALIZEFIELD PARA PROBARLO.
    [SerializeField]
    public FacingDirection currentFacingDirection;

    //Posición a la que tiene que moverse la unidad actualmente
    //La cambio a public para que el LevelManager pueda acceder
    [HideInInspector]
    public Vector3 currentTileVectorToMove;

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

    [Header("ATAQUE")]

    //Variable en la que guardo el daño a realizar
    protected float damageWithMultipliersApplied;

    //Máxima diferencia de altura para atacar
    [SerializeField]
    protected float maxHeightDifferenceToAttack;

    //AÑADO ESTA VARIABLE PARA QUE SEPA SI PUEDE ATACAR A LA UNIDAD DEL SIGUIENTE TILE
    [SerializeField]
    protected float previousTileHeight;

    //Máxima diferencia de altura para moverse
    [SerializeField]
    public float maxHeightDifferenceToMove;

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
    [SerializeField]
    protected GameObject collisionParticlePref;

    [Header("ANIMATIONS")]

    //Animator
    protected Animator myAnimator;

    [Header("FEEDBACK")]

    //Objeto general con todo lo que aparece en el hover
    [SerializeField]
    public GameObject healthBar;

    //Bool que indica si el healthbar puede desaparecer o no
    private bool shouldLockHealthBar;

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
    private Material AvailableToBeAttackedColor;

    //Material inicial y al ser seleccionado
    protected Material initMaterial;

    //Este canvas sirve para mostrar temas de vida al hacer hover en el caso del enemigo y en el caso del player (no está implementado) sirve para mostrar barra de vida.
    [SerializeField]
    private GameObject canvasUnit;




    [Header("INFO")]

    [SerializeField]
    [@TextAreaAttribute(15, 20)]
    public string unitGeneralInfo;
	[SerializeField]
	public Sprite characterImage;

	[SerializeField]
	public Sprite tooltipImage;

	[SerializeField]
	public string unitName;

    //¿SE PUEDE BORRAR?
    //Texto que describe a la unidad.
    //[SerializeField]
    //public string characterDescription;

    //LO COMENTO PORQUE AHORA MISMO NO ESTÁ EN USO
    //Icono que aparece en la lista de turnos.
    //[SerializeField]
    //public Sprite unitIcon;

    //¿SE PUEDE BORRAR?
    //Canvas que muestra la vida de la unidad
    //[SerializeField]
    //protected Canvas myCanvasHealthbar;

    #endregion

    private void Start()
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
        if (myCurrentTile != null)
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
    protected virtual void CalculateDamage(UnitBase unitToDealDamage)
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
            //Ataque por la espalda
            damageWithMultipliersApplied += bonusDamageBackAttack;
        }

        damageWithMultipliersApplied += bonusStateDamage;
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
    }

    //Función para recibir daño
    public virtual void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Cada unidad se resta vida con esta función.
        //Lo pongo en unit base para que sea genérico entre unidades y no tener que hacer la comprobación todo el rato.
        HealthBarOn_Off(true);
        shouldLockHealthBar = true;
        RefreshHealth(false);
        StartCoroutine("WaitBeforeHiding");
    }

    public virtual void Die()
    {
        //Cada unidad hace lo propio al morir
    }

    #endregion

    #region PUSH
    //Función genérica que sirve para calcular a que tile debe ser empujada una unidad
    //La función pide tatno el daño pro caída como el daño de empujón de la unidad atacante ya que pueden existir mejoras que modifiquen estos valores.
    public void CalculatePushPosition(int numberOfTilesMoved, List<IndividualTiles> tilesToCheckForCollision, int attackersDamageByPush, int attackersDamageByFall)
    {
        Debug.Log("Empuje");

        //Si no hay tiles en la lista me han empujado contra un borde
        //Tiene que ser menor o igual que 1 en vez de 0 porque para empujar a una unidad contra el borde, la unidad que empuja siempre va a necesitar 1 tile para atacar (que es donde está la unidad a la que voy a atacar)

        if (!isDead)
        {
            if (tilesToCheckForCollision.Count <= 1)
            {
                Debug.Log("borde");

                //Recibo daño 
                ReceiveDamage(attackersDamageByPush, null);

                //Hago animación de rebote??
            }

            //Si hay tiles en la lista me empjuan contra tiles que no son bordes 
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
                            }

                            else
                            {
                                //Muere la unidad de abajo
                                tilesToCheckForCollision[i].unitOnTile.Die();
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


                            Vector3 test = new Vector3((this.transform.position.x + tilesToCheckForCollision[i].unitOnTile.transform.position.x) / 2, this.transform.position.y, (this.transform.position.z + tilesToCheckForCollision[i].unitOnTile.transform.position.z) / 2);

                            Instantiate(collisionParticlePref, test, collisionParticlePref.transform.rotation);

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

                //Desplazo a la unidad
                MoveToTilePushed(tilesToCheckForCollision[numberOfTilesMoved]);
                Debug.Log(tilesToCheckForCollision[0]);
            }
        }
       
    }

    //Función que ejecuta el movimiento del push
    //Es virtual para que la balista pueda despintar y pintar los nuevos tiles
    protected virtual void MoveToTilePushed(IndividualTiles newTile)
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
    public void ColorAvailableToBeAttacked()
    {
        if (!isDead)
        {
            unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = AvailableToBeAttackedColor;
        }
        
    }

    #endregion

    #region UI_HOVER

    public void EnableCanvasHover(int damageReceived)
    {
        canvasUnit.SetActive(true);
        canvasUnit.GetComponent<CanvasHover>().damageNumber.SetText(damageReceived.ToString());
    }

    public void DisableCanvasHover()
    {
        canvasUnit.SetActive(false);
    }

    //El segundo bool sirve para hacer que el healthbar no desaparezca si se quita el ratón
	public void HealthBarOn_Off(bool isOn)
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
        //Recorro la lista de tokens empezando por el final. 
        //El -1 en el count es porque la lista empieza en el 0 y por tanto es demasiado grande
        //Sin embargo tengo que sumarle 1 en la i porque si no la current health al principio no entra
        for (int i = lifeTokensListInSceneHealthBar.Count - 1; i + 1 > currentHealth; i--)
        {
            if (i >= 0)
            {
                if (lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>())
                {
                    if (undoHealthDamage)
                    {
                        lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().ResetToken();
                    }

                    else if (!lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().haveIFlipped)
                    {
                        lifeTokensListInSceneHealthBar[i].GetComponent<LifeToken>().FlipToken();
                    }
                }
            }
        }
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
            myCurrentTile = hit.collider.gameObject.GetComponent<IndividualTiles>();            
            myCurrentTile.unitOnTile = GetComponent<UnitBase>();
            //Debug.Log(hit);
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

        transform.DOMove(tileToMoveBack.transform.position, 0);
        UpdateInformationAfterMovement(tileToMoveBack);
    }

    public virtual void UndoAttack(int previousHealth)
    {
        currentHealth = previousHealth;
    }

}
