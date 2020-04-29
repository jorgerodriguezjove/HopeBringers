using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : EnemyUnit
{
    BossMultTile dragReference;

    [SerializeField]
    public bool isCrystalActive = false;

    protected override void Awake()
    {
        //Le digo al enemigo cual es el LevelManager del nivel actual
        LevelManagerRef = FindObjectOfType<LevelManager>().gameObject;

        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();

        //SE AÑADE AQUI ÚNICAMENTE PARA SER INICIALIZADO. EN EL LEVEL MANAGER SALE DE LA LISTA
        LM.enemiesOnTheBoard.Add(this);

        initMaterial = unitMaterialModel.GetComponent<MeshRenderer>().material;

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        myCurrentEnemyState = enemyState.Waiting;
        currentHealth = maxHealth;

        initMaterial = unitMaterialModel.GetComponent<MeshRenderer>().material;

        //Esto puede ser null por el nivel de los cristales sin el dragón
        dragReference = FindObjectOfType<BossMultTile>();

        if (dragReference != null)
        {
            dragReference.crystalList.Add(this);

            if (dragReference.crystalList.Count == 1)
            {
                isCrystalActive = true;
            }

            else
            {
                isCrystalActive = false;
            }
        }

        else
        {
            isCrystalActive = true;
        }
    }

    public override void MoveToTilePushed(IndividualTiles newTile)
    {
        Debug.Log("Can't be pushed");
    }

    //HACER QUE NO RECIBA DAÑO POR LA ESPALDA

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("inisializaso");
            InitializeUnitOnTile();
        }
    }

    public override void OnHoverEnterFunctionality()
    {
        //Llamo a LevelManager para activar hover				
        LM.UIM.ShowUnitInfo(this.unitGeneralInfo, this);

        //LM.UIM.ShowCharacterInfo(unitInfo, this); 
        HealthBarOn_Off(true);
        //gameObject.GetComponent<PlayerHealthBar>().ReloadHealth();

        //Cambio el color del personaje
        SelectedColor();
    }

    public override void SelectedFunctionality()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy != null && LM.selectedEnemy != GetComponent<EnemyUnit>())
            {
                LM.HideEnemyHover(LM.selectedEnemy);
                //Llamo a LevelManager para desactivar hover
                if (LM.selectedCharacter != null)
                {
                    LM.selectedCharacter.HideDamageIcons(this);
                }
                LM.HideHover(LM.selectedEnemy);
                LM.selectedEnemy.HealthBarOn_Off(false);
                LM.UIM.HideUnitInfo("");
                //LM.UIM.HideCharacterInfo("");
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }

            else
            {
                LM.DeSelectUnit();
                LM.selectedEnemy = GetComponent<EnemyUnit>();

                LM.UIM.ShowUnitInfo(GetComponent<EnemyUnit>().unitGeneralInfo, GetComponent<EnemyUnit>());

                //Activo la barra de vida
                HealthBarOn_Off(true);

                //Cambio el color del personaje
                SelectedColor();
            }
        }
    }

    #region COLOR_OVERRIDE_MESH_RENDERER

    public override void SelectedColor()
    {
        unitMaterialModel.GetComponent<MeshRenderer>().material = selectedMaterial;
    }

    public override void ResetColor()
    {
        if (!isDead)
        {
            unitMaterialModel.GetComponent<MeshRenderer>().material = initMaterial;
        }
    }

    public override void ColorAvailableToBeAttackedAndNumberDamage(float damageCalculated)
    {
        if (!isDead)
        {
            unitMaterialModel.GetComponent<MeshRenderer>().material = AvailableToBeAttackedColor;

            if (damageCalculated >= 0)
            {
                previsualizeAttackIcon.SetActive(true);
                EnableCanvasHover(damageCalculated);
            }
        }
    }

    #endregion


    public override void OnHoverExitFunctionality()
    {
        if (LM.selectedEnemy == null)
        {
            LM.UIM.HideUnitInfo("");
            if (LM.selectedCharacter != null && !LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
            {
                ResetColor();
                LM.HideHover(this);
                HealthBarOn_Off(false);
            }
        }

        else
        {
            if (LM.selectedEnemy != this)
            {
                LM.HideHover(this);
                HealthBarOn_Off(false);
            }

            LM.UIM.HideUnitInfo("");
            LM.UIM.ShowUnitInfo(LM.selectedEnemy.unitGeneralInfo, LM.selectedEnemy);
            LM.selectedCharacter.HideDamageIcons(this);
            myCurrentTile.ColorDesAttack();
        }

        if (LM.selectedCharacter == null)
        {
            LM.UIM.HideUnitInfo("");
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (LM.selectedCharacter != null)
        {
            LM.UIM.ShowUnitInfo(LM.selectedCharacter.unitGeneralInfo, LM.selectedCharacter);
        }

        else
        {
            ResetColor();
            LM.HideHover(this);
            HealthBarOn_Off(false);
        }
    }



    public override void Die()
    {
        //En caso de que sea el ultimo nivel
        if (dragReference != null)
        {
            dragReference.RemoveCrystal(this);
        }


        //BASE MODIFICADA DEL DIE ENEMIGO
        Debug.Log("Soy " + gameObject.name + " y he muerto");
        //Animación, sonido y partículas de muerte
        SoundManager.Instance.PlaySound(AppSounds.EN_DEATH);
        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        //Cambios en UI
        LM.HideHover(this);
        HealthBarOn_Off(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //Cambios en la lógica para indicar que ha muerto
        myCurrentTile.unitOnTile = null;
        myCurrentTile.WarnInmediateNeighbours();

        //Hago que visualmente desaparezca aunque no lo destryuo todavía.
        unitModel.SetActive(false);
        GetComponent<Collider>().enabled = false;

        //Aviso de que el enemigo está muerto
        isDead = true;

        //Estas dos llamadas tienen que ir despues del bool de isdead = true
        LM.UIM.SetEnemyOrder();
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        if (!isCrystalActive)
        {
            damageReceived = 0;
        }
       

        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public override void EnableCanvasHover(float damageReceived)
    {
        if (!isCrystalActive)
        {
            damageReceived = 0;
        }

        base.EnableCanvasHover(damageReceived);
    }
}
