using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyColorChanger : MonoBehaviour
{
    [SerializeField] private Color NewColor;
    private bool Activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(other.tag == "Player")
        {
            Activated = true;
            StartCoroutine(ChangeColor(Camera.main.backgroundColor, NewColor));

        }
    }

    private IEnumerator ChangeColor(Color a, Color b)
    {
        var cam = Camera.main;
        float lerp = 0;
        while(lerp < 1)
        {
            cam.backgroundColor = Color.Lerp(a, b, lerp);

            lerp += Time.deltaTime;
            yield return null;
        }

        cam.backgroundColor = b;
    }
}
