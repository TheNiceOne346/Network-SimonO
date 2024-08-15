using System;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float _bulletSpeed;
    private float _direction;

    public void InitializeMovement(float direction, float bulletSpeed)
    {
        _direction = direction;
        _bulletSpeed = bulletSpeed;
    }

    private void Update()
    {
        if (IsClient)
        {
            transform.position += new Vector3(_direction, 0, 0) * _bulletSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<HealthSystem>().TakeDamage(20); 
            }

            
            NetworkObject.Despawn();
        }
    }
}


