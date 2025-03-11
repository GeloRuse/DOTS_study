using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct BulletCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {        
        ProjectileCollisionJob collisionJob = new ProjectileCollisionJob
        {
            ProjectileLookup = SystemAPI.GetComponentLookup<Bullet>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<Enemy>(true),
            PlayerLookup = SystemAPI.GetComponentLookup<Player>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false),
            ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                                     .CreateCommandBuffer(state.WorldUnmanaged)
        };

        state.Dependency = collisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public partial struct ProjectileCollisionJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<Bullet> ProjectileLookup;
    [ReadOnly] public ComponentLookup<Enemy> EnemyLookup;
    [ReadOnly] public ComponentLookup<Player> PlayerLookup;
    public ComponentLookup<Health> HealthLookup;

    public EntityCommandBuffer ECB;

    private bool IsProjectile(Entity entity) => ProjectileLookup.HasComponent(entity);
    private bool IsEnemy(Entity entity) => EnemyLookup.HasComponent(entity);
    private bool IsPlayer(Entity entity) => PlayerLookup.HasComponent(entity);
    private bool IsHealth(Entity entity) => HealthLookup.HasComponent(entity);

    public void Execute(TriggerEvent triggerEvent)
    {
        if (IsProjectile(triggerEvent.EntityA))
        {
            if (IsHealth(triggerEvent.EntityB))
            {
                RefRW<Health> health = HealthLookup.GetRefRW(triggerEvent.EntityB);
                if (IsEnemy(triggerEvent.EntityB) && !ProjectileLookup.GetRefRO(triggerEvent.EntityA).ValueRO.enemyBullet)
                {
                    health.ValueRW.healthAmount -= ProjectileLookup.GetRefRO(triggerEvent.EntityA).ValueRO.damage;
                    ECB.DestroyEntity(triggerEvent.EntityA);
                    return;
                }
                if (IsPlayer(triggerEvent.EntityB) && ProjectileLookup.GetRefRO(triggerEvent.EntityA).ValueRO.enemyBullet)
                {
                    health.ValueRW.healthAmount -= ProjectileLookup.GetRefRO(triggerEvent.EntityA).ValueRO.damage;
                    ECB.DestroyEntity(triggerEvent.EntityA);
                    return;
                }
            }
        }
        else if(IsProjectile(triggerEvent.EntityB))
        {
            if (IsHealth(triggerEvent.EntityA))
            {
                RefRW<Health> health = HealthLookup.GetRefRW(triggerEvent.EntityA);
                if (IsEnemy(triggerEvent.EntityA) && !ProjectileLookup.GetRefRO(triggerEvent.EntityB).ValueRO.enemyBullet)
                {
                    health.ValueRW.healthAmount -= ProjectileLookup.GetRefRO(triggerEvent.EntityB).ValueRO.damage;
                    ECB.DestroyEntity(triggerEvent.EntityB);
                    return;
                }
                if (IsPlayer(triggerEvent.EntityA) && ProjectileLookup.GetRefRO(triggerEvent.EntityB).ValueRO.enemyBullet)
                {
                    health.ValueRW.healthAmount -= ProjectileLookup.GetRefRO(triggerEvent.EntityB).ValueRO.damage;
                    ECB.DestroyEntity(triggerEvent.EntityB);
                    return;
                }
            }
        }
    }
}
