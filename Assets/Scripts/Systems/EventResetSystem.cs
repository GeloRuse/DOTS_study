using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
partial struct EventResetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        HealthJob healthJob = new HealthJob();
        healthJob.ScheduleParallel();
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
public partial struct HealthJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onDeath = false;
    }
}
