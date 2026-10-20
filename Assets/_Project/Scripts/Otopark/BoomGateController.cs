
using UnityEngine;

public class BoomGateController : MonoBehaviour
{
    public Transform barrier;
    public float openAngle = -90f;
    public float speed = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (barrier == null)
        {
            Debug.LogError("Barrier atanmamis!");
            enabled = false;
            return;
        }

        closedRotation = barrier.localRotation;

        Vector3 angles = barrier.localEulerAngles;
        angles.z = openAngle;
        openRotation = Quaternion.Euler(angles);

        isOpen = false;
        barrier.localRotation = closedRotation;
    }

    void Update()
    {
        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        barrier.localRotation = Quaternion.Lerp(
            barrier.localRotation,
            targetRotation,
            speed * Time.deltaTime
        );
    }

    public void OpenGate()
    {
        isOpen = true;
    }

    public void CloseGate()
    {
        isOpen = false;
    }
}
