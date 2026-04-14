using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 0.15f;
    //[SerializeField] private Transform cameraPivot;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [Header("BulletStats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float bulletSpeed = 20f;

 

    private void Update()
    {
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement.y = 0f;
        transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);

        transform.Rotate(0f, lookInput.x * lookSensitivity, 0f, Space.World);

        // if (cameraPivot != null)
        // {
        //     pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, minPitch, maxPitch);
        //     cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        // }

        lookInput = Vector2.zero;
    }

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
}
