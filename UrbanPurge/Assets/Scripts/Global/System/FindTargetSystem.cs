using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityManager entityManager = state.WorldUnmanaged.EntityManager;

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach((RefRW<Target> target,RefRW<FindTarget> findTarget,RefRW<LocalTransform> localTransform) in SystemAPI.Query<RefRW<Target>,RefRW<FindTarget>,RefRW<LocalTransform>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(findTarget.ValueRO.timer > 0.0f)
            {
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            distanceHitList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter { 
            
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.PLAYER_LAYER | 1u << GameAssets.BUILDING_LAYER,
                GroupIndex = 0

            };


            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.findTargetDistanceRange,ref distanceHitList,collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    if(!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<LocalTransform>(distanceHit.Entity))
                    {
                        continue;
                    }

                    Faction faction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);

                    if(faction.factionType == findTarget.ValueRO.targetFaction)
                    {
                        target.ValueRW.targetEntity = distanceHit.Entity;
                    }
                }
            }



        }
    }

    
}
