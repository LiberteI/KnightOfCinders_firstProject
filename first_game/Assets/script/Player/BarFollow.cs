using UnityEngine;

public class BarFollow : MonoBehaviour
{   
    [SerializeField] private Transform target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // follows target.
        Vector3 newPos = target.position;
        transform.position = newPos;

        // fix local scale to positive
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }
}
