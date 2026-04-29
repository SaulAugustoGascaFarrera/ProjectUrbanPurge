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

    public Vector2 selectionStartMousePosition;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;

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

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

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

        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);

        Rect selectionAreaRect = GetSelectionAreaRect();
        float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
        float multipleSelectionMinSize = 40.0f;
        bool isMultipleSelection = selectionAreaSize > multipleSelectionMinSize;


        if(isMultipleSelection)
        {
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            for(int i=0;i<entityArray.Length;i++)
            {
                LocalTransform unitLocalTransform = localTransformArray[i];

                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);

                if(selectionAreaRect.Contains(unitScreenPosition))
                {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], true);

                    Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                    selected.OnSelected = true;
                    selected.OnDeselected = false;

                   entityManager.SetComponentData(entityArray[i], selected);

                   
                }

                
            }

        }
        else
        {
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

       

    }

    void UnitMovementImplementation()
    {
        Vector3 mousePosition = MouseManager.Instance.GetMousePosition();

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastInput raycastInput = new RaycastInput
        {
            Start = ray.GetPoint(0.0f),
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.BUILDING_LAYER | 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0

            },
            End = ray.GetPoint(9999.0f),
        };

        bool isAttackingSingleTarget = false;

        if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
        {
            if(entityManager.HasComponent<TargetOverride>(raycastHit.Entity))
            {
                //Hit something with faction component
                Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);

                if(faction.factionType == FactionType.Zombie)
                {
                    isAttackingSingleTarget = true;

                    //right clicking on a zombie

                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<TargetOverride>().Build(entityManager);

                    NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                    NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);

                    for(int i=0;i<entityArray.Length;i++)
                    {
                        TargetOverride targetOverride = targetOverrideArray[i];

                        targetOverride.targetEntity = raycastHit.Entity;

                        targetOverrideArray[i] = targetOverride;

                        entityManager.SetComponentEnabled<MoveOverride>(entityArray[i],false);
                    }

                    entityQuery.CopyFromComponentDataArray(targetOverrideArray);

                    //Debug.Log("Zombie Target Overriude !!!!");
                }


                
            }
        }

        if(!isAttackingSingleTarget)
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride, TargetOverride>().Build(entityManager);


            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);

            NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);

            NativeArray<float3> movePositionArray = GenerateMovePositionArray(mousePosition, entityArray.Length);


            for (int i = 0; i < entityArray.Length; i++)
            {
                MoveOverride moveOverride = moveOverrideArray[i];

                moveOverride.targetPosition = movePositionArray[i];

                moveOverrideArray[i] = moveOverride;

                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);



                TargetOverride targetOverride = targetOverrideArray[i];

                targetOverride.targetEntity = Entity.Null;

                targetOverrideArray[i] = targetOverride;


            }

            entityQuery.CopyFromComponentDataArray(moveOverrideArray);

            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
        }

    }


    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x,selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(
           Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
           Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y)
       );

        return new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x - lowerLeftCorner.x,upperRightCorner.y - lowerLeftCorner.y);
    }

    public NativeArray<float3> GenerateMovePositionArray(float3 targetPosition,int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount,Allocator.Temp);

        if(positionCount == 0)
        {
            return positionArray;
        }

        positionArray[0] = targetPosition;

        if(positionCount == 1)
        {
            return positionArray;
        }

        float ringSize = 3.9f;
        int ring = 0;
        int positionIndex = 1;

        while(positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ring * 2;


            for(int i=0;i<ringPositionCount;i++)
            {

                float angle = i * (math.PI / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle),new float3(ringSize * (ring + 1),0,0));
                float3 ringPosition = targetPosition + ringVector;


                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if(positionIndex >= positionCount)
                {
                    break;
                }
            }

            ring++;
        }

        return positionArray;
    }


}
