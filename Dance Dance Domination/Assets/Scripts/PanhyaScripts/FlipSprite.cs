using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    


        // Update is called once per frame
        void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = false;

        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = true;

        }
    }
}
