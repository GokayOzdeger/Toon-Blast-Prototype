using UnityEditor;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelConfig levelConfig = (LevelConfig)target;
        ArrayLayout gridStartLayout = levelConfig.GridControllerSettings.GridEntitySpawnerSettings.gridStartLayout;
        if (gridStartLayout.RowCount != levelConfig.GridControllerSettings.RowCount || gridStartLayout.CollumnCount != levelConfig.GridControllerSettings.CollumnCount)
        {
            levelConfig.GridControllerSettings.GridEntitySpawnerSettings.gridStartLayout = new ArrayLayout((int)levelConfig.GridControllerSettings.RowCount, (int)levelConfig.GridControllerSettings.CollumnCount);
        }
    }
}
