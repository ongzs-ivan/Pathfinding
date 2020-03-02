using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int row = 5;
    public int column = 5;
    public Transform nodePrefab;
    
    public List<Transform> grid = new List<Transform>();

    void Start()
    {
        this.CreateGrid();
        this.CreateNeighbours();
    }

    private void CreateGrid()
    {
        int counter = 0;
        float tempColumn = 0;
        float tempRow = 0;
        float tempZ = gameObject.transform.position.z;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                tempColumn = j + gameObject.transform.position.x;
                tempRow = i + gameObject.transform.position.y;
                Transform node = Instantiate(nodePrefab, new Vector3(tempColumn, tempRow, tempZ), Quaternion.identity);
                node.name = "Node" + counter;
                grid.Add(node);
                counter++;
            }
        }
    }

    private void CreateNeighbours()
    {
        for (int i = 0; i < grid.Count; i++)
        {
            Node currentNode = grid[i].GetComponent<Node>();
            int index = i + 1;

            if (index%column == 1)
            {
                if (i + column < column*row)
                {
                    currentNode.AddNeighbourNode(grid[i + column]);  // North Node
                }

                if (i - column >= 0)
                {
                    currentNode.AddNeighbourNode(grid[i - column]); // South Node
                }
                currentNode.AddNeighbourNode(grid[i + 1]); // East Node
            }
            else if (index%column == 0)
            {
                if (i + column < column * row)
                {
                    currentNode.AddNeighbourNode(grid[i + column]); // North Node
                }

                if (i - column >= 0)
                {
                    currentNode.AddNeighbourNode(grid[i - column]); // South Node
                }
                currentNode.AddNeighbourNode(grid[i - 1]); // West Node
            }
            else
            {
                if (i + column < column * row)
                {
                    currentNode.AddNeighbourNode(grid[i + column]); // North Node
                }

                if (i - column >= 0)
                {
                    currentNode.AddNeighbourNode(grid[i - column]); // South Node
                }
                currentNode.AddNeighbourNode(grid[i + 1]); // East Node
                currentNode.AddNeighbourNode(grid[i - 1]); // West Node
            }
        }
    }

}
