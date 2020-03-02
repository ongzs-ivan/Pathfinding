using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{
    private Transform node;
    private Transform startNode;
    private Transform endNode;
    private List<Transform> blockedPath = new List<Transform>();
    
    private void Update()
    {
        MouseControl();
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("LMB clicked");
            this.colorBlockPath();
            this.updateNodeColor();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Node")
            {

                Debug.Log("Node hit");
                Renderer rend;
                if (node != null)
                {
                    rend = node.GetComponent<Renderer>();
                    rend.material.color = Color.white;
                }
                
                node = hit.transform;
                
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
    
    public void FindPath()
    {
        if (startNode != null && endNode != null)
        {
            PathManager finder = gameObject.GetComponent<PathManager>();
            List<Transform> paths = finder.findShortestPath(startNode, endNode);
            
            foreach (Transform path in paths)
            {
                Renderer rend = path.GetComponent<Renderer>();
                rend.material.color = Color.red;
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

        //// For selection grid system.
        //UnitSelectionComponent selection = gameObject.GetComponent<UnitSelectionComponent>();
        //List<Transform> selected = selection.getSelectedObjects();

        //foreach (Transform nd in selected)
        //{
        //    // Render the selected node to black.
        //    Renderer rend = nd.GetComponent<Renderer>();
        //    rend.material.color = Color.black;

        //    // Set selected node to not walkable
        //    Node n = nd.GetComponent<Node>();
        //    n.setWalkable(false);

        //    // Add the node to the block path list.
        //    blockPath.Add(nd);

        //    // If the block path is start node, we remove start node.
        //    if (nd == startNode)
        //    {
        //        startNode = null;
        //    }

        //    // If the block path is end node, we remove end node.
        //    if (nd == endNode)
        //    {
        //        endNode = null;
        //    }
        //}

        //selection.clearSelections();
    }
    
    public void RemoveBlock()
    {
        if (node != null)
        {
            Renderer rend = node.GetComponent<Renderer>();
            rend.material.color = Color.white;
            
            Node n = node.GetComponent<Node>();
            n.ChangeWalkability(true);
            
            blockedPath.Remove(node);

            node = null;
        }

        //// For selection grid system.
        //UnitSelectionComponent selection = gameObject.GetComponent<UnitSelectionComponent>();
        //List<Transform> selected = selection.getSelectedObjects();

        //foreach (Transform nd in selected)
        //{
        //    // Set selected node to white.
        //    Renderer rend = nd.GetComponent<Renderer>();
        //    rend.material.color = Color.white;

        //    // Set selected not to walkable.
        //    Node n = nd.GetComponent<Node>();
        //    n.ChangeWalkability(true);

        //    // Remove selected node from the block path list.
        //    blockedPath.Remove(nd);
        //}

        //selection.clearSelections();
    }
    
    public void ClearBlocks()
    {
        foreach (Transform path in blockedPath)
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
        foreach (Transform block in blockedPath)
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
