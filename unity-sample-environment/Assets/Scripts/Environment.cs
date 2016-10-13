using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MLPlayer {
	public class Environment : MonoBehaviour {

        int partCount = 10;
        int itemCount = 5;
        int areaSize = 6;
		[SerializeField] List<GameObject> itemPrefabs;
	

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        public void OnReset(Dictionary<int, Agent> agents) {
            Debug.Log ("地形の初期化");
            foreach(Transform i in transform) {
                Destroy (i.gameObject);
            }

            //実際の地形は上下反転
            //0:使用可 1:使用不可 
            int[,] map = new int[,] {
                {1, 1, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 0},
                {0, 1, 0, 0, 1, 0},
                {0, 1, 0, 0, 1, 0},
                {0, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 1, 1},
            };

            int xi, zi = 0;
            float x, z = 0.0f;
            float y = 1.0f;

            for (int i=0; i<partCount; i++) {
                Debug.Log ("生成");   
                xi = (int)UnityEngine.Random.Range(0, areaSize);
                zi = (int)UnityEngine.Random.Range(0, areaSize);

                while(map[xi, zi] >= 1){
                    Debug.Log ("競合");
                    xi = (int)UnityEngine.Random.Range(0, areaSize);
                    zi = (int)UnityEngine.Random.Range(0, areaSize);
                }

                map [xi, zi] = 2;

                x = (xi - 3) * 6 + 2.5f;
                z = (zi - 3) * 6 + 2.5f;
                Vector3 pos = new Vector3 (x, 0, z);

                pos += transform.position;
                //int itemId = UnityEngine.Random.Range(1, itemPrefabs.Count);
                GameObject obj = (GameObject)GameObject.Instantiate 
                    (itemPrefabs[1], pos, Quaternion.identity);
                obj.transform.parent = transform;
            }

            for (int j = 0; j < itemCount; j++) {
                Debug.Log ("生成2");  
                xi = (int)UnityEngine.Random.Range (0, areaSize);
                zi = (int)UnityEngine.Random.Range (0, areaSize);

                while (map [xi, zi] >= 1) {
                    Debug.Log ("競合2");
                    xi = (int)UnityEngine.Random.Range (0, areaSize);
                    zi = (int)UnityEngine.Random.Range (0, areaSize);
                }

                map [xi, zi] = 2;

                x = (xi - 3) * 5 + 2.5f;
                z = (zi - 3) * 5 + 2.5f;
                Vector3 pos2 = new Vector3 (x, y, z);

                pos2 += transform.position;
                //int itemId = UnityEngine.Random.Range(1, itemPrefabs.Count);
                GameObject obj2 = (GameObject)GameObject.Instantiate 
                    (itemPrefabs [0], pos2, Quaternion.identity);
                obj2.transform.parent = transform;
            }

            List<int[]> emptyArea = new List<int[]>();
            for (int idx0 = 0; idx0 < map.GetLength(0); idx0++)
            {
                for (int idx1 = 1; idx1 < map.GetLength(1); idx1++)
                {
                    if (map[idx0,idx1] <= 1)
                    {
                        emptyArea.Add(new int[]{ idx0, idx1 });
                    }
                }
            }
            int[][] newArea = emptyArea.OrderBy(i => Guid.NewGuid()).ToArray();
            foreach (var key in agents.Keys)
            {
                Vector3 pos = new Vector3((newArea[key][0]-3)*6+2.5f, 0, (newArea[key][1]-3)*6+2.5f);
                agents[key].SetPosition(pos);
            }
		}
	}
}
