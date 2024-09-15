using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Main;

    [SerializeField] public Transform RightmostObject;
    [SerializeField] public Transform LeftmostObject;

    [SerializeField] public float smooth;
    [SerializeField] public float maxSpeed;
    private Vector2 velocity;
    private Camera cam;

    public Func<Vector3> Target = () => Vector3.zero;
    public bool StrictFix;

    void Awake() { Main = this; }
    void Start() { cam = Camera.main; }

    void Update()
    {
        // if(StrictFix) { transform.position = new Vector3(Target().x, Target().y, transform.position.z); return; }

        var next = Vector2.SmoothDamp(transform.position, Target(), ref velocity, StrictFix ? 0.05f : smooth, StrictFix ? 100 : maxSpeed);
        transform.position = new Vector3(next.x, next.y, transform.position.z);

        var offset = cam.orthographicSize * cam.aspect;
        if(RightmostObject.position.x - transform.position.x < offset)
        {
            transform.position = new Vector3(RightmostObject.position.x - offset, transform.position.y, transform.position.z);
        }

        if(transform.position.x - LeftmostObject.position.x < offset)
        {
            transform.position = new Vector3(LeftmostObject.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}
