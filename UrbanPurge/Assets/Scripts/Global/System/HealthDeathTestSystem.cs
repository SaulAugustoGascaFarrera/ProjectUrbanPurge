using Unity.Burst;
using Unity.Entities;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeathTestSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<Health> health,Entity entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
        {
            if(health.ValueRO.healthAmount <= 0.0f && health.ValueRO.onHealthChanged == true)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }

}
