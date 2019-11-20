using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MeshHighlighter : MonoBehaviour
{

    private SkinnedMeshRenderer originalMesh;
    [SerializeField]
    private SkinnedMeshRenderer highlightedMesh;
    // Start is called before the first frame update
    void Start()
    {
        originalMesh = GetComponent<SkinnedMeshRenderer>();
        EnabledHighlight(false);
    }

    public void EnabledHighlight(bool onOff)
    {
        if(highlightedMesh != null)
        {
            highlightedMesh.enabled = onOff;
            originalMesh.enabled = !onOff;
        }
    }

    private void OnMouseEnter()
    {
        EnabledHighlight(true);
    }

    private void OnMouseExit()
    {
        EnabledHighlight(false);
    }
}
