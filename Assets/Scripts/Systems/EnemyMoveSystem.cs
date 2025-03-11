using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
partial struct EnemyMoveSystem : ISystem
{
    private GameControl gameControl;
    public const float REACHED_TARGET_POSITION_DISTANCE_SQ = .05f;
    private Entity playerEntity;
    private LocalTransform playerTransform;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        gameControl = SystemAPI.GetSingleton<GameControl>();
        if (!gameControl.isRunning)
        {
            return;
        }

        playerEntity = SystemAPI.GetSingletonEntity<Player>();
        playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        EnemyMoveJob enemyMove = new EnemyMoveJob
        {
            playerPosition = playerTransform.Position,
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        enemyMove.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    public float3 playerPosition;
    private float3 moveDirection;
    public float deltaTime;

    public void Execute(ref LocalTransform localTransform, ref Move enemyMove, in Enemy enemy, ref PhysicsVelocity physicsVelocity)
    {
        enemyMove.targetPosition = playerPosition;
        moveDirection = enemyMove.targetPosition - localTransform.Position;

        if (math.lengthsq(moveDirection) < EnemyMoveSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);

        physicsVelocity.Linear = moveDirection * enemyMove.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
