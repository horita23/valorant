using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CustomPrefabPool : MonoBehaviour, IPunPrefabPool
{
    // �v���n�u�̔z��
    [Tooltip("�l�b�g�I�u�W�F�N�g")]
    [SerializeField]
    private GameObject[] prefabs; 

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, Stack<GameObject>> inactiveObjectPools = new Dictionary<string, Stack<GameObject>>();

    private void Start()
    {
        // �v���n�u�̔z�񂩂�ID�ƃv���n�u�������ɒǉ�
        foreach (var prefab in prefabs)
        {
            // �v���n�u�̖��O��ID�Ƃ��Ďg�p
            string prefabId = prefab.name; 
            prefabDictionary[prefabId] = prefab;
            inactiveObjectPools[prefabId] = new Stack<GameObject>();
        }
        // Photon��PrefabPool�����̃N���X�ɐݒ�
        PhotonNetwork.PrefabPool = this; 
    }

    GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //prefabId��prefabs�̔z����ɓ����I�u�W�F�N�g�������邩
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            Stack<GameObject> objectPool = inactiveObjectPools[prefabId];
            GameObject obj;

            if (objectPool.Count > 0)
            {
                // ��A�N�e�B�u�ȃI�u�W�F�N�g���ė��p
                obj = objectPool.Pop(); 
                obj.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                // �V�����I�u�W�F�N�g�𐶐�
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
        // �v���n�u�̖��O��ID�Ƃ��Ďg�p
        string prefabId = gameObject.name; 

        if (inactiveObjectPools.ContainsKey(prefabId))
        {
            // �I�u�W�F�N�g���A�N�e�B�u�ȃv�[���ɖ߂�
            inactiveObjectPools[prefabId].Push(gameObject); 
        }
        else
        {
            Debug.LogWarning($"Prefab '{prefabId}' not recognized and not added back to the pool.");
        }
    }
}
