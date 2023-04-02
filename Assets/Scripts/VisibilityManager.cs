using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    public List<string> lightTriggerObjectTags;
    public string layerNameIfVisible;
    public string layerNameIfNotVisible;
    public bool doDarken;
    public bool startDark;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (doDarken)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    void Start()
    {
        if (startDark)
        {
            makeNotVisible();
        }
        else
        {
            makeVisible();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(lightTriggerObjectTags.Contains(collision.gameObject.tag))
        {
            makeVisible();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (lightTriggerObjectTags.Contains(collision.gameObject.tag))
        {
            makeVisible();
        }
    }

    private void makeVisible()
    {
        gameObject.layer = LayerMask.NameToLayer(layerNameIfVisible);
        if(doDarken)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void makeNotVisible()
    {
        gameObject.layer = LayerMask.NameToLayer(layerNameIfNotVisible);
        if (doDarken)
        {
            spriteRenderer.color = Color.gray;
        }
    }
}
