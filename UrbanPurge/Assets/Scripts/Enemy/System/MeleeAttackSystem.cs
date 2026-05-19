using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<RaycastHit> raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);


        foreach((RefRW<MeleeAttack> meleeAttack,RefRW<Target> target,RefRW<LocalTransform> localTransform,RefRW<UnitMover> unitMover) in
            SystemAPI.Query<RefRW<MeleeAttack>,RefRW<Target>,RefRW<LocalTransform>,RefRW<UnitMover>>())
        {

            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float meleeAttackDistanceSq = 2.0f;
            bool isCloseEnoughToAttack = math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) < meleeAttackDistanceSq;

            bool isTouchingTarget = false;

            if(!isCloseEnoughToAttack)
            {
                float3 directionToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                directionToTarget = math.normalize(directionToTarget);

                float distanceExtraToTestRaycast = 0.4f;

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    Filter = CollisionFilter.Default,
                    End = localTransform.ValueRO.Position + directionToTarget * (meleeAttack.ValueRO.colliderSize + distanceExtraToTestRaycast)
                };

                raycastHitList.Clear();

                if(collisionWorld.CastRay(raycastInput,ref raycastHitList))
                {
                    foreach(RaycastHit raycastHit in raycastHitList)
                    {

                        if(raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            isTouchingTarget = true;

                            break;
                        }
                        
                    }
                }
            }


            if(!isCloseEnoughToAttack && !isTouchingTarget)
            {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if(meleeAttack.ValueRO.timer > 0)
                {
                    continue;
                }

                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;

                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;

                meleeAttack.ValueRW.onAttacked = true;

            }


        }

    }

    
}
