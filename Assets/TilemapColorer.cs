using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapColorer : MonoBehaviour
{
    Tilemap tilemap;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
