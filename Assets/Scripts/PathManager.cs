using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private GameObject[] nodes;
    
    public List<Transform> findShortestPath(Transform start, Transform end)
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");

        List<Transform> result = new List<Transform>();
        Transform node = DijkstrasAlgo(start, end);
        
        while (node != null)
        {
            result.Add(node);
            Node currentNode = node.GetComponent<Node>();
            node = currentNode.GetParentNode();
        }
        
        result.Reverse();
        return result;
    }

    private Transform DijkstrasAlgo(Transform start, Transform end)
    {
        double startTime = Time.realtimeSinceStartup;
        List<Transform> unexplored = new List<Transform>();
        
        foreach (GameObject obj in nodes)
        {
            Node n = obj.GetComponent<Node>();
            if (n.isWalkable())
            {
                n.ResetNode();
                unexplored.Add(obj.transform);
            }
        }
        
        Node startNode = start.GetComponent<Node>();
        startNode.SetCost(0);

        while (unexplored.Count > 0)
        {
            unexplored.Sort((x, y) => x.GetComponent<Node>().GetCost().CompareTo(y.GetComponent<Node>().GetCost()));
            
            Transform current = unexplored[0];
            
            unexplored.Remove(current);

            Node currentNode = current.GetComponent<Node>();
            List<Transform> neighbours = currentNode.GetNeighbourNode();
            foreach (Transform neighNode in neighbours)
            {
                Node node = neighNode.GetComponent<Node>();
                
                if (unexplored.Contains(neighNode) && node.isWalkable())
                {
                    float distance = Vector3.Distance(neighNode.position, current.position);
                    distance = currentNode.GetCost() + distance;
                    
                    if (distance < node.GetCost())
                    {
                        node.SetCost(distance);
                        node.SetParent(current);
                    }
                }
            }
        }

        double endTime = (Time.realtimeSinceStartup - startTime);
        print("Compute time: " + endTime);

        print("Path completed!");

        return end;
    }

}
