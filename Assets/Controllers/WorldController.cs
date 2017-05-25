using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public Sprite floorSprite;
	World world;

	// Use this for initialization
	void Start () {
		world = new World ();

        for (int x = 0; x < world.getWidth; x++)
        {
            for (int y = 0; y < world.getHeight; y++)
            {
                GameObject tileGo = new GameObject();
                Tile tileData = world.getTileAt(x, y);
                tileGo.name = "Tile" + x;
                tileGo.transform.position = new Vector3(tileData.getX, tileData.getY);

                SpriteRenderer tileSr = tileGo.AddComponent<SpriteRenderer>();

                //if (tileData.getType == Tile.Type.Floor)
                //{
                //    tileSr.sprite = floorSprite;
                //}

                tileData.registerChangeTileTypeCB( (tile) => { onTileTypeChanged(tile, tileGo); } );
            }
        }
        transform.SetAsLastSibling();

        world.randomizeTiles();
    }

    float randomizeTileTimer = 2f;

	// Update is called once per frame
	void Update () {
        randomizeTileTimer -= Time.deltaTime;

        if(randomizeTileTimer < 0)
        {
            world.randomizeTiles();
            randomizeTileTimer = 2f;
        }
        
	}

    void onTileTypeChanged(Tile tileData, GameObject tileGo)
    {
        if(tileData.getType == Tile.Type.Floor)
        {
            tileGo.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if(tileData.getType == Tile.Type.Empty)
        {
            tileGo.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("ERROR 1");
        }
    }
}
