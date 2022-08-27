﻿using UnityEngine;


[System.Serializable]
public class LevelReferences
{
    [SerializeField][Group] private TileControllerReferences tileManagerReferences;
    [SerializeField][Group] private WordControllerReferences wordControllerReferences;
    [SerializeField][Group] private ScoreControllerReferences scoreControllerReferences;

    public TileControllerReferences TileManagerReferences => tileManagerReferences;
    public WordControllerReferences WordControllerReferences => wordControllerReferences;
    public ScoreControllerReferences ScoreControllerReferences => scoreControllerReferences;
}
