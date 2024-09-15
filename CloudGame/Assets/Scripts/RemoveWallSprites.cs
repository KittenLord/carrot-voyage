using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWallSprites : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform sr in this.transform) sr.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
