using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fall : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PermUI.perm.health--; 
            PermUI.perm.Reset();
            if (PermUI.perm.health <= 0)
            {
                PermUI.perm.health = 5;
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
        }
    }
}
