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
    #region Active
    public static readonly string initialActiveText = "El caballero hace daño a su objetivo y le empuja una casilla";
    public static readonly string initialPasiveText = "El caballero no recibe daño por delante";

    //EMPUJAR MÁS LEJOS

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string pushFurther1 = "pushFurther1";
    public static readonly string pushFurther2 = "pushFurther2";

    ///Textos
    public static readonly string pushFurther1Text = "The knight improves his shield so he can push enemies two tiles further";
    public static readonly string pushFurther2Text = "El enemigo empujado atraviesa al resto de enemigos en su camino haciendoles daño";

    //EMPUJAR MÁS ENEMIGOS
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string pushWider1 = "pushWider1";
    public static readonly string pushWider2 = "pushWider2";

    ///Textos
    public static readonly string pushWider1Text = "El caballero es capaz de empujar a los enemigos laterales de su objetivo";
    public static readonly string pushWider2Text = "El caballero stunea a los enemigos atacados";

    #endregion


    #region Pasive
    //EMPUJAR MÁS LEJOS

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string individualBlock1 = "individualBlock1";
    public static readonly string individualBlock2 = "individualBlock2";

    ///Textos
    public static readonly string individualBlock1Text = "Recibe menos daño por los lados";
    public static readonly string individualBlock2Text = "Recibe menos daño por la espalda";


    //EMPUJAR MÁS ENEMIGOS
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string neighbourBlock1 = "neighbourBlock1";
    public static readonly string neighbourBlock2 = "neighbourBlock2";

    ///Textos
    public static readonly string neighbourBlock1Text = "Los aliados adyacentes al caballero reciben menos daño si el ataque proviene en la dirección contraria al escudo";
    public static readonly string neighbourBlock2Text = "Los aliados no reciben daño al ser protegidos";

    #endregion
}

public class AppRogueUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //MULTI JUMP

    ///Mejoras
    public static readonly string multiJumpAttack1 = "multiJumpAttack1";
    public static readonly string multiJumpAttack2 = "multiJumpAttack2";

    ///Textos
    public static readonly string multiJumpAttack1Text = "";
    public static readonly string multiJumpAttack2Text = "";


    //EXTRA TURN
    public static readonly string extraTurnAfterKill1 = "extraTurnAfterKill1";
    public static readonly string extraTurnAfterKill2 = "extraTurnAfterKill2";

    ///Textos
    public static readonly string extraTurnAfterKill1Text = "";
    public static readonly string extraTurnAfterKill2Text = "";

    #endregion


    #region Pasive
    //BOMBA HUMO

    ///Mejoras
    public static readonly string smokeBomb1 = "smokeBomb1";
    public static readonly string smokeBomb2 = "smokeBomb2";

    ///Textos
    public static readonly string smokeBomb1Text = "";
    public static readonly string smokeBomb2Text = "";


    //BUFO DAÑO
    public static readonly string buffDamageKill1 = "buffDamageKill1";
    public static readonly string buffDamageKill2 = "buffDamageKill2";

    ///Textos
    public static readonly string buffDamageKill1Text = "";
    public static readonly string buffDamageKill2Text = "";

    #endregion

}

public class AppMageUpgrades
{
    public static readonly string initialActiveText = "El mago ataca a un enemigo hasta cinco casillas de distancia";
    public static readonly string initialPasiveText = "Al moverse el mago deja un señuelo en su posición anterior";

    #region Active
    //CADENA DE RAYOS

    ///Mejoras
    public static readonly string lightningChain1 = "lightningChain1";
    public static readonly string lightningChain2 = "lightningChain2";

    ///Textos
    public static readonly string lightningChain1Text = "Al atacar a un objetivo, los personajes adyacentes a este reciben daño formando una cadena de rayos";
    public static readonly string lightningChain2Text = "El daño aumenta con cada eslabón de la cadena dañado hasta un máximo. Los alidos además no reciben daño pero mantienen la cadena";


    //AREA
    public static readonly string crossAreaAttack1 = "crossAreaAttack1";
    public static readonly string crossAreaAttack2 = "crossAreaAttack2";

    ///Textos
    public static readonly string crossAreaAttack1Text = "Al atacar a un objetivo, todos los personajes que le rodeen reciben daño";
    public static readonly string crossAreaAttack2Text = "El área de ataque aumenta hasta incluir las diagonales del objetivo";

    #endregion


    #region Pasive
    //BOMBA

    ///Mejoras
    public static readonly string bombDecoy1 = "bombDecoy1";
    public static readonly string bombDecoy2 = "bombDecoy2";

    ///Textos
    public static readonly string bombDecoy1Text = "";
    public static readonly string bombDecoy2Text = "";


    //MIRROR
    public static readonly string mirrorDecoy1 = "mirrorDecoy1";
    public static readonly string mirrorDecoy2 = "mirrorDecoy2";

    ///Textos
    public static readonly string mirrorDecoy1Text = "";
    public static readonly string mirrorDecoy2Text = "";

    #endregion
}

public class AppBerserkUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //DOUBLE ATTACK

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string areaAttack1 = "areaAttack1";
    public static readonly string areaAttack2 = "areaAttack2";

    ///Textos
    public static readonly string areaAttack1Text = "";
    public static readonly string areaAttack2Text = "";

    //CIRCULAR ATTACK
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string circularAttack1 = "circularAttack1";
    public static readonly string circularAttack2 = "circularAttack2";

    ///Textos
    public static readonly string circularAttack1Text = "";
    public static readonly string circularAttack2Text = "";

    #endregion

    #region Pasive
    //RAGE

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string rageDamage1 = "rageDamage1";
    public static readonly string rageDamage2 = "rageDamage2";

    ///Textos
    public static readonly string rageDamage1Text = "";
    public static readonly string rageDamage2Text = "";


    //FEAR
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string fearRage1 = "fearRage1";
    public static readonly string fearRage2 = "fearRage2";

    ///Textos
    public static readonly string fearRage1Text = "";
    public static readonly string fearRage2Text = "";

    #endregion
}

public class AppSamuraiUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //PARRY

    ///Mejoras
    public static readonly string parry1 = "parry1";
    public static readonly string parry2 = "parry2";

    ///Textos
    public static readonly string parry1Text = "";
    public static readonly string parry2Text = "";

    //MULTI ATTACK
    public static readonly string multiAttack1 = "multiAttack1";
    public static readonly string multiAttack2 = "multiAttack2";

    ///Textos
    public static readonly string multiAttack1Text = "";
    public static readonly string multiAttack2Text = "";

    #endregion

    #region Pasive
    //HONOR

    ///Mejoras
    public static readonly string honor1 = "honor1";
    public static readonly string honor2 = "honor2";

    ///Textos
    public static readonly string honor1Text = "";
    public static readonly string honor2Text = "";


    //SOLITARIO
    public static readonly string loneWolf1 = "loneWolf1";
    public static readonly string loneWolf2 = "loneWolf2";

    ///Textos
    public static readonly string loneWolf1Text = "";
    public static readonly string loneWolf2Text = "";

    #endregion
}

public class AppDruidUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //MORE HEAL

    ///Mejoras
    public static readonly string heal1 = "heal1";
    public static readonly string heal2 = "heal2";

    ///Textos
    public static readonly string heal1Text = "";
    public static readonly string heal2Text = "";

    //AREA HEAL
    public static readonly string areaHeal1 = "areaHeal1";
    public static readonly string areaHeal2 = "areaHeal2";

    ///Textos
    public static readonly string areaHeal1Text = "";
    public static readonly string areaHeal2Text = "";

    #endregion

    #region Pasive
    //TILE HEAL

    ///Mejoras
    public static readonly string tile1 = "tile1";
    public static readonly string tile2 = "tile2";

    ///Textos
    public static readonly string tile1Text = "";
    public static readonly string tile2Text = "";


    //TILE MOVEMENT
    public static readonly string tileMovement1 = "tileMovement1";
    public static readonly string tileMovement2 = "tileMovement2";

    ///Textos
    public static readonly string tileMovement1Text = "";
    public static readonly string tileMovement2Text = "";

    #endregion
}

public class AppMonkUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //TURN 180 AND CHAIN MARK

    ///Mejoras
    public static readonly string turn1 = "turn1";
    public static readonly string turn2 = "turn2";

    ///Textos
    public static readonly string turn1Text = "";
    public static readonly string turn2Text = "";

    //SUPLEX AND UPGRADED MARK
    public static readonly string suplex1 = "suplex1";
    public static readonly string suplex2 = "suplex2";

    ///Textos
    public static readonly string suplex1Text = "";
    public static readonly string suplex2Text = "";

    #endregion

    #region Pasive
    //MARK DEBUFF AND STUN

    ///Mejoras
    public static readonly string markDebuff1 = "markDebuff1";
    public static readonly string markDebuff2 = "markDebuff2";

    ///Textos
    public static readonly string markDebuff1Text = "";
    public static readonly string markDebuff2Text = "";

    //MORE HEAL AND BUFF PLAYER
    public static readonly string markBuff1 = "markBuff1";
    public static readonly string markBuff2 = "markBuff2";

    ///Textos
    public static readonly string markBuff1Text = "";
    public static readonly string markBuff2Text = "";

    #endregion
}

public class AppValkyrieUpgrades
{
    public static readonly string initialActiveText = "";
    public static readonly string initialPasiveText = "";

    #region Active
    //MORE RANGE

    ///Mejoras
    public static readonly string moreRange1 = "moreRange1";
    public static readonly string moreRange2 = "moreRange2";

    ///Textos
    public static readonly string moreRange1Text = "";
    public static readonly string moreRange2Text = "";

    //ARMOUR CHANGE
    public static readonly string armorChange1 = "armorChange1";
    public static readonly string armorChange2 = "armorChange2";

    ///Textos
    public static readonly string armorChange1Text = "";
    public static readonly string armorChange2Text = "";

    #endregion

    #region Pasive
    //SUSTITUTION

    ///Mejoras
    public static readonly string sustitution1 = "sustitution1";
    public static readonly string sustitution2 = "sustitution2";

    ///Textos
    public static readonly string sustitution1Text = "";
    public static readonly string sustitution2Text = "";

    //HEIGHT
    public static readonly string height1 = "height1";
    public static readonly string height2 = "height2";

    ///Textos
    public static readonly string height1Text = "";
    public static readonly string height2Text = "";

    #endregion
}

#endregion

//Nombres de escenas.
public class AppScenes
{
    public static readonly string MENU_SCENE = "MainMenu";
    public static readonly string LOAD_SCENE = "LoadingScene";
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
    public static readonly string QUALITY_LEVEL = "QualityLevel";
    public static readonly string RESOLUTION = "Resolution";
    public static readonly string FULLSCREEN = "FullScreen";
    //public static readonly string VSYNC = "Vsync";
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

//Nombres de escenas.
public class AppAchievements
{
    public static readonly string ACHV_BEGIN = "achievement_00";

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
