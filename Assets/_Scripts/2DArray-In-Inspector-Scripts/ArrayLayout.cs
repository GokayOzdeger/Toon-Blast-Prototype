using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout  {

	[System.Serializable]
	public struct rowData{
		public BasicGridEntityTypeDefinition[] row;
	}

    public rowData[] rows;

    public int RowCount => rows.Length;

    public int CollumnCount => rows[0].row != null ? rows[0].row.Length : 0;

    public ArrayLayout(int rowCount, int collumnCount)
    {
        rowCount = Mathf.Max(1, rowCount);
        collumnCount = Mathf.Max(1, collumnCount);
        rows = new rowData[rowCount];
        for (int i = 0; i < rowCount; i++)
        {
            rows[i].row = new BasicGridEntityTypeDefinition[collumnCount];
        }
    }
}
