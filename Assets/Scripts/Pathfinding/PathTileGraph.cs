using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileGraph {

    public Dictionary<Tile, PathNode<Tile>> nodes;

	public PathTileGraph (World world) {

        nodes = new Dictionary<Tile, PathNode<Tile>>();

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {

                Tile t = world.GetTileAt(x, y);

                if(t.movementCost > 0) {
                    PathNode<Tile> n = new PathNode<Tile>();
                    n.data = t;
                    nodes.Add(t, n);
                }
            }
        }

        Debug.Log("PathTileGraph: Created " + nodes.Count + "nodes.");

        int edgeCount = 0;

        foreach (Tile t in nodes.Keys) {
            PathNode<Tile> n = nodes[t];

            List<PathEdge<Tile>> edges = new List<PathEdge<Tile>>();

            Tile[] neighbours = t.getNeighbours(true);

            for (int i = 0; i < neighbours.Length; i++) {
                if(neighbours[i] != null && neighbours[i].movementCost > 0) {
                    PathEdge<Tile> e = new PathEdge<Tile>();
                    e.cost = neighbours[i].movementCost;
                    e.node = nodes[neighbours[i]];

                    edges.Add(e);

                    edgeCount++;
                }
            }

            Debug.Log("PathTileGraph: Created " + edgeCount + "edges.");

            n.edges = edges.ToArray();
        }
    }

}
