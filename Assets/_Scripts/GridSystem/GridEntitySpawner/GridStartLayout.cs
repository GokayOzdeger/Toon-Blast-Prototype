using UnityEngine;
using System.Collections;

[System.Serializable]
public class GridStartLayout  {

	[System.Serializable]
	public struct rowData{
		public BasicGridEntityTypeDefinition[] row;
	}

    public rowData[] rows;

    public int RowCount => rows.Length;

    public int CollumnCount => rows[0].row != null ? rows[0].row.Length : 0;

    public GridStartLayout(int rowCount, int collumnCount)
    {
        rowCount = Mathf.Max(1, rowCount);
        collumnCount = Mathf.Max(1, collumnCount);
        rows = new rowData[rowCount];
        for (int i = 0; i < rowCount; i++)
        {
            rows[i].row = new BasicGridEntityTypeDefinition[collumnCount];
        }
    }

    public static GridStartLayout FromArray(int rowCount, int collumnCount, BasicGridEntityTypeDefinition[] entityTypes)
    {
        GridStartLayout layout = new GridStartLayout(rowCount, collumnCount);
        for (int i = collumnCount-1; i >= 0; i--)
        {
            for (int j =0; j < rowCount; j++)
            {
                layout.rows[i].row[j] = entityTypes[(collumnCount -1 - i) * rowCount + j];
            }
        }
        return layout;
    }
}
