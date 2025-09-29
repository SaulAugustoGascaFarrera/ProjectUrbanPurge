using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Bullet> bullet,RefRO<Target> target, Entity entity) in 
            SystemAPI.Query<RefRW<LocalTransform>,RefRW<Bullet>,RefRO<Target>>().WithEntityAccess())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetLocation = targetLocalTransform.TransformPoint(shootVictim.hitLocation);

            float distanceSQBefore = math.distancesq(localTransform.ValueRO.Position,targetLocation);

            float3 movementDirection = (targetLocation - localTransform.ValueRO.Position);

            movementDirection = math.normalize(movementDirection);

            localTransform.ValueRW.Position += movementDirection * bullet.ValueRO.bulletMovemntSpeed * SystemAPI.Time.DeltaTime;

            float distanceSQAfter = math.distancesq(localTransform.ValueRO.Position, targetLocation);

            if(distanceSQAfter > distanceSQBefore)
            {
                localTransform.ValueRW.Position = targetLocalTransform.Position;
            }

            if(math.distancesq(localTransform.ValueRO.Position,targetLocation) < UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
            {
                entityCommandBuffer.DestroyEntity(entity);


                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

            }

        }
    }

    
}
