# TankDqn
TankDqn is a multi agent system.
Agents learn combat in virtual environment.

## Install
download data:
```
./fetch.sh
```

### Ubuntu
Install [Unity experimental-build version](http://forum.unity3d.com/threads/unity-on-linux-release-notes-and-known-issues.350256/):

```
wget http://download.unity3d.com/download_unity/linux/unity-editor-installer-5.3.4f1+20160317.sh
sudo sh unity-editor-installer-5.3.4f1+20160317.sh

# run Unity
./unity-editor-5.3.4f1/Editor/Unity

# if background is pink, install:
sudo apt-get install lib32stdc++6 -y
```

install python modules:
```
pip install -r python-agent/requirements.txt
```

### Mac
Install Unity.

install python modules:
```
pip install -r python-agent/requirements.txt
```

### Windows

[Building simulator on Windows10](http://qiita.com/autani/items/4daa5587773631245d86) (Japanese)

## Usage

#### Environment

Open unity-sample-environment with Unity and load Scenes/Sample.
Press Start Button.

#### Agent

Next, run python module as a client.
```
cd gym_client/examples/agents
PYTHONPATH=../../ python Lis_dqn.py
```


## License
138	+ Apache License, Version 2.0
