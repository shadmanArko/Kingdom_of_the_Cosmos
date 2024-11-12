using UnityEngine;

namespace RicochetWeaponSystem
{
    public struct RicochetHitInfo
    {
        public Vector2 hitPoint;
        public DummyEnemy HitDummyEnemy;
        public Vector2 hitNormal;

        public RicochetHitInfo(Vector2 point, DummyEnemy dummyEnemy, Vector2 normal)
        {
            hitPoint = point;
            HitDummyEnemy = dummyEnemy;
            hitNormal = normal;
        }
    }
}