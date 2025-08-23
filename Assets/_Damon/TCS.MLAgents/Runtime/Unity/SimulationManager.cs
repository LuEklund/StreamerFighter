using UnityEngine;
using Unity.MLAgents;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class SimulationManager : MonoBehaviour {
        [SerializeField] SimulationConfig config;
        [SerializeField] PredatorController[] predators;
        [SerializeField] PreyController[] prey;
        [SerializeField] bool autoFindEntities = true;
        
        int activeEpisodes = 0;
        
        void Start() {
            if (autoFindEntities) {
                FindSimulationEntities();
            }
            
            SetupSimulation();
        }
        
        void FindSimulationEntities() {
            if (predators == null || predators.Length == 0) {
                predators = FindObjectsOfType<PredatorController>();
            }
            
            if (prey == null || prey.Length == 0) {
                prey = FindObjectsOfType<PreyController>();
            }
        }
        
        void SetupSimulation() {
            if (config == null) {
                Debug.LogWarning("SimulationManager: No configuration assigned");
                return;
            }
            
            SetupPredators();
            SetupPrey();
            
            Academy.Instance.AgentPreStep += OnAgentPreStep;
        }
        
        void SetupPredators() {
            foreach (var predator in predators) {
                if (predator == null) continue;
                
                var movement = predator.GetComponent<Movement>();
                if (movement != null) {
                    movement.Speed = config.predatorSpeed;
                }
                
                var boundarySystem = predator.GetComponent<BoundarySystem>();
                if (boundarySystem == null) {
                    boundarySystem = predator.gameObject.AddComponent<BoundarySystem>();
                }
                
                var rewardSystem = predator.GetComponent<RewardSystem>();
                if (rewardSystem == null) {
                    rewardSystem = predator.gameObject.AddComponent<RewardSystem>();
                }
            }
        }
        
        void SetupPrey() {
            foreach (var preyController in prey) {
                if (preyController == null) continue;
                
                var movement = preyController.GetComponent<Movement>();
                if (movement != null) {
                    movement.Speed = config.preySpeed;
                }
            }
        }
        
        void OnAgentPreStep(int academyStepCount) {
            UpdateActiveEpisodeCount();
            LogSimulationStats();
        }
        
        void UpdateActiveEpisodeCount() {
            activeEpisodes = 0;
            foreach (var predator in predators) {
                if (predator != null && predator.enabled) {
                    activeEpisodes++;
                }
            }
        }
        
        void LogSimulationStats() {
            if (Academy.Instance.StatsRecorder != null) {
                Academy.Instance.StatsRecorder.Add("Simulation/ActiveEpisodes", activeEpisodes);
                Academy.Instance.StatsRecorder.Add("Simulation/TotalPredators", predators.Length);
                Academy.Instance.StatsRecorder.Add("Simulation/TotalPrey", prey.Length);
            }
        }
        
        public void ResetSimulation() {
            foreach (var predator in predators) {
                if (predator != null) {
                    predator.EndEpisode();
                }
            }
        }
        
        public void PauseSimulation() {
            foreach (var predator in predators) {
                if (predator != null) {
                    predator.enabled = false;
                }
            }
            
            foreach (var preyController in prey) {
                if (preyController != null) {
                    preyController.enabled = false;
                }
            }
        }
        
        public void ResumeSimulation() {
            foreach (var predator in predators) {
                if (predator != null) {
                    predator.enabled = true;
                }
            }
            
            foreach (var preyController in prey) {
                if (preyController != null) {
                    preyController.enabled = true;
                }
            }
        }
        
        void OnDestroy() {
            if (Academy.Instance != null) {
                Academy.Instance.AgentPreStep -= OnAgentPreStep;
            }
        }
        
        void OnValidate() {
            if (Application.isPlaying && autoFindEntities) {
                FindSimulationEntities();
            }
        }
    }
}