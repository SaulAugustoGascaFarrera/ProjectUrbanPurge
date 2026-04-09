using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualSystem1 : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            localTransform.ValueRW.Scale = 0.0f;

        }

        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>())
        {
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            localTransform.ValueRW.Scale = selected.ValueRO.showScale;

        }
    }

   
}
