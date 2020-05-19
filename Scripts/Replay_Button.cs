using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay_Button : MonoBehaviour {

    public void Replay()
    {
        SceneManager.LoadScene(1);
    }
}
