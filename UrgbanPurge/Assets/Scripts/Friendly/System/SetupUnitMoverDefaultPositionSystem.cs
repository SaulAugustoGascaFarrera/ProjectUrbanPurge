using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoverDefaultPositionSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<UnitMover> unitMover,RefRW<SetupUnitMoverDefaultPosition> SetupUnitMoverDefaultPosition,Entity entity) in
            SystemAPI.Query<RefRW<LocalTransform>,RefRW<UnitMover>,RefRW<SetupUnitMoverDefaultPosition>>().WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;


            entityCommandBuffer.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
        }
    }

    
}
