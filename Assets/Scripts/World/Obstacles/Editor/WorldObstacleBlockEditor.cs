using UnityEditor;

namespace VoodooTest.World.Obstacles.Editor
{
    [CustomEditor(typeof(WorldObstacleBlock))]
    [CanEditMultipleObjects]
    public class WorldObstacleBlockEditor : UnityEditor.Editor
    {
        private SerializedProperty angle;
        
        private void OnEnable()
        {
            angle = serializedObject.FindProperty("angle");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(angle);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                
                WorldObstacleBlock block = (WorldObstacleBlock) target;
                block.ApplyAngleEditorMode();
            }
        }
    }
}