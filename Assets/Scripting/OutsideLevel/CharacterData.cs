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

    [Header("Stats genéricos")]
    [SerializeField]
    public int maxHealth;
    [SerializeField]
    public int movementUds;
    [SerializeField]
    public int baseDamage;
    [SerializeField]
    public int bonusBackAttack;
    [SerializeField]
    public int bonusMoreHeight;
    [SerializeField]
    public int bonusLessHeight;
    [SerializeField]
    public int range;
    [SerializeField]
    public float maxHeightDifferenceToAttack;
    [SerializeField]
    public float maxHeightDifferenceToMove;
    [SerializeField]
    public int damageMadeByPush;
    [SerializeField]
    public int damageMadeByFall;


    public bool A1;
    public bool A2;
    public bool A3;
    public bool B1;
    public bool B2;
    public bool B3;



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
            HideShowMeshCharacterData(true);
            
        }

        else
        {
            //Hacer reaparecer el modelo 
            HideShowMeshCharacterData(true);

            
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

    public void HideShowMeshCharacterData(bool isActive)
    {
        GetComponent<MeshRenderer>().enabled = isActive;
        GetComponent<Collider>().enabled = isActive;
    }

   
    public virtual void InitializeMyUnitStats()
    {
        //Cada Data se encarga de pasar los datos a su personaje
    }


    


}



