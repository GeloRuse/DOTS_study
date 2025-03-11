using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct EnemySpawnSystem : ISystem
{
    private GameControl gameControl;
    private EntityRefs entityRefs;
    private EntityCommandBuffer entityCommandBuffer;
    private PlayerStats playerStats;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        gameControl = SystemAPI.GetSingleton<GameControl>();
        if (!gameControl.isRunning)
        {
            return;
        }

        playerStats = SystemAPI.GetSingleton<PlayerStats>();
        entityRefs = SystemAPI.GetSingleton<EntityRefs>();
        entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (localTransform, enemySpawn) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<EnemySpawn>>())
        {
            enemySpawn.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (enemySpawn.ValueRO.timer > 0f)
            {
                continue;
            }
            enemySpawn.ValueRW.timer = 1f / playerStats.level;

            Entity enemyEntity = state.EntityManager.Instantiate(entityRefs.monsterEntity);

            Random random = Random.CreateFromIndex((uint)enemyEntity.Index);

            float3 randomPosition;
            if (localTransform.ValueRO.Position.x != 0f)
            {
                randomPosition = new float3(localTransform.ValueRO.Position.x, 0, random.NextFloat(-5f, 5f));
            }
            else
            {
                randomPosition = new float3(random.NextFloat(-5f, 5f), 0, localTransform.ValueRO.Position.z);
            }
            SystemAPI.SetComponent(enemyEntity, LocalTransform.FromPosition(randomPosition));
        }
    }
}
