using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public GridSystem gridManager;

    List<Node> result = new List<Node>();
    List<Node> unexplored = new List<Node>();

    private GameObject[] nodes;

    private bool isPathfindComplete = false;

    private void Start()
    {
        gridManager = GetComponent<GridSystem>();
    }

    public List<Node> findShortestPath(Node start, Node end)
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");

        Node node = DijkstrasAlgo(start, end);
        
        while (node != null)
        {
            result.Add(node);
            Node currentNode = node.GetComponent<Node>();
            node = currentNode.GetParentNode();
        }
        
        result.Reverse();
        return result;
    }

    private Node DijkstrasAlgo(Node start, Node end)
    {
        double startTime = Time.realtimeSinceStartup;
        
        foreach (GameObject obj in nodes)
        {
            Node n = obj.GetComponent<Node>();
            if (n.isWalkable())
            {
                n.ResetNode();
                unexplored.Add(n);
            }
        }
        
        Node startNode = start.GetComponent<Node>();
        startNode.SetCost(0);

        CheckNeighbours();

        while (!isPathfindComplete)
        {

        }

        double endTime = (Time.realtimeSinceStartup - startTime);
        print("Compute time: " + endTime);

        print("Path completed!");

        return end;
    }
    
    private IEnumerator CheckNeighbours()
    {
        while (unexplored.Count > 0)
        {
            unexplored.Sort((x, y) => x.GetComponent<Node>().GetCost().CompareTo(y.GetComponent<Node>().GetCost()));

            Node current = unexplored[0];
            current.settled = true;
            unexplored.Remove(current);

            Node currentNode = current.GetComponent<Node>();

            Renderer rend = currentNode.GetComponent<Renderer>();
            rend.material.color = Color.grey;

            List<Node> neighbours = currentNode.GetNeighbourNode();
            foreach (Node neighbourNode in neighbours)
            {
                Node node = neighbourNode.GetComponent<Node>();

                if (unexplored.Contains(neighbourNode) && node.isWalkable() && !node.settled)
                {
                    float distance = Vector3.Distance(neighbourNode.transform.position, current.transform.position);
                    distance = currentNode.GetCost() + distance;

                    if (distance < node.GetCost())
                    {
                        node.SetCost(distance);
                        node.SetParent(current);
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        isPathfindComplete = true;
    }

}
