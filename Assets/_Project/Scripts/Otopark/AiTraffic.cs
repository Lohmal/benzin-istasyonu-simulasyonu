using UnityEngine;
using UnityEngine.AI;

public class AITraffic : MonoBehaviour
{
    [Header("Gezilecek Noktalar")]
    public Transform[] waypoints; // Arabanın gideceği yol noktaları

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Başlangıçta ilk hedefe yönlendir
        if (waypoints.Length > 0)
        {
            SetNextDestination();
        }
    }

    void Update()
    {
        // Araba henüz NavMesh yüzeyinde değilse koda devam etme
        if (!agent.isOnNavMesh) return;

        // Hedefe ulaştıysa sıradaki noktaya geç
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            SetNextDestination();
        }
    }

    void SetNextDestination()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        // Sıradaki noktaya geç, son noktadaysa başa dön (Döngü)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}