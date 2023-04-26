using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CustomNetworkManager : NetworkManager
{
    // Reference to your player prefab that has a NetworkObject component
    public GameObject playerPrefab;

    // List of spawn points in your scene

    [SerializeField] 
    private List<Transform> spawnPoints;

    // Index to keep track of the next spawn point
    private int nextSpawnPointIndex = 0;

    private void Start()
    {
        // Register the OnClientConnectedCallback
        OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        // Unregister the OnClientConnectedCallback
        OnClientConnectedCallback -= OnClientConnected;
    }

    // Called when a new client has connected
    private void OnClientConnected(ulong clientId)
    {
        // If the connected client is the server, spawn the player object
        if (IsServer)
        {
            SpawnPlayer(clientId);
        }
    }

    // Method to spawn the player
    private void SpawnPlayer(ulong clientId)
    {
        // Get the next spawn point
        Transform spawnPoint = spawnPoints[nextSpawnPointIndex];

        // Increment the spawn point index for the next player
        nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Count;

        // Instantiate the player prefab at the spawn point
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the NetworkObject component from the instantiated player
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        // Spawn the player object on the network and set ownership to the connected client
        networkObject.SpawnWithOwnership(clientId);
    }
}