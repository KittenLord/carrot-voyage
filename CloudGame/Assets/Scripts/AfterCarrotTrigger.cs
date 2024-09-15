using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCarrotTrigger : MonoBehaviour
{
    [SerializeField] private bool StartsCutscene;
    private bool Activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(!CarrotsPot.Done) return;
        if(other.tag == "Player")
        {
            Activated = true;
            if(StartsCutscene) 
            {
                SpreadClouds.Main.ActivateCoroutine();
            }
            else
            {
                // player notice
            }
        }
    }
}
