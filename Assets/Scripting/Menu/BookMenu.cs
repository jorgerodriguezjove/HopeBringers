using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BookMenu : MonoBehaviour
{
    public PlayableDirector DollyCamera;
    public Animator anim;
    bool isOpen;
    // Start
    void Start()
    {
        anim = GetComponent<Animator>();
        isOpen = anim.GetBool("isOpen?");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)/* && isOpen*/)
        {
            anim.SetBool("isOpen?", true);
            DollyCamera.Play();
        }
        if (Input.GetMouseButtonDown(1)/* && !isOpen*/)
        {
            anim.SetBool("isOpen?", false);
            DollyCamera.Play();
           
        }
    }
}
