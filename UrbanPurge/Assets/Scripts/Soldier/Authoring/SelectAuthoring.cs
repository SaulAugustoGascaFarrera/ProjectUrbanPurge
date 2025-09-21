using Unity.Entities;
using UnityEngine;

public class SelectAuthoring : MonoBehaviour
{
    public GameObject selectedVisualGameObject;

    public class Baker : Baker<SelectAuthoring>
    {
        public override void Bake(SelectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Select 
            { 
                    
                selectedVisualEntity = GetEntity(authoring.selectedVisualGameObject,TransformUsageFlags.Dynamic)

            });

            SetComponentEnabled<Select>(entity, false);
        }
    }
}

public struct Select : IComponentData,IEnableableComponent
{
    public Entity selectedVisualEntity;

    public bool onSelect;
    public bool onDeselect;
}
