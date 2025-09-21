using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{

    public float movemenSpeed;
    public float rotationtSpeed;
    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitMover
            {
                movemenSpeed = authoring.movemenSpeed,
                rotationtSpeed = authoring.rotationtSpeed,
            });
        }
    }
}

public struct UnitMover : IComponentData
{
    public float3 targetPosition;
    public float movemenSpeed;
    public float rotationtSpeed;
}
