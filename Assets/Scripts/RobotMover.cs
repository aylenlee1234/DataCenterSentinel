using UnityEngine;
using UnityEngine.AI;

public class RobotMover : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("El robot no tiene un componente NavMeshAgent.");
            enabled = false;
            return;
        }

        if (target == null)
        {
            Debug.LogError("No se asignó un destino al robot.");
            enabled = false;
            return;
        }

        agent.SetDestination(target.position);
    }
}