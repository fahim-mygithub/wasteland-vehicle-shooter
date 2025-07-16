using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace WastelandVehicleShooter.Networking
{
    /// <summary>
    /// Simple connection handler for testing network functionality
    /// </summary>
    public class ConnectionHandler : MonoBehaviour
    {
        [Header("UI References")]
        public Button hostButton;
        public Button clientButton;
        public Button serverButton;
        public Text statusText;
        
        private void Start()
        {
            // Create simple UI if not assigned
            if (hostButton == null || clientButton == null || serverButton == null)
            {
                CreateSimpleUI();
            }
            
            SetupButtonListeners();
            UpdateStatusText("Ready to connect");
        }
        
        private void CreateSimpleUI()
        {
            // This will be handled by the LobbyManager script
            // For now, we'll just log that the connection handler is ready
            Debug.Log("ConnectionHandler ready - UI will be created by LobbyManager");
        }
        
        private void SetupButtonListeners()
        {
            if (hostButton != null)
                hostButton.onClick.AddListener(StartHost);
                
            if (clientButton != null)
                clientButton.onClick.AddListener(StartClient);
                
            if (serverButton != null)
                serverButton.onClick.AddListener(StartServer);
        }
        
        public void StartHost()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartHost();
                UpdateStatusText("Starting as Host...");
                Debug.Log("Starting as Host");
            }
        }
        
        public void StartClient()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartClient();
                UpdateStatusText("Starting as Client...");
                Debug.Log("Starting as Client");
            }
        }
        
        public void StartServer()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartServer();
                UpdateStatusText("Starting as Server...");
                Debug.Log("Starting as Server");
            }
        }
        
        public void Shutdown()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UpdateStatusText("Disconnected");
                Debug.Log("Network shutdown");
            }
        }
        
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
                statusText.text = message;
                
            Debug.Log($"ConnectionHandler: {message}");
        }
        
        private void Update()
        {
            // Simple status updates
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                    UpdateStatusText($"Host - {NetworkManager.Singleton.ConnectedClients.Count} clients");
                else if (NetworkManager.Singleton.IsClient)
                    UpdateStatusText("Connected as Client");
                else if (NetworkManager.Singleton.IsServer)
                    UpdateStatusText($"Server - {NetworkManager.Singleton.ConnectedClients.Count} clients");
            }
        }
        
        private void OnDestroy()
        {
            if (hostButton != null)
                hostButton.onClick.RemoveListener(StartHost);
                
            if (clientButton != null)
                clientButton.onClick.RemoveListener(StartClient);
                
            if (serverButton != null)
                serverButton.onClick.RemoveListener(StartServer);
        }
    }
}