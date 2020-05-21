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
	public static readonly string KnightDataBaseActive = "Knight deals damage to the target enemy and pushes it back one tile";
	public static readonly string KnightDataBasePasive = "Knight does not recieve damage if he gets hit from the front";
	#region Active
	//EMPUJAR MÁS LEJOS

	///Mejoras a nivel lógico y nombre de ICONOS
	public static readonly string pushFurther1 = "pushFurther1";
    public static readonly string pushFurther2 = "pushFurther2";

    ///Textos
    public static readonly string pushFurther1Text = "Knight pushes enemies two tiles further";
    public static readonly string pushFurther2Text = "Knight pushes enemies two tiles further, the pushed enemy goes through any enemies on its way dealing them damage";

    //EMPUJAR MÁS ENEMIGOS
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string pushWider1 = "pushWider1";
    public static readonly string pushWider2 = "pushWider2";

    ///Textos
    public static readonly string pushWider1Text = "Knight also pushes the two enemies at the sides or his target";
    public static readonly string pushWider2Text = "Knight also pushes the two enemies at the sides or his target, and stuns the pushed enemies";

    #endregion


    #region Pasive
    //EMPUJAR MÁS LEJOS

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string individualBlock1 = "individualBlock1";
    public static readonly string individualBlock2 = "individualBlock2";

    ///Textos
    public static readonly string individualBlock1Text = "Knight recieves reduced damaged when he gets hit by his sides";
    public static readonly string individualBlock2Text = "Knight recieves reduced damaged when he gets hit by his sides and his back";


    //EMPUJAR MÁS ENEMIGOS
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string neighbourBlock1 = "neighbourBlock1";
    public static readonly string neighbourBlock2 = "neighbourBlock2";

    ///Textos
    public static readonly string neighbourBlock1Text = "Knight protects the allies at his sides, making them to recieve less damage of attacks coming from his front";
    public static readonly string neighbourBlock2Text = "Knight protects the allies at his sides, making them to recieve no damage of attacks coming from his front";

    #endregion
}

public class AppRogueUpgrades
{
    public static readonly string RogueDataBaseActive = "Rogue jumps into the opposite tile to the one he is in before attacking";
    public static readonly string RogueDataBasePasive = "Rogue can attack from every direction, not only his front";

    #region Active
    //MULTI JUMP

    ///Mejoras
    public static readonly string multiJumpAttack1 = "multiJumpAttack1";
    public static readonly string multiJumpAttack2 = "multiJumpAttack2";

    ///Textos
    public static readonly string multiJumpAttack1Text = "Rogue can attack again after attacking if he has any valid target where he landed";
    public static readonly string multiJumpAttack2Text = "Rogue can attack multiple times after attacking if he has a valid target";


    //EXTRA TURN
    public static readonly string extraTurnAfterKill1 = "extraTurnAfterKill1";
    public static readonly string extraTurnAfterKill2 = "extraTurnAfterKill2";

    ///Textos
    public static readonly string extraTurnAfterKill1Text = "Rogue gets an extra turn after killing an enemy, one time only";
    public static readonly string extraTurnAfterKill2Text = "Rogue gets an extra turn after killing an enemy, two times";

    #endregion


    #region Pasive
    //BOMBA HUMO

    ///Mejoras
    public static readonly string smokeBomb1 = "smokeBomb1";
    public static readonly string smokeBomb2 = "smokeBomb2";

    ///Textos
    public static readonly string smokeBomb1Text = "When killing an enemy, Rogue drops a smoke bomb in the tile he is. This makes enemies loose track of any unit inside of it";
    public static readonly string smokeBomb2Text = "When killing an enemy, Rogue drops a 3x3 tile area smoke bomb with center in the tile he is. This makes enemies loose track of any unit inside of it";


    //BUFO DAÑO
    public static readonly string buffDamageKill1 = "buffDamageKill1";
    public static readonly string buffDamageKill2 = "buffDamageKill2";

    ///Textos
    public static readonly string buffDamageKill1Text = "Rogue gets a +1 damage buff when killing an enemy";
    public static readonly string buffDamageKill2Text = "Rogue gets a +2 damage buff when killing an enemy";

    #endregion

}

public class AppMageUpgrades
{
    public static readonly string MageDataBaseActive = "Mage can target units as far as 5 tiles from him";
    public static readonly string MageDataBasePasive = "When moving, Mage leaves a decoy at his previous position. The mage can interchange positions with the decoy";

    #region Active
    //CADENA DE RAYOS

    ///Mejoras
    public static readonly string lightningChain1 = "lightningChain1";
    public static readonly string lightningChain2 = "lightningChain2";

    ///Textos
    public static readonly string lightningChain1Text = "When attacking a target, all adjacent units to it recieve damage, this keeps happening until the last damaged units have no more adjacent units";
    public static readonly string lightningChain2Text = "Damage increases with each time new targets recieve damage, with a limit. Allies don't recieve damage from the chain anymore";


    //AREA
    public static readonly string crossAreaAttack1 = "crossAreaAttack1";
    public static readonly string crossAreaAttack2 = "crossAreaAttack2";

    ///Textos
    public static readonly string crossAreaAttack1Text = "When attacking a target, all units adjacent to it recieve damage";
    public static readonly string crossAreaAttack2Text = "When attacking a target, all units in it's surrounding tiles recieve damage";

    #endregion


    #region Pasive
    //BOMBA

    ///Mejoras
    public static readonly string bombDecoy1 = "bombDecoy1";
    public static readonly string bombDecoy2 = "bombDecoy2";

    ///Textos
    public static readonly string bombDecoy1Text = "When defeated, the decoy makes damage in a 3x3 tile area with center on it";
    public static readonly string bombDecoy2Text = "When the mage changes positions with the decoy, it explodes";


    //MIRROR
    public static readonly string mirrorDecoy1 = "mirrorDecoy1";
    public static readonly string mirrorDecoy2 = "mirrorDecoy2";

    ///Textos
    public static readonly string mirrorDecoy1Text = "When the mage attacks, the decoy attacks the first enemy in range in the direction it is looking";
    public static readonly string mirrorDecoy2Text = "When the mage attacks, the decoy attacks every enemy in range in the direction it is looking";

    #endregion
}

public class AppBerserkUpgrades
{
    public static readonly string BerserkDataBaseActive = "Berserk attacks a target in front of him";
    public static readonly string BerserkDataBasePasive = "When Berserk recieves damage, he enters in rage for 3 turns obtaining a +1 damage buff";

    #region Active
    //DOUBLE ATTACK

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string areaAttack1 = "areaAttack1";
    public static readonly string areaAttack2 = "areaAttack2";

    ///Textos
    public static readonly string areaAttack1Text = "Berserk attacks in a 3x1 tile area in front of him";
    public static readonly string areaAttack2Text = "Berserk attacks in a 3x1 tile area in front of him, and deals more damage with it";

    //CIRCULAR ATTACK
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string circularAttack1 = "circularAttack1";
    public static readonly string circularAttack2 = "circularAttack2";

    ///Textos
    public static readonly string circularAttack1Text = "When attacking, Berserk also attacks all the units in his adjacent tiles";
    public static readonly string circularAttack2Text = "When attacking, Berserk also attacks all the units in his adjacent tiles. Berserk no does this twice";

    #endregion

    #region Pasive
    //RAGE

    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string rageDamage1 = "rageDamage1";
    public static readonly string rageDamage2 = "rageDamage2";

    ///Textos
    public static readonly string rageDamage1Text = "When Berserk recieves damage, he enters in rage for 3 turns obtaining a +2 damage buff";
    public static readonly string rageDamage2Text = "When Berserk recieves damage, he enters in rage for 3 turns obtaining a +3 damage buff";


    //FEAR
    ///Mejoras a nivel lógico y nombre de ICONOS
    public static readonly string fearRage1 = "fearRage1";
    public static readonly string fearRage2 = "fearRage2";

    ///Textos
    public static readonly string fearRage1Text = "If Berserk attacks a target while in rage, the target gets a -1 damage debuff for 1 turn";
    public static readonly string fearRage2Text = "If Berserk attacks a target while in rage, the target gets a -1 damage debuff for 2 turns";

    #endregion
}

public class AppSamuraiUpgrades
{
    public static readonly string SamuraiDataBaseActive = "Samurai attacks a target in front of him";
    public static readonly string SamuraiDataBasePasive = "Samurai has a +1 damage buff when attacking enemies from their front, but he can't attack them form their back";

    #region Active
    //PARRY

    ///Mejoras
    public static readonly string parry1 = "parry1";
    public static readonly string parry2 = "parry2";

    ///Textos
    public static readonly string parry1Text = "When attacking a target, Samurai prepares his attack. When is about to get hit, he cancels the damage dealt by the target and deals it back to it";
    public static readonly string parry2Text = "When attacking a target, Samurai prepares his attack. When is about to get hit, he cancels the damage dealt by the target and deals it back to it. " +
		"Also, if with his attack prepared recieves damage from his sides, he attacks again to the target in the front";

    //MULTI ATTACK
    public static readonly string multiAttack1 = "multiAttack1";
    public static readonly string multiAttack2 = "multiAttack2";

    ///Textos
    public static readonly string multiAttack1Text = "Samurai attacks a target in front of him 3 times";
    public static readonly string multiAttack2Text = "Samurai attacks a target in front of him 5 times";

    #endregion

    #region Pasive
    //HONOR

    ///Mejoras
    public static readonly string honor1 = "honor1";
    public static readonly string honor2 = "honor2";

    ///Textos
    public static readonly string honor1Text = "When Samurai and his allies attack enemies from the front, the Honor counter increases one point. " +
		"Any ally attacking an enemy from the back resets the counter. Each point is a +1 damage buff to Samurai";
    public static readonly string honor2Text = "When Samurai and his allies attack enemies from the front, the Honor counter increases one point. " +
		"Any ally attacking an enemy from the back resets the counter. Each point is a +1 damage buff to all the party";


    //SOLITARIO
    public static readonly string loneWolf1 = "loneWolf1";
    public static readonly string loneWolf2 = "loneWolf2";

    ///Textos
    public static readonly string loneWolf1Text = "When there are no allies in a 3 x 3 tile area with center in Samurai, he recieves a +1 damage buff";
    public static readonly string loneWolf2Text = "When there are no allies in a 3 x 3 tile area with center in Samurai, he recieves a +2 damage buff";

    #endregion
}

public class AppDruidUpgrades
{
	public static readonly string DruidDataBaseActive = "Druid can target units as far as 5 tiles from her. When targeting an ally, she loses 1 hp to heal it. " +
		"When targeting an enemy, Druid drains 1 hp of it";
    public static readonly string DruidDataBasePasive = "Special tiles have no effect on Druid";

    #region Active
    //MORE HEAL

    ///Mejoras
    public static readonly string heal1 = "heal1";
    public static readonly string heal2 = "heal2";

    ///Textos
    public static readonly string heal1Text = "Druid heals more hp to her allies";
    public static readonly string heal2Text = "Druid heals more hp to her allies and gives them a +2 movement buff";

    //AREA HEAL
    public static readonly string areaHeal1 = "areaHeal1";
    public static readonly string areaHeal2 = "areaHeal2";

    ///Textos
    public static readonly string areaHeal1Text = "Druid heals in a 3x3 tile area with center on her target";
    public static readonly string areaHeal2Text = "Druid heals in a 3x3 tile area with center on her target and removes all debuffs";

    #endregion

    #region Pasive
    //TILE HEAL

    ///Mejoras
    public static readonly string tile1 = "tile1";
    public static readonly string tile2 = "tile2";

    ///Textos
    public static readonly string tile1Text = "When healing an ally, Druid turns the tile where the target is into a healing tile, which heals 1 hp to the unit on it each turn";
    public static readonly string tile2Text = "When healing an ally, Druid turns the tile where the target is into a healing tile, which heals 1 hp to the ally on it each turn " +
		"and deals damage any enemy on it each turn";


    //TILE MOVEMENT
    public static readonly string tileMovement1 = "tileMovement1";
    public static readonly string tileMovement2 = "tileMovement2";

    ///Textos
    public static readonly string tileMovement1Text = "When Druid on a special tile she turns it into a healing tile, which heals 1 hp to the unit on it each turn";
    public static readonly string tileMovement2Text = "When Druid on a special tile she turns it into a healing tile, " +
		"which heals 1 hp to the unit on it each turn and gives them a +1 damage buff while on it";

    #endregion
}

public class AppMonkUpgrades
{
    public static readonly string MonkDataBaseActive = "Monk attacks a target in front of him";
    public static readonly string MonkDataBasePasive = "When attacking an enemy, Monk leaves a mark on it. When allies attack a marked enemy, the mark explodes healing the attacking unit";

    #region Active
    //TURN 180 AND CHAIN MARK

    ///Mejoras
    public static readonly string turn1 = "turn1";
    public static readonly string turn2 = "turn2";

    ///Textos
    public static readonly string turn1Text = "When attacking a target, Monk rotates it 180 degrees";
    public static readonly string turn2Text = "When attacking a target, Monk rotates it 180 degrees. If it has a mark, it explodes and gives a mark to all adjacent enemies";

    //SUPLEX AND UPGRADED MARK
    public static readonly string suplex1 = "suplex1";
    public static readonly string suplex2 = "suplex2";

    ///Textos
    public static readonly string suplex1Text = "When attacking a target, Monk puts that unit in the tile behind him";
    public static readonly string suplex2Text = "When attacking a target, Monk puts that unit in the tile behind him. If it has a mark Monk upgrades it only once, so it heals more";

    #endregion

    #region Pasive
    //MARK DEBUFF AND STUN

    ///Mejoras
    public static readonly string markDebuff1 = "markDebuff1";
    public static readonly string markDebuff2 = "markDebuff2";

    ///Textos
    public static readonly string markDebuff1Text = "Enemies get a -1 damage debuff while they have a mark on them";
    public static readonly string markDebuff2Text = "Enemies get a -1 damage debuff while they have a mark on them. When the mark explodes it stuns the enemy besides healing the attacking unit";

    //MORE HEAL AND BUFF PLAYER
    public static readonly string markBuff1 = "markBuff1";
    public static readonly string markBuff2 = "markBuff2";

    ///Textos
    public static readonly string markBuff1Text = "Marks heal more to the attacking unit";
    public static readonly string markBuff2Text = "Marks heal more to the attacking unit and gives it a +1 damage buff";

    #endregion
}

public class AppValkyrieUpgrades
{
    public static readonly string ValkyrieDataBaseActive = "Valkyrie can target the first unit in the 2 tiles in front of her. When targeting a unit, she changes positions with it";
    public static readonly string ValkyrieDataBasePasive = "Valkyrie has more base movement than other units";

    #region Active
    //MORE RANGE

    ///Mejoras
    public static readonly string moreRange1 = "moreRange1";
    public static readonly string moreRange2 = "moreRange2";

    ///Textos
    public static readonly string moreRange1Text = "Valkyrie's range is now 3 tiles";
    public static readonly string moreRange2Text = "Valkyrie can target any unit in her range, not only the first one.";

    //ARMOUR CHANGE
    public static readonly string armorChange1 = "armorChange1";
    public static readonly string armorChange2 = "armorChange2";

    ///Textos
    public static readonly string armorChange1Text = "When Valkyrie changes positions with an ally, she gives them 1 armor point";
    public static readonly string armorChange2Text = "When Valkyrie changes positions with an ally, she gives them and herself 2 armor points";

    #endregion

    #region Pasive
    //SUSTITUTION

    ///Mejoras
    public static readonly string sustitution1 = "sustitution1";
    public static readonly string sustitution2 = "sustitution2";

    ///Textos
    public static readonly string sustitution1Text = "Valkyrie can use her movement to change positions with an ally with only 1 hp left in any tile of the level";
    public static readonly string sustitution2Text = "Valkyrie can use her movement to change positions with an ally with only 3 or less hp left in any tile of the level";

    //HEIGHT
    public static readonly string height1 = "height1";
    public static readonly string height2 = "height2";

    ///Textos
    public static readonly string height1Text = "Valkyrie can now move with a height difference between tiles of 2";
    public static readonly string height2Text = "Valkyrie ignores height differences between tiles";

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

    public static readonly string PATH_RESOURCE_GENERIC_ICONS = "Icons/";
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
    public static readonly string PLAYER_SELECTION   = "PlayerClick"; //Implementado
    public static readonly string TILECLICK          = "PlayerTileClick"; //Implementado
    public static readonly string TILEHOVER          = "PlayerTileHover"; //Implementado
    public static readonly string COLLISION          = "Gen_Collision"; //Implementado
    public static readonly string MOVEMENT           = "Gen_Move_1"; //Implementado
    public static readonly string HEALING            = "Healing"; //Implementado
    public static readonly string KNIGHT_ATTACK      = "Knight_Attack"; //Implementado
    public static readonly string MAGE_ATTACK        = "Mage_Attack"; //Implementado
    public static readonly string ROGUE_ATTACK       = "Rogue_Attack"; //Implementado

    public static readonly string RECEIVEDAMAGE      = "ReceiveDamage"; //Implementado
    public static readonly string RECEIVEDAMAGE2     = "ReceiveDamage2"; //Implementado
    public static readonly string RECEIVEDAMAGE3     = "ReceiveDamage3";
    public static readonly string EN_DEATH           = "En_Death"; //Implementado

    public static readonly string ENEMYTURN          = "EnemyTurn"; //Implementado
    public static readonly string ENEMYTURN2         = "EnemyTurn2";
    public static readonly string ENEMYTURN3         = "EnemyTurn3";

    public static readonly string PLAYERTURN         = "PlayerTurn"; //Implementado
    public static readonly string PLAYERTURN2        = "PlayerTurn2";
    public static readonly string PLAYERTURN3        = "PlayerTurn3";

    public static readonly string VICTORY            = "Victory"; //Implementado
    public static readonly string DEFEAT             = "Defeat"; //Implementado

    public static readonly string BUYABILITIES       = "BuyAbilities"; //Implementado
    public static readonly string BUTTONCLICK        = "UIButtonClick"; //Implementado
    public static readonly string BUTTONCLICK2       = "UIButtonClick2";
    public static readonly string COINCLICK          = "UICoinClick"; //Implementado
    public static readonly string DIALOGBUTTONCLICK  = "UIDialogButtonClick";
    public static readonly string TEXTSOUND          = "UITextSound"; 
    public static readonly string UIERROR            = "UIError"; //Implementado

    #region Alphabetical Order

    //public static readonly string BUYABILITIES = "BuyAbilities";
    //public static readonly string DEFEAT = "Defeat";
    //public static readonly string EN_DEATH = "En_Death";
    //public static readonly string ENEMYTURN = "EnemyTurn";
    //public static readonly string ENEMYTURN2 = "EnemyTurn2";
    //public static readonly string ENEMYTURN3 = "EnemyTurn3";
    //public static readonly string COLLISION = "Gen_Collision";
    //public static readonly string MOVEMENT = "Gen_Move_1";
    //public static readonly string HEALING = "Healing";
    //public static readonly string KNIGHT_ATTACK = "Knight_Attack";
    //public static readonly string MAGE_ATTACK = "Mage_Attack";
    //public static readonly string PLAYER_SELECTION = "PlayerClick";
    //public static readonly string TILECLICK = "PlayerTileClick";
    //public static readonly string TILEHOVER = "PlayerTileHover";
    //public static readonly string PLAYERTURN = "PlayerTurn";
    //public static readonly string PLAYERTURN2 = "PlayerTurn2";
    //public static readonly string PLAYERTURN3 = "PlayerTurn3";
    //public static readonly string RECEIVEDAMAGE = "ReceiveDamage";
    //public static readonly string RECEIVEDAMAGE2 = "ReceiveDamage2";
    //public static readonly string RECEIVEDAMAGE3 = "ReceiveDamage3";
    //public static readonly string ROGUE_ATTACK = "Rogue_Attack";
    //public static readonly string BUTTONCLICK = "UIButtonClick";
    //public static readonly string BUTTONCLICK2 = "UIButtonClick2";
    //public static readonly string COINCLICK = "UICoinClick";
    //public static readonly string DIALOGBUTTONCLICK = "UIDialogButtonClick";
    //public static readonly string UIERROR = "UIError";
    //public static readonly string TEXTSOUND = "UITextSound";
    //public static readonly string VICTORY = "Victory";
    #endregion
}

//Nombres de la música en la carpeta de resources.
public class AppMusic
{
    public static readonly string INTRO_MUSIC = "Main"; //Implementado
    public static readonly string COMBAT_MUSIC = "Combat"; //Implementado
    public static readonly string BOSS_MUSIC = "Boss"; //Implementado
    public static readonly string DIALOG_MUSIC = "Dialog"; //Implementado
    public static readonly string SUSPENSE_MUSIC = "Suspense";    
}

//Nombres de escenas.
public class AppAchievements
{
    public static readonly string ACHV_BEGIN = "achievement_00";
    public static readonly string ACHV_KILL1 = "achievement_01";
    public static readonly string ACHV_KILL2 = "achievement_02";
    public static readonly string ACHV_KILL3 = "achievement_03";
    public static readonly string ACHV_LEVEL1 = "achievement_04";
    public static readonly string ACHV_LEVEL2 = "achievement_05";
    public static readonly string ACHV_LEVEL3 = "achievement_07";
    public static readonly string ACHV_UPGRADE1 = "achievement_09";
    public static readonly string ACHV_UPGRADE2 = "achievement_10";
    public static readonly string ACHV_UPGRADE3 = "achievement_11";
    public static readonly string ACHV_RANDOM1 = "achievement_12";
    public static readonly string ACHV_RANDOM2 = "achievement_13";
    public static readonly string ACHV_RANDOM3 = "achievement_14";
    public static readonly string ACHV_ADVANTAGE = "achievement_15";
    public static readonly string ACHV_DISADVANTAGE = "achievement_16";
    public static readonly string ACHV_MISS = "achievement_17";
    public static readonly string ACHV_STUMP = "achievement_18";
    public static readonly string ACHV_GIANT = "achievement_19";
    public static readonly string ACHV_NOBODY = "achievement_20";
    public static readonly string ACHV_FINISH = "achievement_21";

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
