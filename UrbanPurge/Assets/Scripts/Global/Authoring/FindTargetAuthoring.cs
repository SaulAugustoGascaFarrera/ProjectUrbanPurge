using Unity.Entities;
using UnityEngine;

public class FindTargetAuthoring : MonoBehaviour
{
  
    public float timerMax;

    public float findTargetDistanceRange;

    public FactionType targetFaction;
    public class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new FindTarget
            {
                timerMax = authoring.timerMax,
                findTargetDistanceRange = authoring.findTargetDistanceRange,
                targetFaction = authoring.targetFaction,

            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float timer;
    public float timerMax;

    public float findTargetDistanceRange;

    public FactionType targetFaction;
}