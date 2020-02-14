using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField]
    public Vector3 boundsSize;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gameObject.transform.position,boundsSize);
        //Gizmos.DrawWireCube(new Vector3(transform.position.x + boundsSize.x / 2, transform.position.y + boundsSize.y / 2, transform.position.z + boundsSize.z / 2), new Vector3(boundsSize.x, boundsSize.y, boundsSize.z));
    }
}
