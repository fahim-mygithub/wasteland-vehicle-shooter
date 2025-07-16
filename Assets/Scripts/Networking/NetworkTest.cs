using Unity.Netcode;
using UnityEngine;

namespace WastelandVehicleShooter.Networking
{
    /// <summary>
    /// Simple network test script to validate basic networking functionality
    /// </summary>
    public class NetworkTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private KeyCode hostKey = KeyCode.H;
        [SerializeField] private KeyCode clientKey = KeyCode.C;
        [SerializeField] private KeyCode disconnectKey = KeyCode.D;
        
        private void Start()
        {
            Debug.Log("NetworkTest initialized. Press H to host, C to connect as client, D to disconnect");
            
            // Subscribe to network events
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(hostKey))
            {
                StartHost();
            }
            else if (Input.GetKeyDown(clientKey))
            {
                StartClient();
            }
            else if (Input.GetKeyDown(disconnectKey))
            {
                Disconnect();
            }
        }
        
        private void StartHost()
        {
            if (NetworkManager.Singleton != null)
            {
                Debug.Log("Starting Host...");
                bool success = NetworkManager.Singleton.StartHost();
                Debug.Log($"Host start result: {success}");
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
        
        private void StartClient()
        {
            if (NetworkManager.Singleton != null)
            {
                Debug.Log("Starting Client...");
                bool success = NetworkManager.Singleton.StartClient();
                Debug.Log($"Client start result: {success}");
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
        
        private void Disconnect()
        {
            if (NetworkManager.Singleton != null)
            {
                Debug.Log("Disconnecting...");
                NetworkManager.Singleton.Shutdown();
            }
        }
        
        // Network event handlers
        private void OnServerStarted()
        {
            Debug.Log("✓ Server started successfully!");
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"✓ Client {clientId} connected. Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"✗ Client {clientId} disconnected. Remaining clients: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            
            GUILayout.Label("Network Test Controls:");
            GUILayout.Label($"Press {hostKey} to start Host");
            GUILayout.Label($"Press {clientKey} to start Client");
            GUILayout.Label($"Press {disconnectKey} to disconnect");
            
            GUILayout.Space(10);
            
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    GUILayout.Label($"Status: HOST ({NetworkManager.Singleton.ConnectedClients.Count} clients)");
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    GUILayout.Label("Status: CLIENT");
                }
                else if (NetworkManager.Singleton.IsServer)
                {
                    GUILayout.Label($"Status: SERVER ({NetworkManager.Singleton.ConnectedClients.Count} clients)");
                }
                else
                {
                    GUILayout.Label("Status: DISCONNECTED");
                }
            }
            else
            {
                GUILayout.Label("Status: NO NETWORK MANAGER");
            }
            
            GUILayout.EndArea();
        }
        
        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }
}