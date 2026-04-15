using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("PlayerStats")] 
    [SerializeField] private float speed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float jumpForce;
    [SerializeField] private float currentJumpForce;
    [SerializeField] private int health;
    [SerializeField] private int currentHealth;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;

    [Header("References")] [SerializeField]
    private Transform firePoint;

    [SerializeField] private GameObject bulletPrefab;

    [Header("BulletStats")] 
    [SerializeField] private int bulletDamage;

    [SerializeField] private int currentBulletDamage;
    [SerializeField] private float bulletSpeed;

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
        transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);
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
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(JumpTimer());
    }

    //---------------------------------- InputActions ----------------------------------
    private void OnAttack()
    {
        Quaternion bulletRotation = Quaternion.Euler(bulletPrefab.transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * bulletSpeed;
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
            //Hier Game Beendung etc.
        }
    }
}