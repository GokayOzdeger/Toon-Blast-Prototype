using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ScoreCounter")]
public class ScoreCounter : ALevelBehaviourSO
{
    [TextArea][SerializeField] private string scoreTextStartAdd = "";
    [TextArea][SerializeField] private string highScoreTextStartAdd = "";
    public int Score { get; private set; } = 0;

    private TMP_Text _scoreText;
    private TMP_Text _highScoreText;

    public override void OnSetup()
    {
        _scoreText = SceneReferences.ScoreCounterSceneReferences.scoreText;
        _highScoreText = SceneReferences.ScoreCounterSceneReferences.highScoreText;
        UpdateScoreText();
        UpdateHighScoreText();
    }

    public override void OnTick(float deltaTime)
    {
        //
    }
    
    public void ScoreGained(int score)
    {
        Score += score;
        UpdateScoreText();
        CheckNewHighScore();
    }

    private void CheckNewHighScore()
    {
        if (Score <= ScoreSaveData.Data.HighScore) return;
        ScoreSaveData.Data.HighScore = Score;
        ScoreSaveData.Data.Save();
        UpdateHighScoreText();
    }

    private void UpdateHighScoreText()
    {
        if (_highScoreText) _highScoreText.text = highScoreTextStartAdd + ScoreSaveData.Data.HighScore.ToString();
    }

    private void UpdateScoreText()
    {
        if (_scoreText) _scoreText.text = scoreTextStartAdd + Score.ToString();
    }

    [System.Serializable]
    public class ScoreCounterSceneReferences
    {
        public TMP_Text scoreText;
        public TMP_Text highScoreText;
    }

    public class ScoreSaveData : Saveable<ScoreSaveData>
    {
        public int HighScore = 0;
    }
}
