using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{

    public int damageAmount;
    public float bulletMovemntSpeed;
    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Bullet
            {
                damageAmount = authoring.damageAmount,
                bulletMovemntSpeed = authoring.bulletMovemntSpeed,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public int damageAmount;
    public float bulletMovemntSpeed;
}