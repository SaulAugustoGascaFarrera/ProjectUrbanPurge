using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{

    public GameObject testTargetEntity;
   public class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Target
            {

                targetEntity = GetEntity(authoring.testTargetEntity, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct Target : IComponentData
{
    public Entity targetEntity;
}
