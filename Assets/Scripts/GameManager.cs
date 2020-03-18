using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum algoType
{
    Dijkstra,
    AStar
}

public class GameManager : MonoBehaviour
{
    // For reference:
    // http://qiao.github.io/PathFinding.js/visual/

    private GameObject[] nodes;

    List<Node> result = new List<Node>();
    List<Node> unexplored = new List<Node>();
    
    private Color startColor = new Color(0.0f, 0.7f, 0.0f, 1.0f);
    private Color goalColor = Color.red;
    private Color frontierColor = new Color(0.3f, 1.0f, 0.4f, 1.0f);
    private Color exploredColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
    private Color pathColor = Color.yellow;
    private Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
    private Color highlightColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);

    public bool isRunning = false;
    public bool isPathfindComplete = false;
    
    public algoType algorithm = algoType.Dijkstra;

    private GridSystem m_grid;
    private Node currentNode;
    private Node sourceNode;
    private Node destinationNode;

    PriorityQueue<Node> m_frontierNodes; // open list
    List<Node> m_exploredNodes; // settled list
    [SerializeField] List<Node> m_pathNodes; // path list
    
    public bool canDiagonal = false;
    public bool canCrossCorners = false;
    public float heuristicW = 1.0f;

    private void Start()
    {
        m_grid = this.GetComponent<GridSystem>();
    }
    
    public void Init(Node start, Node end)
    {
        sourceNode = start;
        destinationNode = end;

        ShowColors(start, end);

        m_frontierNodes = new PriorityQueue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_grid.GetRowSize(); x++)
        {
            for (int y = 0; y < m_grid.GetColumnSize(); y++)
            {
                if (!m_grid.grid[x, y].walkable)
                    continue;

                if (canDiagonal)
                {
                    if (canCrossCorners)
                    {
                        m_grid.CrossCornerNeighbours(x, y);
                    }
                    else
                    {
                        m_grid.SetAllNeighbours(x, y);
                    }
                }
                else if (!canDiagonal)
                {
                    m_grid.SetSomeNeighbours(x, y);
                }
            }
        }
        isRunning = true;
        isPathfindComplete = false;
        start.cost = 0;
    }

    public void UpdateGrid(Node start, Node end)
    {
        sourceNode = start;
        destinationNode = end;

        ShowColors(start, end);
    }

    void ShowColors()
    {
        ShowColors(sourceNode, destinationNode);
    }

    void ShowColors(Node start, Node end)
    {
        if (start == null || end == null)
            return;

        if (m_frontierNodes != null)
        {
            m_grid.ColorNodeList(m_frontierNodes.ToList(), frontierColor);
        }

        if (m_exploredNodes != null)
        {
            m_grid.ColorNodeList(m_exploredNodes, exploredColor);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            m_grid.ColorNodeList(m_pathNodes, pathColor);
        }

        m_grid.ColorNode(start, startColor);
        m_grid.ColorNode(end, goalColor);
    }

    public IEnumerator SearchRoutine(float timestep = 0.0f)
    {
        float timeStart = Time.time;
        yield return null;
        Debug.Log("Starting search routine");
        while (!isPathfindComplete)
        {
            //Debug.Log(m_frontierNodes.Count);
            if (destinationNode.settled || m_frontierNodes.Count > 0 )
            {
                Node currentNode = m_frontierNodes.Dequeue();

                if (currentNode.walkable )
                {
                    if (!m_exploredNodes.Contains(currentNode))
                    {
                        m_exploredNodes.Add(currentNode);
                    }

                    if (algorithm == algoType.Dijkstra)
                    {
                        ExpandDijkstra(currentNode);
                    }
                    else if (algorithm == algoType.AStar)
                    {
                        ExpandAstar(currentNode);
                    }

                    //if (m_frontierNodes.Contains(destinationNode) )
                    if (destinationNode.settled || m_frontierNodes.Count == 0)
                    {
                        m_pathNodes = GetShortestPath(destinationNode);
                        isPathfindComplete = true;
                        isRunning = false;
                    }

                }
                
                ShowDiagnostics();
                yield return new WaitForSeconds(timestep);
            }
            else
            {
                isPathfindComplete = true;
                isRunning = false;
            }
        }
        ShowDiagnostics();
        Debug.Log("Pathfinder Search Routine: Elapsed time = " + (Time.time - timeStart).ToString() + " seconds");
    }

    public void RestartSearch()
    {
        m_grid.CleanupGrid();
        m_grid.ResetNeighbours();
        m_frontierNodes.Clear();
        m_frontierNodes = new PriorityQueue<Node>();
        m_exploredNodes.Clear();
        m_exploredNodes = new List<Node>();
        m_pathNodes.Clear();
        m_pathNodes = new List<Node>();
        ShowColors();
    }

    private void ShowDiagnostics()
    {
        ShowColors();
        
        if (m_grid != null)
        {

        }
    }

    private List<Node> GetShortestPath(Node endNode)
    {
        Node tempNode = new Node();
        //GameObject pathParent = new GameObject("Shortest Path");
        if (endNode == null)
        {
            Debug.Log("Error: End node not detected");
            return result;
        }
        else
        {
            tempNode = endNode;
            while (tempNode != null && !result.Contains(tempNode))
            {
                result.Add(tempNode);
                //tempNode.transform.SetParent(pathParent.transform);
                Node currentNode = tempNode;
                tempNode = currentNode.GetParentNode();
            }
            result.Reverse();
            return result;
        }
    }
    
    private void ExpandDijkstra(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbourNode.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbourNode[i]))
                {
                    if (!node.neighbourNode[i].walkable)
                        continue;
                    float distanceToNeighbor = m_grid.GetNodeDistance(node, node.neighbourNode[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.cost;

                    if (float.IsPositiveInfinity(node.neighbourNode[i].cost) ||
                        newDistanceTraveled < node.neighbourNode[i].cost)
                    {
                        node.neighbourNode[i].parentNode = node;
                        node.neighbourNode[i].cost = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbourNode[i]))
                    {
                        node.neighbourNode[i].priority = (int)node.neighbourNode[i].cost;
                        m_frontierNodes.Enqueue(node.neighbourNode[i]);
                    }
                }
            }
            node.settled = true;
        }
    }

    private void ExpandAstar(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbourNode.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbourNode[i]))
                {
                    if (!node.neighbourNode[i].walkable)
                        continue;
                    float distanceToNeighbor = m_grid.GetNodeDistance(node, node.neighbourNode[i]);
                    float newDistanceTraveled = (1 / heuristicW) * distanceToNeighbor + node.cost;

                    if (float.IsPositiveInfinity(node.neighbourNode[i].cost) ||
                        newDistanceTraveled < node.neighbourNode[i].cost)
                    {
                        node.neighbourNode[i].parentNode = node;
                        node.neighbourNode[i].cost = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbourNode[i]) && m_grid != null)
                    {
                        int distanceToGoal = (int)m_grid.GetNodeDistance(node.neighbourNode[i], destinationNode);
                        node.neighbourNode[i].priority = (int)node.neighbourNode[i].cost + distanceToGoal;
                        m_frontierNodes.Enqueue(node.neighbourNode[i]);
                    }
                }
            }
            node.settled = true;
        }
    }

    public bool GetPathList(List<Node> pathList)
    {
        if (isPathfindComplete)
        {
            pathList = m_pathNodes;
            return true;
        }
        else
        {
            return false;
        }
    }
}

