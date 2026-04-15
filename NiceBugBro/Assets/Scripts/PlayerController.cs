using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerStats")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float jumpForce = 100f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("BulletStats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float bulletSpeed = 20f;

    private bool isJumping;

    private void Start()
    {
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
        Debug.Log("Collected Upgrade");
        EventManager.OnCollectedUpgrade();
        GameManager.Instance.EnterUpgradeMode();
    }

    private IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }
}
