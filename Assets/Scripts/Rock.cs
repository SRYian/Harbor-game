using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public bool isVisible;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update\

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
    }

    public void HideRock()
    {
        isVisible = false;
        gameObject.layer = LayerMask.NameToLayer("Darken");
        if(spriteRenderer)
            spriteRenderer.color = Color.gray;
    }

    public void ShowRock()
    {
        isVisible = true;
        gameObject.layer = LayerMask.NameToLayer("Obstacles");
        if (spriteRenderer)
            spriteRenderer.color = Color.white;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Light"))
        {
            ShowRock();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Light"))
        {
            HideRock();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
