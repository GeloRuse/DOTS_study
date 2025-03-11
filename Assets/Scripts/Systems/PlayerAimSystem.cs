using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct PlayerAimSystem : ISystem
{
    private MouseInput mouseInput;
    private Entity playerEntity;
    private LocalTransform playerTransform;

    private int i;
    private int weaponNum;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        playerEntity = SystemAPI.GetSingletonEntity<Player>();
        playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);
        mouseInput = SystemAPI.GetSingleton<MouseInput>();

        i = 0;
        weaponNum = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerWeapon>().Build(ref state).CalculateEntityCount();
        foreach(var playerWeapon in SystemAPI.Query<RefRW<PlayerWeapon>>()) 
        {
            playerWeapon.ValueRW.weaponOffset = (i * math.PI * 2) / weaponNum;
            i++;
        }

        PlayerAimJob aimJob = new PlayerAimJob
        {
            mousePosition = mouseInput.mousePosition,
            playerPosition = new float2(playerTransform.Position.x, playerTransform.Position.y),
        };

        aimJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct PlayerAimJob : IJobEntity
{
    public float2 mousePosition;
    public float2 playerPosition;

    public void Execute(ref LocalTransform localTransform, in PlayerWeapon playerWeapon)
    {
        float2 direction = mousePosition - playerPosition;
        float angle = math.atan2(direction.y, direction.x) + playerWeapon.weaponOffset;
        localTransform.Rotation = quaternion.AxisAngle(new float3(0, 1, 0), -angle);
    }
}
