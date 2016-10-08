# coding:utf-8
import argparse
from cnn_dqn_agent import CnnDqnAgent
import gym
from PIL import Image
import numpy as np

parser = argparse.ArgumentParser(description='Process some integers.')
parser.add_argument('--gpu', '-g', default=-1, type=int,
                    help='GPU ID (negative value indicates CPU)')
parser.add_argument('--log-file', '-l', default='reward.log', type=str,
                    help='reward log file name')
args = parser.parse_args()

agent_count = 4

agents = {}
for i in xrange(agent_count):
    agent_id = i + 1
    agents[agent_id] = CnnDqnAgent()

agent_initialized = False
cycle_counter = 0
log_file = args.log_file
reward_sum = 0
depth_image_dim = 32 * 32
depth_image_count = 1
total_episode = 10000
episode_count = 1

while episode_count <= total_episode:
    if not agent_initialized:
        agent_initialized = True
        for key, agent in agents.items():
            print("initializing agent{}...".format(key))
            agent.agent_init(
                use_gpu=args.gpu,
                depth_image_dim=depth_image_dim * depth_image_count,
                agent_id=key)

        print("gym.make...")
        env = gym.make('Lis-v2')

        print("first step...")
        reply = env.reset()

        actions = {}
        for key, agent in agents.items():
            actions[key] = agent.agent_start(reply[key].observation)

        print(actions)
        env_replies = env.step(actions, cycle_counter)
        print(env_replies)
        with open(log_file, 'w') as the_file:
            the_file.write('cycle, episode_reward_sum \n')
    else:
        cycle_counter += 1
        #reward_sum += reward
        end_episode = False
        for rep in env_replies.values():
            if rep.end_episode:
                end_episode = True
                break

        if end_episode:
            actions = {}
            for key, agent in agents.items():
                agent.agent_end(env_replies[key].reward)
                actions[key] = agent.agent_start(reply[key].observation)

            env_replies = env.step(actions, cycle_counter)

            with open(log_file, 'a') as the_file:
                the_file.write(str(cycle_counter) +
                               ',' + str(reward_sum) + '\n')
            reward_sum = 0
            episode_count += 1

        else:
            actions = {}
            for key, agent in agents.items():
                print("agent.agent_step")
                action, eps, q_now, obs_array = agent.agent_step(reply[key].reward, reply[key].observation)
                actions[key] = action
                print("agent.agent_step_update")
                agent.agent_step_update(reply[key].reward, action, eps, q_now, obs_array)
            print("env.step")
            env_replies = env.step(actions, cycle_counter)

env.close()
