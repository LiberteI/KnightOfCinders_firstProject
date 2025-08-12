using UnityEngine;

public class IdlingGuards : MonoBehaviour
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
