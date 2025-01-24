using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class MapDrawer : MonoBehaviour
{
    public bool GizmoActivation = false;
    public Texture2D texture;
    public Texture2D Original;

    public int EveryXFrame = 10;
    int FrameWeAre = 0;

    public int size = 4;

    public Color32 color = new Color32(255,255,255,255);
    public Color32[] toPaintWith;

    public SplineContainer spline;

    public List<Vector2> posOnMap = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        posOnMap = new List<Vector2>(); //Start Position

        ChangePencil();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PlaceSpline();
        FrameWeAre++;
        if(FrameWeAre >= EveryXFrame)
        {
            FrameWeAre = 0;
            SlowUpdate();
        }
        if (Input.GetMouseButton(0))
        {
            paintOnMap();
        }
        else if (Input.GetMouseButton(1))
        {
            ClearMap();
        }
    }
    void SlowUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            RecordPos();
        }
    }
    void RecordPos()
    {
        Vector2 mousepos = Input.mousePosition;

        //refacto to a func toWorldScale
        mousepos.x /= Screen.width;
        mousepos.y /= Screen.height;

        mousepos.y = 1 - Mathf.Clamp01(mousepos.y);
        mousepos.x = 1 - Mathf.Clamp01(mousepos.x);

        mousepos.x *= 2;
        mousepos.x += -.5f;

        mousepos.y *= 1.2f;
        mousepos.y += -.1f;


        //refacto to V3s and add Raycast to ground
        posOnMap.Add(mousepos);
    }
    void paintOnMap()
    {
        Vector2 mousepos = Input.mousePosition;

        mousepos.x /= Screen.width;
        mousepos.y /= Screen.height;

        mousepos.y = 1 - Mathf.Clamp01(mousepos.y);
        mousepos.x = 1 - Mathf.Clamp01(mousepos.x);

        mousepos.x *= 2;
        mousepos.x += -.5f;

        mousepos.y *= 1.2f;
        mousepos.y += -.1f;

        print(mousepos.x + "    " + mousepos.y);

        texture.SetPixels32(
            (int)Mathf.Clamp(mousepos.x * texture.width, 4, texture.width - 4),
            (int)Mathf.Clamp(mousepos.y * texture.height, 4, texture.height - 4),
            size, size,
            toPaintWith,
            0);

        texture.Apply();
    } 
    void ClearMap()
    {
        posOnMap.Clear();
        texture.SetPixels(Original.GetPixels());
        texture.Apply();
        GizmoActivation = true;
    }
    void ChangePencil()
    {
        toPaintWith = new Color32[size * size];
        Array.Fill<Color32>(toPaintWith, color);
    }

    private void OnValidate()
    {
        ChangePencil();

    }

    private void OnApplicationQuit()
    {
        ClearMap();
    }


    private void PlaceSpline()
    {
        GizmoActivation = false;
        spline.Splines[0].Clear();
        {
            for (int i = 0; i < posOnMap.Count; i++)
            {
                spline.Splines[0].Add(new BezierKnot(new Vector3(1-posOnMap[i].x, 0.05f, 1-posOnMap[i].y) * 100));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green/2;
        if(GizmoActivation) foreach (Vector2 pos in posOnMap)
        {
            Gizmos.DrawCube(new Vector3(1-pos.x, 0.05f, 1-pos.y) * 100, Vector3.one);
        }
    }


}
