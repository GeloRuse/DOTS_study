using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
partial struct HealthSystem : ISystem
{
    private EntityCommandBuffer entityCommandBuffer;
    private RefRW<GameControl> gameControl;

    public void OnUpdate(ref SystemState state)
    {
        entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        gameControl = SystemAPI.GetSingletonRW<GameControl>();

        foreach (var (health, entity) in SystemAPI.Query<RefRW<Health>>().WithPresent<Enemy>().WithEntityAccess())
        {
            if (health.ValueRO.onDeath)
            {
                entityCommandBuffer.DestroyEntity(entity);
                gameControl.ValueRW.enemiesKilled++;
            }
        }

        foreach (var health in SystemAPI.Query<RefRW<Health>>())
        {
            if (health.ValueRO.healthAmount <= 0)
            {
                health.ValueRW.onDeath = true;
            }
        }
    }
}
