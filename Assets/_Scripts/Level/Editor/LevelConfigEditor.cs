using UnityEditor;

//[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelConfig levelConfig = (LevelConfig)target;
        
    }
}
