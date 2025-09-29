using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject bulletGameObject;
    public GameObject zombieGameObject;
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new EntitiesReferences
            {
                zombieEntity = GetEntity(authoring.zombieGameObject,TransformUsageFlags.Dynamic),
                bulletEntity = GetEntity(authoring.bulletGameObject,TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity bulletEntity;
    public Entity zombieEntity;
}
