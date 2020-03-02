using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private float weight = int.MaxValue;
    private Transform parentNode = null;
    private List<Transform> neighbourNode;
    private bool isWalkable = true;

    void Start()
    {
        this.resetNode();
    }

    public void ResetNode()
    {
        weight = int.MaxValue;
        parentNode = null;
    }
}
