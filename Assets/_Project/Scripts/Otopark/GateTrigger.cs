using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public BoomGateController gate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gate.OpenGate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gate.CloseGate();
        }
    }
}