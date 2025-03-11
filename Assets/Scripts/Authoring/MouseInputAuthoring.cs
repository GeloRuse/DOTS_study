using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseInputAuthoring : MonoBehaviour
{
    public class Baker : Baker<MouseInputAuthoring>
    {
        public override void Bake(MouseInputAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MouseInput>(entity, new MouseInput());
        }
    }
}

public struct MouseInput : IComponentData
{
    public float2 mousePosition;
    public bool mouseClick;
}

