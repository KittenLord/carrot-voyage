using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoost : MonoBehaviour
{
    public static LightningBoost LastBoost = null;
    public static LightningBoost WhichToLightUp = null;
    private SpriteRenderer sr;

    private Color OriginalColor;

    public void Lightup(Color c, float duration, int times) => StartCoroutine(LightupAnimation(c, duration, times));
    private IEnumerator LightupAnimation(Color c, float duration, int times)
    {
        var originalColor = OriginalColor;
        for(int i = 0; i < times; i++)
        {
            sr.color = c;
            yield return new WaitForSeconds(duration);
            sr.color = originalColor;
            yield return new WaitForSeconds(duration);
        }
        sr.color = originalColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        OriginalColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(this == LastBoost) return;

        if(other.tag == "Player")
        {
            Lightup(HexColor.Get(0x999933ff), 0.1f, 1);
            LastBoost = this;
            WhichToLightUp = this;
            PlayerController.Main.CanJump = true;
            PlayerController.Main.lightningBoost = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            // PlayerController.Main.CanJump = true;
            // PlayerController.Main.lightningBoost = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController.Main.CanJump = false;
            PlayerController.Main.lightningBoost = false;
        }
    }
}

