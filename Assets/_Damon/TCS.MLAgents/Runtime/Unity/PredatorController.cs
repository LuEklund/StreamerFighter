using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class PredatorController : Agent {
        [SerializeField] Transform targetPrey;
        [SerializeField] SimulationConfig config;
        
        Movement movement;
        BoundarySystem boundarySystem;
        RewardSystem rewardSystem;
        StringLogSideChannel logChannel;
        
        float episodeStartTime;
        
        public override void Initialize() {
            movement = GetComponent<Movement>();
            boundarySystem = GetComponent<BoundarySystem>();
            rewardSystem = GetComponent<RewardSystem>();
            
            if (movement == null) movement = gameObject.AddComponent<Movement>();
            if (boundarySystem == null) boundarySystem = gameObject.AddComponent<BoundarySystem>();
            if (rewardSystem == null) rewardSystem = gameObject.AddComponent<RewardSystem>();
            
            SetupFromConfig();
            
            logChannel = new StringLogSideChannel();
            SideChannelManager.RegisterSideChannel(logChannel);
            
            base.Initialize();
        }
        
        void SetupFromConfig() {
            if (config == null) return;
            
            movement.Speed = config.predatorSpeed;
            boundarySystem.GetComponent<BoundarySystem>().enabled = false;
        }
        
        public override void OnEpisodeBegin() {
            episodeStartTime = Time.time;
            
            movement.StopMovement();
            
            if (config != null && config.randomizeStartPositions) {
                movement.SetPosition(config.GetRandomArenaPosition());
            }
            
            ResetPrey();
            logChannel?.Log("Episode started");
        }
        
        void ResetPrey() {
            if (targetPrey == null) return;
            
            var preyController = targetPrey.GetComponent<PreyController>();
            if (preyController != null) {
                preyController.Respawn();
            }
        }
        
        public override void CollectObservations(VectorSensor sensor) {
            if (targetPrey == null || config == null) return;
            
            Vector3 relativePosition = targetPrey.localPosition - transform.localPosition;
            sensor.AddObservation(relativePosition);
            
            if (config.includeVelocityObservations) {
                sensor.AddObservation(movement.Velocity);
                
                var preyMovement = targetPrey.GetComponent<Movement>();
                if (preyMovement != null) {
                    sensor.AddObservation(preyMovement.Velocity);
                } else {
                    sensor.AddObservation(Vector3.zero);
                }
            }
            
            if (config.includeDistanceObservations) {
                float distance = Vector3.Distance(transform.localPosition, targetPrey.localPosition);
                sensor.AddObservation(distance);
            }
        }
        
        public override void OnActionReceived(ActionBuffers actionBuffers) {
            if (config == null) return;
            
            var continuousActions = actionBuffers.ContinuousActions;
            Vector3 force = new Vector3(continuousActions[0], 0, continuousActions[1]);
            movement.ApplyForce(force);
            
            rewardSystem.ApplyTimePenalty();
            
            CheckForCatch();
            CheckBoundaries();
            CheckEpisodeTimeout();
            
            LogStatistics();
        }
        
        void CheckForCatch() {
            if (targetPrey == null || config == null) return;
            
            if (config.IsWithinCatchDistance(transform.localPosition, targetPrey.localPosition)) {
                rewardSystem.RewardCatch();
                logChannel?.Log("Caught the prey!");
                rewardSystem.EndEpisode();
            }
        }
        
        void CheckBoundaries() {
            if (config == null) return;
            
            Vector3 pos = movement.Position;
            bool outOfBounds = pos.x < config.arenaMinBounds.x || pos.x > config.arenaMaxBounds.x ||
                              pos.z < config.arenaMinBounds.z || pos.z > config.arenaMaxBounds.z;
            
            if (outOfBounds) {
                rewardSystem.PenalizeBoundaryViolation();
                logChannel?.Log("Went out of bounds");
                rewardSystem.EndEpisode();
            }
        }
        
        void CheckEpisodeTimeout() {
            if (config == null) return;
            
            if (Time.time - episodeStartTime >= config.maxEpisodeLength) {
                logChannel?.Log("Episode timeout");
                rewardSystem.EndEpisode();
            }
        }
        
        void LogStatistics() {
            if (targetPrey == null) return;
            
            float distance = Vector3.Distance(transform.localPosition, targetPrey.localPosition);
            Academy.Instance.StatsRecorder.Add("Predator/DistanceToPrey", distance);
        }
        
        public override void Heuristic(in ActionBuffers actionsOut) {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = Input.GetAxis("Horizontal");
            continuousActionsOut[1] = Input.GetAxis("Vertical");
        }
        
        void OnCollisionEnter(Collision collision) {
            if (collision.transform == targetPrey) {
                rewardSystem.RewardCatch();
                logChannel?.Log("Caught the prey via collision");
                rewardSystem.EndEpisode();
            }
        }
        
        void OnDestroy() {
            if (logChannel != null) {
                SideChannelManager.UnregisterSideChannel(logChannel);
            }
        }
    }
}