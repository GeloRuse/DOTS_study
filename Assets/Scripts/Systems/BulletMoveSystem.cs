using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
partial struct BulletMoveSystem : ISystem
{
    private Entity playerEntity;
    private LocalTransform playerTransform;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        playerEntity = SystemAPI.GetSingletonEntity<Player>();
        playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        BulletMoveJob bulletMove = new BulletMoveJob();
        bulletMove.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BulletMoveJob : IJobEntity
{
    public void Execute(ref LocalTransform localTransform, in Bullet bullet, ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = bullet.direction;

        physicsVelocity.Linear = moveDirection * bullet.speed;
        physicsVelocity.Angular = float3.zero;
    }
}
