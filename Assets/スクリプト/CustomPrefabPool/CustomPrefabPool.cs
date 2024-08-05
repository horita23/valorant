using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CustomPrefabPool : MonoBehaviour, IPunPrefabPool
{
    // プレハブの配列
    [Tooltip("ネットオブジェクト")]
    [SerializeField]
    private GameObject[] prefabs; 

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, Stack<GameObject>> inactiveObjectPools = new Dictionary<string, Stack<GameObject>>();

    private void Start()
    {
        // プレハブの配列からIDとプレハブを辞書に追加
        foreach (var prefab in prefabs)
        {
            // プレハブの名前をIDとして使用
            string prefabId = prefab.name; 
            prefabDictionary[prefabId] = prefab;
            inactiveObjectPools[prefabId] = new Stack<GameObject>();
        }
        // PhotonのPrefabPoolをこのクラスに設定
        PhotonNetwork.PrefabPool = this; 
    }

    GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //prefabIdとprefabsの配列内に同じオブジェクト名があるか
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            Stack<GameObject> objectPool = inactiveObjectPools[prefabId];
            GameObject obj;

            if (objectPool.Count > 0)
            {
                // 非アクティブなオブジェクトを再利用
                obj = objectPool.Pop(); 
                obj.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                // 新しいオブジェクトを生成
                obj = Instantiate(prefab, position, rotation);
                obj.SetActive(false);
            }
            return obj;
        }
        else
        {
            Debug.LogError($"Prefab with ID '{prefabId}' not found in prefab pool.");
            return null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        // プレハブの名前をIDとして使用
        string prefabId = gameObject.name; 

        if (inactiveObjectPools.ContainsKey(prefabId))
        {
            // オブジェクトを非アクティブなプールに戻す
            inactiveObjectPools[prefabId].Push(gameObject); 
        }
        else
        {
            Debug.LogWarning($"Prefab '{prefabId}' not recognized and not added back to the pool.");
        }
    }
}
