using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PermUI : MonoBehaviour
{
    public int gems = 0;
    public int health = 5;
    public Text gemText;
    public Text healthAmount;

    public static PermUI perm;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (!perm)
        {
            perm = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        gems = 0;
        gemText.text = gems.ToString();
    }

    public void ResetAll()
    {
        Reset();
        health = 5;
        healthAmount.text = health.ToString();
    }
}
