
# Unity ML-Agents Predator vs Prey Sample (2D)

This project demonstrates a simple predator vs prey scenario using Unity ML-Agents 2.x. A predator agent learns to chase a prey in a flat 2D arena. The sample includes Unity C# scripts for the predator and prey, a custom side channel for logging, Python scripts for interacting with the environment, and a sample training configuration.

## Contents

- `Unity/PredatorAgent.cs`: Implements the predator agent. It observes the relative position and velocities of itself and the prey, receives rewards for catching the prey quickly, and penalties for leaving the arena.
- `Unity/Prey.cs`: Defines simple prey behaviour. The prey moves randomly within the arena bounds and respawns on capture.
- `Unity/StringLogSideChannel.cs`: Custom side channel used to send log messages between Unity and Python.
- `Unity/BehaviorSetup.cs`: Sets up `BehaviorParameters` for the predator agent, including the behaviour name and continuous 2D action space.
- `Python/custom_side_channel.py`: Python counterpart of the side channel.
- `Python/train_predator.py`: Sample Python script to connect to the Unity environment and run random episodes.
- `Python/config.yaml`: Sample PPO training configuration for `mlagents-learn`.

## Setting up the Scene

1. **Create a new Unity project** and import the ML-Agents package (v2.x). 
2. **Add a plane** or other ground object scaled to approximately ±5 units to represent the arena.
3. **Add a GameObject** to represent the predator (e.g., a sphere). Attach a `Rigidbody` component, `PredatorAgent` script, and `BehaviorParameters` component (or attach the provided `BehaviorSetup` script). Set `UseChildSensors` to `true` if you add additional sensor components.
4. **Add another GameObject** to represent the prey (e.g., a smaller sphere or cube). Attach a `Rigidbody` component and the `Prey` script.
5. **Assign references**: In the `PredatorAgent` Inspector, set the `preyTransform` field to the prey GameObject. Optionally adjust speeds and other parameters.
6. **Add colliders** or boundary walls if desired. The sample checks bounds based on coordinates (±5 units).
7. **Optionally** attach a `DecisionRequester` to control how often the agent decisions occur.
8. **Press Play** in Unity. The predator will use the heuristic controls by default until connected to Python.

## Python Interaction

Ensure you have the `mlagents` Python package installed (matching your Unity version). Then you can run the provided Python script:

```bash
python train_predator.py
```

This script connects to the running Unity Editor, prints environment details, and plays a few episodes with random actions. Messages from Unity (e.g., episode starts, prey captured) will be logged via the custom side channel.

To train the predator agent, use the `mlagents-learn` CLI:

```bash
mlagents-learn config.yaml --run-id=PredatorRun --env=Path/To/Your/Unity/Executable --force
```

Replace `Path/To/Your/Unity/Executable` with the path to your built Unity environment (or leave it blank and use `--no-graphics` when connecting to the Editor). The trained model can be embedded back into the `BehaviorParameters` component for inference.
