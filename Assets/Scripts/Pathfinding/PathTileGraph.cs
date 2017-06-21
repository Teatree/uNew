//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections.Generic;


public class PathTileGraph {

    // This class constructs a simple path-finding compatible graph
    // of our world.  Each tile is a node. Each WALKABLE neighbour
    // from a tile is linked via an edge connection.

    public Dictionary<Tile, PathNode<Tile>> nodes;

    public PathTileGraph(World world) {

        Debug.Log("Path_TileGraph");

        // Loop through all tiles of the world
        // For each tile, create a node
        //  Do we create nodes for non-floor tiles?  NO!
        //  Do we create nodes for tiles that are completely unwalkable (i.e. walls)?  NO!

        nodes = new Dictionary<Tile, PathNode<Tile>>();

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {

                Tile t = world.GetTileAt(x, y);

                //if(t.movementCost > 0) {	// Tiles with a move cost of 0 are unwalkable
                PathNode<Tile> n = new PathNode<Tile>();
                n.data = t;
                nodes.Add(t, n);
                //}

            }
        }

        Debug.Log("Path_TileGraph: Created " + nodes.Count + " nodes.");


        // Now loop through all nodes again
        // Create edges for neighbours

        int edgeCount = 0;

        foreach (Tile t in nodes.Keys) {
            PathNode<Tile> n = nodes[t];

            List<PathEdge<Tile>> edges = new List<PathEdge<Tile>>();

            // Get a list of neighbours for the tile
            Tile[] neighbours = t.getNeighbours(true);  // NOTE: Some of the array spots could be null.

            // If neighbour is walkable, create an edge to the relevant node.
            for (int i = 0; i < neighbours.Length; i++) {
                if (neighbours[i] != null && neighbours[i].movementCost > 0) {
                    // This neighbour exists and is walkable, so create an edge.

                    if (isClippingCorner (t, neighbours[i])) {
                        continue;
                    }

                    PathEdge<Tile> e = new PathEdge<Tile>();
                    e.cost = neighbours[i].movementCost;
                    e.node = nodes[neighbours[i]];

                    // Add the edge to our temporary (and growable!) list
                    edges.Add(e);

                    edgeCount++;
                }
            }

            n.edges = edges.ToArray();
        }

        Debug.Log("Path_TileGraph: Created " + edgeCount + " edges.");

    }

    bool isClippingCorner(Tile curr, Tile neighbour) {

        int dX = curr.X - neighbour.X;
        int dY = curr.Y - neighbour.Y;

        if (Mathf.Abs(dX) + Mathf.Abs(dY) == 2) { //is there a walkable tile on diagonal points around curr
          
            if (curr.world.GetTileAt(curr.X - dX, curr.Y).movementCost == 0) {
                return true;
            }
            if (curr.world.GetTileAt(curr.X, curr.Y - dY).movementCost == 0) {
                return true;
            }
        }
        return false;
    }
}
