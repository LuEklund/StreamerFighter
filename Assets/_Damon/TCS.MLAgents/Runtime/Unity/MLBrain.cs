using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    [CreateAssetMenu( menuName = "Create MLBrain", fileName = "MLBrain", order = 0 )] 
    public class MlBrain : NNModel {
        [SerializeField] MlBrainData m_modelData;
        
        public string ModelPath => m_modelData != null ? m_modelData.name : "No Model Assigned";
        public bool HasValidModel => m_modelData != null && m_modelData.Value != null && m_modelData.Value.Length > 0;
        
        public override string ToString() {
            return ModelPath;
        }
        
        void OnEnable() {
            UpdateModelData();
        }
        
        void OnValidate() {
            UpdateModelData();
        }
        
        private void UpdateModelData() {
            if (HasValidModel) {
                modelData = m_modelData;
            }
            else {
                modelData = null;
                if (m_modelData != null)
                    Debug.LogWarning($"MLBrain '{name}' has empty or invalid model data. Please assign a trained .onnx model file.");
            }
        }
    }
}