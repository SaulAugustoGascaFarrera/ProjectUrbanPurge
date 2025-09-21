using Unity.Entities;
using UnityEngine;

public class SelectAuthoring : MonoBehaviour
{
    public class Baker : Baker<SelectAuthoring>
    {
        public override void Bake(SelectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Select { });

            SetComponentEnabled<Select>(entity, false);
        }
    }
}

public struct Select : IComponentData,IEnableableComponent
{
    public bool onSelect;
    public bool onDeselect;
}
