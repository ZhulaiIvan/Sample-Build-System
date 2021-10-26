using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameGrid))]
public class GameGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameGrid gameGrid = (GameGrid) target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Grid"))
        {
            gameGrid.CreateGrid();
        }
    }
}
