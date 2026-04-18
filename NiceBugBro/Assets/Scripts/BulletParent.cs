using System;
using UnityEngine;

public class BulletParent : MonoBehaviour
{
    public static BulletParent Instance { get; private set; }
    private void Awake()
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
    }

    public void AddBullet(GameObject bullet)
    {
        bullet.transform.SetParent(transform);
    }
}
