using UnityEditor;
using UnityEngine;

namespace VoodooTest.World.Obstacles.Editor
{
    [CustomEditor(typeof(WorldObstacle))]
    [CanEditMultipleObjects]
    public class WorldObstacleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            DrawButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Apply"))
            {
                WorldObstacle obstacle = (WorldObstacle) target;
                obstacle.UpdateBlocksEditMode();
            }
        }
    }
}