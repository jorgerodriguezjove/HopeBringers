using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageDecoy : Mage
{
    #region VARIABLES

    public Mage myMage;
    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de personajes del jugador
        LM = FindObjectOfType<LevelManager>();
        TM = FindObjectOfType<TileManager>();
        LM.charactersOnTheBoard.Add(this);

        //Referencia al UIM 
        UIM = FindObjectOfType<UIManager>();

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();
        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;
        currentHealth = maxHealth;
    }


    #endregion

    #region INTERACTION

    //Al clickar en una unidad aviso al LM
    //Es virtual para el decoy del mago.
    protected override void OnMouseDown()
    {
        if (LM.selectedCharacter != null)
        {
            if (LM.selectedCharacter == myMage && !myMage.hasMoved)
            {
                ChangePosition(myMage);
            }
            else
            {
                LM.SelectUnitToAttack(GetComponent<UnitBase>());
            }   
        }
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Animación de ataque
        myAnimator.SetTrigger("Damage");

        currentHealth -= damageReceived;

        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public override void Die()
    {
        Debug.Log("Soy " + gameObject.name + " y he muerto");

        //Animación de ataque
        myAnimator.SetTrigger("Death");

        Instantiate(deathParticle, gameObject.transform.position, gameObject.transform.rotation);

        //myCurrentTile.unitOnTile = null;
        //myCurrentTile.WarnInmediateNeighbours();

        if (myMage.isDecoyBomb)
        {            
            TM.GetSurroundingTiles(myCurrentTile, 1, true, false);
           
            //Hago daño a las unidades adyacentes
            for (int i = 0; i < TM.surroundingTiles.Count; ++i)
            {
                if (TM.surroundingTiles[i] != null)
                {
                    TM.surroundingTiles[i].ColorAttack();
                }
            }

            StartCoroutine("WaitToDamageSurrounding");
        }

        else
        {
            myMage.myDecoys.Remove(gameObject);
            LM.charactersOnTheBoard.Remove(this);
            Destroy(gameObject);
        }

     
    }

    IEnumerator WaitToDamageSurrounding()
    {
        //Hago daño a las unidades adyacentes(3x3)
        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
        {
            if (TM.surroundingTiles[i].unitOnTile != null)
            {
                DoDamage(TM.surroundingTiles[i].unitOnTile);
            }
        }

        yield return new WaitForSeconds(2f);

        //Hago daño a las unidades adyacentes(3x3)
        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
        {
            if (TM.surroundingTiles[i] != null)
            {
                TM.surroundingTiles[i].ColorDesAttack();                
            }
        }

        myMage.myDecoys.Remove(gameObject);
        LM.charactersOnTheBoard.Remove(this);
        Destroy(gameObject);
    }

    //Es virtual para el decoy del mago.
    protected override void OnMouseEnter()
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        {
            if (LM.selectedEnemy == null)
            {
                if (LM.selectedCharacter != null && LM.selectedCharacter == myMage && !myMage.hasMoved)
                {
                    ShowAttackEffect(this);

                }
                else if (LM.selectedCharacter != null && LM.selectedCharacter.currentUnitsAvailableToAttack.Contains(this.GetComponent<UnitBase>()))
                {
                    Cursor.SetCursor(LM.UIM.attackCursor, Vector2.zero, CursorMode.Auto);
                }

                else 
                {
                    if (!hasAttacked)
                    {
                        LM.ShowUnitHover(movementUds, this);
                    }
                }
                
            }
        }
    }

    protected override void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (LM.selectedCharacter == null)
        {
            LM.HideUnitHover(this);          
            ResetColor();
        }
        else if (LM.selectedCharacter == this)
        {
            return;
        }
        else if (LM.selectedCharacter != this.gameObject)
        {
            LM.HideUnitHover(this);        
            ResetColor();

        }

        HideAttackEffect(this);
    }

    #endregion

    public override void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
    {
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();
        previousTileHeight = 0;

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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineUp[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineUp[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    //Si no hay obstáculo marco el tile para indicar el rango
                    if (!myCurrentTile.tilesInLineUp[i].isEmpty && !myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    }

                    if (myCurrentTile.tilesInLineUp[i].isEmpty)
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineDown[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineDown[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineDown[i].isEmpty && !myCurrentTile.tilesInLineDown[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    }

                    if (myCurrentTile.tilesInLineDown[i].isEmpty)
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineRight[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineRight[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineRight[i].isEmpty && !myCurrentTile.tilesInLineRight[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[i]);
                    }
                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    }

                    if (myCurrentTile.tilesInLineRight[i].isEmpty)
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineLeft[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineLeft[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineLeft[i].isEmpty && !myCurrentTile.tilesInLineLeft[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    }

                    if (myCurrentTile.tilesInLineLeft[i].isEmpty)
                    {
                        break;
                    }
                }
            }

        }

        if (myMage.mirrorDecoy && LM.selectedCharacter == myMage)
        {
            if (currentUnitsAvailableToAttack.Count > 0)
            {
                if (myMage.mirrorDecoy2)
                {
                    for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
                    {
                        if (currentUnitsAvailableToAttack[i] != null)
                        {
                            DoDamage(currentUnitsAvailableToAttack[i]);

                        }

                    }
                }
                else if (currentUnitsAvailableToAttack[0] != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0]);
                }
            }
        }
    }

    public void ChangePosition(Mage mage2Move)
    {
        IndividualTiles magePreviousTile = mage2Move.myCurrentTile;
        mage2Move.MoveToTilePushed(myCurrentTile);
        mage2Move.UpdateInformationAfterMovement(myCurrentTile);
        this.MoveToTilePushed(magePreviousTile);
        UpdateInformationAfterMovement(magePreviousTile);
        mage2Move.hasMoved = true;
        LM.UnitHasFinishedMovementAndRotation();
        UIM.RefreshTokens();

        if (mage2Move.isDecoyBomb2)
        {
            TM.GetSurroundingTiles(myCurrentTile, 1, true, false);
            
            //Hago daño a las unidades adyacentes
            for (int i = 0; i < TM.surroundingTiles.Count; ++i)
            {
                if (TM.surroundingTiles[i] != null)
                {
                    TM.surroundingTiles[i].ColorAttack();
                }
            }

            StartCoroutine("WaitToDamageSurroundingAfterChangePos");

        }

        HideAttackEffect(null);
    }

    IEnumerator WaitToDamageSurroundingAfterChangePos()
    {
        //Hago daño a las unidades adyacentes(3x3)
        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
        {
            if (TM.surroundingTiles[i].unitOnTile != null)
            {
                DoDamage(TM.surroundingTiles[i].unitOnTile);
            }
        }

        yield return new WaitForSeconds(2f);

        //Despinto los tiles
        for (int i = 0; i < TM.surroundingTiles.Count; ++i)
        {
            if (TM.surroundingTiles[i] != null)
            {
                TM.surroundingTiles[i].ColorDesAttack();
            }
        }

    }

    public void ChangePositionIconFeedback(bool has2Show)
    {
        if (has2Show)
        {
            if(myMage != null)
            {
                myMage.changePositionIcon.SetActive(true);

            }
            changePositionIcon.SetActive(true);

            
        }
        else
        {
            if (myMage != null)
            {
                myMage.changePositionIcon.SetActive(false);

            }
            changePositionIcon.SetActive(false);

        }

    }

    public  void CheckUnitsAndTilesToColorAtHover()
    {
       
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();
        previousTileHeight = 0;

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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineUp[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineUp[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    //Si no hay obstáculo marco el tile para indicar el rango
                    if (!myCurrentTile.tilesInLineUp[i].isEmpty && !myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[i]);
                    }

                    else
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineDown[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineDown[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineDown[i].isEmpty && !myCurrentTile.tilesInLineDown[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[i]);
                    }

                    else
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineRight[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineRight[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineRight[i].isEmpty && !myCurrentTile.tilesInLineRight[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[i]);
                    }
                    else
                    {
                        break;
                    }
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineLeft[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineLeft[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineLeft[i].isEmpty && !myCurrentTile.tilesInLineLeft[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[i]);
                    }

                    else
                    {
                        break;
                    }                 
                }
            }

        }

        if (myMage.mirrorDecoy)
        {
            
            if (myMage.mirrorDecoy2)
            {
                for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
                {
                    if (currentTilesInRangeForAttack[i] != null)
                    {
                        currentTilesInRangeForAttack[i].ColorAttack();

                        if(currentTilesInRangeForAttack[i].unitOnTile != null)
                        {
                            CalculateDamage(currentTilesInRangeForAttack[i].unitOnTile);
                            currentTilesInRangeForAttack[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                        }                      
                    }
                }
            }
            else if (currentTilesInRangeForAttack[0] != null)
            {
                for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
                {
                    if (currentTilesInRangeForAttack[i].unitOnTile != null)
                    {
                        CalculateDamage(currentTilesInRangeForAttack[i].unitOnTile);
                        currentTilesInRangeForAttack[i].ColorAttack();
                        currentTilesInRangeForAttack[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                        break;
                    }
                }
              
            }

        }

    


}

    //En este caso lo uso para ver lo que hace el decoy cuando el mago lee hace hover
    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        if (LM.selectedCharacter != null)
        {
            Cursor.SetCursor(LM.UIM.movementCursor, Vector2.zero, CursorMode.Auto);
            ChangePositionIconFeedback(true);

            if (myMage.isDecoyBomb2)
            {
                TM.GetSurroundingTiles(myMage.myCurrentTile, 1, true, false);

                //Hago daño a las unidades adyacentes
                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    if (TM.surroundingTiles[i] != null)
                    {
                        tilesInEnemyHover.Add(TM.surroundingTiles[i]);
                    }
                }

            }
        }
        else
        {
            if (myMage.isDecoyBomb)
            {
                TM.surroundingTiles.Clear();

                TM.GetSurroundingTiles(myCurrentTile, 1, true, false);

                //Hago daño a las unidades adyacentes
                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    if (TM.surroundingTiles[i] != null)
                    {

                        tilesInEnemyHover.Add(TM.surroundingTiles[i]);
                    }
                }

            }
           

        }

       

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
            }
        }
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        ChangePositionIconFeedback(false);

        if (myMage.mirrorDecoy)
        {

            for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
            {
                if (currentTilesInRangeForAttack[i] != null)
                {
                    currentTilesInRangeForAttack[i].ColorDesAttack();

                    if (currentTilesInRangeForAttack[i].unitOnTile != null)
                    {
                        currentTilesInRangeForAttack[i].unitOnTile.ResetColor();
                        currentTilesInRangeForAttack[i].unitOnTile.DisableCanvasHover();
                    }
                }
            }

            currentTilesInRangeForAttack.Clear();
        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorDesAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ResetColor();
            }
        }
        tilesInEnemyHover.Clear();
    }
}
