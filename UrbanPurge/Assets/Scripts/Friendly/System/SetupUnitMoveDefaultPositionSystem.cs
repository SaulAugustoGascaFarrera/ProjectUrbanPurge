using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoveDefaultPositionSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().
            CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<UnitMover> unitMover,RefRO<SetupUnitMoveDefaultPosition> setupUnitMoveDefaultPosition,Entity entity) in
            SystemAPI.Query<RefRW<LocalTransform>,RefRW<UnitMover>, RefRO<SetupUnitMoveDefaultPosition>>().WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

            entityCommandBuffer.RemoveComponent<SetupUnitMoveDefaultPosition>(entity);
        }
    }

    
}
