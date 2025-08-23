# Unity ML‑Agents 2.0 API — Full Review (C# + Python Interface)

*Last updated: Aug 23, 2025*

This review covers the **C# package** `com.unity.ml-agents@2.0` (the SDK you use inside Unity) and the **Python side** you interact with during training. It focuses on practical use, API shape, and common pitfalls.

---

## 1) Big‑picture architecture

* **Unity side (C#)**: Your scene contains **Agents**. They gather observations (via **Sensors**), receive **Actions** (via **Actuators**), and are orchestrated by the **Academy**. **BehaviorParameters** bind an Agent to a decision‑making mode (external training, inference, or heuristic).
* **Python side**: The training process (e.g., `mlagents-learn`) connects to Unity via the **Communicator**. You can also use the **Low‑Level Python API** to drive environments directly and send actions/receive observations programmatically.
* **Artifacts**: training produces an **ONNX/NN model** you can assign to `BehaviorParameters.Model` to run inference in‑Editor or in builds.

---

## 2) Namespaces & key types (C#)

### 2.1 `Unity.MLAgents`

* **Agent**

    * **Lifecycle**: `Initialize()` → (loop: `CollectObservations(VectorSensor)`, `OnActionReceived(ActionBuffers)`; optional `Heuristic(ActionBuffers)`; end with `EndEpisode()`/`EpisodeInterrupted()`) → `OnEpisodeBegin()`.
    * **Decision flow**: Trigger decisions via `RequestDecision()` or add **DecisionRequester** to schedule them. Physics‑driven agents typically act in **FixedUpdate**.
    * **Episode control**: `MaxStep`, `StepCount`, `CompletedEpisodes`.
    * **Rewards**: `AddReward()`, `SetReward()`, `GetCumulativeReward()`.
    * **Model hot‑swap**: `SetModel(string behaviorName, NNModel model, InferenceDevice device)`.
* **Academy**

    * Manages stepping, connection to Python, and environment‑level resets.
* **DecisionRequester (component)**

    * Requests decisions at a fixed interval; avoids manual `RequestDecision()` calls.
* **StatsRecorder**

    * Log custom metrics (e.g., `Academy.Instance.StatsRecorder.Add("Agent/Health", value)`); appears in TensorBoard.

### 2.2 `Unity.MLAgents.Policies`

* **BehaviorParameters (component)**

    * **BehaviorName** (base), **FullyQualifiedBehaviorName** (includes metadata like Team ID).
    * **BehaviorType**: `Default`, `InferenceOnly`, `HeuristicOnly`.
    * **Model**, **InferenceDevice** (CPU/GPU via Barracuda).
    * **BrainParameters**: observation & action specs used by the policy.
    * **ObservableAttributeHandling**, **UseChildSensors**, **UseChildActuators**.
* **BrainParameters**

    * Defines observation modalities and **ActionSpec** (discrete branch sizes / continuous counts).

### 2.3 `Unity.MLAgents.Sensors`

* **Interfaces & base**: `ISensor`, `SensorComponent`, `ObservationSpec`.
* **Core sensors**:

    * **VectorSensor** (+ `VectorSensorComponent`) — numeric features you add in `CollectObservations`.
    * **CameraSensor** (+ `CameraSensorComponent`) — visual observations from a Unity Camera.
    * **RayPerceptionSensor** (+ `RayPerceptionSensorComponent2D/3D`) — distances/tags via raycasts.
    * **GridSensorBase / GridSensorComponent** — 2D grid encodings (useful for tile/occupancy maps).
    * **BufferSensor / BufferSensorComponent** — variable‑length sets of entities.
    * **StackingSensor** — temporal stacking wrapper.

### 2.4 `Unity.MLAgents.Actuators`

* **ActionSpec** — defines action space shape.
* **ActionBuffers** — actions received by `OnActionReceived(ActionBuffers)`.
* **Interfaces**: `IActionReceiver`, `IActuator`, `IDiscreteActionMask` (for dynamic action masking).
* **ActuatorComponent** — attach custom actuators via components.

### 2.5 `Unity.MLAgents.Demonstrations`

* **DemonstrationRecorder** — record `.demo` files for imitation learning.
* **DemonstrationWriter/Reader** — programmatic access to demonstration data.

### 2.6 `Unity.MLAgents.SideChannels`

* **SideChannel** — custom out‑of‑band messaging between Unity and Python.
* **SideChannelManager** — register/unregister custom channels. Built‑in channels are handled internally (e.g., engine configuration).

---

## 3) Core contracts & usage patterns

### 3.1 Observations

* Keep **shape and ordering stable** across steps.
* For **VectorSensor**, add in a deterministic order every step. Mismatched sizes cause exceptions.
* For visual inputs, prefer modest resolutions (e.g., 84×84 or 96×96) and compress if needed.
* Combine sensors as needed; the policy consumes all attached sensor streams.

### 3.2 Actions

* Implement **`OnActionReceived(ActionBuffers)`** to consume actions.
* For discrete actions, define **branch sizes**; use **`IDiscreteActionMask`** to disable invalid options per step.
* Provide a **`Heuristic(ActionBuffers)`** implementation for debugging and to bootstrap imitation demos.

### 3.3 Episodes & resets

* Call **`EndEpisode()`** when terminal conditions occur; use **`EpisodeInterrupted()`** when ending for external reasons (e.g., time limit) that shouldn’t be treated as failure.
* Initialize/reset environment state in **`OnEpisodeBegin()`**.

### 3.4 Decision timing

* Use **DecisionRequester** to request at a fixed frequency (e.g., every N Academy steps).
* Physics‑heavy environments: act in **FixedUpdate**; keep decision interval aligned to physics.

### 3.5 Model management & behavior modes

* **BehaviorType** determines who makes decisions:

    * `Default` → external trainer if connected; else inference; else heuristic.
    * `InferenceOnly` → always use the attached model.
    * `HeuristicOnly` → always call `Heuristic()`.
* You can **hot‑swap** models at runtime with `SetModel(...)`.

### 3.6 Stats & instrumentation

* Use **StatsRecorder** for custom metrics; group with slashes (e.g., `Agent/WinRate`).
* Trainer logs + TensorBoard visualize episodic reward, entropy, etc.

---

## 4) Python side overview

* **CLI**: `mlagents-learn` reads a YAML config (hyperparameters, behavior settings) and spawns trainers (PPO, SAC, cooperative multi‑agent, etc.).
* **Low‑Level API** (`mlagents_envs`):

    * `UnityEnvironment` for connection & stepping.
    * `BehaviorSpec` (obs & action spec), `DecisionSteps`/`TerminalSteps` (batched per behavior), `ActionTuple` to send actions.
    * Supports **side\_channels** parameter for registering custom channels.
* **Results**: training artifacts under `results/<run-id>`, including model files and TensorBoard summaries.

---

## 5) Versioning & compatibility (what to match)

* Keep **Unity package version** and **Python packages** from the **same ML‑Agents release** when possible.
* **Communicator** uses semantic versioning; mismatches can prevent connection.
* **Model files** are versioned; older package versions may not load newer models.

---

## 6) Practical examples (C#)

### 6.1 Minimal Agent skeleton

```csharp
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    public Rigidbody rb;
    public Transform target;

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.5f, 0);
        target.localPosition = new Vector3(UnityEngine.Random.Range(-3f,3f), 0.5f, UnityEngine.Random.Range(-3f,3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuous = actions.ContinuousActions;
        var force = new Vector3(continuous[0], 0, continuous[1]) * 10f;
        rb.AddForce(force);

        var dist = Vector3.Distance(transform.localPosition, target.localPosition);
        AddReward(-0.001f); // time penalty
        if (dist < 1.2f) { AddReward(+1f); EndEpisode(); }
        if (transform.localPosition.y < 0) { EndEpisode(); }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Horizontal");
        ca[1] = Input.GetAxis("Vertical");
    }
}
```

### 6.2 DecisionRequester setup

* Add **DecisionRequester** component; set **Decision Period** (e.g., 5) and **Take Actions Between Decisions** as needed.

### 6.3 Discrete actions & masking

```csharp
public override void OnActionReceived(ActionBuffers actions)
{
    int move = actions.DiscreteActions[0]; // branch size, e.g., 5
    // ...
}

void UpdateMask(IDiscreteActionMask mask)
{
    // Disable action index 3 for branch 0 this step
    mask.SetActionEnabled(0, 3, false);
}
```

### 6.4 Custom SideChannel skeleton

```csharp
using Unity.MLAgents.SideChannels;

public class JsonSideChannel : SideChannel
{
    public JsonSideChannel() { ChannelId = new Guid("01234567-89ab-cdef-0123-456789abcdef"); }
    protected override void OnMessageReceived(IncomingMessage msg)
    {
        var payload = msg.ReadString();
        Debug.Log($"Python says: {payload}");
    }
    public void Send(string s)
    {
        using var m = new OutgoingMessage();
        m.WriteString(s);
        QueueMessageToSend(m);
    }
}
```

Register with `SideChannelManager.RegisterSideChannel(...)` at startup; remember to unregister on teardown.

---

## 7) Common pitfalls & troubleshooting

* **Behavior name mismatch**: Trainer YAML behavior name must match the Agent’s **FullyQualifiedBehaviorName** (base BehaviorName plus suffixes such as Team ID).
* **Observation shape errors**: Vector observation counts and visual sizes must remain constant; changes at runtime or inconsistencies across agents cause runtime errors.
* **Decision timing**: Calling `RequestDecision()` too frequently (or not aligning with physics) can cause unstable training.
* **Camera inputs**: Large resolutions and many frames stack → heavy memory/compute. Start small; consider grayscale.
* **Ray sensors**: Ensure tags/layers are configured and ray count/angles cover what matters; consider ordering/left‑to‑right settings when available.
* **GPU not utilized**: Most training compute is on Python (PyTorch). Unity Editor rendering may dominate; use headless builds for throughput.

---

## 8) Upgrade/migration notes (2.0 context)

* **GridSensor** moved into the main package around 2.x; older extension‑based GridSensors are incompatible.
* **SideChannelManager** centralizes custom side channel registration; built‑ins are managed for you.
* Always consult the release notes of the specific 2.x point release you’re using for breaking changes.

---

## 9) Quick checklists

**Author a new Agent**

* [ ] Add `Agent`, `BehaviorParameters`, (optional) `DecisionRequester`.
* [ ] Attach needed `SensorComponent`(s) and (optional) `ActuatorComponent`(s).
* [ ] Implement `CollectObservations`, `OnActionReceived`, `OnEpisodeBegin`, and `Heuristic`.
* [ ] Define `ActionSpec` (discrete/continuous) in `BehaviorParameters`.
* [ ] Add reward shaping & terminal conditions; set `MaxStep`.

**Train**

* [ ] Match package versions (Unity ↔ Python release).
* [ ] Verify behavior names vs YAML.
* [ ] Start with small observations; inspect TensorBoard stats; tune.

**Ship**

* [ ] Switch to `InferenceOnly` and assign the trained model.
* [ ] Consider `InferenceDevice` (CPU vs GPU) per target hardware.

---