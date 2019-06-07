using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class title : MonoBehaviour
{
    public string scenename = "MainScene";

    public void Clickstart()
    {
        SceneManager.LoadScene(scenename);
    }

    public void Clickexit()
    {
        Application.Quit();
    }
}
