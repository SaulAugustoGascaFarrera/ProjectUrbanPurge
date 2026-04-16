using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;


[UpdateInGroup(typeof(LateSimulationSystemGroup),OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ResetSelectedEventsJob().ScheduleParallel();

        new ResetHealthEventsJob().ScheduleParallel();
    }

   
}


[BurstCompile]
public partial struct ResetHealthEventsJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
    }
}

[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.OnSelected = false;
        selected.OnDeselected = false;
    }
}
