using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New npc", menuName = "Npc Cinematics Data", order = 51)]
public class NpcSCO : ScriptableObject
{
    //Valores que deben tener todos los items.
    [Header("BasicInfo")]
    public string nameNPC;
    public Sprite portraitNPC;
    public Color colorTextNPC;

}
