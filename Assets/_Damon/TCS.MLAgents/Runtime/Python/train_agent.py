#!/usr/bin/env python3
"""
ML-Agents training script for StreamerFighter predator agents.
This script handles training and model export for Unity integration.
"""

import os
import sys
import subprocess
from pathlib import Path

def setup_environment():
    """Setup the ML-Agents training environment."""
    print("Setting up ML-Agents training environment...")
    
    # Get the directory containing this script
    script_dir = Path(__file__).parent
    unity_project_root = script_dir.parent.parent.parent.parent
    
    print(f"Script directory: {script_dir}")
    print(f"Unity project root: {unity_project_root}")
    
    return script_dir, unity_project_root

def run_training(config_path, run_id="predator_training"):
    """Run ML-Agents training with the specified configuration."""
    print(f"Starting training with config: {config_path}")
    print(f"Run ID: {run_id}")
    
    # ML-Agents training command
    cmd = [
        sys.executable, "-m", "mlagents.trainers.learn",
        str(config_path),
        "--run-id", run_id,
        "--force",
    ]

    print(f"Executing command: {' '.join(cmd)}")
    
    try:
        # Run the training
        result = subprocess.run(cmd, check=True, capture_output=False)
        print("Training completed successfully!")
        return True
    except subprocess.CalledProcessError as e:
        print(f"Training failed with error: {e}")
        return False
    except FileNotFoundError:
        print("Error: mlagents-learn command not found.")
        print("Please install ML-Agents Python package: pip install mlagents")
        return False

def export_model(run_id, script_dir, unity_project_root):
    """Export the trained model to Unity-compatible format."""
    # ML-Agents typically saves models to results/{run_id}/
    model_dir = Path.cwd() / "results" / run_id
    
    print(f"Looking for trained model in: {model_dir}")
    
    # Find the .onnx file
    onnx_files = list(model_dir.glob("**/*.onnx"))
    
    if not onnx_files:
        print("No .onnx model files found!")
        return False
    
    # Copy the model to Unity project
    unity_models_dir = unity_project_root / "Assets" / "_Damon" / "TCS.MLAgents" / "Runtime" / "Models"
    unity_models_dir.mkdir(exist_ok=True)
    
    for onnx_file in onnx_files:
        dest_file = unity_models_dir / onnx_file.name
        print(f"Copying {onnx_file} -> {dest_file}")
        
        import shutil
        shutil.copy2(onnx_file, dest_file)
    
    print(f"Model exported to: {unity_models_dir}")
    return True

def main():
    """Main training workflow."""
    print("=== StreamerFighter ML-Agents Training ===")
    
    # Setup
    script_dir, unity_project_root = setup_environment()
    config_path = script_dir / "config.yaml"
    
    if not config_path.exists():
        print(f"Error: Configuration file not found: {config_path}")
        sys.exit(1)
    
    # Run training
    run_id = "predator_v1"
    success = run_training(config_path, run_id)
    
    if not success:
        print("Training failed!")
        sys.exit(1)
    
    # Export model
    if export_model(run_id, script_dir, unity_project_root):
        print("\n=== Training Complete! ===")
        print("Model has been exported to Unity project.")
        print("You can now load it into your MlBrainData asset in Unity.")
    else:
        print("Model export failed!")
        sys.exit(1)

if __name__ == "__main__":
    main()