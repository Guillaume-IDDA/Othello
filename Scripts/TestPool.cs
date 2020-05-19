using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour {

    [SerializeField] GameObject prefabCanonBall;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PoolManager.Instance.Get(prefabCanonBall);
        }
    }
}
