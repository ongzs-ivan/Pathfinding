using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private int row = minRow;
    private int column = minColumn;
    
    [Space]
    public Sprite tile;

    private const int minRow = 20;
    private const int minColumn = 20;
    private const int maxRow = 108;
    private const int maxColumn = 108;

    public Node[,] grid = new Node[maxRow, maxColumn];

    void Start()
    {
        this.CreateGrid();
    }

    public int GetRowSize()
    {
        return row;
    }

    public int GetColumnSize()
    {
        return column;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                GameObject temp = new GameObject("x: " + x + " y: " + y);
                temp.AddComponent<Node>().CreateNode(new int[] { x, y }, tile);
                temp.transform.SetParent(this.transform);
                grid[x, y] = temp.GetComponent<Node>();
            }
        }
    }

    public void ResetNeighbours()
    {
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                grid[x, y].ResetNeighbourNodes();
            }
        }
    }

    public void Set4Neighbours()
    {
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                if (x - 1 >= 0) // West Node
                    grid[x, y].AddNeighbourNode(grid[x - 1, y]);

                if (x + 1 <= row) // East Node
                    grid[x, y].AddNeighbourNode(grid[x + 1, y]);

                if (y + 1 <= column) // North Node
                    grid[x, y].AddNeighbourNode(grid[x, y + 1]);

                if (y - 1 >= 0) // South Node
                    grid[x, y].AddNeighbourNode(grid[x, y - 1]);
            }
        }
    }

    public void Set8Neighbours()
    {
        int tempRow = 0;
        int tempCol = 0;
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (x + i >= 0)
                            tempRow = (x + i + row) % row;
                        else
                            tempRow = 0;

                        if (y + j >= 0)
                            tempCol = (y + j + column) % column;
                        else
                            tempCol = 0;

                        if (grid [x,y] != grid [tempRow, tempCol] && !grid[x,y].neighbourNode.Contains(grid[tempRow,tempCol]))
                        {
                            grid[x, y].AddNeighbourNode(grid[tempRow, tempCol]);
                        }
                        //Debug.Log("Added node at " + tempRow + tempCol);
                    }
                }
            }
        }
    }

    public void ColorNodeList(List<Node> nodeList, Color newColor)
    {
        foreach (Node tempNode in nodeList)
        {
            Renderer rend = tempNode.GetComponent<Renderer>();
            rend.material.color = newColor;
        }
    }

    public void ColorNode(Node node, Color newColor)
    {
        Renderer rend = node.GetComponent<Renderer>();
        rend.material.color = newColor;
    }

    public void ColorPathLine(List<Node> pathList, Color newColor)
    {
        GL.Begin(GL.LINES);
        GL.Color(newColor);

        for (int i = 0; i < pathList.Count; i++)
        {
            GL.Vertex(pathList[i].transform.position + Vector3.up);
            GL.Vertex(pathList[i + 1].transform.position + Vector3.up);
        }
        GL.End();
    }
}
