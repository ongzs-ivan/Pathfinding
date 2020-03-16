using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float fCost = 0;
    public float gCost = int.MaxValue;
    public float hCost = 0;
    public int[] position;
    public Node parentNode = null;
    public List<Node> neighbourNode = new List<Node>();

    SpriteRenderer sprRend;

    public bool settled = false;
    public bool walkable = true;

    public void CreateNode(int[] pos, Sprite newSpr)
    {
        UpdateCost();
        settled = false;
        parentNode = null;
        walkable = true;
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
        fCost = 0;
        gCost = int.MaxValue;
        hCost = 0;
        UpdateCost();
        parentNode = null;
        settled = false;
        walkable = true;
    }

    public void SetParent(Node node)
    {
        this.parentNode = node;
    }

    public void UpdateCost(float H = 0.0f)
    {
        fCost = gCost + H * hCost;
    }
    
    public void AddNeighbourNode(Node node)
    {
		if (node.walkable)
		{
			this.neighbourNode.Add(node);
		}
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
        return this.fCost;
    }

    public Node GetParentNode()
    {
        return this.parentNode;
    }
}
