using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class ArrayPropertyDrawer : PropertyDrawer {

	private readonly float cellSize = 40;
	private readonly float soHandlePixelSize = 20;

	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		SerializedProperty data = property.FindPropertyRelative("rows");
		newposition.y += cellSize;
        
		for (int j=0;j<data.arraySize;j++){
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			newposition.height = cellSize;
			newposition.width = cellSize*1.6f;
            
			for(int i=0;i<row.arraySize;i++){
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
                BasicGridEntityTypeDefinition typeDef = (row.GetArrayElementAtIndex(i).objectReferenceValue as BasicGridEntityTypeDefinition);
				newposition.width -= soHandlePixelSize;
                if (typeDef != null) EditorGUI.DrawTextureTransparent(newposition, typeDef.DefaultEntitySprite.texture);
				newposition.width += soHandlePixelSize;
                newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += cellSize;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return cellSize * (property.FindPropertyRelative("rows").arraySize+2);
	}
}
