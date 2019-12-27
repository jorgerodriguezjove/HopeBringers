using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class DragonSceneScript : MonoBehaviour
{

    public CinemachineStateDrivenCamera dragonCamera;
    public PlayableDirector charactersAnim;
    public PlayableDirector dragonWalk1Anim;
    public PlayableDirector dragonWalk2Anim;
    public Animator dragonanim;

    public GameObject spawnParticle;
    public GameObject trailParticle;
    public GameObject screamParticle;
    public GameObject fireBreathParticle;



    // Start
    void Start()
    {
        dragonanim = GetComponent<Animator>();
        dragonanim.SetBool("Fly Idle", true);
    }

    private void Update()
    {       
        if (Input.GetKeyDown("1"))
        {
            //dragonanim.SetBool("Idle", false);
            dragonanim.SetBool("Fly Idle", true);
            dragonanim.SetBool("Fly Forward", true);
            spawnParticle.SetActive(true);
        }
        if (Input.GetKeyDown("2"))
        {           
            dragonanim.SetBool("Idle", false);
            dragonanim.SetBool("Fly Forward", false);
            dragonanim.SetBool("Fly Idle", false);
            dragonanim.SetBool("Walk Forward", true);
            
            
            dragonWalk1Anim.Play();
        }
        if (Input.GetKeyDown("3"))
        {
            dragonanim.SetBool("Walk Forward", false);
            dragonanim.SetBool("Idle", true);
        }
        if (Input.GetKeyDown("4"))
        {
            dragonanim.SetBool("Cast Spell", true);            
        }
        if (Input.GetKeyDown("5"))
        {
            dragonanim.SetBool("Walk Fast Forward", true);         
        }
        if (Input.GetKeyDown("6"))
        {
            dragonanim.SetBool("Walk Fast Forward", false);
            dragonanim.SetBool("Take Damage", true);
        }
        if (Input.GetKeyDown("7"))
        {          
            dragonanim.SetBool("Fire Breath Attack", true);
            fireBreathParticle.SetActive(true);

        }

    }
    //void changeCamera()
    //{
    //    if (isFlying)
    //    {
    //        DollyfollowCharacters.Play();

    //    }
    //    if (isRawring)
    //    {
    //        DollyCameraDragon.Play();

    //    }
    //    if (isWalking)
    //    {
    //        DollyCameraClose.Play();

    //    }
    //}

    //public void ShowButtons()
    //{

    //    canvasLevels.SetActive(true);
    //}

    //public void HideButtons()
    //{

    //    canvasLevels.SetActive(false);
    //}
}
