//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

public class Tile {

	// TileType is the base type of the tile. In some tile-based games, that might be
	// the terrain type. For us, we only need to differentiate between empty space
	// and floor (a.k.a. the station structure/scaffold). Walls/Doors/etc... will be
	// InstalledObjects sitting on top of the floor.
	public enum TileType { Empty, Floor };

	private TileType _type = TileType.Empty;
	public TileType Type {
		get { return _type; }
		set {
			TileType oldType = _type;
			_type = value;
			// Call the callback and let things know we've changed.

			if(cbTileChanged != null && oldType != _type)
				cbTileChanged(this);
		}
	}

	// LooseObject is something like a drill or a stack of metal sitting on the floor
	LooseObject looseObject;

    // InstalledObject is something like a wall, door, or sofa.
    public Furniture furniture { get; protected set; }

    public Job pendingFurnitureJob;

	// We need to know the context in which we exist. Probably. Maybe.
    public World world { get; protected set; }

    public int X { get; protected set; }
	public int Y { get; protected set; }

	// The function we callback any time our type changes
	Action<Tile> cbTileChanged;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">A World instance.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile( World world, int x, int y ) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	/// <summary>
	/// Register a function to be called back when our tile type changes.
	/// </summary>
	public void RegisterTileChangedCallback(Action<Tile> callback) {
		cbTileChanged += callback;
	}
	
	/// <summary>
	/// Unregister a callback.
	/// </summary>
	public void UnegisterTileChangedCallback(Action<Tile> callback) {
		cbTileChanged -= callback;
	}

    public bool assignIntalledObj(Furniture objInstance) {
        if(objInstance == null) {
            furniture = null;
            return true;
        }

        if(furniture != null) {
            Debug.LogError("Trying to assign an isntalled object to a tile that already has one!");
            return false;
        }

        furniture = objInstance;
        return true;
    }
	
}
