using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct EnemyCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EnemyCollisionJob collisionJob = new EnemyCollisionJob
        {
            EnemyLookup = SystemAPI.GetComponentLookup<Enemy>(true),
            PlayerLookup = SystemAPI.GetComponentLookup<Player>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false),
        };

        state.Dependency = collisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public partial struct EnemyCollisionJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<Enemy> EnemyLookup;
    [ReadOnly] public ComponentLookup<Player> PlayerLookup;
    public ComponentLookup<Health> HealthLookup;

    private bool IsEnemy(Entity entity) => EnemyLookup.HasComponent(entity);
    private bool IsPlayer(Entity entity) => PlayerLookup.HasComponent(entity);
    private bool IsHealth(Entity entity) => HealthLookup.HasComponent(entity);

    public void Execute(TriggerEvent triggerEvent)
    {
        if (IsEnemy(triggerEvent.EntityA))
        {
            if (IsPlayer(triggerEvent.EntityB) && IsHealth(triggerEvent.EntityB))
            {
                RefRW<Health> health = HealthLookup.GetRefRW(triggerEvent.EntityB);
                health.ValueRW.healthAmount -= EnemyLookup.GetRefRO(triggerEvent.EntityA).ValueRO.collisionDamage;
                RefRW<Health> enemyHealth = HealthLookup.GetRefRW(triggerEvent.EntityA);
                enemyHealth.ValueRW.onDeath = true;
                return;
            }
        }
        else if (IsEnemy(triggerEvent.EntityB))
        {
            if (IsPlayer(triggerEvent.EntityA) && IsHealth(triggerEvent.EntityA))
            {
                RefRW<Health> health = HealthLookup.GetRefRW(triggerEvent.EntityA);
                health.ValueRW.healthAmount -= EnemyLookup.GetRefRO(triggerEvent.EntityB).ValueRO.collisionDamage;
                RefRW<Health> enemyHealth = HealthLookup.GetRefRW(triggerEvent.EntityB);
                enemyHealth.ValueRW.onDeath = true;
                return;
            }
        }
    }
}

