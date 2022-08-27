using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class LinkedTree<TData> where TData : ISerializable
{
    public TreeNode<TData> Root => _root;
    private TreeNode<TData> _root;

    public LinkedTree()
    {
        _root = new TreeNode<TData>(default(TData));
    }

    private TreeNode<TData> Find(TData data)
    {
        // need iterator
        return null;
    }
}


public class TreeNode<TData> where TData : ISerializable
{
    public int Id { get; set; }
    public HashSet<TreeNode<TData>> ChildNodes { get; private set; } = new HashSet<TreeNode<TData>>();
    public TreeNode<TData> ParentNode { get; set; }

    public TData Data
    {
        get => _data;
        set => _data = value;
    }

    private TData _data;

    public TreeNode(TData data)
    {
        _data = data;
    }

    public void RemoveChild(TreeNode<TData> node)
    {
        ChildNodes.Remove(node);
    }

    public void RemoveChild(TData data)
    {
        ChildNodes.RemoveWhere((node)=> node.Data.Equals(data));
    }

    public void AddChild(TreeNode<TData> node)
    {
        node.ParentNode = this;
        ChildNodes.Add(node);
    }

    public void Clear()
    {
        ChildNodes.Clear();
        ParentNode = null;
    }
}

[System.Serializable]
public class TreeNodeSerialized<TData> where TData : ISerializable
{
    public int id;
    public int[] childIds;
    public int parentId;
    public TData data;

    public TreeNodeSerialized(TreeNode<TData> _node)
    {
        data = _node.Data;
        id = _node.Id;
        parentId = _node.ParentNode.Id;
        childIds = new int[_node.ChildNodes.Count];
        int counter = 0;
        foreach(var node in _node.ChildNodes)
        {
            childIds[counter] = node.Id;
            counter++;
        }
    }
}

public class TreeReader<TData> where TData : ISerializable
{
    public TreeNode<TData> CurrentNode { get; set; }
    public int ChildCount => CurrentNode.ChildNodes.Count;

    public TreeReader(LinkedTree<TData> tree)
    {
        CurrentNode = tree.Root;
    }

    public bool ExistsInChildren(TData data)
    {
        foreach(TreeNode<TData> node in CurrentNode.ChildNodes)
        {
            if (node.Data.Equals(data)) return true;
        }
        return false;
    }

    public bool Traverse(TData data)
    {
        foreach (TreeNode<TData> node in CurrentNode.ChildNodes)
        {
            if (node.Data.Equals(data))
            {
                CurrentNode = node;
                return true;
            }
        }
        return false;
    }
}
