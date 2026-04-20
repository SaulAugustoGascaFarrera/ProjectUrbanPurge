using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LoseTargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LoseTarget> loseTarget,RefRW<LocalTransform> localTransform,RefRW<Target> target,RefRO<TargetOverride> targetOverride) in
            SystemAPI.Query<RefRW<LoseTarget>,RefRW<LocalTransform>,RefRW<Target>,RefRO<TargetOverride>>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            //if(SystemAPI.Exists(targetOverride.ValueRO.targetEntity))
            //{
            //    continue;
            //}

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float targetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(targetDistance > loseTarget.ValueRO.loseTargetDistance)
            {
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }

}
