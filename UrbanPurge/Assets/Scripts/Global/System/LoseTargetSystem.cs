using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LoseTargetSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LocalTransform> localTransform,RefRW<Target> target,RefRW<LoseTarget> loseTarget) in 
            SystemAPI.Query<RefRW<LocalTransform>,RefRW<Target>,RefRW<LoseTarget>>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float targetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(targetDistance > loseTarget.ValueRO.loseTargetDistance)
            {
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }

    
}
