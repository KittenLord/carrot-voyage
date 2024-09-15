using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotsPot : MonoBehaviour
{
    public static bool Done => CarrotAmount <= 0;
    public static int CarrotAmount = 0;
    void Awake() { CarrotAmount++; }
    private bool Watered = false;

    void OnMouseDown()
    {
        if(Watered) return;
        Watered = true;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("carrotsgood");
        CarrotAmount--;
    }
}
