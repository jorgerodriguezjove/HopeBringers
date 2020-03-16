using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerUnit
{
    #region VARIABLES

    [Header("STATS DE CLASE")]

    [SerializeField]
    public int tilesToPush;

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS
    //Empuje en línea
    public bool pushFarther;
    //Empuje en línea mejorado (superempuje)
    public bool pushFarther2;


    //Empuje ancho
    public bool pushWider;
    //Empuje ancho mejorado (stunea a los enemigos atacados)
    public bool pushWider2;



    [Header("Pasivas")]
    //PASIVAS

    //Bloqueo individual (por los lados recibe menos daño)
    public bool lateralBlock;
    //Daño reducido para el knight por los lados
    public int damageLateralBlocked;
    //Bloqueo individual (por la espalda recibe menos daño)
    public bool backBlock;
    //Daño reducido para el knight por la espalda
    public int damageBackBlocked;

    //Los personajes a mi lado reciben menos daño si les atacan por delante
    public bool isBlockingNeighbours;
    //Daño reducido para los personajes atacados
    public int shieldDef;

    //Los personajes a mi lado reciben menos daño si les atacan por delante
    public bool isBlockingNeighboursFull;



    #endregion

    public void SetSpecificStats(bool _pushFarther, bool _pushWider)
    {
        pushFarther = _pushFarther;
        pushWider = _pushWider;
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.

    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
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

        //Este primer if  lo pongo de momento para seguir la misma estructura que con los otros personajes y por si hay que cambiar algo específico como la animación, el sonido...
        if (pushFarther)
        {
            if (pushFarther2)
            {
                //Hay que cambiarlo/quitarlo después si se quiere aumentar más
                tilesToPush = 3;
                myAnimator.SetTrigger("Attack");

               
                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineUp[tilesToPush + 1]);

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {

                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                        {
                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineUp[i].unitOnTile);
                            if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                            {
                                myCurrentTile.tilesInLineUp[i].unitOnTile.MoveToTilePushed(myCurrentTile.tilesInLineUp[i].unitOnTile.myCurrentTile.tilesInLineDown[0]);
                            }
                        }
                    }
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineUp[tilesToPush]);
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineDown[tilesToPush + 1]);

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {

                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                        {
                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineDown[i].unitOnTile);
                            if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                            {
                                myCurrentTile.tilesInLineDown[i].unitOnTile.MoveToTilePushed(myCurrentTile.tilesInLineDown[i].unitOnTile.myCurrentTile.tilesInLineUp[0]);
                            }
                        }
                    }
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineDown[tilesToPush]);
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineRight[tilesToPush + 1]);

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                        {
                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineRight[i].unitOnTile);
                            if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                            {

                                myCurrentTile.tilesInLineRight[i].unitOnTile.MoveToTilePushed(myCurrentTile.tilesInLineRight[i].unitOnTile.myCurrentTile.tilesInLineLeft[0]);
                            }
                        }
                    }
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineRight[tilesToPush]);
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineLeft[tilesToPush+1]);

                    for (int i = 0; i-1 < tilesToPush; i++)
                    {
                       
                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                        {
                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                            if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                            {                                
                                    myCurrentTile.tilesInLineLeft[i].unitOnTile.MoveToTilePushed(myCurrentTile.tilesInLineLeft[i].unitOnTile.myCurrentTile.tilesInLineRight[0]);                                
                            }
                        }
                    }
                    unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineLeft[tilesToPush]);
                }

                //Hago daño
                DoDamage(unitToAttack);


                SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
            }
            else
            {

                //Hay que cambiarlo/quitarlo después si se quiere aumentar más
                tilesToPush = 2;
                //Animación de ataque
                myAnimator.SetTrigger("Attack");

                //Hago daño
                DoDamage(unitToAttack);

                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
                }

                SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
            }
            

        }

        else if (pushWider)
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
            if (pushWider2)
            {
                StunUnit(unitToAttack, 1);
            }
            //Hago daño
            DoDamage(unitToAttack);


            if (currentFacingDirection == FacingDirection.North)
            {              
                

                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                       
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile, 1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                   

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineUp, damageMadeByPush, damageMadeByFall);
                      
                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                     StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile,1);
                    }

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineUp, damageMadeByPush, damageMadeByFall);
                    }
                }

                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile, 1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineDown, damageMadeByPush, damageMadeByFall);
                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile,1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineDown, damageMadeByPush, damageMadeByFall);
                    }
                }

               

                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                       StunUnit( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile,1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                     currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineRight, damageMadeByPush, damageMadeByFall);
                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                       StunUnit( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile,1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                     currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineRight, damageMadeByPush, damageMadeByFall);
                    }
                }              
                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
                
            }

            else if (currentFacingDirection == FacingDirection.West)            
            {

                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                       StunUnit( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile,1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall);
                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile,1);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall);
                    }
                }

                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);           
            }

            SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
        }

        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Hago daño
            DoDamage(unitToAttack);

            if (currentFacingDirection == FacingDirection.North)
            {
                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
            }

            SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
        }
                
        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {

        if (unitAttacker != null)
        {
            if (currentFacingDirection == FacingDirection.North && unitAttacker.currentFacingDirection == FacingDirection.South
             || currentFacingDirection == FacingDirection.East && unitAttacker.currentFacingDirection == FacingDirection.West
             || currentFacingDirection == FacingDirection.South && unitAttacker.currentFacingDirection == FacingDirection.North
             || currentFacingDirection == FacingDirection.West && unitAttacker.currentFacingDirection == FacingDirection.East)
            {
                //No recibe daño
                Debug.Log("bloqueado el ataque");
            }
            else if (lateralBlock)
            {
                //Este if sirve para comprobar en que dirección atacan los enemigos y en que dirección está el knight
                if ((currentFacingDirection == FacingDirection.North || currentFacingDirection == FacingDirection.South) 
                    && (unitAttacker.currentFacingDirection == FacingDirection.West || unitAttacker.currentFacingDirection == FacingDirection.East)
                    ||
                    (currentFacingDirection == FacingDirection.East || currentFacingDirection == FacingDirection.West)
                    && (unitAttacker.currentFacingDirection == FacingDirection.North || unitAttacker.currentFacingDirection == FacingDirection.South))
                {

                    damageReceived -= damageLateralBlocked;

                    base.ReceiveDamage(damageReceived, unitAttacker);
                }

                else if (backBlock)
                {

                    if (currentFacingDirection == FacingDirection.North && unitAttacker.currentFacingDirection == FacingDirection.North
                || currentFacingDirection == FacingDirection.East && unitAttacker.currentFacingDirection == FacingDirection.East
                || currentFacingDirection == FacingDirection.South && unitAttacker.currentFacingDirection == FacingDirection.South
                || currentFacingDirection == FacingDirection.West && unitAttacker.currentFacingDirection == FacingDirection.West)
                    {

                        damageReceived -= damageBackBlocked;

                        base.ReceiveDamage(damageReceived, unitAttacker);

                    }
                    else
                    {
                        base.ReceiveDamage(damageReceived, unitAttacker);
                    }
                }
                else
                {
                    base.ReceiveDamage(damageReceived, unitAttacker);
                }

            }
            else
            {
                base.ReceiveDamage(damageReceived, unitAttacker);
            }
        }

        //Si el atacante es null probablemente es un tile de daño o algo por el estilo
        else
        {
            base.ReceiveDamage(damageReceived, unitAttacker);
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
}
