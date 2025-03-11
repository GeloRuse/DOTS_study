using Unity.Entities;
using UnityEngine;

public class WallAuthoring : MonoBehaviour
{
    public class Baker : Baker<WallAuthoring>
    {
        public override void Bake(WallAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Wall());
        }
    }
}

public struct Wall : IComponentData
{

}

