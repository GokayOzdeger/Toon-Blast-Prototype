using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedTree<TData>
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


public class TreeNode<TData>
{
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

public class TreeReader<TData>
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
