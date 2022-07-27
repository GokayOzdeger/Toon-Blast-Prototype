using UnityEditor;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelConfig levelConfig = (LevelConfig)target;
        GridStartLayout gridStartLayout = levelConfig.GridEntitySpawnerSettings.gridStartLayout;
        if (gridStartLayout.RowCount != levelConfig.GridControllerSettings.RowCount || gridStartLayout.CollumnCount != levelConfig.GridControllerSettings.ColumnCount)
        {
            levelConfig.GridEntitySpawnerSettings.gridStartLayout = new GridStartLayout((int)levelConfig.GridControllerSettings.RowCount, (int)levelConfig.GridControllerSettings.ColumnCount);
        }
    }
}
