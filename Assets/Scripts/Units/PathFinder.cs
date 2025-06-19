using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    static GamePlayer owner;
    static bool pathExists;

    public static List<Tile> FindPath(Tile origin, Tile destination, GamePlayer player)
    {
        owner = player;
        pathExists = true;
        Dictionary<Tile, Node> nodesNotEvaluated = new Dictionary<Tile, Node>();
        Dictionary<Tile, Node> nodesAlreadyEvaluated = new Dictionary<Tile, Node>();

        Node startNode = new Node(origin, origin, destination, 0);
        nodesNotEvaluated.Add(origin, startNode);

        bool gotPath = EvaluateNextNode(nodesNotEvaluated, nodesAlreadyEvaluated, origin, destination, out List<Tile> path);

        while (!gotPath && pathExists)
        {
            gotPath = EvaluateNextNode(nodesNotEvaluated, nodesAlreadyEvaluated, origin, destination, out path);
        }

        if (!pathExists)
        {
            return null;
        }

        path.Reverse();
        return path;
    }

    static bool EvaluateNextNode(Dictionary<Tile, Node> nodesNotEvaluated, Dictionary<Tile, Node> nodesEvaluated, 
        Tile origin, Tile destination, out List<Tile> path)
    {
        Node currentNode = GetCheapestNode(nodesNotEvaluated.Values.ToArray());

        if (currentNode == null)
        {
            path = new List<Tile>();
            return false;
        }

        nodesNotEvaluated.Remove(currentNode.target);
        nodesEvaluated.Add(currentNode.target, currentNode);

        path = new List<Tile>();

        // if path is found
        if (currentNode.target == destination) 
        {
            path.Add(currentNode.target);
            while (currentNode.target != origin)
            {
                path.Add(currentNode.parent.target);
                currentNode = currentNode.parent;
            }

            return true;
        }

        // try to traveserse the neighbours
        List<Node> neighbours = new List<Node>();
        foreach (Tile tile in currentNode.target.GetNeighbours())
        {
            if (tile.GetOwner() != owner && tile.GetOwner() != null && destination.GetOwner() != tile.GetOwner()) continue;

            Node node = new Node(tile, origin, destination, currentNode.GetCost());

            neighbours.Add(node);
        }

        if (neighbours.Count == 0)
        {
            pathExists = false;
            return true;
        }

        foreach (Node neighbour in neighbours)
        {
            // if the tile has already been evaluated
            if (nodesEvaluated.Keys.Contains(neighbour.target)) { continue; }

            if (neighbour.GetCost() < currentNode.GetCost() || !nodesNotEvaluated.Keys.Contains(neighbour.target))
            {
                neighbour.SetParent(currentNode);
                if (!nodesNotEvaluated.Keys.Contains(neighbour.target))
                {
                    nodesNotEvaluated.Add(neighbour.target, neighbour);
                }
            }
        }

        if (nodesNotEvaluated.Count == 0)
        {
            pathExists = false;
            return true;
        }

        return false;
    }

    static Node GetCheapestNode(Node[] nodesNotEvaluated)
    {
        if(nodesNotEvaluated.Length == 0) { return null; }

        Node selectedNode = nodesNotEvaluated[0];

        for (int i = 1; i < nodesNotEvaluated.Length; i++)
        {
            Node currentNode = nodesNotEvaluated[i];
            if (currentNode.GetCost() < selectedNode.GetCost())
            {
                selectedNode = currentNode;
            }
        }

        return selectedNode;
    }
}
