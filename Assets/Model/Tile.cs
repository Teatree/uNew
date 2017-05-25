using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {

	public enum Type {Empty, Floor };

	Type type = Type.Empty;

    Action<Tile> cbTileTypeChange;

	LooseObj looseObj;
	InstalledObj installedObj;

	World world;
	int x;
	int y;

	public Tile(World world, int x, int y){
		this.world = world;
		this.x = x;
		this.y = y;
	}

    public Type getType {
        get
        {
            return type;
        }
        set
        {
            Type oldType = type;
            type = value;

            if (cbTileTypeChange != null && oldType != type)
            { 
                cbTileTypeChange(this);
            }
        }
    }

    public int getX
    {
        get
        {
            return x;
        }
    }

    public int getY
    {
        get
        {
            return y;
        }
    }

    public void registerChangeTileTypeCB(Action<Tile> cb)
    {
        cbTileTypeChange += cb;
    }
}
