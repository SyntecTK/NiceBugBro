using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action CollectedUpgrade;
    public static Action<EnemyBehaviour> EnemySpawnedEvent;
    public static Action<EnemyBehaviour> EnemyDiedEvent;

    public static void OnCollectedUpgrade()
    {
        CollectedUpgrade?.Invoke();
    }

    public static void EnemySpawned(EnemyBehaviour newEnemy)
    {
        EnemySpawnedEvent?.Invoke(newEnemy);
    }
    
    public static void EnemyDied(EnemyBehaviour deadEnemy)
    {
        EnemyDiedEvent?.Invoke(deadEnemy);
    }
}