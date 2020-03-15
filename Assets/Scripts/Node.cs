using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private float cost = int.MaxValue;
    public int[] position;
    public Node parentNode = null;
    public List<Node> neighbourNode = new List<Node>();

    SpriteRenderer sprRend;

    public bool settled = false;
    public bool walkable = true;

    public void CreateNode(int[] pos, Sprite newSpr)
    {
        cost = int.MaxValue;
        settled = false;
        parentNode = null;
        position = pos;
        this.transform.position = new Vector2(position[0], position[1]);
        this.gameObject.AddComponent<BoxCollider>();
        this.gameObject.AddComponent<SpriteRenderer>();
        this.gameObject.tag = "Node";
        sprRend = this.gameObject.GetComponent<SpriteRenderer>();
        SetSprite(newSpr);
    }

    void Start()
    {
        this.ResetNode();
    }

    void SetSprite(Sprite newSpr)
    {
        sprRend.sprite = newSpr;
        sprRend.color = Color.white;
    }

    public void ResetNode()
    {
        cost = int.MaxValue;
        parentNode = null;
        settled = false;
        walkable = true;
    }

    public void SetParent(Node node)
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

    public void AddNeighbourNode(Node node)
    {
        this.neighbourNode.Add(node);
    }

    public List<Node> GetNeighbourNode()
    {
        List<Node> result = this.neighbourNode;
        return result;
    }

    public void ResetNeighbourNodes()
    {
        neighbourNode.Clear();
        neighbourNode = new List<Node>();
    }

    public float GetCost()
    {
        return this.cost;
    }

    public Node GetParentNode()
    {
        return this.parentNode;
    }

    public bool isWalkable()
    {
        return this.walkable;
    }
}
