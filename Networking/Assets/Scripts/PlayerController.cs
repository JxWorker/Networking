using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    
    [SerializeField] private GameObject healthBar;
    private NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [ServerRpc]
    private void SpawnBallServerRPC()
    {
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        ball.GetComponent<NetworkObject>().Spawn(true);
    }
    
    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (previous, current) =>
        {
            healthBar.transform.localScale = new Vector3(health.Value / 100f, healthBar.transform.localScale.y,
                healthBar.transform.localScale.z);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float xDir = Input.GetAxis("Horizontal");
            float zDir = Input.GetAxis("Vertical");
            float speed = 4f;

            transform.position += speed * Time.deltaTime * new Vector3(xDir, 0, zDir);

            if (Input.GetKeyDown(KeyCode.H))
            {
                health.Value -= 5;
                if (health.Value < 0)
                {
                    health.Value = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBallServerRPC();
            }
        }
    }
}