using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    private int _damage;
    private bool _ricochet;
    private int _ricochetAmount;
    [SerializeField] private Rigidbody _rigidbody;
    public void Initialize()
    {
        _damage = 10;
        _ricochet = false;
        _ricochetAmount = 0;
        BulletParent.Instance.AddBullet(gameObject);
    }

    public void Initialize(int damage)
    {
        _damage = damage;
        _ricochet = false;
        _ricochetAmount = 0;
        BulletParent.Instance.AddBullet(gameObject);
    }

    public void Initialize(int damage, float size)
    {
        _damage = damage;
        _ricochet = false;
        _ricochetAmount = 0;
        transform.localScale = new Vector3(size, size, size);
        BulletParent.Instance.AddBullet(gameObject);
    }

    public void Initialize(int damage, bool ricochet, int ricochetAmount)
    {
        _damage = damage;
        _ricochet = ricochet;
        _ricochetAmount = ricochetAmount;
        BulletParent.Instance.AddBullet(this.gameObject);
    }

    public void Initialize(int damage, bool ricochet, int ricochetAmount, float size)
    {
        _damage = damage;
        _ricochet = ricochet;
        _ricochetAmount = ricochetAmount;
        transform.localScale = new Vector3(size, size, size);
        BulletParent.Instance.AddBullet(this.gameObject);
    }
    private void FixedUpdate()
    {
        OutOfBoundsCheck();
        if (_rigidbody.linearVelocity.sqrMagnitude < 0.0001f) return;

        Vector3 direction = _rigidbody.linearVelocity.normalized;
        float distance = _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;

        if (!_ricochet) return;

        if (!Physics.Raycast(_rigidbody.position, direction, out RaycastHit hit, distance,
                LayerMask.GetMask("Ground"))) return;

        if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<IDamageable>()?.TakeDamage(_damage);
            DestroyBullet();
            return;
        }

        Ricochet(hit.normal, hit.point);
    }


    

    private IEnumerator SelfDestroy()
    {
        //yield return new WaitForSeconds(_lifeTime);
        yield return null;
        DestroyBullet();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
        }
        if(!other.gameObject.CompareTag("Bullet") && !_ricochet) DestroyBullet();

        // if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Enemy"))
        // {
        //     Ricochet(other);
        // }
    }

    // private void Ricochet(Collision other)
    // {
    //     if (_ricochetAmount < 1) DestroyBullet();

    //     ContactPoint contact = other.contacts[0];
    //     Vector3 normal = contact.normal;

    //     Vector3 reflectedDirection = Vector3.Reflect(_rigidbody.linearVelocity.normalized, normal);

    //     _rigidbody.linearVelocity = reflectedDirection * _rigidbody.linearVelocity.magnitude;

    //     _ricochetAmount--;
    // }

    private void Ricochet(Vector3 normal, Vector3 point)
    {
        if (_ricochetAmount < 1)
        {
            DestroyBullet();
            return;
        }

        Vector3 reflectedDirection = Vector3.Reflect(_rigidbody.linearVelocity.normalized, normal);
        float speed = _rigidbody.linearVelocity.magnitude;
        _rigidbody.position = point + normal * 0.01f;
        _rigidbody.linearVelocity = reflectedDirection * speed;
        _ricochetAmount--;
        AudioManager.Instance.Play3DSound(SoundType.Ricochete, transform.position);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OutOfBoundsCheck()
    {
        if (transform.position.x < -130 || transform.position.x > 130 || transform.position.z < -130 ||
            transform.position.z > 130 || transform.position.y < -100 || transform.position.y > 150) DestroyBullet();
    }
}