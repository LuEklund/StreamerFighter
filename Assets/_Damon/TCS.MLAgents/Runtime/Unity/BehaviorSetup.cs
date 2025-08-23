using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    /// <summary>
    ///     Configures BehaviorParameters for the predator agent. Sets the behavior name and a 2D continuous action space.
    /// </summary>
    [RequireComponent( typeof(BehaviorParameters) )]
    public class BehaviorSetup : MonoBehaviour {
        public string m_behaviorName = "PredatorBehavior";
        public BehaviorType m_behaviorType = BehaviorType.Default;

        void Awake() {
            var behaviorParameters = GetComponent<BehaviorParameters>();
            if ( behaviorParameters != null ) {
                behaviorParameters.BehaviorName = m_behaviorName;
                behaviorParameters.BehaviorType = m_behaviorType;
                behaviorParameters.BrainParameters.ActionSpec = ActionSpec.MakeContinuous( 2 );
                behaviorParameters.UseChildSensors = true;
            }
        }
    }
}