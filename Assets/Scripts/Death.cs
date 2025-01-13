using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public string level;


    void Update()
    {
       if(Input.GetMouseButton(0))
        {
            SceneManager.LoadScene(level);
        }
    }
}
