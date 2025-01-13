using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private Death exit;
    public string level = "Level 2";
      
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            exit.level = level;
            SceneManager.LoadScene(level);

        }
    }
    
}
