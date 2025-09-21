using Unity.Entities;
using UnityEngine;

public class SetupDefaultPositionAuthoring : MonoBehaviour
{
    public class Baker : Baker<SetupDefaultPositionAuthoring>
    {
        public override void Bake(SetupDefaultPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);


            AddComponent(entity, new SetupDefaultPosition { });
        }
    }
}

public struct SetupDefaultPosition : IComponentData
{

}
