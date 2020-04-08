using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EN ESTE SCRIPT ÚNICAMENTE VAN LAS VARIABLES QUE SE DEBEN GUARDAR EN EL ARCHIVO DE GUARDADO
[System.Serializable]
public class Save
{
    #region BASE

    public int s_currentXp;

    //Progresión de niveles
    public List<int> s_levelIDsUnlocked = new List<int>();

    //Progresión de personajes
    public List<int> s_charactersUnlocked = new List<int>();
    #endregion

    #region CHARACTERS
    //Knight
    public List<int> s_KnightSkillsIds = new List<int>();
    public int s_KnightPowerLevel;

    //Rogue
    public List<int> s_RogueSkillsIds = new List<int>();
    public int s_RoguePowerLevel;

    //Mage
    public List<int> s_MageSkillsIds = new List<int>();
    public int s_MagePowerLevel;

    //Berserker
    public List<int> s_BerserkerSkillsIds = new List<int>();
    public int s_BerserkerPowerLevel;

    //Valkyrie
    public List<int> s_ValkyrieSkillsIds = new List<int>();
    public int s_ValkyriePowerLevel;

    //Druid
    public List<int> s_DruidSkillsIds = new List<int>();
    public int s_DruidPowerLevel;

    //Monk
    public List<int> s_MonkSkillsIds = new List<int>();
    public int s_MonkPowerLevel;

    //Samurai
    public List<int> s_SamuraiSkillsIds = new List<int>();
    public int s_SamuraiPowerLevel;

    #endregion


    #region ACHIEVEMENTS


    #endregion
}
