using UnityEngine;

namespace zzz_TestScripts
{
    public class CustomCollider : MonoBehaviour
    {
        public Vector2[] colliderPoints;
        private PolygonCollider2D polygonCollider;

        private void Awake()
        {
            // Get or add the PolygonCollider2D component
            polygonCollider = GetComponent<PolygonCollider2D>();
            if (polygonCollider == null)
            {
                polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            }
        }

        [ContextMenu("Create Collider")]
        public void CreateCollider()
        {
            if (polygonCollider != null && colliderPoints != null && colliderPoints.Length >= 3)
            {
                polygonCollider.SetPath(0, colliderPoints);
            }
            else
            {
                Debug.LogWarning("Cannot create collider. Ensure you have at least 3 points defined.");
            }
        }

        public void UpdateColliderPoints(Vector2[] newPoints)
        {
            colliderPoints = newPoints;
            CreateCollider();
        }

        // Optional: Create the collider automatically on start
        private void Start()
        {
            // CreateCollider();
        }

        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                Debug.Log("Contact Enter Wall");
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                Debug.Log("Contact Stay Wall");
            }
        }
    }
}