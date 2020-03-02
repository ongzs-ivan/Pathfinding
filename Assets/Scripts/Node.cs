using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private float cost = int.MaxValue;
    public Transform parentNode = null;
    public List<Transform> neighbourNode;

    public bool visited = false;
    public bool walkable = true;

    void Start()
    {
        this.ResetNode();
    }

    public void ResetNode()
    {
        cost = int.MaxValue;
        parentNode = null;
    }

    public void SetParent(Transform node)
    {
        this.parentNode = node;
    }

    public void SetCost(float newCost)
    {
        cost = newCost;
    }

    public void ChangeWalkability(bool newState)
    {
        walkable = newState;
    }

    public void AddNeighbourNode(Transform node)
    {
        this.neighbourNode.Add(node);
    }

    public List<Transform> GetNeighbourNode()
    {
        List<Transform> result = this.neighbourNode;
        return result;
    }

    public float GetCost()
    {
        return this.cost;
    }

    public Transform GetParentNode()
    {
        return this.parentNode;
    }

    public bool isWalkable()
    {
        return this.walkable;
    }
}
