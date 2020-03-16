using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // For reference:
    // http://qiao.github.io/PathFinding.js/visual/

    private GameObject[] nodes;

    List<Node> result = new List<Node>();
    List<Node> unexplored = new List<Node>();
    
    public Color startColor = new Color(0.0f, 0.7f, 0.0f, 1.0f);
    public Color goalColor = Color.red;
    public Color frontierColor = new Color(0.3f, 1.0f, 0.4f, 1.0f);
    public Color exploredColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
    public Color pathColor = Color.yellow;
    public Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
    public Color highlightColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);

    private bool isPathfindComplete = false;
    
    private GridSystem m_grid;
    private Node currentNode;
    private Node sourceNode;
    private Node destinationNode;

    Queue<Node> m_frontierNodes; // open list
    List<Node> m_exploredNodes; // settled list
    [SerializeField] List<Node> m_pathNodes; // path list

    public bool isDijkstra = false;
    public bool isAstar = true;
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

        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_grid.GetRowSize(); x++)
        {
            for (int y = 0; y < m_grid.GetColumnSize(); y++)
            {
                if (canDiagonal)
                    m_grid.Set8Neighbours();
                else
                    m_grid.Set4Neighbours(x, y);
            }
        }

        isPathfindComplete = false;
        start.fCost = 0;
        start.gCost = 0;
        start.hCost = 0;
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

                if (currentNode.walkable)
                {
                    if (!m_exploredNodes.Contains(currentNode))
                    {
                        m_exploredNodes.Add(currentNode);
                    }

                    if (isDijkstra)
                    {
                        ExpandDijkstra(currentNode);
                    }
                    else if (isAstar)
                    {
                        ExpandAstar(currentNode);
                    }

                    //if (m_frontierNodes.Contains(destinationNode) )
                    if (destinationNode.settled || m_frontierNodes.Count == 0)
                    {
                        m_pathNodes = GetShortestPath(destinationNode);
                        isPathfindComplete = true;
                    }

                }
                
                ShowDiagnostics();
                yield return new WaitForSeconds(timestep);
            }
            else
            {
                isPathfindComplete = true;
            }
        }
        ShowDiagnostics();
        Debug.Log("Pathfinder Search Routine: Elapsed time = " + (Time.time - timeStart).ToString() + " seconds");
    }

    public void RestartSearch()
    {
        m_frontierNodes.Clear();
        m_exploredNodes.Clear();
        m_pathNodes.Clear();
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
                    if (node.neighbourNode[i] != null && node.neighbourNode[i].walkable)
                    {
                        float distanceToNeighbor = Vector3.Distance(node.neighbourNode[i].transform.position, node.transform.position);
                        float newCost = distanceToNeighbor + node.gCost;

                        if (float.IsPositiveInfinity(node.neighbourNode[i].gCost) || newCost < node.neighbourNode[i].gCost)
                        {
                            node.neighbourNode[i].SetParent(node);
                            node.neighbourNode[i].gCost = newCost;
                        }

                        if (!m_frontierNodes.Contains(node.neighbourNode[i]))
                        {
                            m_frontierNodes.Enqueue(node.neighbourNode[i]);
                        }
                    }
                }
            }
        }
        node.settled = true;
    }

    private void ExpandAstar(Node node)
    {
        if (node.settled || !node.walkable)
        {
            return;
        }

        foreach (Node neighbour in node.neighbourNode)
        {
            float Dx = neighbour.transform.position.x - node.transform.position.x;
            float Dy = neighbour.transform.position.y - node.transform.position.y;

            float tempG = Vector3.Distance(neighbour.transform.position, node.transform.position);
            float tempH = Mathf.Abs(Dx) + Mathf.Abs(Dy);
            float tempF = neighbour.gCost + heuristicW * neighbour.hCost;
            if (!m_frontierNodes.Contains(neighbour))
            {
                m_frontierNodes.Enqueue(neighbour);
                neighbour.SetParent(node);
                neighbour.gCost = tempG;
                neighbour.hCost = tempH;
                neighbour.UpdateCost(heuristicW);

                //float Dx = neighbour.transform.position.x - node.transform.position.x;
                //float Dy = neighbour.transform.position.y - node.transform.position.y;

                //neighbour.gCost = Vector3.Distance(neighbour.transform.position, node.transform.position);
                //neighbour.hCost = Mathf.Abs(Dx) + Mathf.Abs(Dy);
                //neighbour.fCost = neighbour.gCost + heuristicW * neighbour.hCost;
            }
            else if (m_frontierNodes.Contains(neighbour))
            {
                if (tempG < neighbour.gCost)
                {
                    neighbour.SetParent(node);
                    neighbour.gCost = tempG;
                    neighbour.hCost = tempH;
                    neighbour.UpdateCost(heuristicW);
                }
            }
        }
        node.settled = true;
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

