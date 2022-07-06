using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField] private List<PathNode> nextNodes = new List<PathNode>();
    [SerializeField] private float nodeRadius = 30;
    [SerializeField] private bool isEntranceNode;

    public bool IsEntranceNode => isEntranceNode;

    private void Awake()
    {
        Path.Instance.RegisterNode(this);
    }

    public PathNode GetRandomNextNode()
    {
        if (nextNodes.Count == 0) return null;
        return nextNodes[Random.Range(0, nextNodes.Count)];
    }

    public Vector2 GetRandomPoint()
    {
        return (Vector2) transform.position + Random.insideUnitCircle * Random.Range(0, nodeRadius);
    }

#if UNITY_EDITOR
    
    private void OnDestroy()
    {
        // check if object destruction occured while closing play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (Time.frameCount == 0 || Time.renderedFrameCount == 0) return;

        
        PathNode[] allNodes = Resources.FindObjectsOfTypeAll<PathNode>();
        foreach (PathNode node in allNodes)
        {
            foreach (PathNode nextNode in node.nextNodes)
            {
                if (nextNode == this)
                {
                    node.nextNodes.Remove(this);
                }
            }
        }
    }

    public void SetNodeRadius(float radius)
    {
        nodeRadius = radius;
    }

    [EasyButtons.Button("Create Node Forward")]
    private void CreateNewForwardNode()
    {
        PathNode newNode = new GameObject("Node").AddComponent<PathNode>();
        
        // create undo point
        Undo.RecordObject(this, "Add Node Forward");
        Undo.RegisterCreatedObjectUndo(newNode.gameObject, "Add Node Forward");
        
        newNode.transform.SetParent(transform.parent);
        newNode.transform.position = (Vector2) transform.position + Vector2.up * 100;
        newNode.SetNodeRadius(nodeRadius);
        nextNodes.Add(newNode);

        Selection.SetActiveObjectWithContext(newNode.gameObject, this);
    }

    [EasyButtons.Button("Add Node In Between")]
    private void AddNodeInBetween(int pathDivisionIndex)
    {
        if (pathDivisionIndex >= nextNodes.Count) return;

        PathNode inBetweenNode = new GameObject("Node").AddComponent<PathNode>();

        // create undo point
        Undo.RecordObject(this, "Add Node Forward");
        Undo.RegisterCreatedObjectUndo(inBetweenNode.gameObject, "Add Node Forward");

        // place new node in the middle of path
        inBetweenNode.transform.SetParent(transform.parent);
        inBetweenNode.transform.position = ((Vector2)transform.position + (Vector2)(nextNodes[pathDivisionIndex].transform.position)) / 2f;
        inBetweenNode.SetNodeRadius(nodeRadius);

        //add new node in between
        PathNode nodeToAssing = nextNodes[pathDivisionIndex];
        nextNodes[pathDivisionIndex] = inBetweenNode;
        inBetweenNode.nextNodes.Add(nodeToAssing);

        Selection.SetActiveObjectWithContext(inBetweenNode.gameObject, this);
    }

    private void OnDrawGizmos()
    {
        if (isEntranceNode) Handles.color = Color.green;
        else Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.forward, nodeRadius);
        
        for (int i = 0; i < nextNodes.Count; i++)
        {
            PathNode node = nextNodes[i];
            if (node == null)
            {
                nextNodes.Remove(node);
                i--;
                continue;
            }
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }
#endif
}
