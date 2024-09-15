using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormCutscene : MonoBehaviour
{
    [SerializeField] private Transform Orientation;
    private bool Activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated) return;
        if(other.tag != "Player") return;
        Activated = true;

        Music.Main.PlayStorm();

        PlayerController.Main.transform.position = transform.position;
        PlayerController.Main.State  = PlayerState.Locked;
        PlayerController.Main.rb.gravityScale = 0;
        CameraFollow.Main.maxSpeed = 300f;
        CameraFollow.Main.smooth = 0.2f;

        StartCoroutine(FirstScene());
    }

    private IEnumerator FirstScene()
    {
        float GetY(float t) => 5 * Mathf.Sin(t) + t;
        float GetX(float t) => Mathf.Sin(0.5f * t);

        float t = 0;
        float speed = 5;
        while(t < 32.5f)
        {
            var position = transform.position + (5 * GetX(t) * Orientation.up) + (1.5f * GetY(t) * Orientation.right);
            PlayerController.Main.transform.position = position;

            t += Time.deltaTime * speed;
            yield return null;
        }

        PlayerController.Main.rb.velocity = Vector2.zero;
        PlayerController.Main.rb.gravityScale = 3f;
        yield return new WaitForSeconds(1.5f);

        float quarter = 0.45f;
        PlayerController.Main.rb.velocity = Vector2.zero;
        PlayerController.Main.rb.AddForce(new Vector2(0, 1) * 20f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(quarter);

        PlayerController.Main.rb.velocity = Vector2.zero;
        PlayerController.Main.rb.AddForce(new Vector2(3, 1) * 25f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(quarter);

        PlayerController.Main.rb.velocity = Vector2.zero;
        PlayerController.Main.rb.AddForce(new Vector2(0, 1) * 20f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(quarter);
        
        PlayerController.Main.rb.velocity = Vector2.zero;
        PlayerController.Main.rb.AddForce(new Vector2(-0.5f, 1) * 18f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(quarter);

        PlayerController.Main.rb.velocity = Vector2.zero;
        // PlayerController.Main.rb.AddForce(new Vector2(1, 1) * 20f, ForceMode2D.Impulse);
    }

}
