using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindFlow : MonoBehaviour
{
    public float Speed;
    private List<Sprite> SpritePool;
    private List<SpriteRenderer> srs;

    public const float EndOffset = 2.20f;

    public Vector3 EndPoint => Parts.Last().position + (EndOffset * Parts.Last().right);
    public Vector3 StartPoint => Parts.First().position - (EndOffset * Parts.First().right);

    public List<Transform> PartsForward => Parts;
    public List<Transform> PartsBackwards => Parts.Reverted();

    private List<Transform> Parts;

    // Start is called before the first frame update
    void Start()
    {
        Sprite[] supersprite = Resources.LoadAll<Sprite>("wind");
        SpritePool = new();
        foreach(Sprite s in supersprite) SpritePool.Add(s);
        srs = new();
        foreach(Transform child in this.transform)
        {
            srs.Add(child.GetChild(0).GetComponent<SpriteRenderer>());
        }
        Parts = new();
        foreach(Transform child in this.transform) { Parts.Add(child.transform); }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % 20 == 0) foreach(var sr in srs) sr.sprite = SpritePool[Random.Range(0, SpritePool.Count)];
    }

    public void OnMouseDown()
    {
        PlayerController.Main.ClickWind(this);
    }
}
