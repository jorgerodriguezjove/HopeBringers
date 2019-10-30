using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    #region VARIABLES

    //Tile en el que está el personaje actualmente. Se setea desde el editor.
    public IndividualTiles myCurrentTile;

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

    //Enum con las cuatro posibles direcciones en las que puede estar mirando una unidad.
    [HideInInspector]
    public enum FacingDirection { North, East, South, West }

    //Dirección actual. ESTÁ EN SERIALIZEFIELD PARA PROBARLO.
    [SerializeField]
    public FacingDirection currentFacingDirection;



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
