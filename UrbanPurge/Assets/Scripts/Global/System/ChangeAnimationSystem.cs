using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;


[UpdateBefore(typeof(ActiveAnimation))]
partial struct ChangeAnimationSystem : ISystem
{
   
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();


        

        foreach((RefRW<MaterialMeshInfo> materialMeshInfo,RefRW<ActiveAnimation> activeAnimation) in 
            SystemAPI.Query<RefRW<MaterialMeshInfo>,RefRW<ActiveAnimation>>())
        {

            if(activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
            {
                continue;
            }

            if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
            {
                continue;
            }


            if (activeAnimation.ValueRO.activeAnimationType != activeAnimation.ValueRO.nextAnimationType)
            {
                activeAnimation.ValueRW.frame = 0;
                activeAnimation.ValueRW.frameTimer = 0f;
                activeAnimation.ValueRW.activeAnimationType = activeAnimation.ValueRO.nextAnimationType;

                ref AnimationData animationData = ref animationDataHolder.animationDataBlobArrayBlobAsset.Value[(int)activeAnimation.ValueRO.activeAnimationType];

                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIdArray[0];
            }
        }
    }

}
