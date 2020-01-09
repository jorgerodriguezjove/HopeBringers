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

    //La referencia a mi unidad dentro del nivel. (No puedo usar myUnit porque es  una referencia al prefab).
    //Cada personaje en su data se encarga de inicializarlo
    protected PlayerUnit myUnitReferenceOnLevel;

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

    //Objeto hijo que guarda el modelo del personaje
    [SerializeField]
    private GameObject unitModel;

    //Posición inicial en la caja de la unidad
    [SerializeField]
    public Vector3 initialPosition;

    [HideInInspector]
    public GameObject panelOfTheBookImIn;

    [Header("Stats genéricos")]

    [Header("Referencias")]
    [HideInInspector]
    private TableManager TM;

    //public bool A1;
    //public bool A2;
    //public bool A3;
    //public bool B1;
    //public bool B2;
    //public bool B3;

    //Diccionario con las mejoras de stats tipo int que tienen todos los personajes.
    public Dictionary<string,int> genericUpgrades = new Dictionary<string, int>();

    //Diccionario con las mejoras de stats tipo int que tiene cada personaje. Cada personaje inicializa las suyas en su script de Data.
    public Dictionary<string, int> specificIntCharacterUpgrades = new Dictionary<string, int>();
   
    //Diccionario con las mejoras de stats tipo bool que tiene cada personaje. Cada personaje inicializa las suyas en su script de Data.
    public Dictionary<string, bool> specificBoolCharacterUpgrades = new Dictionary<string, bool>();


    #endregion

    #region INIT

    //Añado la función a la carga de escenas y hago que el objeto no se destruya entre escenas
    private void OnEnable()
    {
        SceneManager.sceneLoaded += UpdateInitialized;
        DontDestroyOnLoad(gameObject);

        //Inicializo los diccionarios con los valores
        InitializeGenericUpgrades();
        InitializeSpecificUpgrades();

        
        initialPosition = gameObject.transform.position;
    }

    //Si se carga una escena que no es ni el menú ni el mapa (es decir se carga un nivel) se actualiza el bool para que al volver al mapa no se borre.
    public void UpdateInitialized(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != AppScenes.MAP_SCENE && scene.name != AppScenes.MENU_SCENE)
        {
            initialized = true;

            //Hacer desaparecer el modelo
            HideShowMeshCharacterData(false);
        }

        else
        {
            //Hacer reaparecer el modelo 
            HideShowMeshCharacterData(true);
        }
    }

    #endregion

    #region INIT_&_UPDATE_STATS
    
    //NO CONFUNDIR INIT CON UPDATE.
    //Init se encarga de crear la mejora dentro del diccionario.
    //Update se encarga de actualizar el valor de la variable que tiene el personaje en escena

    //Inicializo las mejoras genéricas en el diccionario para poder comprarlas
    private void InitializeGenericUpgrades()
    {
        genericUpgrades.Add(AppGenericUpgrades.maxHealth, myUnit.maxHealth);
        genericUpgrades.Add(AppGenericUpgrades.baseDamage, myUnit.baseDamage);
        genericUpgrades.Add(AppGenericUpgrades.attackRange, myUnit.attackRange);
        
        //Aquí es donde irían el resto de mejoras genéricas
    }

    //Cada personaje en su data inicializa las mejoras esepcíficas en el diccionario para poder comprarlas
    protected virtual void InitializeSpecificUpgrades()
    {
        //Cada data se encarga de inicializar las suyas
    }

    //Al colocar las unidades en el nivel se llama a esta función para actualizar los valores de las stats.
    public virtual void UpdateMyUnitStatsForTheLevel()
    {
        //Primero se llama al override para inicializar myUnitReferenceOnLevel.

        //Una vez inicializado el override llama al base para que todos usen esta misma función y se seteen las mejoras genéricas.
        myUnitReferenceOnLevel.SetMyGenericStats(genericUpgrades[AppGenericUpgrades.maxHealth], genericUpgrades[AppGenericUpgrades.baseDamage],
                                                 genericUpgrades[AppGenericUpgrades.attackRange]);
        //Después de esto vuelve al override donde se setean las mejoras especificas de cada personaje.
    }

    #endregion

    #region MISCELLANEOUS

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

    //Motrar u ocultar el modelo de la figura para que aparezca en LevelSelection pero no en los niveles.
    public void HideShowMeshCharacterData(bool isActive)
    {
        unitModel.SetActive(isActive);
        GetComponent<Collider>().enabled = isActive;
    }

    #endregion
}



