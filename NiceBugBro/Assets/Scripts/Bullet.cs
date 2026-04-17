using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    private int _damage;
    private float _lifeTime;
    private bool _ricochet;
    private int _ricochetAmount;
    [SerializeField] private Rigidbody _rigidbody;

    private void FixedUpdate()
    {
        if (_rigidbody.linearVelocity.sqrMagnitude < 0.0001f) return;

        Vector3 direction = _rigidbody.linearVelocity.normalized;
        float distance = _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;

        if (!Physics.Raycast(_rigidbody.position, direction, out RaycastHit hit, distance, LayerMask.GetMask("Ground"))) return;

        if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<IDamageable>()?.TakeDamage(_damage);
            DestroyBullet();
            return;
        }

        Ricochet(hit.normal, hit.point);
    }


    public void Initialize()
    {
        _damage = 10;
        _lifeTime = 3f;
        _ricochet = false;
        _ricochetAmount = 0;
        StartCoroutine(SelfDestroy());
    }

    public void Initialize(int damage)
    {
        _damage = damage;
        _lifeTime = 3f;
        _ricochet = false;
        _ricochetAmount = 0;
        StartCoroutine(SelfDestroy());
    }

    public void Initialize(int damage, float lifeTime, bool ricochet, int ricochetAmount)
    {
        _damage = damage;
        _lifeTime = lifeTime;
        _ricochet = ricochet;
        _ricochetAmount = ricochetAmount;
        StartCoroutine(SelfDestroy());
    }

    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(_lifeTime);
        DestroyBullet();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
            DestroyBullet();
            return;
        }
        
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
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}