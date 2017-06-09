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

    Dictionary<string, Furniture> furniturePrototypes;

	// The tile width of the world.
	public int Width { get; protected set; }

	// The tile height of the world
	public int Height { get; protected set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    // should be replaced by JobQueue class later
    public Queue<Job> jobQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    /// <param name="width">Width in tiles.</param>
    /// <param name="height">Height in tiles.</param>
    public World(int width = 100, int height = 100) {
        jobQueue = new Queue<Job>();

        Width = width;
		Height = height;

		tiles = new Tile[Width,Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tiles[x,y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileChangedCallback(OnTileChanged);
            }
		}

		Debug.Log ("World created with " + (Width*Height) + " tiles.");

        initFurniture();
    }

    void initFurniture() {
        furniturePrototypes = new Dictionary<string, Furniture>();
  
        Furniture wallProt = Furniture.createPrototype(
            "wall_",
            0f,
            1,
            1,
            true); // links to neighbours

        furniturePrototypes.Add("wall_", wallProt);
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
			//Debug.LogError("Tile ("+x+","+y+") is out of range.");
			return null;
		}
		return tiles[x, y];
	}

    public void placeFurniture(string objectType, Tile t) {

        //Debug.Log("placeFurniture!");

        if (!furniturePrototypes.ContainsKey(objectType)) {
            Debug.LogError("installedObjectprototypes doesn;t contain a rpoto for key: " + objectType);
            return;
        }

        Furniture obj = Furniture.place(furniturePrototypes[objectType], t);

        if (obj == null) {
            Debug.LogError("Trying to install a wall, probably there is already a wall there " + obj);
            return;
        }

        if(cbFurnitureCreated != null) {
            cbFurnitureCreated(obj);
        }
    }

    public void RegisterFurnitureCreated(Action<Furniture> callfackFunc) {
        cbFurnitureCreated += callfackFunc;
    }

    public void UnregisterFurnitureCreated(Action<Furniture> callfackFunc) {
        cbFurnitureCreated -= callfackFunc;
    }

    public void RegisterTileChanged(Action<Tile> callfackFunc) {
        cbTileChanged += callfackFunc;
    }

    public void UnregisterTileChanged(Action<Tile> callfackFunc) {
        cbTileChanged -= callfackFunc;
    }

    void OnTileChanged(Tile t) {
        if (cbTileChanged == null) {
            return;
        }
        cbTileChanged(t);
    }

    public bool isFurniturePlacementValid(string furnType, Tile t) {
        return furniturePrototypes[furnType].isValidPosition(t);
    }
}
