using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("PlayerStats")]
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    private int health = 100;
    [SerializeField] private float currentSpeed;
    private float speed = 10;
    [SerializeField] private float currentJumpForce;
    private float jumpForce = 15;
    [SerializeField] private float currentMass;
    private float mass = 2;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    private float lookSensitivity = 0.15f;
    [SerializeField] private float currentLookSensitivity;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;
    private Vector3 startPos;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform firePointParent;
    [SerializeField] private Transform raycastPoint;

    [SerializeField] private GameObject bulletPrefab;

    [Header("BulletStats")]
    [SerializeField] private int currentBulletDamage;
    private int bulletDamage = 10;
    [SerializeField] private float currentBulletSpeed;
    private float bulletSpeed = 50;
    [SerializeField] private float currentBulletLifeTime;
    private float bulletLifeTime = 3f;
    [SerializeField] private float currentBulletSize;
    private float bulletSize = 0.1f;

    [Header("GamePlay Changes")]
    [SerializeField] private bool minimap;
    [SerializeField] private bool burstShot;
    [SerializeField] private bool spreadShot;
    [SerializeField] private bool ricochet;
    [SerializeField] private bool bulletSizeChanged;

    private int ricochetAmount;
    private int burstShotAmount;
    private int burstShotCounter;
    private bool isJumping;
    [SerializeField] private bool isGrounded;

    [Header("Audio")]
    [SerializeField] private float baseFootstepInterval = 0.4f;
    private float footstepTimer;

    private Rigidbody rb;
    private float gravity = -5f;
    [SerializeField] private float currentGravity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //TODO: Check obs OnLoad resettet wird
        rb = GetComponent<Rigidbody>();
        InitializePlayer();
    }

    private void Update()
    {
        if (GameManager.Instance.IsMovementLocked) return;
        HandleMovement();
        HandleLook();
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        if (Physics.Raycast(raycastPoint.position, -raycastPoint.up, out RaycastHit hit, 1f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        rb.angularVelocity = Vector3.zero;
    }

    private void HandleMovement()
    {
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement.y = 0f;

        bool isMoving = movement.sqrMagnitude > 0.01f;

        if (!isGrounded)
        {
            transform.Translate(movement.normalized * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(movement.normalized * (currentSpeed * 2) * Time.deltaTime, Space.World);

            if (isMoving)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    AudioManager.Instance.Play2DSound(SoundType.Footstep);
                    float speedMultiplier = (currentSpeed * 2) / (speed * 2);
                    footstepTimer = baseFootstepInterval / Mathf.Max(speedMultiplier, 0.1f);
                }
            }
            else
            {
                if (footstepTimer > 0f)
                {
                    footstepTimer -= Time.deltaTime;
                }
            }
        }

        rb.AddForce(transform.up * currentGravity, ForceMode.Force);
    }

    private void HandleLook()
    {
        transform.Rotate(0f, lookInput.x * lookSensitivity, 0f, Space.World);
        pitch -= lookInput.y * currentLookSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Camera.main.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        lookInput = Vector2.zero;
    }

    private void HandleJump()
    {
        if (isJumping) return;
        isJumping = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
        StartCoroutine(JumpTimer());
        AudioManager.Instance.Play2DSound(SoundType.Jump);
    }

    //---------------------------------- InputActions ----------------------------------
    private void OnAttack()
    {
        if (burstShot)
        {
            burstShotCounter++;
            if (burstShotCounter >= burstShotAmount)
            {
                StartCoroutine(BurstShotStart(burstShotCounter));
                burstShotCounter = 0;
            }
            else
            {
                AudioManager.Instance.Play2DSound(SoundType.DryFire);
            }
            return;
        }

        ShootBullet();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        HandleJump();
    }

    private IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;
        currentHealth -= damage;
        Debug.Log("player took damage");
        AudioManager.Instance.Play2DSound(SoundType.PlayerDamageTaken);
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
            AudioManager.Instance.Play2DSound(SoundType.GameOver);
        }
    }


    private IEnumerator BurstShotStart(int fiveShotCount)
    {
        for (int i = 0; i < fiveShotCount; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void InitializePlayer()
    {
        currentHealth = health;
        currentBulletDamage = bulletDamage;
        currentJumpForce = jumpForce;
        currentSpeed = speed;
        currentLookSensitivity = lookSensitivity;
        currentBulletSpeed = bulletSpeed;
        currentBulletLifeTime = bulletLifeTime;
        currentMass = mass;
        rb.mass = currentMass;
        currentGravity = gravity;
        currentBulletSize = bulletSize;
        startPos = transform.position;

        //TODO: Hier resets für die FuckeryAbilities
        burstShot = false;
        burstShotAmount = 0;
        burstShotCounter = 0;
        minimap = false;
        ricochet = false;
        ricochetAmount = 0;
        bulletSizeChanged = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHUD();
        }
    }

    public void UpgradePlayerSpeed(int amount)
    {
        currentSpeed += amount;
    }

    public void UpgradePlayerLookSensitivity(float amount)
    {
        currentLookSensitivity += amount;
    }

    public void UpgradeBulletSpeed(int amount)
    {
        currentBulletSpeed += amount;
    }

    public void UpgradeBulletDamage(int amount)
    {
        currentBulletDamage += amount;
    }
    public void UpgradeBulletLifeTime(float amount)
    {
        currentBulletLifeTime += amount;
    }

    public void UpgradeBulletSize(float amount)
    {
        currentBulletSize += amount;
        // float firePointParentPlus = amount / 2;
        firePointParent.localPosition = new Vector3(firePointParent.localPosition.x + amount * 2f, firePointParent.localPosition.y, firePointParent.localPosition.z);
    }

    public void UpgradeHealth(int amount)
    {
        currentHealth += amount;
    }

    public void UpgradeJump(int amount)
    {
        currentJumpForce += amount;
    }
    public void UpgradeGravity(float amount)
    {
        currentGravity -= amount;
    }

    public void MinimapUpgrade()
    {
        minimap = true;
        GameManager.Instance.ShowMinimap();
    }

    public void BurstShotUpgrade()
    {
        if (!burstShot) burstShot = true;
        burstShotAmount += 5;
    }

    public void RicochetUpgrade()
    {
        if (!ricochet) ricochet = true;

        ricochetAmount += 2;
    }

    public void SpreadShotUpgrade()
    {
        if (!spreadShot) spreadShot = true;
    }

    public void BulletSizeChangedUpgrade()
    {
        if (!bulletSizeChanged) bulletSizeChanged = true;
    }

    private void ShootBullet()
    {
        if (spreadShot)
        {
            ShootSpreadBullet();
            return;
        }
        Quaternion bulletRotation = Quaternion.Euler(bulletPrefab.transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        InitializeBullet(bullet);

        AudioManager.Instance.Play2DSound(SoundType.GunShot);

        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * currentBulletSpeed;
    }

    private void ShootSpreadBullet()
    {

        Quaternion bulletRotation = Quaternion.Euler(bulletPrefab.transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        GameObject leftBullet = Instantiate(bulletPrefab, firePointLeft.position, bulletRotation);
        GameObject rightBullet = Instantiate(bulletPrefab, firePointRight.position, bulletRotation);

        InitializeBullet(leftBullet);
        InitializeBullet(rightBullet);

        leftBullet.GetComponent<Rigidbody>().linearVelocity = firePointLeft.forward * currentBulletSpeed;
        rightBullet.GetComponent<Rigidbody>().linearVelocity = firePointRight.forward * currentBulletSpeed;
    }

    private void InitializeBullet(GameObject bullet)
    {
        if (!ricochet && !bulletSizeChanged)
        {
            bullet.GetComponent<Bullet>().Initialize(currentBulletDamage);
        }
        else if (bulletSizeChanged && !ricochet)
        {
            bullet.GetComponent<Bullet>().Initialize(currentBulletDamage, currentBulletSize);
        }
        else if (!bulletSizeChanged && ricochet)
        {
            bullet.GetComponent<Bullet>().Initialize(currentBulletDamage, true, ricochetAmount);
        }
        else
        {
            bullet.GetComponent<Bullet>().Initialize(currentBulletDamage, true, ricochetAmount, currentBulletSize);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------

    public void UpgradeChosen(Upgrade upgrade)
    {
        //TODO: Hier upgrade anwenden auf player
        if (upgrade.playerSpeedUpgrade != 0) UpgradePlayerSpeed(upgrade.playerSpeedUpgrade);
        if (upgrade.playerLookSensitivityUpgrade != 0) UpgradePlayerLookSensitivity(upgrade.playerLookSensitivityUpgrade);
        if (upgrade.bulletSpeedUpgrade != 0) UpgradeBulletSpeed(upgrade.bulletSpeedUpgrade);
        if (upgrade.bulletDamageUpgrade != 0) UpgradeBulletDamage(upgrade.bulletDamageUpgrade);
        //if (upgrade.bulletLifeTimeUpgrade != 0) UpgradeBulletLifeTime(upgrade.bulletLifeTimeUpgrade);
        if (upgrade.healthUpgrade != 0) UpgradeHealth(upgrade.healthUpgrade);
        if (upgrade.jumpUpgrade != 0) UpgradeJump(upgrade.jumpUpgrade);
        if (upgrade.gravityUpgrade != 0) UpgradeGravity(upgrade.gravityUpgrade);
        if (upgrade.bulletSizeUpgrade != 0) UpgradeBulletSize(upgrade.bulletSizeUpgrade);

        if (upgrade.minimap) MinimapUpgrade();
        if (upgrade.fiveShot) BurstShotUpgrade();
        if (upgrade.ricochet) RicochetUpgrade();
        if (upgrade.spreadShot) SpreadShotUpgrade();
        if (upgrade.bulletSize) BulletSizeChangedUpgrade();

        //ExitUpgradeMode(upgrade);
    }

}