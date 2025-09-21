using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupDefaultPositionSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRO<LocalTransform> localTransform,RefRW<UnitMover> unitMover,RefRW<SetupDefaultPosition> setupDefaultPosition,Entity entity) 
            in SystemAPI.Query<RefRO<LocalTransform>,RefRW<UnitMover>,RefRW<SetupDefaultPosition>>().WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

            entityCommandBuffer.RemoveComponent<SetupDefaultPosition>(entity);
        }
    }

   
}
