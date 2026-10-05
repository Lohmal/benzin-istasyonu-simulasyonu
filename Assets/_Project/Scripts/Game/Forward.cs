using UnityEngine;
public class Forward : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2.0f;
    private Vector3 BlueCar;
    private bool goingToB = true;
    void Start()
    {
        BlueCar = transform.position;
    }
    void Update()
    {
        Vector3 target = goingToB ? pointB.position : BlueCar;
        Vector3 p = transform.position;
        p.y = 2f;
        transform.position = p;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            goingToB = !goingToB;
            
        }
    }
}