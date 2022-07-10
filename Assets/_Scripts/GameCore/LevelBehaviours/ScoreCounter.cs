using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ScoreCounter")]
public class ScoreCounter : ALevelBehaviourSO
{
    [SerializeField] private string textStartAdd = "";
    public int Score { get; private set; } = 0;

    private TMP_Text _scoreText;

    public override void OnSetup()
    {
        _scoreText = SceneReferences.ScoreCounterSceneReferences.scoreText;
        UpdateScoreText();
    }

    public override void OnTick(float deltaTime)
    {
        //
    }
    
    public void ScoreGained(int score)
    {
        Score += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (_scoreText) _scoreText.text = textStartAdd + Score.ToString();
    }

    [System.Serializable]
    public class ScoreCounterSceneReferences
    {
        public TMP_Text scoreText;
    }
}
