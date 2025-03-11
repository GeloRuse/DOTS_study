using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
partial struct WeaponSystem : ISystem
{
    private GameControl gameControl;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        gameControl = SystemAPI.GetSingleton<GameControl>();
        if (!gameControl.isRunning)
        {
            return;
        }

        EntityRefs entityRefs = SystemAPI.GetSingleton<EntityRefs>();

        foreach (var (weapon, localTransform, entity) in SystemAPI.Query<RefRW<Weapon>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            weapon.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(weapon.ValueRO.timer > 0f)
            {
                continue;
            }
            weapon.ValueRW.timer = weapon.ValueRW.timerMax;

            Entity bulletEntity = state.EntityManager.Instantiate(entityRefs.bulletEntity);
            float3 bulletWorldPosition = localTransform.ValueRO.TransformPoint(weapon.ValueRO.shootOrigin);
            float3 gunRotation = math.mul(localTransform.ValueRO.Rotation, new float3(1f, 0f, 0f));
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletWorldPosition));

            RefRW<LocalTransform> bulletTranform = SystemAPI.GetComponentRW<LocalTransform>(bulletEntity);
            bulletTranform.ValueRW.Scale = .3f;

            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damage = weapon.ValueRO.damage;
            bullet.ValueRW.direction = gunRotation;
            weapon.ValueRW.onShoot.isTriggered = true;
            if (SystemAPI.HasComponent<Enemy>(entity))
            {
                bullet.ValueRW.enemyBullet = true;
            }
        }
    }
}
