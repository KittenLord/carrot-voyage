using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomDetector : MonoBehaviour
{
    private PlayerController pc;
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Ground" && PlayerController.Main.State == PlayerState.Regular) 
        { 
            pc.CanJump = true; 
            LightningBoost.LastBoost = null;
        }
    }

    private IEnumerator CoyoteTimer()
    {
        for(int i = 0; i < 5; i++) yield return null;
        pc.CanJump = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Ground" && PlayerController.Main.State == PlayerState.Regular && pc.CanJump) 
        { 
            // pc.CanJump = false;
            StartCoroutine(CoyoteTimer());
        }
    }

}
