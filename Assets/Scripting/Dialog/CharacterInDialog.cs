using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInDialog : MonoBehaviour
{
    //La 0 es fuera de escena y 1,2,3,4 son las que se ven en cámara
    public List<Transform> positionsToMove;

    public void MoveToPosition(int posToMove)
    {
        this.transform.position = positionsToMove[posToMove].position;
    }

    public void HighlightSpeaker()
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void DesHighlightSpeaker()
    {
        transform.localScale = Vector3.one;
    }

    public void DeactivateCharacters()
    {
        this.transform.position = positionsToMove[0].position;
        gameObject.SetActive(false);
    }
}
