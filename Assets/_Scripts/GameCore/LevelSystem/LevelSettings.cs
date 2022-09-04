using System;
using UnityEngine;

[Serializable]
public class LevelSettings
{
    [SerializeField] [Group] private TileControllerSettings tileManagerSettings;
    [SerializeField] [Group] private WordControllerSettings wordControllerSettings;
    [SerializeField] [Group] private ScoreControllerSettings scoreControllerSettings;

    public TileControllerSettings TileManagerSettings => tileManagerSettings;
    public WordControllerSettings WordControllerSettings => wordControllerSettings;
    public ScoreControllerSettings ScoreControllerSettings => scoreControllerSettings;
}