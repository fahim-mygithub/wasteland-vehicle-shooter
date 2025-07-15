# Wasteland Vehicle Extraction Shooter MVP

A Unity 6 multiplayer vehicle extraction shooter prototype designed to establish and validate core technical systems for future development. This MVP focuses on proving multiplayer networking, vehicle physics, combat synchronization, inventory management, and map traversal using Unity primitives.

## 🎯 Project Overview

This project serves as a technical foundation prototype for a Mad Max-style extraction shooter, emphasizing:

- **Multiplayer Foundation**: Stable 4-8 player sessions with <100ms latency
- **Vehicle Physics**: Realistic vehicle handling with multiplayer support
- **Combat System**: Server-authoritative hit detection with lag compensation
- **Inventory Management**: Grid-based inventory with network synchronization
- **Performance**: 60+ FPS with 8 concurrent players

## 🛠️ System Requirements

### Development Environment
- **Unity Version**: Unity 6 (2023.3 LTS or later)
- **Platform**: Windows 10/11, macOS 10.15+, or Ubuntu 18.04+
- **RAM**: 8GB minimum, 16GB recommended
- **Storage**: 10GB free space
- **Network**: Stable internet connection for multiplayer testing

### Target Runtime
- **Platform**: PC (Windows/Mac/Linux)
- **CPU**: Quad-core 2.5GHz or equivalent
- **RAM**: 4GB minimum, 8GB recommended
- **GPU**: DirectX 11 compatible
- **Network**: Broadband internet connection

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/fahim-mygithub/wasteland-vehicle-shooter.git
cd wasteland-vehicle-shooter
```

### 2. Unity Setup
1. Open Unity Hub
2. Click "Add" and select the project folder
3. Ensure Unity 6 (2023.3 LTS+) is installed
4. Open the project in Unity

### 3. Package Installation
The project uses Unity Package Manager for dependencies:
- **Unity Netcode for GameObjects**: Multiplayer networking
- **Unity Transport**: Low-level networking transport
- **Unity Relay**: NAT traversal service

Packages will be automatically installed when opening the project.

### 4. Initial Setup
1. Open the `MainScene` in `Assets/Scenes/`
2. Press Play to test basic functionality
3. For multiplayer testing, build the project and run multiple instances

## 📁 Project Structure

```
Assets/
├── Scenes/          # Game scenes
├── Scripts/         # C# scripts
├── Prefabs/         # Reusable game objects
├── Materials/       # Primitive materials
├── Audio/           # Sound effects and music
└── UI/              # User interface elements

ProjectSettings/     # Unity project configuration
Packages/           # Package dependencies (auto-generated)
```

## 🏗️ Architecture Overview

### Network Architecture
- **Client-Server Model**: Host acts as authoritative server
- **Unity Netcode for GameObjects**: High-level networking framework
- **Unity Relay Service**: NAT traversal and connection management
- **Server Authority**: Critical game state managed server-side

### Core Systems
- **Player System**: Movement, health, respawn mechanics
- **Vehicle System**: Physics-based driving with multiplayer support
- **Combat System**: Raycast hit detection with lag compensation
- **Inventory System**: Grid-based item management
- **Map System**: Terrain generation and object placement

## 🧪 Development Workflow

### Branching Strategy
- `main`: Stable, tested code
- `develop`: Integration branch for features
- `feature/*`: Individual feature development
- `hotfix/*`: Critical bug fixes

### Testing Procedures
1. **Unit Tests**: Core system functionality
2. **Integration Tests**: System interaction validation
3. **Multiplayer Tests**: Network synchronization verification
4. **Performance Tests**: Frame rate and latency validation

### Code Standards
- Follow Unity C# coding conventions
- Use meaningful variable and method names
- Include XML documentation for public APIs
- Implement proper error handling and logging

## 🎮 Gameplay Features

### Current Implementation
- ✅ Basic multiplayer connectivity (4-8 players)
- ✅ Player movement and controls (WASD + mouse)
- ✅ Simple test environment (100m x 100m terrain)
- ✅ Primitive-based prototyping approach

### Planned Features
- 🔄 Vehicle system (Buggy, Truck, Tank)
- 🔄 Combat system (Rifle, Shotgun)
- 🔄 Inventory management (4x4 grid)
- 🔄 Full map environment (1km x 1km)
- 🔄 Performance optimization

## 🐛 Known Issues

- Initial setup requires manual package installation
- Multiplayer testing requires building separate instances
- Performance optimization pending for 8-player scenarios

## 📊 Performance Targets

- **Frame Rate**: 60+ FPS with 8 players
- **Network Latency**: <100ms average
- **Physics Update**: 50Hz minimum
- **Network Update**: 30Hz for standard systems, 60Hz for critical

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Test all changes with multiple clients
- Maintain 60+ FPS performance target
- Follow established coding standards
- Update documentation for new features

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Resources

- [Unity Netcode for GameObjects Documentation](https://docs-multiplayer.unity3d.com/)
- [Unity 6 Documentation](https://docs.unity3d.com/)
- [Project Specifications](.kiro/specs/wasteland-vehicle-shooter/)

## 📞 Support

For questions, issues, or contributions:
- Create an issue in this repository
- Check the project specifications in `.kiro/specs/wasteland-vehicle-shooter/`
- Review the development workflow documentation

---

**Status**: Active Development | **Version**: MVP Prototype | **Unity**: 6.0+