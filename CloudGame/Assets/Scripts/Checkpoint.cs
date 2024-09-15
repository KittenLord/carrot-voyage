using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public float CameraSize;

    public static Checkpoint Last = null;
    private bool Activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(other.tag == "Player")
        {
            Activated = true;
            Last = this;
        }
    }
}
