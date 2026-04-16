using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 10f;
    private float spawnTimer;

    private void Start()
    {
        ResetTimer();
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
        Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    private void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
