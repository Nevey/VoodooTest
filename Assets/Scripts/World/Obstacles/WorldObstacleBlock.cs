using UnityEngine;

namespace VoodooTest.World.Obstacles
{
    public class WorldObstacleBlock : MonoBehaviour
    {
        [SerializeField, Range(0, 360)] private float angle;

        private Vector3 offset;

        private void Awake()
        {
            offset = transform.position;
        }

        public void ApplyAngleEditorMode()
        {
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(
                rotation.eulerAngles.x,
                rotation.eulerAngles.y,
                angle);

            transform.rotation = rotation;
            
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

            transform.position = matrix.MultiplyPoint3x4(offset);
        }
    }
}