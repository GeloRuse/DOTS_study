using Unity.Burst;
using Unity.Entities;

partial struct PlayerStatsSystem : ISystem
{
    private PlayerStats playerStats;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        playerStats = SystemAPI.GetSingleton<PlayerStats>();

        foreach(var (enemy, enemyHealth) in SystemAPI.Query<RefRO<Enemy>, RefRO<Health>>())
        {
            if (enemyHealth.ValueRO.onDeath)
            {
                playerStats.currentExp += enemy.ValueRO.expGain;
                playerStats.coins++;
                CheckLevelUp();
                SystemAPI.SetSingleton(playerStats);
            }
        }

        foreach (var weapon in SystemAPI.Query<RefRW<Weapon>>().WithPresent<PlayerWeapon>())
        {
            weapon.ValueRW.timerMax = 1f / (playerStats.level * 5);
        }
    }

    [BurstCompile]
    private void CheckLevelUp()
    {
        playerStats.nextLevelExp = playerStats.level * 10;
        if (playerStats.currentExp >= playerStats.nextLevelExp) 
        {
            playerStats.currentExp = 0;
            playerStats.level++;
        }
    }
}
