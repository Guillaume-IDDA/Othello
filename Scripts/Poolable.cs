using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour {

    public Pool pool;

    private void OnDisable()
    {
        pool.Set(gameObject);
    }
}
