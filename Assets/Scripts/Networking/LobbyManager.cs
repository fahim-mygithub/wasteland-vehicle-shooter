using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace WastelandVehicleShooter.Networking
{
    /// <summary>
    /// Manages lobby functionality including host/join operations with Unity Relay Service
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private InputField joinCodeInput;
        [SerializeField] private Text statusText;
        [SerializeField] private Text joinCodeDisplay;
        
        [Header("Connection Settings")]
        [SerializeField] private int maxConnections = 7; // 7 clients + 1 host = 8 total players
        
        // Connection state tracking
        private bool isConnecting = false;
        private string currentJoinCode = "";
        
        // Events
        public static event Action<string> OnJoinCodeGenerated;
        public static event Action<bool> OnConnectionStateChanged;
        public static event Action<string> OnConnectionError;
        
        private void Start()
        {
            InitializeUI();
            InitializeNetworkCallbacks();
        }
        
        private void InitializeUI()
        {
            if (hostButton != null)
                hostButton.onClick.AddListener(OnHostButtonClicked);
                
            if (joinButton != null)
                joinButton.onClick.AddListener(OnJoinButtonClicked);
                
            UpdateUI();
        }
        
        private void InitializeNetworkCallbacks()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }
        
        private async void OnHostButtonClicked()
        {
            if (isConnecting) return;
            
            try
            {
                isConnecting = true;
                UpdateStatusText("Starting host...");
                
                string joinCode = await StartHostWithRelay(maxConnections);
                
                if (!string.IsNullOrEmpty(joinCode))
                {
                    currentJoinCode = joinCode;
                    UpdateStatusText($"Host started! Join Code: {joinCode}");
                    OnJoinCodeGenerated?.Invoke(joinCode);
                    
                    if (joinCodeDisplay != null)
                        joinCodeDisplay.text = $"Join Code: {joinCode}";
                }
                else
                {
                    UpdateStatusText("Failed to start host");
                    OnConnectionError?.Invoke("Failed to start host");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Host error: {ex.Message}");
                OnConnectionError?.Invoke(ex.Message);
                Debug.LogError($"Host error: {ex}");
            }
            finally
            {
                isConnecting = false;
                UpdateUI();
            }
        }
        
        private async void OnJoinButtonClicked()
        {
            if (isConnecting) return;
            
            string joinCode = joinCodeInput?.text?.Trim();
            if (string.IsNullOrEmpty(joinCode))
            {
                UpdateStatusText("Please enter a join code");
                return;
            }
            
            try
            {
                isConnecting = true;
                UpdateStatusText("Joining game...");
                
                bool success = await StartClientWithRelay(joinCode);
                
                if (success)
                {
                    UpdateStatusText("Connected to game!");
                }
                else
                {
                    UpdateStatusText("Failed to join game");
                    OnConnectionError?.Invoke("Failed to join game");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Join error: {ex.Message}");
                OnConnectionError?.Invoke(ex.Message);
                Debug.LogError($"Join error: {ex}");
            }
            finally
            {
                isConnecting = false;
                UpdateUI();
            }
        }
        
        /// <summary>
        /// Creates a relay server allocation and starts a host
        /// </summary>
        /// <param name="maxConnections">The maximum amount of clients that can connect to the relay</param>
        /// <returns>The join code for clients to use</returns>
        public async Task<string> StartHostWithRelay(int maxConnections = 7)
        {
            try
            {
                // Initialize Unity Services
                await UnityServices.InitializeAsync();
                
                // Authenticate user
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                
                // Request allocation and join code
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                // Configure transport with relay data
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));
                
                // Start host
                bool hostStarted = NetworkManager.Singleton.StartHost();
                
                return hostStarted ? joinCode : null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"StartHostWithRelay failed: {ex}");
                throw;
            }
        }
        
        /// <summary>
        /// Joins a relay server based on the join code received from the host
        /// </summary>
        /// <param name="joinCode">The join code generated on the host</param>
        /// <returns>True if the connection was successful</returns>
        public async Task<bool> StartClientWithRelay(string joinCode)
        {
            try
            {
                // Initialize Unity Services
                await UnityServices.InitializeAsync();
                
                // Authenticate user
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                
                // Join allocation
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
                
                // Configure transport with relay data
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                
                // Start client
                return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Debug.LogError($"StartClientWithRelay failed: {ex}");
                throw;
            }
        }
        
        /// <summary>
        /// Disconnects from the current session
        /// </summary>
        public void Disconnect()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.Shutdown();
                    UpdateStatusText("Host stopped");
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.Shutdown();
                    UpdateStatusText("Disconnected from game");
                }
            }
            
            currentJoinCode = "";
            if (joinCodeDisplay != null)
                joinCodeDisplay.text = "";
                
            UpdateUI();
        }
        
        // Network event handlers
        private void OnServerStarted()
        {
            Debug.Log("Server started successfully");
            OnConnectionStateChanged?.Invoke(true);
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} connected");
            
            if (NetworkManager.Singleton.IsHost)
            {
                UpdateStatusText($"Player joined! ({NetworkManager.Singleton.ConnectedClients.Count}/{maxConnections + 1} players)");
            }
            else if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                UpdateStatusText("Connected to game!");
                OnConnectionStateChanged?.Invoke(true);
            }
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} disconnected");
            
            if (NetworkManager.Singleton.IsHost)
            {
                UpdateStatusText($"Player left ({NetworkManager.Singleton.ConnectedClients.Count}/{maxConnections + 1} players)");
            }
            else if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                UpdateStatusText("Disconnected from game");
                OnConnectionStateChanged?.Invoke(false);
                UpdateUI();
            }
        }
        
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
                statusText.text = message;
                
            Debug.Log($"LobbyManager: {message}");
        }
        
        private void UpdateUI()
        {
            bool isConnected = NetworkManager.Singleton != null && 
                              (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient);
            
            if (hostButton != null)
                hostButton.interactable = !isConnected && !isConnecting;
                
            if (joinButton != null)
                joinButton.interactable = !isConnected && !isConnecting;
                
            if (joinCodeInput != null)
                joinCodeInput.interactable = !isConnected && !isConnecting;
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
            
            if (hostButton != null)
                hostButton.onClick.RemoveListener(OnHostButtonClicked);
                
            if (joinButton != null)
                joinButton.onClick.RemoveListener(OnJoinButtonClicked);
        }
        
        // Public properties for external access
        public bool IsConnecting => isConnecting;
        public string CurrentJoinCode => currentJoinCode;
        public int MaxConnections => maxConnections;
        public int CurrentPlayerCount => NetworkManager.Singleton?.ConnectedClients?.Count ?? 0;
    }
}