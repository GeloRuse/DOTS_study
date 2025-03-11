using Unity.Entities;
using UnityEngine;

public class PlayerStatsAuthoring : MonoBehaviour
{
    public class Baker : Baker<PlayerStatsAuthoring>
    {
        public override void Bake(PlayerStatsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerStats { level = 1, });
        }
    }
}

public struct PlayerStats : IComponentData
{
    public int coins;
    public int level;
    public int currentExp;
    public int nextLevelExp;
}

