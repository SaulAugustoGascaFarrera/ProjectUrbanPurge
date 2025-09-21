using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRO<LocalTransform>localTransform,RefRW<UnitMover> unitMover,RefRW<MoveOverride> moveOverride,EnabledRefRW<MoveOverride> enableMoveOverride) in 
            SystemAPI.Query<RefRO<LocalTransform>,RefRW<UnitMover>, RefRW<MoveOverride>, EnabledRefRW<MoveOverride>>())
        {
            if(math.distancesq(moveOverride.ValueRO.targetPosition,localTransform.ValueRO.Position) > UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                enableMoveOverride.ValueRW = false;
            }
        }
    }

   
}
