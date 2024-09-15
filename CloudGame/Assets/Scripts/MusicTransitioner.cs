using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransitioner : MonoBehaviour
{
    [SerializeField] private MusicState State;
    private bool Activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(other.tag == "Player")
        {
            Activated = true;
            if(State == MusicState.Sunny)
            {
                Music.Main.PlaySunny();
            }
            else if(State == MusicState.Thunder)
            {
                Music.Main.PlayThunder();
            }
        }
    }
}
