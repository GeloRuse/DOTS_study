using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveAuthoring : MonoBehaviour
{
    public float moveSpeed;

    public class Baker : Baker<MoveAuthoring>
    {
        public override void Bake(MoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Move
            {
                moveSpeed = authoring.moveSpeed,
            });
        }
    }
}

public struct Move : IComponentData
{
    public float moveSpeed;
    public float3 targetPosition;
}

