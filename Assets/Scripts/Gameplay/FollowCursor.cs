using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField] private Sprite clickSprite;
    //private Sprite pointSprite;
    //private SpriteRenderer spriteRenderer;
    void Start()
    {
        /*spriteRenderer = GetComponent<SpriteRenderer>();
        pointSprite = spriteRenderer.sprite;
        Cursor.visible = false;*/
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0.3f, 0);
        transform.position = cursorPos;
        /*if (Input.GetMouseButtonDown(0))
        {
            spriteRenderer.sprite = clickSprite;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            spriteRenderer.sprite = pointSprite;
        }*/
    }
}
