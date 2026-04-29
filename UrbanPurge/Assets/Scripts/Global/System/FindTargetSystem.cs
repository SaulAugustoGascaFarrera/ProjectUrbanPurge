using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);


        foreach ((RefRW<LocalTransform> localTransform,RefRW<FindTarget> findTarget,RefRW<Target> target,RefRO<TargetOverride> targetOverride) in 
            SystemAPI.Query<RefRW<LocalTransform>,RefRW<FindTarget>,RefRW<Target>, RefRO<TargetOverride>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (findTarget.ValueRO.timer > 0.0f)
            {
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            if (SystemAPI.Exists(targetOverride.ValueRO.targetEntity))
            {
                target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                continue;
            }

            distanceHitList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.BUILDING_LAYER | 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0
            };

            Entity closestTargetEntity = Entity.Null;

            float closestTargetDistance = float.MaxValue;

            float currentTargetDistanceOffset = 0.0f;

            if(SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                closestTargetEntity = target.ValueRO.targetEntity;

                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

                closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

                currentTargetDistanceOffset = 2.0f;
            }


            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.findDistanceRange,ref distanceHitList,collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    Faction targetFaction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);

                    if(targetFaction.factionType == findTarget.ValueRO.targetFaction)
                    {

                      
                        //target.ValueRW.targetEntity = distanceHit.Entity;

                        //UnityEngine.Debug.Log(distanceHit.Entity);


                        if(closestTargetEntity == Entity.Null)
                        {
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        }
                        else
                        {
                            if(distanceHit.Distance + currentTargetDistanceOffset < closestTargetDistance)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }


                    }
                }
            }

            if (SystemAPI.Exists(closestTargetEntity))
            {
                target.ValueRW.targetEntity = closestTargetEntity;
            }
        }
    }

    
}
