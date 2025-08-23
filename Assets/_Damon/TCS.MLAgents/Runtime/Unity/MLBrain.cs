using Unity.Barracuda;
using UnityEngine.Serialization;
namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    [CreateAssetMenu( menuName = "Create MLBrain", fileName = "MLBrain", order = 0 )] 
    public class MlBrain : NNModel {
        [SerializeField] MlBrainData m_modelData;
        
        public string ModelPath => m_modelData != null ? m_modelData.name : "No Model Assigned";
        
        public override string ToString() {
            return ModelPath;
        }
        
        void OnValidate() {
            if (m_modelData != null) {
                modelData = m_modelData;
            }
        }
    }
}