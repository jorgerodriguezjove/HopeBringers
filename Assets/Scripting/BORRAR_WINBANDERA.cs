using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BORRAR_WINBANDERA : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerUnit>())
        {
            StartCoroutine("CorrutinaToGuapa");
        }
    }

    IEnumerator CorrutinaToGuapa()
    {
        FindObjectOfType<LevelManager>().InstaWin();
        yield return new WaitForSecondsRealtime(1f);
        FindObjectOfType<UIManager>().EndTurn();
    }
}
