using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarborGoal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            Debug.Log("YOU WINNNN");
            collision.gameObject.SetActive(false);
        }
    }
}
