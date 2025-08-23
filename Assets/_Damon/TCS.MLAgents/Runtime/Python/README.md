# StreamerFighter ML-Agents Training

This folder contains the Python training scripts and configuration for ML-Agents.

## Setup

1. **Install Python dependencies:**
   ```bash
   pip install -r requirements.txt
   ```

2. **Verify ML-Agents installation:**
   ```bash
   mlagents-learn --help
   ```

## Training Workflow

### Method 1: Automated Training (Recommended)
Run the complete training pipeline:
```bash
python train_agent.py
```

This script will:
- Start ML-Agents training with the config.yaml
- Export the trained model to Unity's Models folder
- Handle file management automatically

### Method 2: Manual Training
1. **Start training:**
   ```bash
   mlagents-learn config.yaml --run-id predator_v1 --force
   ```

2. **In Unity:**
   - Open your scene with ML-Agents
   - Press Play to start training
   - Training progress will be shown in the console

3. **Load the trained model:**
   - Select your MLBrain asset in Unity
   - Click "Load Latest Trained Model" button
   - The model will be automatically loaded

## Configuration

Edit `config.yaml` to adjust training parameters:
- `batch_size`: Training batch size
- `learning_rate`: Learning rate for the neural network
- `max_steps`: Maximum training steps
- `time_horizon`: How many steps before episode ends

## Files

- `config.yaml` - ML-Agents training configuration
- `train_agent.py` - Automated training script
- `custom_side_channel.py` - Communication between Unity and Python
- `requirements.txt` - Python package dependencies
- `README.md` - This file