using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringBanks : MonoBehaviour
{
   
}

//Nombres de escenas.
public class AppScenes
{
    public static readonly string MENU_SCENE = "MainMenu";
    public static readonly string GAME_SCENE = "GameScene";
    public static readonly string CREDITS_SCENE = "Credits";

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
    public static readonly string OPENDOOR_SFX = "OPENDOOR_SFX";

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
