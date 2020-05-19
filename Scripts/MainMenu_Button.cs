using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Button : MonoBehaviour {

    [SerializeField] int idButton;

    public void StartGame()
    {
        SceneController.Instance.idButton = idButton;
        SceneManager.LoadScene(1);
    }
}
