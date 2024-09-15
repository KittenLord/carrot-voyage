using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private static bool AlreadyActivated = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(AlreadyActivated) return;
        if(other.transform.tag == "Player")
        {
            AlreadyActivated = true;
            StartCoroutine(Animation());
        }
    }

    private IEnumerator Animation()
    {
        PlayerController.Main.State = PlayerState.Locked;
        PlayerController.Main.GetComponent<BoxCollider2D>().enabled = false;
        PlayerController.Main.PlayerAnimator.Play("PlayerVanish");
        CameraFollow.Main.Target = () => this.transform.position;
        yield return new WaitForSeconds(0.85f);
        Destroy(PlayerController.Main.gameObject);

        yield return new WaitForSeconds(1.4f);

        Instantiate<GameObject>(Resources.Load<GameObject>("Player"), Checkpoint.Last.transform.position, Quaternion.identity);
        var cameraSpeed = CameraFollow.Main.maxSpeed;
        CameraFollow.Main.maxSpeed = 300;
        yield return new WaitForSeconds(1.5f);
        Camera.main.orthographicSize = Checkpoint.Last.CameraSize;
        CameraFollow.Main.maxSpeed = cameraSpeed;
        AlreadyActivated = false;
    }
}
