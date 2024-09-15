using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadClouds : MonoBehaviour
{
    public static SpreadClouds Main;
    void Start() { Main = this; }

    [SerializeField] private Transform CameraPosition;

    [SerializeField] private Transform MovingCloud;
    [SerializeField] private Transform MovingCloudTarget;

    [SerializeField] private Transform[] Clouds;
    [SerializeField] private Vector2[] CloudsDirections;

    public void ActivateCoroutine() 
    { 
        StartCoroutine(CameraAnimation());
        StartCoroutine(CloudAnimation()); 
    }

    // private void Update()
    // {
    //     // if(Input.GetKeyDown(KeyCode.P)) ActivateCoroutine();
    // }

    private IEnumerator CameraAnimation()
    {
        PlayerController.Main.State = PlayerState.Locked;
        CameraFollow.Main.Target = () => CameraPosition.position;
        var oldSpeed = CameraFollow.Main.maxSpeed;
        CameraFollow.Main.maxSpeed = 20;
        var camera = Camera.main;

        var originalCameraSize = camera.orthographicSize;
        var targetCameraSize = originalCameraSize + 13;

        while(camera.orthographicSize < targetCameraSize)
        {
            camera.orthographicSize += Time.deltaTime * 4;

            yield return null;
        }

        yield return new WaitForSeconds(5f);
        StartCoroutine(MovingCloudAnimation());
        yield return new WaitForSeconds(2f);

        CameraFollow.Main.Target = () => PlayerController.Main.transform.position;
        CameraFollow.Main.maxSpeed = 30;

        originalCameraSize = 7; // increase for harder segment
        while(camera.orthographicSize > originalCameraSize)
        {
            camera.orthographicSize -= Time.deltaTime * 10;

            yield return null;
        }

        camera.orthographicSize = originalCameraSize;

        CameraFollow.Main.maxSpeed = oldSpeed;
    }

    private IEnumerator MovingCloudAnimation()
    {
        var originalSkyColor = Camera.main.backgroundColor;
        var targetSkyColor = HexColor.Get(0x578DB4FF);

        PlayerController.Main.transform.parent = MovingCloud.transform;
        Vector3 originalPos = MovingCloud.position;
        float lerp = 0;
        while(lerp < 1)
        {
            Camera.main.backgroundColor = Color.Lerp(originalSkyColor, targetSkyColor, lerp);
            MovingCloud.position = Vector2.Lerp(originalPos, MovingCloudTarget.position, lerp);
            lerp += Time.deltaTime * 0.1f;

            yield return null;
        }
        MovingCloud.position = MovingCloudTarget.position;

        PlayerController.Main.State = PlayerState.Regular;
        PlayerController.Main.transform.parent = null;
    }

    private IEnumerator CloudAnimation()
    {
        yield return new WaitForSeconds(1f);
        float speed = 1;
        while(speed > 0)
        {
            for(int i = 0; i < Clouds.Length; i++)
            {
                Clouds[i].Translate(CloudsDirections[i] * speed * 0.0035f);
            }
            speed -= Time.deltaTime * 0.10f;

            yield return null;
        }
    }
}
