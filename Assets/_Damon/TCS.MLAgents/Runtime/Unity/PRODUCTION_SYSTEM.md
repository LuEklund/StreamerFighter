# Architecture Overview

This system has been redesigned using **composition over inheritance** principles, with clear separation of concerns and no unnecessary abstractions.

## Core Components

### 1. Movement Component (`Movement.cs`)
- Handles all entity movement logic
- Provides clean interface for physics-based movement
- Configurable speed and constraints

### 2. BoundarySystem Component (`BoundarySystem.cs`)
- Manages arena boundaries
- Supports bouncing, clamping, and teleportation
- Provides utilities for boundary checks and random positioning

### 3. RewardSystem Component (`RewardSystem.cs`)
- Encapsulates all reward/penalty logic
- Configurable reward values
- Clean interface for different reward types

### 4. SimulationConfig ScriptableObject (`SimulationConfig.cs`)
- Centralized configuration for all simulation parameters
- Arena settings, speeds, rewards, episode settings
- Easy to modify without touching code

### 5. PredatorController (`PredatorController.cs`)
- **Uses composition instead of direct Agent inheritance**
- Composes Movement, BoundarySystem, and RewardSystem components
- Clean ML Agent integration
- Configuration-driven behavior

### 6. PreyController (`PreyController.cs`)
- Simple prey behavior using composition
- No inheritance from complex base classes
- Movement and boundary system integration

### 7. SimulationManager (`SimulationManager.cs`)
- Orchestrates the entire simulation
- Manages multiple predators and prey
- Statistics tracking and simulation control

### 8. BehaviorSetup (`BehaviorSetup.cs`)
- Enhanced behavior parameter configuration
- Support for custom brains
- Flexible action space configuration

## Key Improvements

### ✅ Composition Over Inheritance
- No complex inheritance hierarchies
- Components are reusable and testable
- Clear single responsibilities

### ✅ Configuration-Driven
- All parameters centralized in SimulationConfig
- Easy to tweak without code changes
- Runtime configuration support

### ✅ Separation of Concerns
- Movement logic separated from ML logic
- Reward system is standalone
- Boundary management is independent

### ✅ Production Features
- Proper null checks and error handling
- Statistics tracking
- Simulation management capabilities
- Configurable and extensible

### ✅ Simple and Clean
- No unnecessary abstractions
- Direct and readable code
- Easy to understand and maintain

## Usage

1. Create a `SimulationConfig` asset via Create menu
2. Set up GameObjects with:
   - `PredatorController` + required components
   - `PreyController` + required components
   - `SimulationManager` to orchestrate
3. Assign the config to all controllers
4. Configure ML-Agents behavior parameters

## Migration from Old System

The old `PredatorAgent` and `Prey` classes can be replaced with the new component-based system without losing functionality. The new system is more flexible and maintainable.