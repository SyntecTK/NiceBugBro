using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Movement Settings")]
    [SerializeField] private float hoverHeight = 3f;
    [SerializeField] private float bobAmplitude = 0.3f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float shootInterval = 2f;

    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    private Vector3 hoverPosition;
    private float shootTimer;
    private float bobOffset;

    private void Start()
    {
        hoverPosition = transform.position + Vector3.up * hoverHeight;
        transform.position = hoverPosition;
        shootTimer = shootInterval;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        currentHealth = maxHealth;
    }

    private void Update()
    {
        MoveTowardsPlayer();
        Hover();
        HandleShooting();
    }

    private void MoveTowardsPlayer()
    {
        if (PlayerController.Instance == null) return;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 direction = playerPos - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Hover()
    {
        float bobY = Mathf.Sin(Time.time * bobFrequency + bobOffset) * bobAmplitude;

        Vector3 pos = transform.position;
        pos.y = hoverHeight + bobY;

        transform.position = pos;
    }

    private void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    private void Shoot()
    {
        if (PlayerController.Instance == null || firePoint == null) return;

        Vector3 direction = (PlayerController.Instance.transform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Bullet>().Initialize(bulletDamage);
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
