using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

    [System.Serializable]
    public class InitModel
    {
        public GameObject model;
        public int initSize;
    }

    Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();

    public List<InitModel> models;

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        models.ForEach(x => {
            GameObject go = new GameObject("Pool - " + x.model.name);
            Pool pool = go.AddComponent<Pool>();
            pool.Init(x.model, x.initSize);
            pools[x.model] = pool;
            pool.transform.parent = transform;
        }
        );
    }

    public void Register(GameObject _model, Pool _pool)
    {
        pools[_model] = _pool;
    }

    public GameObject Get(GameObject _model)
    {
        return pools[_model].Get();
    }
}
