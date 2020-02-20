using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile : MonoBehaviour
{
    public enum tileType {Enemy,Obstacle, Height}
    [SerializeField]
    public tileType thisTileType;
}
