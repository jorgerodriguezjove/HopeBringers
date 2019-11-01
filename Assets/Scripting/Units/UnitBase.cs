using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitBase : MonoBehaviour
{
    #region VARIABLES

    [Header("STATS")]

    //Variable que se usará para ordenar a las unidades
    [SerializeField]
    public int speed;

    //Vida máxima que tiene cada unidad
    [SerializeField]
    public int maxHealth;

    //Uds movimiento máximas de la unidad.
    [SerializeField]
    public int movementUds;

    //Daño de la unidad
    [SerializeField]
    protected int damage;

    //Rango del ataque (en general será 1 a no ser que ataquen a distancia).
    [SerializeField]
    protected int range;


    [Header("LOGIC")]

    //Tile en el que está el personaje actualmente. Se setea desde el editor.
    public IndividualTiles myCurrentTile;

    //Enum con las cuatro posibles direcciones en las que puede estar mirando una unidad.
    [HideInInspector]
    public enum FacingDirection { North, East, South, West }

    //Dirección actual. ESTÁ EN SERIALIZEFIELD PARA PROBARLO.
    [SerializeField]
    public FacingDirection currentFacingDirection;

    //Posición a la que tiene que moverse la unidad actualmente
    protected Vector3 currentTileVectorToMove;

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    [SerializeField]
    protected float timePushAnimation;

    [Header("STATS GENÉRICOS")]

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByPush;

    //Daño que hace cada unidad por choque
    [SerializeField]
    protected int damageMadeByFall;

    //[Header("TEXT")]

    ////Texto que describe a la unidad.
    //[SerializeField]
    //public string characterDescription;

    ////Icono que aparece en la lista de turnos.
    //[SerializeField]
    //public Sprite unitIcon;

    ////Canvas que muestra la vida de la unidad
    //[SerializeField]
    //protected Canvas myCanvasHealthbar;

    #endregion

    #region COMMON_FUNCTIONS

    //Función para recibir daño
    public virtual void ReceiveDamage(int damageReceived)
    {
        //Cada unidad se resta vida con esta función.
        //Lo pongo en unit base para que sea genérico entre unidades y no tener que hacer la comprobación todo el rato.
    }

    //Función genérica que sirve para calcular a que tile debe ser empujada una unidad
    public void CalculatePushPosition(int numberOfTilesMoved, List<IndividualTiles> tilesToCheckForCollision, int attackersDamageByPush)
    {
        Debug.Log("Empuje");

        //Si no hay tiles en la lista me han empujado contra un borde
        if (tilesToCheckForCollision.Count == 0)
        {
            Debug.Log("borde");

            //Recibo daño 
            ReceiveDamage(attackersDamageByPush);

            //Hago animación de rebote
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
                    ReceiveDamage(attackersDamageByPush);

                    //Desplazo a la unidad
                    MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                    //Animación de rebote??

                    return;
                }

                //El tile al que empujo está más bajo (caída)
                else if (tilesToCheckForCollision[i].height < myCurrentTile.height)
                {

                    Debug.Log("caída");
                    //if (tilesToCheckForCollision[i].height -myCurrentTile.height < )
                    //{

                    //}

                    //Compruebo la altura de la que lo tiro 
                    //Compruebo si hay otra unidad
                    //Compruebo si es tile vacío y entonces cuenta simplemente cómo choque con pared
                    //Que pasa si hay un obstáculo en el tile de abajo?

                    return;
                }

                //Si la altura del tile al que empujo y la mía son iguales compruebo si el tile está vacío, es un obstáculo o tiene una unidad.
                else
                {
                    //Es tile vacío
                    if (tilesToCheckForCollision[i].isEmpty)
                    {
                        Debug.Log("vacío");
                        //Recibo daño 
                        ReceiveDamage(attackersDamageByPush);

                        // Desplazo a la unidad
                        MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                        //Animación de rebote??

                        return;
                    }

                    //Es tile con obstáculo
                    else if (tilesToCheckForCollision[i].isObstacle)
                    {
                        Debug.Log("obstáculo");
                        //Recibo daño 
                        ReceiveDamage(attackersDamageByPush);

                        //Desplazo a la unidad
                        MoveToTilePushed(tilesToCheckForCollision[i - 1]);

                        //Animación de rebote??

                        return;
                    }

                    //Es tile con unidad
                    else if (tilesToCheckForCollision[i].unitOnTile != null)
                    {
                        Debug.Log("otra unidad");
                        //Recibo daño 
                        ReceiveDamage(attackersDamageByPush);

                        //Hago daño a la otra unidad
                        tilesToCheckForCollision[i].unitOnTile.ReceiveDamage(attackersDamageByPush);

                        //Desplazo a la unidad
                        MoveToTilePushed(tilesToCheckForCollision[i-1]);

                        //Animación de rebote??

                        return;
                    }

                    Debug.Log(i);
                }
            }

            //Si sale del for entonces es que todos los tiles que tiene que comprobar son normales y simplemente lo muevo al último tile

            Debug.Log("normal");

            //Desplazo a la unidad
            MoveToTilePushed(tilesToCheckForCollision[numberOfTilesMoved]);
        }
    }

    //Función que ejecuta el movimiento del push
    private void MoveToTilePushed(IndividualTiles newTile)
    {
        //Mover al nuevo tile
        currentTileVectorToMove = new Vector3(newTile.tileX, newTile.height + 1, newTile.tileZ);
        transform.DOMove(currentTileVectorToMove, timePushAnimation).SetEase(Ease.OutElastic);

        //Aviso a los tiles del cambio de posición
        myCurrentTile.unitOnTile = null;
        myCurrentTile = newTile;
        myCurrentTile.unitOnTile = this;
    }

    #endregion

  
    //private void OnMouseEnter()
    //{
    //    myCanvasHealthbar.gameObject.SetActive(true);
    //}

    //private void OnMouseExit()
    //{
    //    myCanvasHealthbar.gameObject.SetActive(false);
    //}

    ////Enseña u oculta el rombo que indica a que personaje le toca (Hace lo contrario de lo actual.)
    //public void ShowAndHideArrow()
    //{
    //    myCanvasArrow.gameObject.SetActive(!myCanvasArrow.gameObject.activeSelf);
    //}

}
