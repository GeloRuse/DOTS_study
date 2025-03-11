using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    public float timerMax;
    public Transform shootOrigin;
    public int damage;

    public class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Weapon
            {
                timerMax = authoring.timerMax,
                shootOrigin = authoring.shootOrigin.localPosition,
                damage = authoring.damage,
            });
        }
    }
}


public struct Weapon : IComponentData
{
    public float timer;
    public float timerMax;
    public float3 shootOrigin;
    public int damage;

    public OnShootEvent onShoot;

    public struct OnShootEvent
    {
        public bool isTriggered;
    }
}
