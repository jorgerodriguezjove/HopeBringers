﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    #region VARIABLES

    //Lista de unidades que se tienen que cargar en el nivel
    [HideInInspector]
    public List<PlayerUnit> unitsForCurrentLevel = new List<PlayerUnit>();

    //Experiencia actual. 
    ///El serialized y el igual a 100 es para testear.
    [SerializeField]
    public int CurrentExp = 100;

    //Lista que va a guardar todos los objetos que tengan el componente Character Data
    CharacterData[] oldCharacterDataList;

    //Array con todos los niveles del juego.
    [HideInInspector]
    public LevelNode[] allLevelNodes;

    //Lista con los ids de los niveles completados
    [HideInInspector]
    public List<int> levelIDsUnlocked = new List<int>();

    //Referencia al nodo del nivel que ha sido empezado
    public int currentLevelNode;

    #endregion

    #region INIT
    
    //Añado la función a la carga de escenas
    private void OnEnable()
    {
        SceneManager.sceneLoaded += RemoveOldCharacterData;
        SceneManager.sceneLoaded += UpdateLevelStates;
    }

    #endregion

    public void UpdateLevelStates(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == AppScenes.MAP_SCENE)
        {
            allLevelNodes = FindObjectsOfType<LevelNode>();

            for (int i = 0; i < allLevelNodes.Length; i++)
            {
                for (int j = 0; j < levelIDsUnlocked.Count; j++)
                {
                    if (allLevelNodes[i].idLevel == levelIDsUnlocked[j])
                    {
                        allLevelNodes[i].UnlockThisLevel();
                        allLevelNodes[i].UnlockConnectedLevels();
                        break;
                    }
                }
               
            }
        }
    }

    //Al cargar la escena de mapa borro los personajes desactualizados
    public void RemoveOldCharacterData(Scene scene, LoadSceneMode mode)
    {
        oldCharacterDataList = FindObjectsOfType<CharacterData>();

        //FALTA DESTRUIR LOS QUE YA EXISTEN EN LA ESCENA
        for (int i = 0; i < oldCharacterDataList.Length; i++)
        {
            if (!oldCharacterDataList[i].initialized)
            {
                Destroy(oldCharacterDataList[i].gameObject);
            }
        }
    }

    //Al completar un nivel el levelManager avisa de que en la escena de mapa va a tener que desbloquear niveles.
    public void VictoryAchieved()
    {
        levelIDsUnlocked.Add(currentLevelNode);
    }
    
}