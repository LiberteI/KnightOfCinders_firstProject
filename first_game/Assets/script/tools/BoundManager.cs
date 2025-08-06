using UnityEngine;

public class BoundManager : MonoBehaviour
{
    public Skeleton skeleton;
    public GameObject bound2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(skeleton.parameter.isDead == true){
            BoxCollider2D box = bound2.GetComponent<BoxCollider2D>();
            box.isTrigger = true;
        }
    }
}
