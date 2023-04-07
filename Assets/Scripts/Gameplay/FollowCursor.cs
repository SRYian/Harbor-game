using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] Collider2D boundingShape;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 boundedPos;
        boundedPos.x = Mathf.Clamp(cursorPos.x, boundingShape.bounds.min.x, boundingShape.bounds.max.x);
        boundedPos.y = Mathf.Clamp(cursorPos.y, boundingShape.bounds.min.y, boundingShape.bounds.max.y);
        transform.position = boundedPos;
        

    }
}
