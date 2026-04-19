using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnTime = 2f;
    [SerializeField] private float maxSpawnTime = 4f;
    [SerializeField] private bool ricochet = false;
    [SerializeField] private int ricochetAmount = 0;
    [SerializeField] private float bulletSize = 0.1f;
    [SerializeField] private float enemySpeed = 3f;
    private float spawnTimer;

    private List<EnemyBehaviour> spawnedEnemies = new List<EnemyBehaviour>();

    private void Start()
    {
        ResetTimer();
        for (int i = 7; i > 0; i--)
        {
            SpawnEnemy();
        }
    }

    private void OnEnable()
    {
        EventManager.EnemySpawnedEvent += AddEnemyToList;
        EventManager.EnemyDiedEvent += RemoveEnemyFromList;
    }

    private void OnDisable()
    {
        EventManager.EnemySpawnedEvent -= AddEnemyToList;
        EventManager.EnemyDiedEvent -= RemoveEnemyFromList;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            ResetTimer();
        }
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        GameObject spawnPoint = spawnPoints[randomIndex];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyBehaviour enemyBehaviour = newEnemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.UpgradeApply(ricochet, ricochetAmount, bulletSize, enemySpeed);
    }

    private void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }


    public void SpeedUpgrade(int amount)
    {
        enemySpeed += 1.5f; //TODO Burn this
        UpdateEnemyUpgrades();
    }

    public void RicochetUpgrade()
    {
        if (!ricochet) ricochet = true;
        ricochetAmount += 2;
        UpdateEnemyUpgrades();
    }
    public void BulletSizeUpgrade(float amount)
    {
        bulletSize += amount;
        UpdateEnemyUpgrades();
    }

    private void UpdateEnemyUpgrades()
    {
        foreach (EnemyBehaviour enemy in spawnedEnemies)
        {
            enemy.UpgradeApply(ricochet, ricochetAmount, bulletSize, enemySpeed);
        }
    }

    public void AddEnemyToList(EnemyBehaviour newEnemy)
    {
        spawnedEnemies.Add(newEnemy);
    }

    public void RemoveEnemyFromList(EnemyBehaviour enemyToRemove)
    {
        if (spawnedEnemies.Contains(enemyToRemove))
            spawnedEnemies.Remove(enemyToRemove);
    }

}
