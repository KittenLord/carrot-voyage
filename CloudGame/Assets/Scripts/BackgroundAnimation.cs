using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private float Parallax;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private Vector2 previousCamPosition;
    void Update()
    {
        Vector2 currentCamPosition = cam.transform.position;
        var delta = currentCamPosition - previousCamPosition;
        transform.position = transform.position.v2() + delta * Parallax;

        previousCamPosition = currentCamPosition;
    }
}
