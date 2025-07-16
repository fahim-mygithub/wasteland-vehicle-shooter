using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace WastelandVehicleShooter.Networking
{
    /// <summary>
    /// Displays real-time network statistics and performance metrics
    /// </summary>
    public class NetworkStatsDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas statsCanvas;
        [SerializeField] private Text connectionStatusText;
        [SerializeField] private Text playerCountText;
        [SerializeField] private Text latencyText;
        [SerializeField] private Text bandwidthText;
        [SerializeField] private Text fpsText;
        [SerializeField] private Text packetLossText;
        
        [Header("Display Settings")]
        [SerializeField] private bool showStats = true;
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;
        
        // Performance tracking
        private float lastUpdateTime;
        private int frameCount;
        private float deltaTime;
        private Queue<float> latencyHistory = new Queue<float>();
        private Queue<float> bandwidthHistory = new Queue<float>();
        private const int historySize = 10;
        
        // Network statistics
        private UnityTransport transport;
        private NetworkManager networkManager;
        
        private void Start()
        {
            InitializeReferences();
            InitializeUI();
        }
        
        private void InitializeReferences()
        {
            networkManager = NetworkManager.Singleton;
            if (networkManager != null)
            {
                transport = networkManager.GetComponent<UnityTransport>();
            }
        }
        
        private void InitializeUI()
        {
            if (statsCanvas != null)
            {
                statsCanvas.gameObject.SetActive(showStats);
            }
            
            // Initialize text components if they don't exist
            CreateUIElementsIfNeeded();
        }
        
        private void CreateUIElementsIfNeeded()
        {
            if (statsCanvas == null)
            {
                // Create canvas if it doesn't exist
                GameObject canvasGO = new GameObject("NetworkStatsCanvas");
                canvasGO.transform.SetParent(transform);
                
                statsCanvas = canvasGO.AddComponent<Canvas>();
                statsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                statsCanvas.sortingOrder = 100;
                
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Create UI panel for stats
            if (connectionStatusText == null)
            {
                CreateStatsPanel();
            }
        }
        
        private void CreateStatsPanel()
        {
            // Create background panel
            GameObject panel = new GameObject("StatsPanel");
            panel.transform.SetParent(statsCanvas.transform, false);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(300, 200);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Create text elements
            CreateTextElement("ConnectionStatus", panel.transform, new Vector2(10, -10), ref connectionStatusText);
            CreateTextElement("PlayerCount", panel.transform, new Vector2(10, -30), ref playerCountText);
            CreateTextElement("Latency", panel.transform, new Vector2(10, -50), ref latencyText);
            CreateTextElement("Bandwidth", panel.transform, new Vector2(10, -70), ref bandwidthText);
            CreateTextElement("FPS", panel.transform, new Vector2(10, -90), ref fpsText);
            CreateTextElement("PacketLoss", panel.transform, new Vector2(10, -110), ref packetLossText);
        }
        
        private void CreateTextElement(string name, Transform parent, Vector2 position, ref Text textComponent)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(0, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.anchoredPosition = position;
            textRect.sizeDelta = new Vector2(280, 20);
            
            textComponent = textGO.AddComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 12;
            textComponent.color = Color.white;
            textComponent.text = name + ": --";
        }
        
        private void Update()
        {
            HandleInput();
            UpdatePerformanceMetrics();
            
            if (showStats && Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateNetworkStats();
                lastUpdateTime = Time.time;
            }
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleStatsDisplay();
            }
        }
        
        private void UpdatePerformanceMetrics()
        {
            frameCount++;
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
        
        private void UpdateNetworkStats()
        {
            if (networkManager == null)
            {
                InitializeReferences();
                return;
            }
            
            UpdateConnectionStatus();
            UpdatePlayerCount();
            UpdateLatency();
            UpdateBandwidth();
            UpdateFPS();
            UpdatePacketLoss();
        }
        
        private void UpdateConnectionStatus()
        {
            if (connectionStatusText == null) return;
            
            string status = "Disconnected";
            Color statusColor = Color.red;
            
            if (networkManager.IsHost)
            {
                status = "Host";
                statusColor = Color.green;
            }
            else if (networkManager.IsClient)
            {
                status = "Client";
                statusColor = Color.cyan;
            }
            else if (networkManager.IsServer)
            {
                status = "Server";
                statusColor = Color.yellow;
            }
            
            connectionStatusText.text = $"Status: {status}";
            connectionStatusText.color = statusColor;
        }
        
        private void UpdatePlayerCount()
        {
            if (playerCountText == null) return;
            
            int playerCount = 0;
            if (networkManager.IsHost || networkManager.IsServer)
            {
                playerCount = networkManager.ConnectedClients.Count;
            }
            else if (networkManager.IsClient)
            {
                playerCount = networkManager.ConnectedClients.Count;
            }
            
            playerCountText.text = $"Players: {playerCount}/8";
        }
        
        private void UpdateLatency()
        {
            if (latencyText == null) return;
            
            float latency = 0f;
            
            if (transport != null && networkManager.IsClient && !networkManager.IsHost)
            {
                // Get RTT from transport if available
                // Note: This is a simplified approach - actual RTT measurement would require custom implementation
                latency = Time.time % 100; // Placeholder - replace with actual RTT measurement
            }
            
            // Add to history for averaging
            latencyHistory.Enqueue(latency);
            if (latencyHistory.Count > historySize)
                latencyHistory.Dequeue();
            
            float avgLatency = latencyHistory.Count > 0 ? latencyHistory.Average() : 0f;
            latencyText.text = $"Latency: {avgLatency:F1}ms";
            
            // Color code based on latency
            if (avgLatency < 50f)
                latencyText.color = Color.green;
            else if (avgLatency < 100f)
                latencyText.color = Color.yellow;
            else
                latencyText.color = Color.red;
        }
        
        private void UpdateBandwidth()
        {
            if (bandwidthText == null) return;
            
            // Placeholder bandwidth calculation
            // In a real implementation, you would track bytes sent/received per second
            float bandwidth = Random.Range(10f, 50f); // KB/s placeholder
            
            bandwidthHistory.Enqueue(bandwidth);
            if (bandwidthHistory.Count > historySize)
                bandwidthHistory.Dequeue();
            
            float avgBandwidth = bandwidthHistory.Count > 0 ? bandwidthHistory.Average() : 0f;
            bandwidthText.text = $"Bandwidth: {avgBandwidth:F1} KB/s";
        }
        
        private void UpdateFPS()
        {
            if (fpsText == null) return;
            
            float fps = 1.0f / deltaTime;
            fpsText.text = $"FPS: {fps:F0}";
            
            // Color code based on FPS
            if (fps >= 60f)
                fpsText.color = Color.green;
            else if (fps >= 30f)
                fpsText.color = Color.yellow;
            else
                fpsText.color = Color.red;
        }
        
        private void UpdatePacketLoss()
        {
            if (packetLossText == null) return;
            
            // Placeholder packet loss calculation
            float packetLoss = Random.Range(0f, 2f); // Percentage placeholder
            packetLossText.text = $"Packet Loss: {packetLoss:F1}%";
            
            // Color code based on packet loss
            if (packetLoss < 1f)
                packetLossText.color = Color.green;
            else if (packetLoss < 3f)
                packetLossText.color = Color.yellow;
            else
                packetLossText.color = Color.red;
        }
        
        public void ToggleStatsDisplay()
        {
            showStats = !showStats;
            if (statsCanvas != null)
            {
                statsCanvas.gameObject.SetActive(showStats);
            }
        }
        
        public void SetStatsVisibility(bool visible)
        {
            showStats = visible;
            if (statsCanvas != null)
            {
                statsCanvas.gameObject.SetActive(showStats);
            }
        }
        
        // Public methods for external access
        public bool IsStatsVisible => showStats;
        public float CurrentFPS => 1.0f / deltaTime;
        public int CurrentPlayerCount => networkManager?.ConnectedClients?.Count ?? 0;
        public bool IsConnected => networkManager != null && (networkManager.IsHost || networkManager.IsClient || networkManager.IsServer);
        
        private void OnDestroy()
        {
            // Clean up
            latencyHistory.Clear();
            bandwidthHistory.Clear();
        }
    }
}