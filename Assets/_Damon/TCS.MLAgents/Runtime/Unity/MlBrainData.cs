using Unity.Barracuda;
namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    [CreateAssetMenu( menuName = "Create MlBrainData", fileName = "MlBrainData", order = 0 )] 
    public class MlBrainData : NNModelData {
        public byte[] Data {
            get => Value;
            set => Value = value;
        }
    }
}