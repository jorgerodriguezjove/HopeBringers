using System.Collections;
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

    #endregion

    #region INIT
    
    //Añado la función a la carga de escenas
    private void OnEnable()
    {
        SceneManager.sceneLoaded += RemoveOldCharacterData;
    }

    #endregion

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
}
