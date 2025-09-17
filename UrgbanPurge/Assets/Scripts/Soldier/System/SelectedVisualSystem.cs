using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystem))]
partial struct SelectedVisualSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            if(selected.ValueRO.onDeselected)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                localTransform.ValueRW.Scale = 0.0f;
            }

            if(selected.ValueRO.onSelected)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                localTransform.ValueRW.Scale = 2.7f;
            }
        }
    }

    
}
