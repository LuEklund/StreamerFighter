using UnityEngine;
using Random = UnityEngine.Random;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    [CreateAssetMenu(menuName = "ML Simulation/Simulation Config", fileName = "SimulationConfig")]
    public class SimulationConfig : ScriptableObject {
        [Header("Arena Settings")]
        public Vector3 arenaMinBounds = new Vector3(-4f, 0f, -4f);
        public Vector3 arenaMaxBounds = new Vector3(4f, 1f, 4f);
        
        [Header("Agent Settings")]
        public float predatorSpeed = 5f;
        public float preySpeed = 3f;
        
        [Header("Reward Settings")]
        public float catchReward = 1.0f;
        public float boundaryPenalty = -1.0f;
        public float timePenalty = -0.001f;
        public float catchDistance = 1.2f;
        
        [Header("Episode Settings")]
        public float maxEpisodeLength = 30f;
        public bool randomizeStartPositions = true;
        
        [Header("Prey Behavior")]
        public float preyDirectionChangeInterval = 2f;
        public bool preyBouncesOffWalls = true;
        
        [Header("Observation Settings")]
        public bool includeVelocityObservations = true;
        public bool includeDistanceObservations = true;
        public bool normalizeObservations = true;
        
        public Vector3 GetRandomArenaPosition() {
            return new Vector3(
                Random.Range(arenaMinBounds.x, arenaMaxBounds.x),
                Random.Range(arenaMinBounds.y, arenaMaxBounds.y),
                Random.Range(arenaMinBounds.z, arenaMaxBounds.z)
            );
        }
        
        public bool IsWithinCatchDistance(Vector3 pos1, Vector3 pos2) {
            return Vector3.Distance(pos1, pos2) <= catchDistance;
        }
    }
}