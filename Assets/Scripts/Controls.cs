using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    public GridSystem gridSys;

    public Dropdown algoType;
    public Toggle diagonal;
    public Toggle crossCorners;
    public InputField weight;

    private GameManager finder;
    [SerializeField] private Node currentNode = new Node();
    [SerializeField] private Node previousNode = new Node();
    private Queue<Node> previousNodes = new Queue<Node>();
    private bool startNodeSelected = false;
    private bool endNodeSelected = false;
    private bool buildWallMode = false;
    private bool destroyWallMode = false;
    private bool isComplete = false;
    private Vector3 mousePos;

    public Node startNode;
    public Node endNode;
    [SerializeField] private List<Node> blockedPath = new List<Node>();

    private void Start()
    {
        finder = gameObject.GetComponent<GameManager>();
        CreateInitialNodes();
        finder.UpdateGrid(startNode, endNode);

        algoType.value = (int)finder.algorithm;
        diagonal.isOn = finder.canDiagonal;
        crossCorners.isOn = finder.canCrossCorners;
        weight.text = finder.heuristicW.ToString();
        weight.characterValidation = InputField.CharacterValidation.Decimal;
    }

    private void Update()
    {
        newMouseControl();

        algoType.interactable = !finder.isRunning;
        diagonal.interactable = !finder.isRunning;
        crossCorners.interactable = !finder.isRunning;
        weight.interactable = !finder.isRunning;
    }

    private void CreateInitialNodes()
    {
        int tempX = gridSys.GetRowSize() / 2;
        int tempY = gridSys.GetColumnSize() / 2;
        startNode = gridSys.grid[tempX - 5, tempY];
        endNode = gridSys.grid[tempX + 5, tempY];
    }

    private void newMouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentNode = gridSys.grid[(int)(mousePos.x + 0.5f), (int)(mousePos.y + 0.5f)];

            if (currentNode != null) // Selects a node
            {
                previousNode = currentNode;
                
                if (currentNode == startNode) // Start node selected
                {
                    startNodeSelected = true;
                }
                else if (currentNode == endNode) // End node selected
                {
                    endNodeSelected = true;
                }
                else if (previousNode.walkable) // Create wall on click + activates wall-building mode
                {
                    buildWallMode = true;
                    CreateWall(previousNode);
                }
                else if (!previousNode.walkable) // Delete wall on click + activates wall-destroying mode
                {
                    destroyWallMode = true;
                    DestroyWall(previousNode);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (currentNode != null)
            {
                previousNode = currentNode;
            }

            if (previousNode.walkable)
            {
                previousNodes.Enqueue(previousNode);
            }

            currentNode = gridSys.grid[(int)(mousePos.x + 0.5f), (int)(mousePos.y + 0.5f)]; // Update current mouse-on-grid position

            if (startNodeSelected) // Moving start node
            {
                if (currentNode != endNode && currentNode.walkable)
                {
                    startNode = currentNode;
                }
            }
            else if (endNodeSelected)
            {
                if (currentNode != startNode && currentNode.walkable)
                {
                    endNode = currentNode;
                }
            }
            else if (buildWallMode && previousNode.walkable)
            {
                CreateWall(previousNode);
            }
            else if (destroyWallMode && !previousNode.walkable)
            {
                DestroyWall(previousNode);
            }

            if (previousNodes.Count != 0) // starts cleaning up mouse trail
                StartCoroutine(ClearPreviousNodes());

            finder.UpdateGrid(startNode, endNode);
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentNode = null;
            startNodeSelected = false;
            endNodeSelected = false;
            buildWallMode = false;
            destroyWallMode = false;
        }
    }

    public void OnAlgorithmTypeChange()
    {
        if (!finder.isRunning)
        {
            finder.algorithm = (algoType)algoType.value;
        }
    }

    public void OnDiagonalToggle()
    {
        if (!finder.isRunning)
        {
            finder.canDiagonal = diagonal.isOn;
        }

    }

    public void OnCrossCornersToggle()
    {
        if (!finder.isRunning)
        {
            finder.canCrossCorners = crossCorners.isOn;
        }
    }

    public void OnWeightInput()
    {
        if (!finder.isRunning)
        {
            finder.heuristicW = float.Parse(weight.text);
        }
    }

    private IEnumerator ClearPreviousNodes()
    {
        Node tempNode = new Node();

        while (previousNodes.Count > 0)
        {
            tempNode = previousNodes.Dequeue();
            if (tempNode != startNode && tempNode != endNode && tempNode.walkable)
                gridSys.ColorNode(tempNode, Color.white);
            yield return new WaitForSeconds(0.0f);
        }
    }

    public void FindPathButton()
    {
        ClearSearch();
        StartCoroutine(FindPath());
    }

    public IEnumerator FindPath()
    {
        if (startNode != null && endNode != null)
        {
            List<Node> paths = new List<Node>();
            finder.Init(startNode, endNode);
            StartCoroutine(finder.SearchRoutine());

            while(!isComplete)
            {
                isComplete = finder.GetPathList(paths);

                yield return null;

            }
        }
    }
    
    public void CreateWall(Node node)
    {
        node.walkable = false;
        blockedPath.Add(node);
        gridSys.ColorNode(node, Color.black);
    }

    public void DestroyWall(Node node)
    {
        node.walkable = true;
        gridSys.ColorNode(node, Color.white);
        blockedPath.Remove(node);
    }

    public void ClearBlocks()
    {
        
        StartCoroutine(DestroyAllWalls());
        
    }

    public IEnumerator DestroyAllWalls()
    {

        Node tempNode = new Node();

        while (blockedPath.Count > 0)
        {
            tempNode = blockedPath[0];
            gridSys.ColorNode(tempNode, Color.white);
            tempNode.walkable = true;
            blockedPath.Remove(tempNode);

            yield return new WaitForSeconds(0.0f);
        }
        yield return null;
    }
    
    public void CleanFromSearch()
    {
        finder.RestartSearch();
        for (int x = 0; x < gridSys.GetRowSize(); x++)
        {
            for (int y = 0; y < gridSys.GetColumnSize(); y++)
            {
                if (gridSys.grid[x, y] != startNode && gridSys.grid[x, y] != endNode && gridSys.grid[x, y].walkable)
                    gridSys.ColorNode(gridSys.grid[x, y], Color.white);
            }
        }
    }

    public void Restart()
    {
        Debug.Log("Reloading Scene");
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }
    
    private void ClearSearch()
    {
        for (int x = 0; x < gridSys.GetRowSize(); x++)
        {
            for (int y = 0; y < gridSys.GetColumnSize(); y++)
            {
                if (gridSys.grid[x,y] != startNode && gridSys.grid[x,y] != endNode && gridSys.grid[x,y].walkable)
                {
                    gridSys.ColorNode(gridSys.grid[x, y], Color.white);
                    gridSys.grid[x, y].ResetNode();
                }
            }
        }
    }
    
}
