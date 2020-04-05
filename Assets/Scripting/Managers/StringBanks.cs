using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringBanks : MonoBehaviour
{
   
}

#region CHARACTER_UPGRADES

public class AppGenericUpgrades
{
    //Mejoras pequeñas
    public static readonly string maxHealth = "maxHealth";
    public static readonly string baseDamage = "baseDamage";
    public static readonly string attackRange = "attackRange";

    //Mejoras medianas
    public static readonly string movementUds = "movementUds";
    public static readonly string bonusBackAttack = "bonusBackAttack";
    public static readonly string bonusMoreHeight = "bonusMoreHeight";
    public static readonly string bonusLessHeight = "bonusLessHeight";
    public static readonly string maxHeightDifferenceToAttack = "maxHeightDifferenceToAttack";
    public static readonly string maxHeightDifferenceToMove = "maxHeightDifferenceToMove";
    public static readonly string damageMadeByPush = "damageMadeByPush";
    public static readonly string damageMadeByFall = "damageMadeByFall";
}


public class AppKnightUpgrades
{
    public static readonly string pushFurther1 = "pushFurther1";
    public static readonly string pushWider1 = "pushWider1";



    //Textos de mejoras activas
    public static readonly string genericAttackText = "El caballero hace daño a su objetivo y le empuja una casilla";

    public static readonly string pusFurther1Text = "El caballero es capaz de empujar a sus objetivos a dos casillas de distancia";
    public static readonly string pushWider1Text = "El caballero es capaz de empujar a los enemigos laterales de su objetivo";
    //Textos de mejoras pasivas

}

public class AppMageUpgrades
{
    public static readonly string lightningChain1 = "lightningChain1";
    public static readonly string crossAreaAttack1 = "crossAreaAttack1";
}

public class AppRogueUpgrades
{
    public static readonly string multiJumpAttack1 = "multiJumpAttack1";
    public static readonly string extraTurnAfterKill1 = "extraTurnAfterKill1";
}

public class AppBerserkUpgrades
{
    public static readonly string doubleAttack1 = "doubleAttack1";
    public static readonly string circularAttack1 = "circularAttack1";
}

#endregion

//Nombres de escenas.
public class AppScenes
{
    public static readonly string MENU_SCENE = "MainMenu";
    public static readonly string GAME_SCENE = "GameScene";
    public static readonly string CREDITS_SCENE = "Credits";
    public static readonly string MAP_SCENE = "LevelSelection";

}

//Paths a carpetas dentro del proyecto.
public class AppPaths
{
    public static readonly string PERSISTENT_DATA = Application.persistentDataPath;
    public static readonly string PATH_RESOURCE_SFX = "Audio/Sounds/";
    public static readonly string PATH_RESOURCE_MUSIC = "Audio/Music/";
}

//Nombres de PlayerPrefKeys
public class AppPlayerPrefKeys
{
    public static readonly string MUSIC_VOLUME = "MusicVolume";
    public static readonly string SFX_VOLUME = "SfxVolume";
}

//Nombres de los sonidos en la carpeta de resources.
public class AppSounds
{
    public static readonly string MOVEMENT = "Gen_Move_1";
    public static readonly string EN_DEATH = "En_Death";
    public static readonly string COLLISION = "Gen_Collision";
    public static readonly string KNIGHT_ATTACK = "Knight_Attack";
    public static readonly string MAGE_ATTACK = "Mage_Attack";
    public static readonly string PLAYER_SELECTION = "Player_Selection";
    public static readonly string ROGUE_ATTACK = "Rogue_Attack";

}

//Nombres de la música en la carpeta de resources.
public class APppMusic
{
    //public static readonly string INTRO_MUSIC = "INTRO_MUSIC";
   
}

//Nombres de Inputs Para el futuro.
//public class AppInputs
//{
//    public static readonly string A = "FireA";
//    public static readonly string X = "FireX";
//    public static readonly string B = "Cancel";
//    public static readonly string Y = "FireY";
//    public static readonly string Joystick = "Horizontal";
//    public static readonly string JoystickV = "Vertical"; //Prueba
//    public static readonly string Pause = "Start";
//}
