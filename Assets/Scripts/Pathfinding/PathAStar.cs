using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class PathAStar {

    Queue<Tile> path;

	public PathAStar(World world, Tile tileStart, Tile tileEnd) {

        if(world.tileGraph == null) {
            world.tileGraph = new PathTileGraph(world);
        }

        Dictionary<Tile, PathNode<Tile>> nodes = world.tileGraph.nodes;

        PathNode<Tile> start = nodes[tileStart];
        PathNode<Tile> end = nodes[tileStart];

        if (nodes.ContainsKey(tileStart) == false) {
            Debug.LogError("PathAStar: the starting tile isn't in the list of tiles");
            return;
        }
        if (nodes.ContainsKey(tileEnd) == false) {
            Debug.LogError("PathAStar: the starting tile isn't in the list of tiles");
            return;
        }

        List<PathNode<Tile>> CloseSet = new List<PathNode<Tile>>();

        //List<PathNode<Tile>> OpenSet = new List<PathNode<Tile>>();
        //OpenSet.Add(start);

        SimplePriorityQueue<PathNode<Tile>> OpenSet = new SimplePriorityQueue<PathNode<Tile>>();
        OpenSet.Enqueue(start, 0);

        Dictionary<PathNode<Tile>, PathNode<Tile>> cameFrome = new Dictionary<PathNode<Tile>, PathNode<Tile>>();

        Dictionary<PathNode<Tile>, float> gScore = new Dictionary<PathNode<Tile>, float>();
        foreach (PathNode<Tile> n in nodes.Values) {
            gScore[n] = Mathf.Infinity;
        }
        gScore[start] = 0;

        Dictionary<PathNode<Tile>, float> fScore = new Dictionary<PathNode<Tile>, float>();
        foreach (PathNode<Tile> n in nodes.Values) {
            fScore[n] = Mathf.Infinity;
        }
        fScore[start] = heuristicCostEst(start, end);

        while(OpenSet.Count > 0) {
            PathNode<Tile> currentNode = OpenSet.Dequeue();

            if(currentNode == end) {
                reconsturctPath(cameFrome, currentNode);
                return;
            }

            CloseSet.Add(currentNode);

            foreach(PathEdge<Tile> neighbour in currentNode.edges) {
                if(CloseSet.Contains(neighbour.node) == true) {
                    continue;
                }

                float tentativeGScore = gScore[currentNode] + distBetween(currentNode, neighbour.node);

                if(OpenSet.Contains(neighbour.node) && tentativeGScore >= gScore[neighbour.node]) 
                    continue;

                cameFrome[neighbour.node] = currentNode;
                gScore[neighbour.node] = tentativeGScore;
                fScore[neighbour.node] = gScore[neighbour.node] + heuristicCostEst(neighbour.node, end);
                
                if(OpenSet.Contains(neighbour.node) == false) {
                    OpenSet.Enqueue(neighbour.node, fScore[neighbour.node]);
                }
            }
        }

        return;
    }

    float heuristicCostEst(PathNode<Tile> start, PathNode<Tile> end) {
        return Mathf.Sqrt(Mathf.Pow(start.data.X - end.data.X, 2) + Mathf.Pow(start.data.Y - end.data.Y, 2));
    }

    float distBetween(PathNode<Tile> start, PathNode<Tile> end) {
        if(Mathf.Abs(start.data.X - end.data.X) + Mathf.Abs(start.data.Y - end.data.Y) == 1) 
            return 1f;

        if (Mathf.Abs(start.data.X - end.data.X) == 1 && Mathf.Abs(start.data.Y - end.data.Y) == 1) 
            return 1.41421356f;

        return Mathf.Sqrt(Mathf.Pow(start.data.X - end.data.X, 2) + Mathf.Pow(start.data.Y - end.data.Y, 2));
    }

    void reconsturctPath(Dictionary<PathNode<Tile>, PathNode<Tile>> cameFrom, PathNode<Tile> currentNode) {
        Queue<Tile> totalPath = new Queue<Tile>();
        totalPath.Enqueue(currentNode.data);

        while (cameFrom.ContainsKey(currentNode)) {
            currentNode = cameFrom[currentNode];
            totalPath.Enqueue(currentNode.data);
        }

        path = new Queue<Tile>( totalPath.Reverse() );
    }

    public Tile GetNextTile() {
        return path.Dequeue();
    }
}
