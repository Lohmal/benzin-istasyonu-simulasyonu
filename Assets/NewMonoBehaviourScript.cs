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
        closedRotation = barrier.localRotation;
        openRotation = Quaternion.Euler(
            barrier.localEulerAngles.x,
            barrier.localEulerAngles.y,
            openAngle
        );
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

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