using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDuplicator : MonoBehaviour
{

    private bool isOriginal = true;
    public Color copyColor;
    public RuleTile copyRuleTile;
    private GameObject copyObject;

    // Start is called before the first frame update
    void Start()
    {
        if (isOriginal)
            CreateCopy();
    }

    private void CreateCopy()
    {
        copyObject = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        copyObject.GetComponent<TilemapDuplicator>().isOriginal = false;
        copyObject.GetComponent<TilemapColorer>().color = copyColor;
        Tilemap tilemap = copyObject.GetComponent<Tilemap>();
        ReplaceAllTiles(tilemap, copyRuleTile);
        //tilemap.ClearAllTiles();
        //tilemap.SetTile(new Vector3Int(0, 0, 0), copyRuleTile);
        copyObject.GetComponent<TilemapRenderer>().sortingOrder = GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    private void ReplaceAllTiles(Tilemap tilemap, RuleTile newTile)
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Loop through all the positions in the bounds
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(position);
                if(tile)
                    tilemap.SetTile(position, newTile);
            }
        }
    }
}
