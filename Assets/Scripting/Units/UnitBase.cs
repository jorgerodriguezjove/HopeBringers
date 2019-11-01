using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    #region VARIABLES

    //STATS--------------------------------------------------------------------

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

    //LOGIC--------------------------------------------------------------------

    //Tile en el que está el personaje actualmente. Se setea desde el editor.
    public IndividualTiles myCurrentTile;

    //Enum con las cuatro posibles direcciones en las que puede estar mirando una unidad.
    [HideInInspector]
    public enum FacingDirection { North, East, South, West }

    //Dirección actual. ESTÁ EN SERIALIZEFIELD PARA PROBARLO.
    [SerializeField]
    public FacingDirection currentFacingDirection;

    //TEXT--------------------------------------------------------------------

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

    //Función genérica que sirve para que las unidades se muevan al ser empujadas.
    public void MoveByPush(int numberOfTilesMoved, List<IndividualTiles> tilesToCheckForCollision)
    {
        //Comprobar si tiles vacios
        //Comprobar si tiles con obstáculo
        //Comprobar tiles con unidad
        //Comprobar si es borde
        //Comprobar si choca con tile más alto

        for (int i = 0; i < tilesToCheckForCollision.Count; i++)
        {
            if (tilesToCheckForCollision.Count == 0)
            {
                //Es un borde
            }

            //else if (tilesToCheckForCollision[i].)
            //{

            //}

            //else if ()
            //{

            //}

            //else if ()
            //{

            //}
        }
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
