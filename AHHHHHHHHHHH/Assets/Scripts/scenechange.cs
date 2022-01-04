using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenechange : MonoBehaviour
{
    [SerializeField] private string sceneName;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        
        if (collision.gameObject.tag == "Player")
        {
             
            SceneManager.LoadScene(sceneName);
            collision.gameObject.transform.position = new Vector3(12, 2, 0);
        }
    }


}
