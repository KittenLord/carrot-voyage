using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    [SerializeField] private float WidthAngle;
    [SerializeField] private float OffsetAngle;

    [SerializeField] private float UpperHeightWorld;
    [SerializeField] private float LowerHeightWorld;
    [SerializeField] private float MaxHeightWorld;

    [SerializeField] private MeshFilter OriginMesh;
    [SerializeField] private MeshFilter MidMesh;
    [SerializeField] private MeshFilter MaxMesh;

    private PolygonCollider2D Collider;

    public Vector3 BottomPosition { get; private set; }
    public Vector3 TopPosition { get; private set; }
    public int MaxLines { get; private set; }

    private float Cot(float f) => 1 / Mathf.Tan(f);

    private static Color cIn = HexColor.Get(0xf5e34210);
    private static Color cBs = HexColor.Get(0xf5e3425e);
    private static Color cHi = HexColor.Get(0xf5e34275);
    private static Color cFc = HexColor.Get(0xf5e342f0);

    private Color HighlightColor = cHi;

    void Start()
    {
        var origin = transform.position;
        var UpperHeight = Mathf.Abs(UpperHeightWorld - origin.y);
        var LowerHeight = Mathf.Abs(LowerHeightWorld - origin.y);
        var MaxHeight = Mathf.Abs(MaxHeightWorld - origin.y);
        Collider = GetComponent<PolygonCollider2D>();
        transform.position = Vector3.zero;

        var ur = Cot(Mathf.Deg2Rad * (OffsetAngle)) * UpperHeight;
        var ul = Mathf.Tan(Mathf.Deg2Rad * (90 - OffsetAngle - WidthAngle)) * UpperHeight;

        var br = Cot(Mathf.Deg2Rad * (OffsetAngle)) * LowerHeight;
        var bl = Mathf.Tan(Mathf.Deg2Rad * (90 - OffsetAngle - WidthAngle)) * LowerHeight;

        var mr = Cot(Mathf.Deg2Rad * (OffsetAngle)) * MaxHeight;
        var ml = Mathf.Tan(Mathf.Deg2Rad * (90 - OffsetAngle - WidthAngle)) * MaxHeight;

        TopPosition = new Vector3((origin.x + ul + origin.x + ur) / 2, origin.y - UpperHeight, 0);
        BottomPosition = new Vector3((origin.x + bl + origin.x + br) / 2, origin.y - LowerHeight, 0);
        var verts = new Vector3[] { 
            origin,                                                             // 0 - source of the ray
            new Vector3(origin.x + ur, origin.y - UpperHeight, 0),               // 1 - right point of the upper line
            new Vector3(origin.x + ul, origin.y - UpperHeight, 0),               // 2 - left point of the upper line
            new Vector3(origin.x + br, origin.y - LowerHeight, 0),               // 3 - right point of the lower line
            new Vector3(origin.x + bl, origin.y - LowerHeight, 0),               // 4 - left point of the lower line
            new Vector3(origin.x + mr, origin.y - MaxHeight, 0),                // 5 - right point of the max line
            new Vector3(origin.x + ml, origin.y - MaxHeight, 0),                // 6 - left point of the max line
        };

        var colliderForgiveness = 0.15f;
        Collider.SetPath(0, new Vector2[] { verts[1] + Vector3.right * colliderForgiveness, verts[2] - Vector3.right * colliderForgiveness, verts[4] - Vector3.right * colliderForgiveness, verts[3] + Vector3.right * colliderForgiveness });


        var mesh = new Mesh { name = "OriginMesh" };
        mesh.vertices = new Vector3[] { verts[0], verts[1], verts[2] };
        mesh.triangles = new int[] { 0, 1, 2, };
        mesh.colors = new Color[] { cIn, cIn, cIn };
        OriginMesh.mesh = mesh;


        mesh = new Mesh { name = "MaxMesh" };
        mesh.vertices = new Vector3[] { verts[3], verts[4], verts[5], verts[6] };
        mesh.triangles = new int[] { 1, 0, 2,        2, 3, 1 };
        mesh.colors = new Color[] { cIn, cIn, cIn, cIn };
        MaxMesh.mesh = mesh;





        mesh = new Mesh { name = "MidMesh" };
        List<Vector3> midVerts = new();
        List<int> midTris = new();
        int triCounter = 0;
        int last = 0;
        for(float lerp = 0; lerp <= 1.001f; lerp += 0.1f)
        {
            var height = Mathf.Lerp(UpperHeight, LowerHeight, lerp);
            var left = Mathf.Lerp(ul, bl, lerp);
            var right = Mathf.Lerp(ur, br, lerp);
            midVerts.Add(new(origin.x + left, origin.y - height, 0));
            midVerts.Add(new(origin.x + right, origin.y - height, 0));

            if(triCounter == 0)
            {
                midTris.Add(0);
                midTris.Add(1);
                triCounter = 2;
            }
            else
            {
                var li = triCounter;
                var ri = triCounter + 1;
                triCounter += 2;

                midTris.Add(ri);
                midTris.Add(ri);
                midTris.Add(li);
                midTris.Add(last);
                midTris.Add(li);
                midTris.Add(ri);
                last = li;
            }
        }

        MaxLines = midVerts.Count / 2;

        mesh.vertices = midVerts.ToArray();
        mesh.colors = midVerts.Select(_ => cBs).ToArray();
        mesh.triangles = midTris.Take(midTris.Count - 2).ToArray();
        MidMesh.mesh = mesh;

        IdleAnimationCoroutine = StartCoroutine(IdleAnimation(0.0005f));
    }

    public void StartIdle()
    {
        IdleAnimationCoroutine = StartCoroutine(IdleAnimation(0.0005f));
    }

    public Coroutine IdleAnimationCoroutine = null;
    public IEnumerator IdleAnimation(float speed)
    {
        if(LastLerpCoroutine != null) { StopCoroutine(LastLerpCoroutine); LastLerpCoroutine = null; }
        HighlightColor = cHi;
        var current = Random.Range(0, 1.0f);
        SetFocus(current);
        while(true)
        {
            var dest = Random.Range(0, 1.0f);
            while(Mathf.Abs(current - dest) > 0.01f)
            {
                current += Mathf.Sign(dest - current) * speed;
                SetFocus(current);
                yield return null;
            }
        }
    }

    private float LastLerpValue = 0;
    private Coroutine LastLerpCoroutine = null;

    public void SetPlayerLerp(float lerp, float speed)
    {
        if(IdleAnimationCoroutine != null) { StopCoroutine(IdleAnimationCoroutine); IdleAnimationCoroutine = null; }
        HighlightColor = cFc;
        if(LastLerpCoroutine != null) { StopCoroutine(LastLerpCoroutine); LastLerpCoroutine = null; }
        LastLerpCoroutine = StartCoroutine(SetPlayerLerpAnimation(lerp, speed));
    }

    private IEnumerator SetPlayerLerpAnimation(float lerp, float speed)
    {
        while(Mathf.Abs(lerp - LastLerpValue) > 0.01f)
        {
            var sign = Mathf.Sign(lerp - LastLerpValue);
            SetFocus(Mathf.Abs(speed) > Mathf.Abs(lerp - LastLerpValue) ? lerp : LastLerpValue + speed * sign);
            yield return null;
        }
        yield break;
    }

    public void SetFocus(float lerp)
    {
        LastLerpValue = lerp;

        float thresh = 2;
        float findex = Mathf.Lerp(0, MaxLines - 1, lerp);
        for(int i = 0; i < MaxLines; i++)
        {
            var d = Mathf.Abs(findex - i);
            var ll = Mathf.InverseLerp(0, thresh, Mathf.Clamp(thresh - d, 0, thresh));
            var color = Color.Lerp(cBs, HighlightColor, ll);
            SetLine(i, color);
        }
    }

    public void SetLine(int index, Color color)
    {
        if(index >= MaxLines) return;
        index *= 2;
        var c = MidMesh.mesh.colors;
        c[index] = color;
        c[index+1] = color;
        MidMesh.mesh.colors = c;
    }

    public void OnMouseDown()
    {
        Debug.Log(transform.position);
        PlayerController.Main.ClickBeam(this);
    }
}
