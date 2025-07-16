# Unity Netcode for GameObjects and Relay Service Research

## Key Findings

### NetworkManager Setup
- NetworkManager GameObject is the core component for multiplayer networking
- Must have NetworkManager component and UnityTransport selected as transport
- UnityTransport is the only transport that supports Unity Relay Service

### Unity Relay Service Integration
- Provides NAT traversal for multiplayer games
- Requires Unity Services initialization and user authentication
- Uses join codes for players to connect to sessions
- Supports up to 8 concurrent connections per allocation

### Core Implementation Pattern
1. Initialize Unity Services
2. Authenticate user (anonymous authentication is sufficient)
3. Create allocation (host) or join allocation (client) 
4. Configure UnityTransport with relay server data
5. Start NetworkManager as host or client

### Essential Code Components

#### Host Setup
```csharp
public async Task<string> StartHostWithRelay(int maxConnections = 5)
{
    await UnityServices.InitializeAsync();
    if (!AuthenticationService.Instance.IsSignedIn)
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
    var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    
    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
    
    return NetworkManager.Singleton.StartHost() ? joinCode : null;
}
```

#### Client Setup
```csharp
public async Task<bool> StartClientWithRelay(string joinCode)
{
    await UnityServices.InitializeAsync();
    if (!AuthenticationService.Instance.IsSignedIn)
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
    
    return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
}
```

### Required Packages
- Unity Netcode for GameObjects
- Unity Transport
- Unity Relay
- Unity Services Core
- Unity Authentication

### Best Practices
- Always initialize Unity Services before using Relay
- Use anonymous authentication for prototypes
- Configure UnityTransport with SetRelayServerData, not SetConnectionData
- Handle connection errors and timeouts gracefully
- Use "dtls" connection type for secure connections
- Maximum 8 players per relay allocation (7 clients + 1 host)

### Network Architecture
- Client-Server model with host authority
- Server-authoritative gameplay for fairness
- Client prediction for responsive movement
- Lag compensation for hit detection
- 30Hz minimum update rate for multiplayer synchronization