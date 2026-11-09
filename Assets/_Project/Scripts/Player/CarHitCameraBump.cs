using UnityEngine;

[DefaultExecutionOrder(1000)]
public class CarHitCameraBump : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;   // boşsa Main Camera otomatik bulunur
    [SerializeField] private string carTag = "Car";
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float magnitude = 0.15f;
    [SerializeField] private float cooldown = 0.3f;
    [SerializeField] private bool debugLog = true;

    private float bumpEndTime;
    private float lastBumpTime = -10f;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    // Araba (kinematic Rigidbody) sana çarparsa
    private void OnTriggerEnter(Collider other)
    {
        if (debugLog) Debug.Log("Trigger: " + other.name + " | tag: " + other.tag);
        if (IsCar(other)) Bump();
    }

    // Sen arabaya çarparsan (CharacterController)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsCar(hit.collider)) Bump();
    }

    // Tag, collider'da ya da parent'larından birinde olsa yeter
    private bool IsCar(Collider col)
    {
        if (col.CompareTag(carTag)) return true;
        Transform t = col.transform.parent;
        while (t != null)
        {
            if (t.CompareTag(carTag)) return true;
            t = t.parent;
        }
        return false;
    }

    public void Bump()
    {
        if (Time.time - lastBumpTime < cooldown) return;
        lastBumpTime = Time.time;
        bumpEndTime = Time.time + duration;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null || Time.time >= bumpEndTime) return;

        float fade = (bumpEndTime - Time.time) / duration;
        cameraTransform.position += Random.insideUnitSphere * magnitude * fade;
    }
}