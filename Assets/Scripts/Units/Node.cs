public class Node
{
    public Node parent;
    public Tile target, destination, origin;
    public int baseCost, pathCost;

    public Node(Tile current, Tile origin, Tile destination, int pathCost)
    {
        parent = null;
        target = current;
        this.origin = origin;
        this.destination = destination;

        baseCost = current.GetPathCost();
        this.pathCost = pathCost;
    }

    public int GetCost()
    {
        return baseCost + pathCost;
    }

    public void SetParent(Node node)
    {
        parent = node;
    }
}
