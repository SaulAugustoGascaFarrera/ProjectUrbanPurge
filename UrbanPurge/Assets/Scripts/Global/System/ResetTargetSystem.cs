using System.ComponentModel;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst =  true)]
partial struct ResetTargetSystem : ISystem
{
    private ComponentLookup<LocalTransform> localTransformComponentLookUp;
    private EntityStorageInfoLookup entityStorageInfo;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookUp = state.GetComponentLookup<LocalTransform>(true);
        entityStorageInfo = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        localTransformComponentLookUp.Update(ref state);
        entityStorageInfo.Update(ref state);

        ResetTargetJob resetTargetJob = new ResetTargetJob
        {
            localTransformComponentLookUp = localTransformComponentLookUp,
            entityStorageInfoLookUp = entityStorageInfo,
        };

        resetTargetJob.ScheduleParallel();
    }

    
}


[BurstCompile]
public partial struct ResetTargetJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookUp;
    [Unity.Collections.ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookUp;

    public void Execute(ref Target target)
    {
        if(target.targetEntity != Entity.Null)
        {
            if(!localTransformComponentLookUp.HasComponent(target.targetEntity) || !entityStorageInfoLookUp.Exists(target.targetEntity))
            {
                target.targetEntity = Entity.Null;
            }
        }
    }
}
