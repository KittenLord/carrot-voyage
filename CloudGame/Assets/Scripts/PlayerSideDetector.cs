using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" && PlayerController.Main.State == PlayerState.Regular && !PlayerController.Main.CanJump) 
        { 
            var isLeftFromPlayer = (other.transform.position - transform.position).x < 0;
            PlayerController.Main.IgnoreInputDirection(isLeftFromPlayer, true);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Ground" && PlayerController.Main.State == PlayerState.Regular) 
        { 
            var isLeftFromPlayer = (other.transform.position - transform.position).x < 0;
            if(PlayerController.Main.CanJump) PlayerController.Main.IgnoreInputDirection(isLeftFromPlayer, false);
            else PlayerController.Main.IgnoreInputDirection(isLeftFromPlayer, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Ground" && PlayerController.Main.State == PlayerState.Regular) 
        { 
            var isLeftFromPlayer = (other.transform.position - transform.position).x < 0;
            PlayerController.Main.IgnoreInputDirection(isLeftFromPlayer, false);
        }
    }
}
