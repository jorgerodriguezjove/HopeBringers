using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    #region VARIABLES

    //Cámara del mapa
    [SerializeField]
    private GameObject mapCamera;
    //Cámara de la selección de unidades
    [SerializeField]
    private GameObject selectCamera;
    //Cámara de la progresión de unidades
    [SerializeField]
    private GameObject progresionCamera;

    [Header("Referencias")]
    [SerializeField]
    private UITableManager UIMM;

    #endregion

    #region INIT

    private void Start()
    {
        GameManager.Instance.unitsForCurrentLevel.Clear();
    }

    #endregion

    //Al seleccionar un nivel se setea todo para que aparezca la parte de selección de unidades
    public void MoveToCharacterSelectionForLevel(LevelNode levelClicked)
    {
        mapCamera.SetActive(false);
        selectCamera.SetActive(true);

        UIMM.SetBookInfo(levelClicked);
    }

    public void BackToLevelSelection()
    {
        ///EN EL FUTURO MOVER LA FICHA AQUÍ y cambiar el nombre del capítulo arriba a la izquierda y poner confirmación del nivel

        //Movimiento de cámara
        mapCamera.SetActive(true);
        selectCamera.SetActive(false);
        //progresionCamera.SetActive(false);

        //Reseteo personajes seleccionados
        GameManager.Instance.unitsForCurrentLevel.Clear();
    }


}
