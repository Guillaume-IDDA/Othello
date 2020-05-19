using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {

    GameObject model;

    List<GameObject> pooledObjects = new List<GameObject>();

    void InstantiateModel()
    {
        GameObject go = Instantiate(model);
        go.AddComponent<Poolable>().pool = this;
        pooledObjects.Add(go);
        go.transform.parent = transform;
    }

    public void Init(GameObject _model, int _size)
    {
        model = _model;
        for (int i = 0; i < _size; i++)
        {
            InstantiateModel();
        }
    }

    public GameObject Get()
    {
        if (pooledObjects.Count == 0)
        {
            InstantiateModel();
        }

        GameObject go = pooledObjects[0];
        pooledObjects.RemoveAt(0);
        go.SetActive(true);
        return go;
    }

    public void Set(GameObject _go)
    {
        _go.SetActive(false);
        pooledObjects.Add(_go);
    }
}
