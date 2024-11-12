using System.Collections.Generic;
using RicochetWeaponSystem;
using UnityEngine;

namespace RicochetSystem
{
    public struct RayCastHitInfo
    {
        public Vector2 hitPoint;
        public DummyEnemy HitDummyEnemy;
        public Vector2 hitNormal;

        public RayCastHitInfo(Vector2 point, DummyEnemy dummyEnemy, Vector2 normal)
        {
            hitPoint = point;
            HitDummyEnemy = dummyEnemy;
            hitNormal = normal;
        }
    }

    public class RayCastSystem
    {
        [Header("Raycast Settings")] 
        public float maxDistance = 1000f;

        public int maxBounces = 3;
        public LayerMask enemyLayer;

        [Header("Debug Visualization")] [SerializeField]
        public bool showDebugRays = true;

        public float debugRayDuration = 10f;
        public Color normalRayColor = Color.red;
        public Color ricochetRayColor = Color.yellow;

        public List<RayCastHitInfo> CastRay(Vector2 origin, Vector2 direction)
        {
            List<RayCastHitInfo> hits = new List<RayCastHitInfo>();
            Vector2 currentOrigin = origin;
            Vector2 currentDirection = direction.normalized;
            int bounceCount = 0;

            while (bounceCount <= maxBounces)
            {
                var hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, LayerMask.GetMask("Default"));
                Debug.Log($"hit is null: {hit.collider == null}");
                
                if (hit.collider == null)
                {
                    // Draw debug ray for miss
                    if (showDebugRays)
                    {
                        Debug.DrawRay(currentOrigin, currentDirection * maxDistance,
                            bounceCount == 0 ? normalRayColor : ricochetRayColor,
                            debugRayDuration);
                    }

                    break;
                }

                Debug.Log($"hit collider of {hit.collider.gameObject.name}");
                DummyEnemy dummyEnemy = hit.collider.GetComponent<DummyEnemy>();
                if (dummyEnemy == null)
                {
                    Debug.Log("Dummy Enemy is null");
                    break;
                }

                // Record the hit
                Vector2 hitNormal = dummyEnemy.IsShielded ? dummyEnemy.ShieldNormal : hit.normal;
                hits.Add(new RayCastHitInfo(hit.point, dummyEnemy, hitNormal));

                // Draw debug ray for hit
                if (showDebugRays)
                {
                    Debug.DrawLine(currentOrigin, hit.point,
                        bounceCount == 0 ? normalRayColor : ricochetRayColor,
                        debugRayDuration);
                }

                // If enemy is not shielded, stop here
                if (!dummyEnemy.IsShielded)
                    break;

                // Calculate ricochet
                currentOrigin = hit.point;
                currentDirection = Vector2.Reflect(currentDirection, hitNormal);
                bounceCount++;
            }

            return hits;
        }
    }
    
}