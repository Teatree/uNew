using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public enum Type {Empty, Floor };

	Type type = Type.Empty;

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
}
