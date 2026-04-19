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
    [SerializeField] private float bulletSpeed = 25f;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private bool enemyBullet = true;
    [SerializeField] private bool ricochet = false;
    [SerializeField] private int ricochetAmount = 0;
    [SerializeField] private float bulletSize = 0.1f;

    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    private Vector3 hoverPosition;
    private float shootTimer;
    private float bobOffset;
    private int timeFactor = 1;

    private void Start()
    {
        hoverPosition = transform.position + Vector3.up * hoverHeight;
        transform.position = hoverPosition;
        shootTimer = shootInterval;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        currentHealth = maxHealth;
        EventManager.EnemySpawned(this);
    }

    public void UpgradeApply(bool ricochet, int ricochetAmount, float bulletSize, float speed)
    {
        this.ricochet = ricochet;
        this.ricochetAmount = ricochetAmount;
        this.bulletSize = bulletSize;
        moveSpeed = speed;
    }

    private void Update()
    {
        MoveTowardsPlayer();
        //Hover();
        HandleShooting();
        timeFactor = GameManager.Instance.GetTimeFactor();
    }

    private void MoveTowardsPlayer()
    {
        if (!GameManager.Instance.PlayerExists()) return;

        Vector3 playerPos = GameManager.Instance.GetPlayerPosition();
        Vector3 direction = playerPos - transform.position;
        //direction.y = 0f;
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
    }

    private void Hover()
    {
        float bobY = Mathf.Sin(Time.time * bobFrequency + bobOffset) * bobAmplitude;

        Vector3 pos = transform.localPosition;
        pos.y = hoverHeight + bobY;

        transform.localPosition = pos;
    }

    private void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        //shootInterval -= timeFactor / 10f;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    private void Shoot()
    {
        if (!GameManager.Instance.PlayerExists() || firePoint == null) return;

        Vector3 direction = (GameManager.Instance.GetPlayerPosition() - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Bullet>().Initialize(enemyBullet, bulletDamage, ricochet, ricochetAmount, bulletSize);
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
        AudioManager.Instance.Play3DSound(SoundType.EnemyShot, transform.position);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            EventManager.EnemyDied(this);
            AudioManager.Instance.Play3DSound(SoundType.EnemyKill, transform.position);
            GameManager.Instance.KillEnemy();
            Destroy(gameObject);
        }
    }
}
