//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

public class World : IXmlSerializable {

	// A two-dimensional array to hold our tile data.
	Tile[,] tiles;
    List<Character> characters;

    public PathTileGraph tileGraph;

	Dictionary<string, Furniture> furniturePrototypes;

	// The tile width of the world.
	public int Width { get; protected set; }

	// The tile height of the world
	public int Height { get; protected set; }

	Action<Furniture> cbFurnitureCreated;
    Action<Character> cbCharacterCreated;
    Action<Tile> cbTileChanged;

	// TODO: Most likely this will be replaced with a dedicated
	// class for managing job queues (plural!) that might also
	// be semi-static or self initializing or some damn thing.
	// For now, this is just a PUBLIC member of World
	public JobQueue jobQueue;

	/// <summary>
	/// Initializes a new instance of the <see cref="World"/> class.
	/// </summary>
	/// <param name="width">Width in tiles.</param>
	/// <param name="height">Height in tiles.</param>
	public World(int width, int height) {
        SetUpWorld(width, height);
    }

    void SetUpWorld(int width, int height) {
        jobQueue = new JobQueue();

        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }

        //Debug.Log ("World created with " + (Width*Height) + " tiles.");

        CreateFurniturePrototypes();

        characters = new List<Character>();
    }

    public void Update (float deltaTime) {

        foreach(Character c in characters) {
            c.Update(deltaTime);
        }
    }

    public Character createCharacter(Tile t) {
        Character c = new Character(t);

        characters.Add(c);

        if(cbCharacterCreated != null)
            cbCharacterCreated(c);

        return c;
    }

	void CreateFurniturePrototypes() {
		furniturePrototypes = new Dictionary<string, Furniture>();

		furniturePrototypes.Add("wall_", 
			Furniture.CreatePrototype(
								"wall_",
								0,	// Impassable
								1,  // Width
								1,  // Height
								true // Links to neighbours and "sort of" becomes part of a large object
							)
		);
	}

	/// <summary>
	/// A function for testing out the system
	/// </summary>
	public void RandomizeTiles() {
		Debug.Log ("RandomizeTiles");
        int xC = UnityEngine.Random.Range(10, 20);
        int yC = UnityEngine.Random.Range(10, 20);
        for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
                
                var perlin = Mathf.PerlinNoise((float)x / xC, (float)y / yC);                              // just remove this
                //Debug.Log("x: " + x + " y: "+ y + " || perlin - " + perlin);
                if (perlin > .4f) {
                    tiles[x, y].Type = TileType.Grass;
                }
                if (perlin < .4f && perlin > .3f) {
                    tiles[x, y].Type = TileType.Earth;
                }
                if (perlin < .3f) {
                    tiles[x, y].Type = TileType.Water;
                }
                // tile_go = (GameObject)Instantiate(tile_go, new Vector3(x, y, 2), Quaternion.identity);

                //if (UnityEngine.Random.Range(0, 2) == 0) {
                //   tiles[x, y].Type = TileType.Empty;
                //}
                //else {
                //  tiles[x, y].Type = TileType.Floor;
                //}


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
		if( x >= Width || x < 0 || y >= Height || y < 0) {
			//Debug.LogError("Tile ("+x+","+y+") is out of range.");
			return null;
		}
		return tiles[x, y];
	}


	public void PlaceFurniture(string objectType, Tile t) {
		//Debug.Log("PlaceInstalledObject");
		// TODO: This function assumes 1x1 tiles -- change this later!

		if( furniturePrototypes.ContainsKey(objectType) == false ) {
			Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
			return;
		}

		Furniture obj = Furniture.PlaceInstance( furniturePrototypes[objectType], t);

		if(obj == null) {
			// Failed to place object -- most likely there was already something there.
			return;
		}

		if(cbFurnitureCreated != null) {
			cbFurnitureCreated(obj);
            InvalidateTimeGraph();
        }
	}

	public void RegisterFurnitureCreated(Action<Furniture> callbackfunc) {
		cbFurnitureCreated += callbackfunc;
	}

	public void UnregisterFurnitureCreated(Action<Furniture> callbackfunc) {
		cbFurnitureCreated -= callbackfunc;
	}

    public void RegisterCharacterCreated(Action<Character> callbackfunc) {
        cbCharacterCreated += callbackfunc;
    }

    public void UnregisterCharacterCreated(Action<Character> callbackfunc) {
        cbCharacterCreated -= callbackfunc;
    }

    public void RegisterTileChanged(Action<Tile> callbackfunc) {
		cbTileChanged += callbackfunc;
	}

	public void UnregisterTileChanged(Action<Tile> callbackfunc) {
		cbTileChanged -= callbackfunc;
	}

	void OnTileChanged(Tile t) {
		if(cbTileChanged == null)
			return;
		
		cbTileChanged(t);

        InvalidateTimeGraph();
	}

    public void InvalidateTimeGraph() {
        tileGraph = null;
    }

	public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
		return furniturePrototypes[furnitureType].IsValidPosition(t);
	}

	public Furniture GetFurniturePrototype(string objectType) {
		if(furniturePrototypes.ContainsKey(objectType) == false) {
			Debug.LogError("No furniture with type: " + objectType);
			return null;
		}

		return furniturePrototypes[objectType];
	}

    public XmlSchema GetSchema() {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader) {
        Debug.Log("XML RUNS!");
        int width = int.Parse(reader.GetAttribute("Width"));
        int height = int.Parse(reader.GetAttribute("Height"));

        SetUpWorld(width, height);

        reader.ReadToDescendant("Tiles");
        reader.ReadToDescendant("Tile");
        while (reader.IsStartElement("Tile")) {

            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            tiles[x, y].ReadXml(reader);

            reader.ReadToNextSibling("Tile");
        }
        Debug.Log(reader.ToString());
    }

    public void WriteXml(XmlWriter writer) {
        //width x height
        writer.WriteAttributeString( "Width", Width.ToString() );
        writer.WriteAttributeString( "Height", Height.ToString() );
        // Tiles
        writer.WriteStartElement("Tiles");
        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();
    }

    public World() {

    }
}
