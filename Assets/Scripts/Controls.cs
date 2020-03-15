using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{
    public GridSystem gridSys;

    private Node node;
    private bool isComplete = false;

    public Node startNode;
    public Node endNode;
    private List<Node> blockedPath = new List<Node>();
    
    private void Update()
    {
        MouseControl();
    }

    private void newMouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ray ray = Camera.main.WorldToScreenPoint(Input.mousePosition);
            // mouse click -> compare mouse coordinates to grid coordinates
            // get node at the grid coordinates
            // can remove box colliders then to optimize system

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Node")
            {
                Renderer rend;
                if (node != null)
                {
                    rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.white;
                }

                node = hit.transform.gameObject.GetComponent<Node>();

                rend = node.GetComponent<Renderer>();
                rend.material.color = Color.green;
            }
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.colorBlockPath();
            this.updateNodeColor();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Node")
            {
                //Debug.Log("Node hit");
                Renderer rend;
                if (node != null)
                {
                    rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.white;
                }

                node = hit.transform.gameObject.GetComponent<Node>();

                rend = node.GetComponent<Renderer>();
                rend.material.color = Color.green;
            }
        }
    }
    
    public void StartNode()
    {
        if (node != null)
        {
            Node n = node.GetComponent<Node>();
            if (n.isWalkable())
            {
                if (startNode == null)
                {
                    Renderer rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.blue;
                }
                else
                {
                    Renderer rend = startNode.GetComponent<Renderer>();
                    rend.material.color = Color.white;
                    
                    rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.blue;
                }

                startNode = node;
                node = null;
            }
        }
    }
    
    public void EndNode()
    {
        if (node != null)
        {
            Node n = node.GetComponent<Node>();
            if (n.isWalkable())
            {
                if (endNode == null)
                {
                    Renderer rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.cyan;
                }
                else
                {
                    Renderer rend = endNode.GetComponent<Renderer>();
                    rend.material.color = Color.white;
                    rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.cyan;
                }

                endNode = node;
                node = null;
            }
        }
    }

    public void FindPathButton()
    {
        StartCoroutine(FindPath());
    }

    public IEnumerator FindPath()
    {
        if (startNode != null && endNode != null)
        {
            //Debug.Log("Starting pathfind...");
            //PathManager finder = gameObject.GetComponent<PathManager>();
            //List<Node> paths = finder.findShortestPath(startNode, endNode);

            List<Node> paths = new List<Node>();
            GameManager finder = gameObject.GetComponent<GameManager>();
            finder.Init(gridSys, startNode, endNode);
            StartCoroutine(finder.SearchRoutine());
            //Debug.Log("Searching...");

            while(!isComplete)
            {
                isComplete = finder.GetPathList(paths);

                yield return null;

            }

            if (isComplete)
            {
                Debug.Log("Path Complete");
                foreach (Node path in paths)
                {
                    Renderer rend = path.GetComponent<Renderer>();
                    rend.material.color = Color.red;
                }
            }
        }
    }
    

    public void BlockPath()
    {
        if (node != null)
        {
            Renderer rend = node.GetComponent<Renderer>();
            rend.material.color = Color.black;
            
            Node n = node.GetComponent<Node>();
            n.ChangeWalkability(false);
            
            blockedPath.Add(node);
            
            if (node == startNode)
            {
                startNode = null;
            }
            
            if (node == endNode)
            {
                endNode = null;
            }

            node = null;
        }
    }
    
    public void RemoveBlock()
    {
        if (node != null)
        {
            Renderer rend = node.GetComponent<Renderer>();
            rend.material.color = Color.white;
            
            Node n = node.GetComponent<Node>();
            n.ChangeWalkability(true);
            n.ResetNode();
            blockedPath.Remove(node);

            node = null;
        }
    }
    
    public void ClearBlocks()
    {
        foreach (Node path in blockedPath)
        {
            Node n = path.GetComponent<Node>();
            n.ChangeWalkability(true);
            
            Renderer rend = path.GetComponent<Renderer>();
            rend.material.color = Color.white;

        }
        blockedPath.Clear();
    }
    
    public void Restart()
    {
        Debug.Log("Reloading Scene");
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }
    
    private void colorBlockPath()
    {
        foreach (Node block in blockedPath)
        {
            Renderer rend = block.GetComponent<Renderer>();
            rend.material.color = Color.black;
        }
    }
    
    private void updateNodeColor()
    {
        if (startNode != null)
        {
            Renderer rend = startNode.GetComponent<Renderer>();
            rend.material.color = Color.blue;
        }

        if (endNode != null)
        {
            Renderer rend = endNode.GetComponent<Renderer>();
            rend.material.color = Color.cyan;
        }
    }
}
