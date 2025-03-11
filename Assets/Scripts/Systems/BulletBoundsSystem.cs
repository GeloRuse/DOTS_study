using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(BulletCollisionSystem))]
partial struct BulletBoundsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BulletDestroyJob collisionJob = new BulletDestroyJob
        {
            ProjectileLookup = SystemAPI.GetComponentLookup<Bullet>(true),
            WallLookup = SystemAPI.GetComponentLookup<Wall>(true),
            ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                                     .CreateCommandBuffer(state.WorldUnmanaged)
        };

        state.Dependency = collisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public partial struct BulletDestroyJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<Bullet> ProjectileLookup;
    [ReadOnly] public ComponentLookup<Wall> WallLookup;

    public EntityCommandBuffer ECB;

    private bool IsProjectile(Entity entity) => ProjectileLookup.HasComponent(entity);
    private bool IsWall(Entity entity) => WallLookup.HasComponent(entity);

    public void Execute(TriggerEvent triggerEvent)
    {
        if (IsProjectile(triggerEvent.EntityA))
        {
            if (IsWall(triggerEvent.EntityB))
            {
                ECB.DestroyEntity(triggerEvent.EntityA);
                return;
            }
        }
        else if (IsProjectile(triggerEvent.EntityB))
        {
            if (IsWall(triggerEvent.EntityA))
            {
                ECB.DestroyEntity(triggerEvent.EntityB);
                return;
            }
        }
    }
}

