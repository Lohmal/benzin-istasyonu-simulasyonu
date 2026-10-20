using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public BoomGateController gate;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Giren obje: " + other.name);

        // Player'in kendisi veya alt objesi olabilir.
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            if (gate != null)
                gate.OpenGate();
            else
                Debug.LogError("Gate baglantisi yapilmamis!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            if (gate != null)
                gate.CloseGate();
        }
    }
}
