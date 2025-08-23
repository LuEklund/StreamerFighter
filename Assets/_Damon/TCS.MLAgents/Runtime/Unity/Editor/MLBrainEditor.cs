using UnityEngine;
using UnityEditor;
using System.IO;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity.Editor {
    [CustomEditor(typeof(MlBrain))]
    public class MLBrainEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            GUILayout.Space(10);
            
            MlBrain brain = (MlBrain)target;
            
            // Status display
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Model Status", EditorStyles.boldLabel);
            
            if (brain.HasValidModel) {
                EditorGUILayout.HelpBox("✓ Model is loaded and ready", MessageType.Info);
            }
            else {
                EditorGUILayout.HelpBox("⚠ No valid model data found. Load a trained .onnx model.", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
            
            // Model loading
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Model Loading", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Load Model from File...")) {
                LoadModelFromFile(brain);
            }
            
            if (GUILayout.Button("Load Latest Trained Model")) {
                LoadLatestTrainedModel(brain);
            }
            EditorGUILayout.EndVertical();
        }
        
        private void LoadModelFromFile(MlBrain brain) {
            string path = EditorUtility.OpenFilePanel("Select ONNX Model", "", "onnx");
            if (!string.IsNullOrEmpty(path)) {
                LoadModelFile(brain, path);
            }
        }
        
        private void LoadLatestTrainedModel(MlBrain brain) {
            // Look for models in the Models folder
            string modelsPath = Path.Combine(Application.dataPath, "_Damon/TCS.MLAgents/Runtime/Models");
            
            if (!Directory.Exists(modelsPath)) {
                EditorUtility.DisplayDialog("No Models Found", 
                    "No Models folder found. Please train a model first using the Python training script.", 
                    "OK");
                return;
            }
            
            string[] onnxFiles = Directory.GetFiles(modelsPath, "*.onnx");
            
            if (onnxFiles.Length == 0) {
                EditorUtility.DisplayDialog("No Models Found", 
                    "No .onnx files found in the Models folder. Please train a model first.", 
                    "OK");
                return;
            }
            
            // Use the most recently modified file
            string latestFile = onnxFiles[0];
            System.DateTime latestTime = File.GetLastWriteTime(latestFile);
            
            foreach (string file in onnxFiles) {
                System.DateTime writeTime = File.GetLastWriteTime(file);
                if (writeTime > latestTime) {
                    latestFile = file;
                    latestTime = writeTime;
                }
            }
            
            LoadModelFile(brain, latestFile);
        }
        
        private void LoadModelFile(MlBrain brain, string filePath) {
            if (brain.m_modelData == null) {
                // Create a new MlBrainData asset
                string assetPath = AssetDatabase.GetAssetPath(brain);
                string directory = Path.GetDirectoryName(assetPath);
                string dataAssetPath = Path.Combine(directory, brain.name + "_Data.asset");
                
                MlBrainData brainData = CreateInstance<MlBrainData>();
                AssetDatabase.CreateAsset(brainData, dataAssetPath);
                
                brain.m_modelData = brainData;
                EditorUtility.SetDirty(brain);
            }
            
            // Load the model data
            brain.m_modelData.LoadFromFile(filePath);
            EditorUtility.SetDirty(brain.m_modelData);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Successfully loaded model from: {filePath}");
        }
    }
}