using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 오브젝트 풀링을 위한 기능들을 구현한 스크립트입니다.


// 풀링을 미리 인스펙터창에서 설정합니다.
// 프리팹의 이름이 key값으로 지정되어 사용됩니다.
// 풀링을 받아오기 위해 ObjectPoolManager.Instance.Get 을 사용합니다.

[Serializable]
public struct PoolData
{
    public Transform parent;
    public GameObject prefab;
    public int defaultCapacity;

    public PoolData(Transform parent, string key, GameObject prefab, int defaultCapacity)
    {
        this.parent = parent;
        this.prefab = prefab;
        this.defaultCapacity = defaultCapacity;
    }
}

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField]
    private PoolData[] poolDatas;

    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    public static ObjectPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        foreach (var pooldata in poolDatas)
        {
            string key = pooldata.prefab.name;

            ObjectPool pool;
            if (pooldata.parent == null)
                pool = new ObjectPool(transform, pooldata.prefab, pooldata.defaultCapacity);
            else
                pool = new ObjectPool(pooldata.parent, pooldata.prefab, pooldata.defaultCapacity);

            pools.Add(key, pool);
        }
    }

    // count 크기만큼 풀 오브젝트를 생성합니다
    public void Create(string key, int count)
    {
        pools[key].Create(count);
    }

    public GameObject Get(string key)
    {
        return pools[key].Get();
    }
    public T Get<T>(string key)
    {
        return pools[key].Get().GetComponent<T>();
    }

    public GameObject Get(string key, Vector3 position)
    {
        return pools[key].Get(position);
    }

    public GameObject Get(string key, Transform parent)
    {
        return pools[key].Get(parent);
    }
    public GameObject Get(string key, Transform parent, Vector3 position)
    {
        return pools[key].Get(parent, position);
    }

}

[System.Serializable]
public class ObjectPool
{
    public Transform parent;
    public GameObject prefab = null;
    public Queue<GameObject> queue = new Queue<GameObject>();

    public ObjectPool(Transform parent, GameObject prefab, int capacity)
    {
        this.prefab = prefab;
        for (int i = 0; i < capacity; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.GetComponent<Poolable>()?.Init(o => Return(o));
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public void Create(int count)
    {
        while (queue.Count < count)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.GetComponent<Poolable>()?.Init(o => Return(o));
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    int count = 0;
    public GameObject Get()
    {
        GameObject obj;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            Debug.Log($"{prefab.name}을 인게임중 생성하였습니다 - {++count}개");
            obj = GameObject.Instantiate(prefab, parent);
            obj.GetComponent<Poolable>()?.Init(o => Return(o));
        }

        obj.SetActive(true);
        obj.GetComponent<Poolable>().OnSpawn();
        return obj;
    }

    public GameObject Get(Vector3 position)
    {
        GameObject obj = Get();
        obj.transform.position = position;
        return obj;
    }

    public GameObject Get(Transform parent)
    {
        GameObject obj = Get();
        obj.transform.SetParent(parent, false);
        return obj;
    }

    public GameObject Get(Transform parent, Vector3 position)
    {
        GameObject obj = Get();
        obj.transform.SetParent(parent);
        obj.transform.localPosition = position;
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.GetComponent<Poolable>().OnDespawn();
        queue.Enqueue(obj);
    }
}