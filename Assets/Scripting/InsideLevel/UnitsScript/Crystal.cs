using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : EnemyUnit
{
    protected override void Awake()
    {
        //Le digo al enemigo cual es el LevelManager del nivel actual
        LevelManagerRef = FindObjectOfType<LevelManager>().gameObject;

        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();

        initMaterial = unitMaterialModel.GetComponent<MeshRenderer>().material;

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        myCurrentEnemyState = enemyState.Waiting;
        currentHealth = maxHealth;

        initMaterial = unitMaterialModel.GetComponent<MeshRenderer>().material;

        
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

    public override void ColorAvailableToBeAttacked(float damageCalculated)
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
        if (LM.selectedCharacter != null && LM.selectedCharacter.shaderHover != null)
        {
        }
        if (LM.selectedCharacter != null)
        {
        }

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
}
