using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WhenKilledEnemys : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        if(!(other.CompareTag("Enemy") || other.CompareTag("Player")))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    
}
