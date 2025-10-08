using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public event EventHandler OnStartSelection;
    public event EventHandler OnEndSelection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnStartSelection?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnEndSelection?.Invoke(this, EventArgs.Empty);

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Select, Unit>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<Select> selectArray = entityQuery.ToComponentDataArray<Select>(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++)
            {
                Select select = selectArray[i];

                select.onSelect = false;
                select.onDeselect = true;

                entityManager.SetComponentEnabled<Select>(entityArray[i], false);

                entityManager.SetComponentData(entityArray[i], select);

            }

            //entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Select>().Build(entityManager);

            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0),
                End = cameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PLAYER_LAYER,
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                if(entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Select>(raycastHit.Entity))
                {
                    Select select = entityManager.GetComponentData<Select>(raycastHit.Entity);

                    entityManager.SetComponentEnabled<Select>(raycastHit.Entity, true);
                    select.onSelect = true;
                    select.onDeselect = false;

                    entityManager.SetComponentData(raycastHit.Entity, select);
                }

               
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = MouseManager.Instance.GetMousePosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PLAYER_LAYER,
                    GroupIndex = 0
                }
            };

            bool isSAttackingSingleTarget = false;

            if (collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
            {
                if(entityManager.HasComponent<Faction>(raycastHit.Entity))
                {

                    //Hit something with Faction Component
                    Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);

                    if (faction.factionType == FactionType.Zombie)
                    {
                        //Debug.Log("OVERRIDE EN UN ZOMBIEEEE");

                        isSAttackingSingleTarget = true;

                        //right click on a zombie (enemy layer)
                        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Select>().WithPresent<TargetOverride>().Build(entityManager);

                        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                        NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);

                        for (int i = 0; i < entityArray.Length; i++)
                        {
                            TargetOverride targetOverride = targetOverrideArray[i];

                            targetOverride.targetEntity = raycastHit.Entity;

                            targetOverrideArray[i] = targetOverride;

                            entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], false);
                        }

                        entityQuery.CopyFromComponentDataArray(targetOverrideArray);

                    }



                }
            }


            if(!isSAttackingSingleTarget)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Select>().WithPresent<MoveOverride, TargetOverride>().Build(entityManager);

                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);
                NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);

                for (int i = 0; i < entityArray.Length; i++)
                {
                    //Move Override
                    MoveOverride moveOverride = moveOverrideArray[i];

                    moveOverride.targetPosition = mousePosition;

                    entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);

                    moveOverrideArray[i] = moveOverride;

                    //Target Override
                    TargetOverride targetOverride = targetOverrideArray[i];

                    targetOverride.targetEntity = Entity.Null;

                    targetOverrideArray[i] = targetOverride;
                }

                entityQuery.CopyFromComponentDataArray(moveOverrideArray);

                entityQuery.CopyFromComponentDataArray(targetOverrideArray);
            }

           
        }
    }
}
