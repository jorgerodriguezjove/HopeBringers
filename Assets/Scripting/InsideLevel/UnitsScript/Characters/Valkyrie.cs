using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Valkyrie : PlayerUnit
{
    public IndividualTiles previousTile;

    public GameObject changePosArrows;

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS

    //bool para la mejora de la activa 1
    public bool canChooseEnemy;

    //bool para la activa 2
    public bool armorMode;

    //bool para la mejora de la activa 2
    public bool armorMode2;

    //int que indica cuanto añade de armadura
    public int numberOfArmorAdded;

    [Header("Pasivas")]
    //PASIVAS

    //bool para la pasiva 1
    public bool changePositions;

    //bool para la mejora de la pasiva 1
    public bool changePositions2;

    //El número de vida la unidad al que deja la valquiria cambiarse por dicha unidad
    public int numberCanChange;

    //Estas son las partículas que salen cuando le queda poca vida a la unidad
    public GameObject valkHalo;
    public GameObject unitHalo;

    //Estas son las partículas que salen cuando la valquiria está seleccionada
    public GameObject valkHalo2;
    public GameObject unitHalo2;

    
    [SerializeField]
    public List<PlayerUnit> myHaloUnits = new List<PlayerUnit>();

    [SerializeField]
    public List<GameObject> myHaloInstancies = new List<GameObject>();

    protected override void OnMouseDown()
    {
        base.OnMouseDown();

        CheckValkyrieHalo();
    }

    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;


        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
            unitToAttack.monkMark.SetActive(false);
            currentHealth += FindObjectOfType<Monk>().healerBonus * unitToAttack.numberOfMarks;
            unitToAttack.numberOfMarks = 0;

            if (FindObjectOfType<Monk>().debuffMark2)
            {
                if (!unitToAttack.isStunned)
                {
                    StunUnit(unitToAttack, 1);
                }

            }
            else if (FindObjectOfType<Monk>().healerMark2)
            {
                ApplyBuffOrDebuffdamage(this, 1, 3);
              

            }
            UIM.RefreshTokens();

        }

        if (canChooseEnemy)
        {

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                if (currentUnitsAvailableToAttack[i] == unitToAttack)
                {
                    break;
                }
                else if (currentUnitsAvailableToAttack[i] != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[i]);
                }

            }


            //Hago daño antes de que se produzca el intercambio
            DoDamage(unitToAttack);

            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);

            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);

            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else if (armorMode)
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            if (unitToAttack.GetComponent<PlayerUnit>())
            {

                if (armorMode2)
                {
                    currentArmor += numberOfArmorAdded;
                    if (currentArmor > currentHealth)
                    {
                        currentArmor = currentHealth;
                    }
                }

                unitToAttack.currentArmor += numberOfArmorAdded;
                if (unitToAttack.currentArmor > unitToAttack.currentHealth)
                {
                    unitToAttack.currentArmor = unitToAttack.currentHealth;
                }
            }
            else
            {
                //Hago daño
                DoDamage(unitToAttack);

            }

            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);


            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);


            UIM.RefreshHealth();
            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);


        }
        else
        {

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            if (unitToAttack.GetComponent<PlayerUnit>())
            {


            }
            else
            {
                //Hago daño
                DoDamage(unitToAttack);

            }

            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);

            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);

           
            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {

        //Añado este if para el count de honor del samurai
        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
       || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
       || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
       || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
       )
        {
            LM.honorCount++;
        }
        base.DoDamage(unitToDealDamage);
    }

    public override void CheckUnitsAndTilesInRangeToAttack()
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
                        if (!canChooseEnemy)
                        {
                            break;
                        }

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

                        if (!canChooseEnemy)
                        {
                            break;
                        }
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

                        if (!canChooseEnemy)
                        {
                            break;
                        }
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

                        if (!canChooseEnemy)
                        {
                            break;
                        }
                    }

                    if (myCurrentTile.tilesInLineLeft[i].isEmpty)
                    {
                        break;
                    }
                }
            }

        }

        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            CalculateDamage(currentUnitsAvailableToAttack[i]);
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
            currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
            currentUnitsAvailableToAttack[i].myCurrentTile.ColorInteriorRed();

        }

        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }


    }

    public void ChangePosition(PlayerUnit unitToMove)
    {
        IndividualTiles valkyriePreviousTile = unitToMove.myCurrentTile;
        unitToMove.MoveToTilePushed(myCurrentTile);
        unitToMove.UpdateInformationAfterMovement(myCurrentTile);
        this.MoveToTilePushed(valkyriePreviousTile);
        UpdateInformationAfterMovement(valkyriePreviousTile);
        hasMoved = true;
        LM.UnitHasFinishedMovementAndRotation();
    }

    public void ChangePositionIconFeedback(bool has2Show, UnitBase otherUnit)
    {
        if (has2Show)
        {
            if (otherUnit != null)
            {
                otherUnit.changePositionIcon.SetActive(true);
            }
            changePositionIcon.SetActive(true);
        }
        else
        {
            if (otherUnit != null)
            {
                otherUnit.changePositionIcon.SetActive(false);
            }
            changePositionIcon.SetActive(false);
        }

    }
    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        if ((_unitToAttack.GetComponent<PlayerUnit>()) && !currentUnitsAvailableToAttack.Contains((_unitToAttack)))
        {
            Debug.Log("ha entrado");
            if ( LM.selectedCharacter == this && !hasMoved && changePositions)
            {
                if (_unitToAttack.currentHealth <= numberCanChange)
                {
                    ChangePositionIconFeedback(true, _unitToAttack);
                    Cursor.SetCursor(LM.UIM.movementCursor, Vector2.zero, CursorMode.Auto);
                }
            }
          
        }
        else
        {
            Debug.Log("No ha entrado");
            shaderHover.SetActive(true);
            Vector3 vector2Spawn = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            shaderHover.transform.position = vector2Spawn;

            _unitToAttack.shaderHover.SetActive(true);
            Vector3 vector2SpawnEnemy = new Vector3(_unitToAttack.transform.position.x, transform.position.y + 2.5f, _unitToAttack.transform.position.z);
            _unitToAttack.shaderHover.transform.position = vector2SpawnEnemy;

            changePosArrows.SetActive(true);
            changePosArrows.transform.position = Vector3.Lerp(vector2Spawn, vector2SpawnEnemy, 0.5f);
            changePosArrows.transform.position = new Vector3(changePosArrows.transform.position.x, transform.position.y + 4.5f, changePosArrows.transform.position.z);


            if (armorMode)
            {
                if (_unitToAttack.GetComponent<PlayerUnit>())
                {
                    if (armorMode2)
                    {
                        if (currentArmor > currentHealth)
                        {

                        }
                        else
                        {
                            canvasUnit.SetActive(true);
                            canvasUnit.GetComponent<CanvasHover>().damageNumber.SetText("+" + numberOfArmorAdded.ToString());
                        }
                    }

                    if (_unitToAttack.currentArmor > _unitToAttack.currentHealth)
                    {
                    }
                    else
                    {
                        _unitToAttack.canvasUnit.SetActive(true);
                        _unitToAttack.canvasUnit.GetComponent<CanvasHover>().damageNumber.SetText("+" + numberOfArmorAdded.ToString());
                    }
                }
            }

        }
        
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {

        if ((_unitToAttack.GetComponent<PlayerUnit>()) && !currentUnitsAvailableToAttack.Contains((_unitToAttack)))
        {

            ChangePositionIconFeedback(false, _unitToAttack);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);


        }
        else
        {

            if (armorMode)
            {
                if (_unitToAttack.GetComponent<PlayerUnit>())
                {
                    if (armorMode2)
                    {
                        canvasUnit.SetActive(false);
                    }
                    _unitToAttack.canvasUnit.SetActive(false);
                }
            }


        }
        shaderHover.SetActive(false);
        _unitToAttack.shaderHover.SetActive(false);
        changePosArrows.SetActive(false);


        
    }


    public void CheckValkyrieHalo()
    {
        if (LM.selectedCharacter == this)
        {

            valkHalo2.SetActive(true);
            valkHalo.SetActive(false);

             if (myHaloInstancies.Count > 0)
             {
                 for (int i = 0; i < myHaloInstancies.Count; i++)
                 {
                     Destroy(myHaloInstancies[i]);
                 }
                 myHaloInstancies.Clear();
             }
             
             if (myHaloUnits.Count>0)
             {
                 for (int i = 0; i < myHaloUnits.Count; i++)
                 {
                     if(myHaloUnits[i].currentHealth <= numberCanChange)
                     {
                     GameObject unitHaloRef = Instantiate(unitHalo2, myHaloUnits[i].transform.position, unitHalo2.transform.rotation);
                     myHaloInstancies.Add(unitHaloRef);
                     }                   
                 }
             }
                     
        }
        else
        {
            valkHalo.SetActive(true);
            valkHalo2.SetActive(false);

            if (myHaloInstancies.Count > 0)
            {
                for (int i = 0; i < myHaloInstancies.Count; i++)
                {
                    Destroy(myHaloInstancies[i]);
                }
                myHaloInstancies.Clear();
            }

            if (myHaloUnits.Count > 0)
            {
                for (int i = 0; i < myHaloUnits.Count; i++)
                {
                    if (myHaloUnits[i].currentHealth <= numberCanChange)
                    {
                        GameObject unitHaloRef = Instantiate(unitHalo, myHaloUnits[i].transform.position, unitHalo.transform.rotation);
                        myHaloInstancies.Add(unitHaloRef);
                    }
                }
            }
    }
    
}
}


