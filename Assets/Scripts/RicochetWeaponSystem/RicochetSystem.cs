using System.Collections.Generic;
using UnityEngine;

namespace RicochetWeaponSystem
{
    public class RicochetSystem : MonoBehaviour
    {
        [Header("Ricochet Settings")] public float maxDistance = 1000f;
        public int maxBounces = 3;
        public float hitDetectionRadius = 0.5f;
        
        [Header("Debug Visualization")] public bool showDebugLines = true;
        public bool showCollisionRadius = true;
        public float debugLineDuration = 10f;
        public Color normalLineColor = Color.red;
        public Color ricochetLineColor = Color.yellow;
        public Color collisionRadiusColor = Color.cyan;
        
        [SerializeField] private List<DummyEnemy> enemies;
        
        public void RegisterEnemy(DummyEnemy enemy)
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
        
        public void UnregisterEnemy(DummyEnemy enemy)
        {
            enemies.Remove(enemy);
        }
        
        private void Start()
        {
            FindAllEnemies();
        }
        
        private void FindAllEnemies()
        {
            var enemyList = FindObjectsOfType<DummyEnemy>();
            foreach (var dummyEnemy in enemyList)
            {
                RegisterEnemy(dummyEnemy);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (showCollisionRadius && enemies != null)
            {
                Gizmos.color = collisionRadiusColor;
                foreach (var enemy in enemies)
                {
                    if (enemy != null)
                    {
                        float radius = enemy.CollisionRadius > 0 ? enemy.CollisionRadius : hitDetectionRadius;
                        Gizmos.DrawWireSphere(enemy.transform.position, radius);
                    }
                }
            }
        }
        
        public List<RicochetHitInfo> CalculateRicochetPath(Vector2 origin, Vector2 direction)
        {
            List<RicochetHitInfo> hits = new List<RicochetHitInfo>();
            Vector2 currentOrigin = origin;
            Vector2 currentDirection = direction.normalized;
            int bounceCount = 0;
        
            while (bounceCount <= maxBounces)
            {
                var hitInfo = FindCircleIntersection(currentOrigin, currentDirection);
        
                if (!hitInfo.HasValue)
                {
                    if (showDebugLines)
                    {
                        Debug.DrawLine(currentOrigin, currentOrigin + currentDirection * maxDistance,
                            bounceCount == 0 ? normalLineColor : ricochetLineColor,
                            debugLineDuration);
                    }
        
                    break;
                }
        
                var (hitEnemy, hitPoint, hitNormal) = hitInfo.Value;
        
                hits.Add(new RicochetHitInfo(hitPoint, hitEnemy, hitNormal));
        
                if (showDebugLines)
                {
                    Debug.DrawLine(currentOrigin, hitPoint,
                        bounceCount == 0 ? normalLineColor : ricochetLineColor,
                        debugLineDuration);
                }
        
                if (!hitEnemy.IsShielded)
                    break;
        
                currentOrigin = hitPoint;
                currentDirection = Vector2.Reflect(currentDirection, hitNormal);
                bounceCount++;
            }
        
            return hits;
        }
        
        private (DummyEnemy enemy, Vector2 hitPoint, Vector2 normal)? FindCircleIntersection(Vector2 origin,
            Vector2 direction)
        {
            float closestIntersectionDistance = float.MaxValue;
            DummyEnemy hitEnemy = null;
            Vector2 closestIntersectionPoint = Vector2.zero;
            Vector2 intersectionNormal = Vector2.zero;
        
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
        
                float radius = enemy.CollisionRadius > 0 ? enemy.CollisionRadius : hitDetectionRadius;
                Vector2 enemyPosition = enemy.transform.position;
        
                // Calculate intersection with circle (enemy's collision radius)
                var intersection = RayCircleIntersection(origin, direction, enemyPosition, radius);
        
                if (intersection.HasValue)
                {
                    float distance = Vector2.Distance(origin, intersection.Value);
                    if (distance < closestIntersectionDistance)
                    {
                        closestIntersectionDistance = distance;
                        hitEnemy = enemy;
                        closestIntersectionPoint = intersection.Value;
                        // Calculate normal at intersection point
                        intersectionNormal = (closestIntersectionPoint - enemyPosition).normalized;
                    }
                }
            }
        
            if (hitEnemy != null)
            {
                return (hitEnemy, closestIntersectionPoint,
                    hitEnemy.IsShielded ? hitEnemy.ShieldNormal : intersectionNormal);
            }
        
            return null;
        }
        
        private Vector2? RayCircleIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter,
            float radius)
        {
            Vector2 toCircle = circleCenter - rayOrigin;
        
            float a = Vector2.Dot(rayDirection, rayDirection);
            float b = -2f * Vector2.Dot(toCircle, rayDirection);
            float c = Vector2.Dot(toCircle, toCircle) - (radius * radius);
        
            float discriminant = (b * b) - (4f * a * c);
        
            // No intersection
            if (discriminant < 0)
                return null;
        
            // Calculate both intersection points
            float t1 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
            float t2 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
        
            // Check if intersections are in front of the ray origin
            if (t1 < 0 && t2 < 0)
                return null;
        
            // Return the closest intersection point that's in front of the ray
            float t = t1; //t1 < 0 ? t2 : t1;
            if (t > maxDistance)
                return null;
        
            return rayOrigin + rayDirection * t;
        }

    }
}