using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        foreach ((RefRW<LocalTransform> localTransform, RefRW<MoveOverride> moveOverride, EnabledRefRW<MoveOverride> moveOverrideEnabled, RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MoveOverride>, EnabledRefRW<MoveOverride>, RefRW<UnitMover>>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                moveOverrideEnabled.ValueRW = false;

            }
        }

    }  
}
