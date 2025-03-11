using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public int expGain;
    public int collisionDamage;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Enemy
            {
                collisionDamage = authoring.collisionDamage,
                expGain = authoring.expGain,
            });
        }
    }
}

public struct Enemy : IComponentData
{
    public int expGain;
    public int collisionDamage;
}

