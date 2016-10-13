using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace MLPlayer
{
	public class SceneController : MonoBehaviour
	{
		//singleton
		protected static SceneController instance;

		public static SceneController Instance {
			get {
				if (instance == null) {
					instance = (SceneController)FindObjectOfType (typeof(SceneController));
					if (instance == null) {
						Debug.LogError ("An instance of" + typeof(SceneController) + "is needed in the scene,but there is none.");
					}
				}
				return instance;
			}
		}

		[SerializeField] float cycleTimeStepSize;
		[SerializeField] float episodeTimeLength;
		[Range (0.1f, 10.0f)]
		[SerializeField] float timeScale = 1.0f;

        public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
		public AIServer server;
		public static bool FinishFlag = false;

		[SerializeField] Environment environment;
		private float lastSendTime;
		private float episodeStartTime = 0f;
		public static ManualResetEvent received = new ManualResetEvent (false);

        [SerializeField] GameObject tankPrefab;

        [SerializeField] private int agentCount;

		void Start ()
		{
            instance = this;
            CreateAgent();
            server.Init(agents);
            server.AIServerStart();
			StartNewEpisode ();
			lastSendTime = -cycleTimeStepSize;
        }

        void CreateAgent()
        {            
            for (int id=1; id<=agentCount; id++)
            {
                GameObject obj = Instantiate<GameObject>(tankPrefab);

                Agent a = obj.GetComponent<Agent>();
                a.Initialize(id);

                agents.Add(a.state.agent_id, a);
            }
        }

		public void TimeOver ()
		{
            foreach (var a in agents.Values)
            {
                a.EndEpisode();
            }
		}

		public void StartNewEpisode ()
		{
			episodeStartTime = Time.time;
            environment.OnReset (agents);
            foreach (var a in agents.Values)
            {
                a.StartEpisode();
            }
		}

		public void FixedUpdate ()
		{
			if (FinishFlag == false) {
				Time.timeScale = timeScale;
				if (lastSendTime + cycleTimeStepSize <= Time.time) {
					lastSendTime = Time.time;
	
					if (Time.time - episodeStartTime > episodeTimeLength) {
						TimeOver ();
					}
                    foreach (var a in agents.Values)
                    {
                        if (a.state.endEpisode) {
                            StartNewEpisode ();
                            break;
                        }
                    }
                    received.Reset ();
                    List<State> stateList = new List<State>();
                    foreach (var agent in agents.Values)
                    {
                        agent.UpdateState();
                        stateList.Add(agent.state);
                    }
                    server.PushAgentState(stateList.ToArray());
					received.WaitOne ();
                    foreach (var agent in agents.Values)
                    {
                        agent.ResetState ();
                    }
				}
			} else {
				//EditorApplication.isPlaying = false;
			}
		}
	}
}
