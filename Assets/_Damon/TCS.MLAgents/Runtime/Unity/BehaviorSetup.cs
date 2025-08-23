using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    [RequireComponent(typeof(BehaviorParameters))]
    public class BehaviorSetup : MonoBehaviour {
        [SerializeField] string behaviorName = "PredatorBehavior";
        [SerializeField] BehaviorType behaviorType = BehaviorType.Default;
        [SerializeField] int continuousActionCount = 2;
        [SerializeField] int discreteActionBranches = 0;
        [SerializeField] bool useChildSensors = true;
        [SerializeField] MlBrain customBrain;

        void Awake() {
            SetupBehaviorParameters();
        }

        void SetupBehaviorParameters() {
            var behaviorParameters = GetComponent<BehaviorParameters>();
            if (behaviorParameters == null) return;

            behaviorParameters.BehaviorName = behaviorName;
            behaviorParameters.BehaviorType = behaviorType;
            behaviorParameters.UseChildSensors = useChildSensors;

            if (discreteActionBranches > 0) {
                int[] branchSizes = new int[discreteActionBranches];
                for (int i = 0; i < discreteActionBranches; i++) {
                    branchSizes[i] = 2;
                }
                behaviorParameters.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(branchSizes);
            } else {
                behaviorParameters.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(continuousActionCount);
            }

            if (customBrain != null && customBrain.HasValidModel) {
                behaviorParameters.Model = customBrain;
            }
        }

        public void SetBehaviorName(string name) {
            behaviorName = name;
            SetupBehaviorParameters();
        }

        public void SetBehaviorType(BehaviorType type) {
            behaviorType = type;
            SetupBehaviorParameters();
        }
    }
}