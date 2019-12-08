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

    //Referencia al personaje que esta siendo mejorado actualemente
    private CharacterData currentCharacterUpgrading;

    //Objeto que se mueve entre los niveles e indica el nivel actual.
    private GameObject levelIndicator;

    //Nombre del nivel actual que hay que cargar si el jugador le da a ready
    [HideInInspector]
    public string currentClickedSceneName;

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

    #region CAMERA_MOVEMENT

    //Al seleccionar un nivel se setea todo para que aparezca la parte de selección de unidades
    public void OnLevelClicked(LevelNode levelClicked)
    {
        mapCamera.SetActive(false);
        selectCamera.SetActive(true);

        UIMM.SetLevelBookInfo(levelClicked);
        currentClickedSceneName = levelClicked.sceneName;
    }

    public void BackToLevelSelection()
    {
        ///EN EL FUTURO MOVER LA FICHA AQUÍ y cambiar el nombre del capítulo arriba a la izquierda y poner confirmación del nivel

        //Movimiento de cámara
        mapCamera.SetActive(true);
        selectCamera.SetActive(false);
        progresionCamera.SetActive(false);

        //Reseteo personajes seleccionados
        GameManager.Instance.unitsForCurrentLevel.Clear();
        currentCharacterUpgrading = null;

        UIMM.ResetCharacterUpbradeBookInfo();
    }

    public void MoveToUpgradeTable()
    {
        mapCamera.SetActive(false);
        progresionCamera.SetActive(true);
    }

    #endregion

    //Esto en el futuro se hará arrastrando al personaje pero de momento lo hago con click
    public void OnClickCharacter(CharacterData unitClicked)
    {
        if (selectCamera.activeSelf)
        {
            GameManager.Instance.unitsForCurrentLevel.Add(unitClicked.myUnit);
        }

        else if (progresionCamera.activeSelf)
        {
            UIMM.SetCharacterUpgradeBookInfo(unitClicked);
            currentCharacterUpgrading = unitClicked;
        }
        
    }

    public void BuyUpgrade(UpgradeNode upgradeClicked)
    {
        //Comprobar si tengo exp suficiente
        if (currentCharacterUpgrading.powerLevel <= GameManager.Instance.CurrentExp)
        {
            //Gastar Exp
            GameManager.Instance.CurrentExp -= currentCharacterUpgrading.powerLevel;

            //Avisar al nodo de mejora de que ha sido comprado  y Desbloquear los siguientes nodos
            upgradeClicked.UpgradeBought();

            //Aumentar power level del personaje
            //Añadir id a la lista del personaje
            currentCharacterUpgrading.UpgradeAcquired(upgradeClicked.upgradeCost,upgradeClicked.idUpgrade);
        }


        else
        {
            //¿Dar feedback de que no hay suficiente exp?
        }



    }


}
