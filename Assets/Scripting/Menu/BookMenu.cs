using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BookMenu : MonoBehaviour
{
    public PlayableDirector DollyCamera;
    public Animator anim;
    // Start
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isOpen?", true);
            DollyCamera.Play();
        }
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("isOpen?", false);
            
           
        }
    }
}
