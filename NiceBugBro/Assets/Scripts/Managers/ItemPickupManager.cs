using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour
{
    public static ItemPickupManager Instance { get; private set; }
    
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private GameObject pickupPrefab;

    private readonly float pickupCooldown = 5f;
    private float currentPickupCooldown;
    private bool pickupCurrentlyReady;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        ResetPickupCooldown();
    }

    private void SpawnNewPickup()
    {
        GameObject newPickup = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(pickupPrefab, newPickup.transform.position, pickupPrefab.transform.rotation);
        pickupCurrentlyReady = true;
    }

    private void OnEnable()
    {
        EventManager.CollectedUpgrade += UpgradeCollected;
    }

    private void OnDisable()
    {
        EventManager.CollectedUpgrade -= UpgradeCollected;
    }

    private void UpgradeCollected()
    {
        ResetPickupCooldown();
    }

    private void Update()
    {
        if (!pickupCurrentlyReady)
        {
            currentPickupCooldown -= Time.deltaTime;
            if (currentPickupCooldown <= 0) SpawnNewPickup();
        }
    }

    private void ResetPickupCooldown()
    {
        pickupCurrentlyReady = false;
        currentPickupCooldown = pickupCooldown;
    }
}