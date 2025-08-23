
import numpy as np
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.base_env import ActionTuple
from custom_side_channel import StringLogSideChannel


def main():
    side_channel = StringLogSideChannel()
    env = UnityEnvironment(file_name=None, seed=1, side_channels=[side_channel])
    env.reset()
    behavior_name = list(env.behavior_specs.keys())[0]
    spec = env.behavior_specs[behavior_name]
    print(f"Connected to behavior: {behavior_name}")
    print(f"Observation space: {spec.observation_specs}")
    print(f"Action space: {spec.action_spec}")
    # Run a few episodes with random actions
    num_episodes = 3
    for episode in range(num_episodes):
        env.reset()
        decision_steps, terminal_steps = env.get_steps(behavior_name)
        side_channel.log(f"Python starting episode {episode+1}")
        while len(decision_steps) > 0:
            actions = np.random.uniform(-1, 1, (len(decision_steps), spec.action_spec.continuous_size))
            action_tuple = ActionTuple(continuous=actions)
            env.set_actions(behavior_name, action_tuple)
            env.step()
            decision_steps, terminal_steps = env.get_steps(behavior_name)
        print(f"Episode {episode+1} complete")
    env.close()

if __name__ == "__main__":
    main()
