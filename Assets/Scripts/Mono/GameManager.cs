using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject gunPrefab;

    [SerializeField]
    private int totalCoins;
    private int baseUpgradePrice = 10;
    private int currentUpgradeLevel = 1;
    private bool gameStarted = false;

    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]
    private TextMeshProUGUI priceText;

    [SerializeField]
    private GameObject gamePanel;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private TextMeshProUGUI gameCoinsText;
    [SerializeField]
    private TextMeshProUGUI enemiesText;
    [SerializeField]
    private TextMeshProUGUI enemiesKilledText;
    [SerializeField]
    private TextMeshProUGUI bulletsText;

    private EntityManager entityManager;
    private EntityQuery entityQuery;
    private Entity gameControlEntity;
    private Entity playerStatsEntity;
    private Entity playerEntity;

    private GameControl gameControl;
    private PlayerStats playerStats;
    private Health playerHealth;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameControl>().Build(entityManager);
        gameControlEntity = entityQuery.GetSingletonEntity();
        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerStats>().Build(entityManager);
        playerStatsEntity = entityQuery.GetSingletonEntity();
        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Player, Health>().Build(entityManager);
        playerEntity = entityQuery.GetSingletonEntity();
        playerHealth = entityManager.GetComponentData<Health>(playerEntity);

        coinsText.text = totalCoins.ToString();
        priceText.text = baseUpgradePrice.ToString();
    }

    private void Update()
    {
        if (gameStarted)
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameControl>().Build(entityManager);
            gameControl = entityQuery.GetSingleton<GameControl>();
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerStats>().Build(entityManager);
            playerStats = entityQuery.GetSingleton<PlayerStats>();
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Player, Health>().Build(entityManager);
            playerEntity = entityQuery.GetSingletonEntity();
            playerHealth = entityManager.GetComponentData<Health>(playerEntity);
            
            SetGameInfo();

            if (!gameControl.isRunning)
            {
                menuPanel.SetActive(true);
                gamePanel.SetActive(false);

                totalCoins += playerStats.coins;
                gameStarted = false;

                coinsText.text = totalCoins.ToString();

                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Player, Health>().Build(entityManager);
                playerEntity = entityQuery.GetSingletonEntity();
                playerHealth = entityManager.GetComponentData<Health>(playerEntity);
                playerHealth.healthAmount = playerHealth.healthMax;
                entityManager.SetComponentData(playerEntity, playerHealth);
            }
        }
    }

    public void StartGame()
    {
        if (gameStarted)
        {
            return;
        }

        gameControl = new GameControl
        {
            isRunning = true,
            enemiesKilled = 0,
        };
        playerStats = new PlayerStats
        {
            coins = 0,
            level = 1,
            currentExp = 0,
        };

        entityManager.SetComponentData(gameControlEntity, gameControl);
        entityManager.SetComponentData(playerStatsEntity, playerStats);

        SetGameInfo();

        gameStarted = true;
    }

    public void BuyGun()
    {
        if (gameStarted)
        {
            return;
        }

        if (currentUpgradeLevel * baseUpgradePrice <= totalCoins)
        {
            totalCoins -= currentUpgradeLevel * baseUpgradePrice;
            currentUpgradeLevel++;

            coinsText.text = totalCoins.ToString();
            priceText.text = (currentUpgradeLevel * baseUpgradePrice).ToString();

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameControl>().Build(entityManager);
            gameControl = entityQuery.GetSingleton<GameControl>();
            gameControl.currentUpgrade = currentUpgradeLevel;
            gameControl.onUpgrade = true;

            entityManager.SetComponentData(gameControlEntity, gameControl);
        }
    }

    private void SetGameInfo()
    {
        levelText.text = playerStats.level.ToString();
        healthText.text = playerHealth.healthAmount.ToString();
        gameCoinsText.text = playerStats.coins.ToString();
        enemiesText.text = gameControl.currentEnemies.ToString();
        bulletsText.text = gameControl.currentBullets.ToString();
        enemiesKilledText.text = gameControl.enemiesKilled.ToString();
    }
}
