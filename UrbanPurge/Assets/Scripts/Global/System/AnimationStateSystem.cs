using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;


[UpdateAfter(typeof(ShootAttackSystem))]
partial struct AnimationStateSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRO<UnitMover> unitMover,RefRO<AnimatedMesh> animatedMesh,RefRO<UnitAnimations> unitAnimations) in
            SystemAPI.Query<RefRO<UnitMover>,RefRO<AnimatedMesh>,RefRO<UnitAnimations>>())
        {
            RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);


            if(unitMover.ValueRO.isMoving)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.ValueRO.walkAnimationType;
            }
            else
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.ValueRO.idleAnimationType;
            }
        }

        foreach ((RefRO<ShootAttack> shootAttack, RefRO<AnimatedMesh> animatedMesh, RefRO<UnitAnimations> unitAnimations,RefRO<UnitMover> unitMover,RefRO<Target> target) in
            SystemAPI.Query<RefRO<ShootAttack>, RefRO<AnimatedMesh>, RefRO<UnitAnimations>,RefRO<UnitMover>,RefRO<Target>>())
        {
            

            if(!unitMover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
            {
                RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.ValueRO.aimAnimationType;
            }


            if(shootAttack.ValueRO.onShoot.isTriggered)
            {
                RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.ValueRO.shootAnimationType;

                //UnityEngine.Debug.Log("Shoot anim");

                
            }
            
        }

        foreach ((RefRO<MeleeAttack> meeleAttack, RefRO<AnimatedMesh> animatedMesh, RefRO<UnitAnimations> unitAnimations) in
            SystemAPI.Query<RefRO<MeleeAttack>, RefRO<AnimatedMesh>, RefRO<UnitAnimations>>())
        {


            if (meeleAttack.ValueRO.onAttacked)
            {
                RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.ValueRO.meeleAttackAnimationType;

                //UnityEngine.Debug.Log("Shoot anim");


            }

        }
    }

}
