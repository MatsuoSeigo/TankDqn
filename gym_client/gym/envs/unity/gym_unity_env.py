# -*- coding: utf-8 -*-

import websocket
import msgpack
import gym
import io
from PIL import Image
from PIL import ImageOps
from gym import spaces
import numpy as np

class GymUnityEnv(gym.Env):

    def __init__(self): #環境が作られたとき
        websocket.enableTrace(True)
    	self.ws = websocket.create_connection("ws://localhost:4649/CommunicationGym")
        self.action_space = spaces.Discrete(3)  # 3つのアクションをセット
        self.depth_image_dim = 32 * 32
        self.depth_image_count = 1
        self.reply_temp = self.receive()


    def reset(self):
        return self.reply_temp



    def step(self, actions, step_id):  # ステップ処理 、actionを外から受け取る
        data = []
        for key, action in actions.items():
            dic = {"agent_id":int(key), "step_id":int(step_id), "command":str(action)}
            data.append(dic)

        msg = msgpack.packb(data)  # アクションをpack
        self.ws.send(msg)  # 送信
        print("step send message")
        print(msg)

        # Unity Process

        #observation, reward, end_episode = self.receive()
        env_replies = self.receive()
        print(env_replies.keys())

        return env_replies

    def receive(self):
        statedata = self.ws.recv()  # 状態の受信
        stateList = msgpack.unpackb(statedata)  # 受け取ったデータをunpack

        replies = {}
        for state in stateList:
            image = []
            for i in xrange(self.depth_image_count):
                image.append(Image.open(io.BytesIO(bytearray(state['image'][i]))))
            depth = []
            for i in xrange(self.depth_image_count):
                d = (Image.open(io.BytesIO(bytearray(state['depth'][i]))))

                #d.save('stephoge.png')

                depth.append(np.array(ImageOps.grayscale(d)).reshape(self.depth_image_dim))

            observation = {"image": image, "depth": depth}
            reward = state['reward']
            end_episode = state['endEpisode']

            replies[int(state["agent_id"])] = EnvReply(observation, reward, end_episode)

        return replies


    def close(self):  # コネクション終了処理
        self.ws.close()  # コネクション終了

class EnvReply(object):
    def __init__(self, observation, reward, end_episode):
        self.observation = observation
        self.reward = reward
        self.end_episode = end_episode
