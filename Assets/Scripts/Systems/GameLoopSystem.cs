using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct GameLoopSystem : ISystem
{
    private EntityCommandBuffer entityCommandBuffer;
    private EntityRefs entityRefs;
    private RefRW<GameControl> gameControl;
    private Entity playerEntity;
    private RefRO<Health> playerHealth;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityRefs = SystemAPI.GetSingleton<EntityRefs>();
        entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        gameControl = SystemAPI.GetSingletonRW<GameControl>();
        playerEntity = SystemAPI.GetSingletonEntity<Player>();
        playerHealth = SystemAPI.GetComponentRO<Health>(playerEntity);

        if (gameControl.ValueRO.onUpgrade)
        {
            Entity gunEntity = state.EntityManager.Instantiate(entityRefs.playerWeaponEntity);
            entityCommandBuffer.AddComponent(gunEntity, new Parent { Value = playerEntity });
            gameControl.ValueRW.onUpgrade = false;
        }

        if(playerHealth.ValueRO.onDeath)
        {
            gameControl.ValueRW.isRunning = false;
            foreach(var (enemy, entity) in SystemAPI.Query<RefRO<Enemy>>().WithEntityAccess())
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }

        gameControl.ValueRW.currentBullets = 0;
        foreach(var bullet in SystemAPI.Query<RefRO<Bullet>>())
        {
            gameControl.ValueRW.currentBullets++;
        }

        gameControl.ValueRW.currentEnemies = 0;
        foreach (var enemy in SystemAPI.Query<RefRO<Enemy>>()) 
        {
            gameControl.ValueRW.currentEnemies++;
        }
    }
}
