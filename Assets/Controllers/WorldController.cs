//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	// The only tile sprite we have right now, so this
	// it a pretty simple way to handle it.
	public Sprite floorSprite;
    public Sprite emptySprite;

    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<Tile, GameObject> tileGameObjectMap;

    Dictionary<string, Sprite> furnitureSprites;

	// The world and tile data
	public World World { get; protected set; }

	// Use this for initialization
	void OnEnable () {

        loadFurnitureSprites();

		if(Instance != null) {
			Debug.LogError("There should never be two world controllers.");
		}
		Instance = this;

		// Create a world with Empty tiles
		World = new World();

        World.RegisterFurnitureCreated(OnFurnitureCreated);

        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				// Get the tile data
				Tile tile_data = World.GetTileAt(x, y);

				// This creates a new GameObject and adds it to our scene.
				GameObject tile_go = new GameObject();

                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);

				// Add a sprite renderer, but don't bother setting a sprite
				// because all the tiles are empty right now.
				tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;

                // Use a lambda to create an anonymous function to "wrap" our callback function
                //tile_data.RegisterTileChangedCallback( OnTileChanged );
			}
		}

        World.RegisterTileChanged(OnTileChanged);

        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);

        // Shake things up, for testing.
        //World.RandomizeTiles();
    }

	// Update is called once per frame
	void Update () {

	}

    void loadFurnitureSprites() {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");

        foreach (Sprite s in sprites) {
            furnitureSprites[s.name] = s;
        }
    }

	// This function should be called automatically whenever a tile's type gets changed.
	void OnTileChanged(Tile tile_data) {

        if (tileGameObjectMap.ContainsKey(tile_data) == false) {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null) {
            Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        if (tile_data.Type == Tile.TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
		}
		else if( tile_data.Type == Tile.TileType.Empty ) {
			tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
		}
		else {
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
		}
        
	}

	/// <summary>
	/// Gets the tile at the unity-space coordinates
	/// </summary>
	/// <returns>The tile at world coordinate.</returns>
	/// <param name="coord">Unity World-Space coordinates.</param>
	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);
		
		return World.GetTileAt(x, y);
	}

    public void OnFurnitureCreated(Furniture furn) {
        Debug.Log("OnInstallObjectCreated");
        // This creates a new GameObject and adds it to our scene.
        GameObject furn_go = new GameObject();

        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);

        // Add a sprite renderer, but don't bother setting a sprite
        // because all the tiles are empty right now.
        furn_go.AddComponent<SpriteRenderer>().sprite = getSpriteForFurniture(furn);
        furn_go.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";

        furnitureGameObjectMap.Add(furn, furn_go);
        // Use a lambda to create an anonymous function to "wrap" our callback function
        furn.RegisterOnChangeCallback(OnFurnitureChanged);
        //tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
        
    }

    void OnFurnitureChanged(Furniture furn) {
        // Make sure the graphics are correct

        if (!furnitureGameObjectMap.ContainsKey(furn)) {
            Debug.LogError("onFurnitureChanged -- Trying to change visuals for furniture not in our map");
            return;
        }

        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = getSpriteForFurniture(furn);
    }

    Sprite getSpriteForFurniture(Furniture obj) {
        if(obj.linksToNeighbour == false) {
            return furnitureSprites[obj.objectType];
        }

        string spriteName = obj.objectType;

        //Check for neighbours 
        int x = obj.tile.X;
        int y = obj.tile.Y;

        Tile t;

        t = World.GetTileAt(x, y + 1);
        if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }
        t = World.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }
        t = World.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }
        t = World.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "W";
        }

        if (!furnitureSprites.ContainsKey(spriteName)) {
            Debug.LogError("getSpriteForInstallObject -- not sprite with name " + spriteName);
        }

        return furnitureSprites[spriteName];
    }

}
