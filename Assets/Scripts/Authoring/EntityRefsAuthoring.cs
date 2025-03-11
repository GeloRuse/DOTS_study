using Unity.Entities;
using UnityEngine;

public class EntityRefsAuthoring : MonoBehaviour
{
    public GameObject playerWeaponPrefab;
    public GameObject bulletPrefab;
    public GameObject monsterPrefab;
    public GameObject monsterEyePrefab;

    public class Baker : Baker<EntityRefsAuthoring>
    {
        public override void Bake(EntityRefsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityRefs
            {
                playerWeaponEntity = GetEntity(authoring.playerWeaponPrefab, TransformUsageFlags.Dynamic),
                bulletEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                monsterEntity = GetEntity(authoring.monsterPrefab, TransformUsageFlags.Dynamic),
                monsterEyeEntity = GetEntity(authoring.monsterEyePrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct EntityRefs : IComponentData
{
    public Entity playerWeaponEntity;
    public Entity bulletEntity;
    public Entity monsterEntity;
    public Entity monsterEyeEntity;
}

