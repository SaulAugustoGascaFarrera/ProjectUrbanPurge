using JetBrains.Annotations;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionStart;
    public event EventHandler OnSelectionEnd;

    private Vector2 selectionStartMousePosition;
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;

            OnSelectionStart?.Invoke(this,EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnSelectionEnd?.Invoke(this,EventArgs.Empty);

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected,Unit>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for(int i=0;i<entityArray.Length;i++)
            {
                Selected selected  = selectedArray[i];

                selected.onSelected = false;
                selected.onDeselected = true;

                entityManager.SetComponentData(entityArray[i], selected);
                entityManager.SetComponentEnabled<Selected>(entityArray[i],false);
            }


            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);


            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = ray.GetPoint(0f),
                End = ray.GetPoint(9999.0f),
                Filter = new CollisionFilter
                {
                    GroupIndex = 0,
                    CollidesWith = 1u << GameAssets.UNIT_LAYER,
                    BelongsTo = ~0u
                }
                
            };

            if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
            {
                if(entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
                {
                    entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                    Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                    selected.onSelected = true;
                    selected.onDeselected = false;
                    entityManager.SetComponentData(raycastHit.Entity, selected);
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
                End = cameraRay.GetPoint(9999.0f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    GroupIndex = 0,
                    CollidesWith = 1u << GameAssets.UNIT_LAYER | 1u << GameAssets.BUILDING_LAYER
                }
            };

            bool isAttackingSingleTarget = false;

            if(!isAttackingSingleTarget)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride>().Build(entityManager);

                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);

                for(int i=0;i<entityArray.Length;i++)
                {
                    MoveOverride moveOverride = moveOverrideArray[i];

                    moveOverride.targetPosition = mousePosition;

                    moveOverrideArray[i] = moveOverride;

                    entityManager.SetComponentEnabled<MoveOverride>(entityArray[i],true);

                    
                }

               entityQuery.CopyFromComponentDataArray(moveOverrideArray);

                
            }

           

        }

       
    }

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition  = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x,selectionEndMousePosition.x),Mathf.Min(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),Mathf.Max(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Rect rect = new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x - lowerLeftCorner.x,upperRightCorner.y - lowerLeftCorner.y);

        return rect;
    }
}
