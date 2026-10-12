using UnityEngine;
public class Forward2 : MonoBehaviour
{
    [SerializeField] private Transform pointC;
    [SerializeField] private float speed = 2.0f;
    private Vector3 Prometheus;
    private bool goingToC = true;
    void Start()
    {
        Prometheus = transform.position;
    }
    void Update()
    {
        Vector3 target = goingToC ? pointC.position : Prometheus;
        Vector3 p = transform.position;
        p.y = 2f;
        transform.position = p;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            goingToC = !goingToC;
            Debug.Log(name + " turns around");
        }
    }
}