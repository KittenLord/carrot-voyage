using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsiveWall : MonoBehaviour
{
    // i gotta stop using unity
    void OnCollisionStay2D(Collision2D other)
    {
        if(other.collider.tag == "Player") 
        { 
            // Debug.Log("collide");
            // other.collider.GetComponent<Rigidbody2D>().AddForce(Vector3.right * (other.transform.position.x - transform.position.x) * 200, ForceMode2D.Impulse);
            // other.collider.GetComponent<Rigidbody2D>().AddForce(Vector3.right * 500, ForceMode2D.Impulse);

            // PlayerController.Main.Repulse(Mathf.Sign(other.transform.position.x - transform.position.x) * 1);
            // PlayerController.Main.CanJump = false;
        }
    }
}
