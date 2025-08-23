using UnityEngine;
using Unity.MLAgents;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class RewardSystem : MonoBehaviour {
        [SerializeField] float catchReward = 1.0f;
        [SerializeField] float boundaryPenalty = -1.0f;
        [SerializeField] float timePenalty = -0.001f;
        [SerializeField] float proximityRewardMultiplier = 0.01f;
        [SerializeField] float maxProximityReward = 0.1f;
        
        Agent agent;
        
        public float CatchReward => catchReward;
        public float BoundaryPenalty => boundaryPenalty;
        public float TimePenalty => timePenalty;
        
        void Awake() {
            agent = GetComponent<Agent>();
        }
        
        public void GiveReward(float reward) {
            if (agent != null) {
                agent.AddReward(reward);
            }
        }
        
        public void SetReward(float reward) {
            if (agent != null) {
                agent.SetReward(reward);
            }
        }
        
        public void RewardCatch() {
            SetReward(catchReward);
        }
        
        public void PenalizeBoundaryViolation() {
            SetReward(boundaryPenalty);
        }
        
        public void ApplyTimePenalty() {
            GiveReward(timePenalty);
        }
        
        public void RewardProximity(float distance, float maxDistance) {
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
            float proximityReward = (1f - normalizedDistance) * proximityRewardMultiplier;
            proximityReward = Mathf.Min(proximityReward, maxProximityReward);
            GiveReward(proximityReward);
        }
        
        public void EndEpisode() {
            if (agent != null) {
                agent.EndEpisode();
            }
        }
    }
}