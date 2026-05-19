using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct ActiveAnimationSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();

        foreach((RefRW<ActiveAnimation> activeAnimation,RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>,RefRW<MaterialMeshInfo>>())
        {

            //if(!activeAnimation.ValueRO.animationDataBlobAssetReference.IsCreated)
            //{
            //    activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdleBlobAsset;
            //}

            //if(Input.GetKeyDown(KeyCode.T))
            //{
            //    activeAnimation.ValueRW.nextAnimationType = AnimationDataSO.AnimationType.SoldierIdle;
            //}

            //if (Input.GetKeyDown(KeyCode.Y))
            //{
            //    activeAnimation.ValueRW.nextAnimationType = AnimationDataSO.AnimationType.SoldierWalk;
            //}


            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArrayBlobAsset.Value[(int)activeAnimation.ValueRO.activeAnimationType];

            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;

            if(activeAnimation.ValueRO.frameTimer > animationData.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimerMax;

                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % animationData.frameMax;

                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIdArray[activeAnimation.ValueRO.frame];


                if(activeAnimation.ValueRO.frame == 0 && 
                    activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }


                if (activeAnimation.ValueRO.frame == 0 &&
                    activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }
            }

        }
    }

   
}
