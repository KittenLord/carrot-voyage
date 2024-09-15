using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private AudioClip EndChord;
    [SerializeField] private GameObject CreditsText;

    private bool Activated;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(!CarrotsPot.Done) return;
        if(other.tag == "Player")
        {
            Activated = true;
            Music.Main.Stop();
            Music.Main.Sources[0].PlayOneShot(EndChord);
            Camera.main.backgroundColor = HexColor.Get(0x5BCFF7ff);
            CreditsText.SetActive(true);
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
