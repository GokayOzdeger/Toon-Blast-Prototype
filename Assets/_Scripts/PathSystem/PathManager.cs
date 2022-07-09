using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PathManager : AutoSingleton<PathManager>
{
    private List<PathNode> EntranceNodes { get; set; } = new List<PathNode>();
    private List<PathNode> AllNodes { get; set; } = new List<PathNode>();

    public void RegisterNode(PathNode node)
    {
        AllNodes.Add(node);
        if (node.IsEntranceNode) EntranceNodes.Add(node);
    }

    public PathNode GetRandomStartNode()
    {
        if (EntranceNodes.Count == 0) return null;
        return EntranceNodes[Random.Range(0, EntranceNodes.Count)];
    }
}
