using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Windows;

public class Player : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;

    NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();
    NetworkVariable<Vector3> playerScale = new NetworkVariable<Vector3>();

    [SerializeField] float Speed;
    [SerializeField] GameObject bulletPrefab; 
    [SerializeField] Transform bulletSpawnPoint; 
    [SerializeField] float bulletSpeed = 10f; 

    public Transform spawnPosition;
    public GameObject playerPrefab;

    private void Start()
    {
        if (IsServer)
        {
            playerScale.OnValueChanged += OnPlayerScaleChanged;
        }

        if (inputReader != null && IsLocalPlayer)
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += OnShoot; 
        }
    }

    private void OnPlayerScaleChanged(Vector3 previousValue, Vector3 newValue)
    {
        transform.localScale = newValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndLine"))
        {
            Debug.Log("You WIIIINN");
        }
    }

    private void OnMove(Vector2 input)
    {
       
        MoveServerRpc(input);

      
    }
   

    private void Update()
    {
        if (IsServer)
        {
            transform.position += (Vector3)moveInput.Value * Speed * Time.deltaTime;
            UpdateFlip();
        }
    }


   
    private void UpdateFlip()
    {
        Vector2 input = (Vector3)moveInput.Value;

       
        if (input.x > 0)
        {
            
            playerScale.Value = new Vector3(0.934f, 0.47f, 1);
        }
        else if (input.x < 0)
        {
           
            playerScale.Value = new Vector3(-0.934f, 0.47f, 1);
        }
    }




    private void OnShoot()
    {
        ShootServerRpc();
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 data)
    {
        moveInput.Value = data;
    }



    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        NetworkObject networkObJ = bullet.GetComponent<NetworkObject>();
        networkObJ.Spawn();

        
        float direction;

        if (transform.localScale.x > 0)
        {
           
            direction = 1;
        }
        else
        {
           
            direction = -1;
        }

        // Notify all clients to move the bullet
        MoveBulletClientRpc(bullet.GetComponent<NetworkObject>().NetworkObjectId, direction);
    }

    [ClientRpc]
    private void MoveBulletClientRpc(ulong bulletNetworkObjectId, float direction)
    {
        NetworkObject bulletNetworkObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[bulletNetworkObjectId];
        Bullet bullet = bulletNetworkObj.GetComponent<Bullet>();
        bullet.InitializeMovement(direction, bulletSpeed);
    }
}









