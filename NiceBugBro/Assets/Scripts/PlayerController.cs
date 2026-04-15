using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("PlayerStats")] private int health = 100;
    [SerializeField] private int currentHealth;
    private float speed = 5;
    [SerializeField] private float currentSpeed;
    private float jumpForce = 10;
    [SerializeField] private float currentJumpForce;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float lookSensitivity;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;

    [Header("References")] [SerializeField]
    private Transform firePoint;

    [SerializeField] private GameObject bulletPrefab;

    [Header("BulletStats")] 
    private int bulletDamage = 10;
    [SerializeField] private int currentBulletDamage;
    private float bulletSpeed = 20;
    [SerializeField] private float currentBulletSpeed;

    [Header("GamePlay Changes")]
    [SerializeField] private bool minimap;
    [SerializeField] private bool fiveShot;
    
    [SerializeField] private int fiveShotCounter;
    private bool isJumping;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InitializePlayer();
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    private void HandleMovement()
    {
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement.y = 0f;
        transform.Translate(movement.normalized * currentSpeed * Time.deltaTime, Space.World);
    }

    private void HandleLook()
    {
        transform.Rotate(0f, lookInput.x * lookSensitivity, 0f, Space.World);
        pitch -= lookInput.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Camera.main.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        lookInput = Vector2.zero;
    }

    private void HandleJump()
    {
        if (isJumping) return;
        isJumping = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
        StartCoroutine(JumpTimer());
    }

    //---------------------------------- InputActions ----------------------------------
    private void OnAttack()
    {
        if (fiveShot)
        {
            fiveShotCounter++;
            if (fiveShotCounter >= 5)
            {
                StartCoroutine(FiveShotStart(fiveShotCounter));
                fiveShotCounter = 0;
            }
            return;
        }
        Quaternion bulletRotation = Quaternion.Euler(bulletPrefab.transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * currentBulletSpeed;
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

    private void OnInteract(InputValue value)
    {
        EventManager.OnCollectedUpgrade();
        GameManager.Instance.EnterUpgradeMode();
    }

    private IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    private void OnDamaged(int damage)
    {
        if (damage < 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //TODO: Hier Game Beendung etc.
        }
    }
    
        
    private IEnumerator FiveShotStart(int fiveShotCount)
    {
        for (int i = 0; i < fiveShotCount; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(bulletPrefab.transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * currentBulletSpeed;
            yield return new WaitForSeconds(0.02f);
        }
    }


    private void InitializePlayer()
    {
        currentHealth = health;
        currentBulletDamage = bulletDamage;
        currentJumpForce = jumpForce;
        currentSpeed = speed;
        currentBulletSpeed = bulletSpeed;
        
        //TODO: Hier resets für die FuckeryAbilities
        fiveShotCounter = 0;
    }

    public void UpgradePlayerSpeed(int amount)
    {
        currentSpeed = speed;
    }
    
    public void UpgradeBulletSpeed(int amount)
    {
        currentBulletSpeed = bulletSpeed;
    }
    
    public void UpgradeBulletDamage(int amount)
    {
        currentBulletDamage += amount;
    }
    
    public void UpgradeHealth(int amount)
    {
        currentHealth += amount;
    }
    
    public void UpgradeJump(int amount)
    {
        currentJumpForce += amount;
    }

    public void MinimapUpgrade()
    {
        //TODO: Hier MinimapFuckery
    }
    
    public void FiveShotUpgrade()
    {
        //TODO: Hier FiveshotFuckery
    }
    
}