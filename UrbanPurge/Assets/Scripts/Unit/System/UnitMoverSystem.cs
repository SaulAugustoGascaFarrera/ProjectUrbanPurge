using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    public const float UNIT_MOVER_REACHED_DISTANCE_SQ = 0.2f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        unitMoverJob.ScheduleParallel();
    }

   
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;
    public void Execute(ref LocalTransform localTransform,ref UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
    {
        float3 movementDirection = (unitMover.targetPosition - localTransform.Position);

        movementDirection = math.normalize(movementDirection);

        if(math.distancesq(unitMover.targetPosition,localTransform.Position) > UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
        {

            localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(movementDirection, math.up()), unitMover.rotationtSpeed * deltaTime);

            physicsVelocity.Linear = movementDirection * unitMover.movemenSpeed;

            physicsVelocity.Angular = 0.0f;
        }
        else
        {
            physicsVelocity.Linear = 0.0f;

            physicsVelocity.Angular = 0.0f;
        }
    }
}