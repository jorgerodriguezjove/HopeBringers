using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile : MonoBehaviour
{
    public enum tileType {Enemy,Obstacle, Height}
    [SerializeField]
    public tileType thisTileType;

    [Header("ENEMIES")]
    [SerializeField]
    UnitBase.FacingDirection facingDirection;
    [Header("OBSTACLES")]
    //Si esto es verdadero esta en la layer obstáculo, si no está en la layer notilehere
    [SerializeField]
    public bool isObstacle;
}
