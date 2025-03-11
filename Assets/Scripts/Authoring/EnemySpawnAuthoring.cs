using Unity.Entities;
using UnityEngine;

public class EnemySpawnAuthoring : MonoBehaviour
{
    public class Baker : Baker<EnemySpawnAuthoring>
    {
        public override void Bake(EnemySpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawn());
        }
    }
}

public struct EnemySpawn : IComponentData
{
    public float timer;
}

