using UnityEngine;
using UnityEngine.AI;

public class RobotMover : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;

    public bool HasArrived
    {
        get
        {
            if (
                agent == null ||
                target == null ||
                agent.pathPending
            )
            {
                return false;
            }

            // Medimos solamente X y Z.
            // Ignoramos la altura Y de la esfera de destino.
            Vector2 robotPosition =
                new Vector2(
                    transform.position.x,
                    transform.position.z
                );

            Vector2 targetPosition =
                new Vector2(
                    target.position.x,
                    target.position.z
                );

            float planarDistance =
                Vector2.Distance(
                    robotPosition,
                    targetPosition
                );

            return planarDistance <=
                agent.stoppingDistance + 0.35f;
        }
    }

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(
                "RobotMover: falta NavMeshAgent."
            );

            enabled = false;
            return;
        }

        agent.stoppingDistance = 0.25f;
    }

    public void MoveTo(
        Transform newTarget
    )
    {
        if (
            agent == null ||
            newTarget == null
        )
        {
            return;
        }

        target = newTarget;

        agent.isStopped = false;

        bool destinationAccepted =
            agent.SetDestination(
                target.position
            );

        if (!destinationAccepted)
        {
            Debug.LogWarning(
                "No se pudo calcular la ruta hacia: " +
                target.name
            );
        }
    }

    public void ResetRobot(
        Vector3 startPosition
    )
    {
        if (agent == null)
        {
            return;
        }

        agent.ResetPath();
        agent.Warp(startPosition);
        agent.isStopped = false;
    }
}