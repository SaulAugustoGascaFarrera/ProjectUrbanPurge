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
        foreach (RefRW<Select> select in SystemAPI.Query<RefRW<Select>>().WithPresent<Select>())
        {
            if (select.ValueRO.onSelect)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.selectedVisualEntity);
                localTransform.ValueRW.Scale = 3.0f;
            }

            if (select.ValueRO.onDeselect)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.selectedVisualEntity);
                localTransform.ValueRW.Scale = 0.0f;
            }
        }
    }

}
