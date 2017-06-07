//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class World {

	// A two-dimensional array to hold our tile data.
	Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;

	// The tile width of the world.
	public int Width { get; protected set; }

	// The tile height of the world
	public int Height { get; protected set; }

    Action<InstalledObject> cbInstalledObjectCreated;

	/// <summary>
	/// Initializes a new instance of the <see cref="World"/> class.
	/// </summary>
	/// <param name="width">Width in tiles.</param>
	/// <param name="height">Height in tiles.</param>
	public World(int width = 100, int height = 100) {
		Width = width;
		Height = height;

		tiles = new Tile[Width,Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tiles[x,y] = new Tile(this, x, y);
			}
		}

		Debug.Log ("World created with " + (Width*Height) + " tiles.");

        initInstalledObjects();
    }

    void initInstalledObjects() {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();
  
        InstalledObject wallProt = InstalledObject.createPrototype(
            "wall",
            0f,
            1,
            1,
            true); // links to neighbours

        installedObjectPrototypes.Add("wall", wallProt);
    }


	/// <summary>
	/// A function for testing out the system
	/// </summary>
	public void RandomizeTiles() {
		Debug.Log ("RandomizeTiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {

				if(UnityEngine.Random.Range(0, 2) == 0) {
					tiles[x,y].Type = Tile.TileType.Empty;
				}
				else {
					tiles[x,y].Type = Tile.TileType.Floor;
				}

			}
		}
	}

	/// <summary>
	/// Gets the tile data at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y) {
		if( x > Width || x < 0 || y > Height || y < 0) {
			Debug.LogError("Tile ("+x+","+y+") is out of range.");
			return null;
		}
		return tiles[x, y];
	}

    public void placeInstalledObject(string objectType, Tile t) {

        Debug.Log("PlaceInstalledObject!");

        if (!installedObjectPrototypes.ContainsKey(objectType)) {
            Debug.LogError("installedObjectprototypes doesn;t contain a rpoto for key: " + objectType);
            return;
        }

        InstalledObject obj = InstalledObject.place(installedObjectPrototypes[objectType], t);

        if (obj == null) {
            Debug.LogError("Trying to install a wall, probably there is already a wall there");
            return;
        }

        if(cbInstalledObjectCreated != null) {
            cbInstalledObjectCreated(obj);
        }
    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callfackFunc) {
        cbInstalledObjectCreated += callfackFunc;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callfackFunc) {
        cbInstalledObjectCreated -= callfackFunc;
    }
}
