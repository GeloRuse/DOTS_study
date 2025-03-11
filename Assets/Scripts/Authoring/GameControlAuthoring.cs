using Unity.Entities;
using UnityEngine;

public class GameControlAuthoring : MonoBehaviour
{
    public class Baker : Baker<GameControlAuthoring>
    {
        public override void Bake(GameControlAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameControl());
        }
    }
}

public struct GameControl : IComponentData
{
    public bool onUpgrade;
    public bool isRunning;
    public int currentEnemies;
    public int enemiesKilled;
    public int currentBullets;
    public int currentUpgrade;
}

