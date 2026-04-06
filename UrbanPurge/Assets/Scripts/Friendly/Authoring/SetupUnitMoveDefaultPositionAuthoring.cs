using Unity.Entities;
using UnityEngine;

public class SetupUnitMoveDefaultPositionAuthoring : MonoBehaviour
{
    public class Baker : Baker<SetupUnitMoveDefaultPositionAuthoring>
    {
        public override void Bake(SetupUnitMoveDefaultPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SetupUnitMoveDefaultPosition { });
        }
    }
}

public struct SetupUnitMoveDefaultPosition : IComponentData
{

}
