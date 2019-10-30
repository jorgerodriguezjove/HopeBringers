using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUnit : UnitBase
{
    #region VARIABLES

    //Vida actual de la unidad.
    [HideInInspector]
    public int currentHealth;

    //Bools que indican si el personaje se ha movido y si ha atacado.
    [HideInInspector]
    public bool hasMoved = false;
    [HideInInspector]
    public bool hasAttacked = false;

    //Tiempo que tarda en rotar a la unidad.
    private float timeDurationRotation = 0.2f;

    //REFERENCIAS
    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;

    #endregion

    #region INIT

    private void Start()
    {
        currentHealth = maxHealth;
        LM = LevelManagerRef.GetComponent<LevelManager>();
        //Aviso al tile en el que empiezo que soy su unidad.
        myCurrentTile.unitOnTile = this;
    }

    #endregion

    #region INTERACTION


    private void OnMouseDown()
    {
        if (!hasMoved)
        {
            LM.SelectUnit(movementUds, this);

            //ESTO DEBERÍA COMPROBARLO AL MOVERSE Y AL EMPEZAR EL TURNO
            CheckUnitsInRangeToAttack();
        }

        
        if (!hasAttacked && !hasMoved)
        {
            //Avisa al LM
            //LM Comprueba que no hay unidad seleccionada actualmente y la selecciona
        }

        //Primero compruebo si me he movido

        //Avisar a LM de click

    }


    //En caso de querer generalizar la comprobación de en que dirección está un tile en comparación a mi posición, lo que se puede hacer es que la función no sea un void, si no que 
    //devuelva un valor de un enum como el de la rotación del personaje, de tal forma que los 4 ifs solo se ponen una vez y siempre devuelven una dirección

    //De momento esta función simplemente sirve para girar al personaje.
    public void CheckTileDirection(IndividualTiles tileToCheck)
    {
        //Arriba o abajo
        if (tileToCheck.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (tileToCheck.tileZ > myCurrentTile.tileZ)
            {
                transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
            //Abajo
            else
            {
                transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileToCheck.tileX > myCurrentTile.tileX)
            {
                transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
            //Izquierda
            else
            {
                transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
        }
    }

    //Variable que guarda el número más pequeño al comparar el rango con el número de tiles disponibles para atacar
    int rangeVSTilesInLineLimitant;

    //Comprueba las unidades (tanto aliadas como enemigas) que están en alcance para ser atacadas.
    public void CheckUnitsInRangeToAttack()
    {
        if (currentFacingDirection == FacingDirection.North)
        {
           if (range <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
           else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                {
                    //Puedo atacar
                    Debug.Log("up");
                    break;
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            if (range <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                {
                    //Puedo atacar
                    Debug.Log("down");
                    break;
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            if (range <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                {
                    //Puedo atacar
                    Debug.Log("right");
                    break;
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            if (range <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = range;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                {
                    //Puedo atacar
                    Debug.Log("left");
                    break;
                }
            }
        }
    }


    #endregion


}
