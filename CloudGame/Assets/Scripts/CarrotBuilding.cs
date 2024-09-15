using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBuilding : MonoBehaviour
{
    [SerializeField] private Transform InsidePosition;
    [SerializeField] private Transform OutsidePosition;

    [SerializeField] private Transform NewLeftmost;
    [SerializeField] private Transform NewRightmost;

    [SerializeField] private Transform OldLeftmost;
    [SerializeField] private Transform OldRightmost;

    [SerializeField] private GameObject Instructions;
    [SerializeField] private GameObject GoBack;

    public bool Entered = false;
    private bool CanUse = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player") return;
        if(!CanUse) return;
        if(!Entered)
        {
            Destroy(Instructions);
            GoBack.SetActive(true);
            PlayerController.Main.transform.position = InsidePosition.position;
            CameraFollow.Main.LeftmostObject = NewLeftmost;
            CameraFollow.Main.RightmostObject = NewRightmost;
            Entered = true;
            Music.Main.PlayCarrot();
        }
        else
        {
            if(!CarrotsPot.Done)
            {
                return;
            }

            PlayerController.Main.transform.position = OutsidePosition.position;
            CameraFollow.Main.LeftmostObject = OldLeftmost;
            CameraFollow.Main.RightmostObject = OldRightmost;

            // Destroy(this.gameObject);
            CanUse = false;

            Camera.main.backgroundColor = HexColor.Get(0x65BDDBFF);
            Music.Main.Stop();
        }
    }
}
