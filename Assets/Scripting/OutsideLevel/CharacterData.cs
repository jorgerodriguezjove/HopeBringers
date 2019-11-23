using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterData : MonoBehaviour
{
    #region VARIABLES

    //Referencia a la unidad que representa la figura
    [SerializeField]
    public PlayerUnit myUnit;

    [SerializeField]
    public GameObject skillTreePrefab;

    //Coste actual de la siguiente mejora del personaje
    [SerializeField]
    public int powerLevel;

    //Lista que guarda las habilidades que han sido compradas
    [SerializeField]
    public List<int> idSkillsBought;

    //Bool que indica que el objeto ya ha sido inicializado anteriormente y que no debe ser destruido al borrar las copias redundantes por el dontDestroyOnload;
    [SerializeField]
    public bool initialized = false;

    [Header("Referencias")]
    [HideInInspector]
    private TableManager TM;

    #endregion

    #region INIT

    //Añado la función a la carga de escenas y hago que el objeto no se destruya entre escenas
    private void OnEnable()
    {
        SceneManager.sceneLoaded += UpdateInitialized;
        DontDestroyOnLoad(gameObject);
    }

    //Si se carga una escena que no es ni el menú ni el mapa (es decir se carga un nivel) se actualiza el bool para que al volver al mapa no se borre.
    public void UpdateInitialized(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != AppScenes.MAP_SCENE && scene.name != AppScenes.MENU_SCENE)
        {
            initialized = true;
            //Hacer desaparecer el modelo
        }

        else
        {
            //Hacer reaparecer el modelo 
        }
    }

    #endregion


    private void OnMouseDown()
    {
        //Si es null es porque al cambiar de escena ha desaparecido la referencia
        if (TM == null)
        {
            TM = FindObjectOfType<TableManager>();
            TM.OnClickCharacter(GetComponent<CharacterData>());
        }
        else
        {
            TM.OnClickCharacter(GetComponent<CharacterData>());
        }
    }

    //Al ser avisado de que se ha comprado una mejora aumento el powerlevel y guardo en la lista de ids la nueva habilidad para luego reactivar los nodos adecuados del árbol.
    public void UpgradeAcquired(int upgradeCost,int idSkill)
    {
        powerLevel += upgradeCost;
        idSkillsBought.Add(idSkill);
    }


    //int maxHealth;
    //int movementUds;
    //int baseDamage;
    //float multiplicatorBackAttack;
    //float multiplicatorMoreHeight;
    //float multiplicatorLessHeight;
    //int range;
    //float maxHeightDifferenceToAttack;
    //float maxHeightDifferenceToMove;
    //int damageMadeByPush;
    //int damageMadeByFall;


}



