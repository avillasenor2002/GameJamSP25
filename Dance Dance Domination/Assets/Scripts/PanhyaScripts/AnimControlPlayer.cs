using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControlPlayer : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = FindObjectOfType<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("Jamming", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("Jamming", false);
        }
    }
}
