using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{

    public float timerMax;

    public float shootAttackDistance;

    public Transform shootSpawnPoint;
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                shootAttackDistance = authoring.shootAttackDistance,
                shootSpawnPoint = authoring.shootSpawnPoint.localPosition
            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    public float timerMin;
    public float timerMax;

    public float shootAttackDistance;

    public float3 shootSpawnPoint;
}
