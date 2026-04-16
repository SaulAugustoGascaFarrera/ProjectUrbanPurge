using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{

    public event EventHandler OnSelectionStart;
    public event EventHandler OnSelectionEnd;


   
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnSelectionStart?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnSelectionEnd?.Invoke(this,EventArgs.Empty);

            UnitSelectionImplementation();

        }

        if(Input.GetMouseButtonUp(1))
        {
            UnitMovementImplementation();
        }


    }


    void UnitSelectionImplementation()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, Unit>().Build(entityManager);

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Selected selected = selectedArray[i];

            selected.OnSelected = false;
            selected.OnDeselected = true;

            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            entityManager.SetComponentData(entityArray[i], selected);
        }

        entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastInput raycastInput = new RaycastInput
        {
            Start = cameraRay.GetPoint(0.0f),
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0,
            },
            End = cameraRay.GetPoint(9999.0f),
        };

        if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
        {
            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);

            Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
            selected.OnSelected = true;
            selected.OnDeselected = false;

            entityManager.SetComponentData(raycastHit.Entity, selected);
        }

    }

    void UnitMovementImplementation()
    {
        Vector3 mousePosition = MouseManager.Instance.GetMousePosition();

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

        //PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

        //CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        //UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //RaycastInput raycastInput = new RaycastInput
        //{
        //    Start = ray.GetPoint(0.0f),
        //    Filter = new CollisionFilter
        //    {
        //        BelongsTo = ~0u,
        //        CollidesWith = 1u << GameAssets.BUILDING_LAYER | 1u << GameAssets.UNIT_LAYER,
        //        GroupIndex = 0

        //    },
        //    End =ray.GetPoint(9999.0f),
        //};

        bool isAttackingSingleTarget = false;

        //EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected,UnitMover>().Build(entityManager);

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride,TargetOverride>().Build(entityManager);


        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);

        NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);


        for(int i=0;i<entityArray.Length;i++)
        {
            MoveOverride moveOverride = moveOverrideArray[i];

            moveOverride.targetPosition = mousePosition;

            moveOverrideArray[i] = moveOverride;

            entityManager.SetComponentEnabled<MoveOverride>(entityArray[i],true);



            TargetOverride targetOverride = targetOverrideArray[i];

            targetOverride.targetEntity = Entity.Null;

            targetOverrideArray[i] = targetOverride;

           
        }

        entityQuery.CopyFromComponentDataArray(moveOverrideArray);

        entityQuery.CopyFromComponentDataArray(targetOverrideArray);

    }

    
}
