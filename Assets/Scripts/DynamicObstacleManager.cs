using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicObstacleManager : MonoBehaviour
{
    public int minObstacles = 1;
    public int maxObstacles = 4;

    private readonly List<GameObject> activeObstacles =
        new List<GameObject>();

    private readonly List<Vector3> candidatePositions =
        new List<Vector3>
        {
            new Vector3(-5.5f, 0.5f, 0f),
            new Vector3(-3.0f, 0.5f, 0f),
            new Vector3(-0.8f, 0.5f, 0f),
            new Vector3(2.5f, 0.5f, 0f),
            new Vector3(5.5f, 0.5f, 0f),
            new Vector3(7.0f, 0.5f, 1.8f),
            new Vector3(0f, 0.5f, 2.0f),
            new Vector3(0f, 0.5f, -2.0f),
            new Vector3(3.0f, 0.5f, -1.7f)
        };

    public void GenerateObstaclesForMission(int missionNumber)
    {
        RemoveCurrentObstacles();

        // Una semilla distinta por misión:
        // cada misión genera una configuración reproducible.
        Random.InitState(missionNumber * 100);

        int obstacleCount = Random.Range(
            minObstacles,
            maxObstacles + 1
        );

        List<Vector3> availablePositions =
            new List<Vector3>(candidatePositions);

        for (int index = 0; index < obstacleCount; index++)
        {
            if (availablePositions.Count == 0)
            {
                break;
            }

            int selectedIndex = Random.Range(
                0,
                availablePositions.Count
            );

            Vector3 position =
                availablePositions[selectedIndex];

            availablePositions.RemoveAt(selectedIndex);

            CreateObstacle(index + 1, position);
        }

        Debug.Log(
            $"Obstáculos generados para misión {missionNumber}: " +
            activeObstacles.Count
        );
    }

    public int GetCurrentObstacleCount()
    {
        return activeObstacles.Count;
    }

    private void CreateObstacle(
        int obstacleNumber,
        Vector3 position
    )
    {
        GameObject obstacle =
            GameObject.CreatePrimitive(PrimitiveType.Cube);

        obstacle.name =
            $"DynamicObstacle_{obstacleNumber:00}";

        obstacle.transform.position = position;
        obstacle.transform.localScale =
            new Vector3(1.1f, 1f, 1.1f);

        Renderer renderer =
            obstacle.GetComponent<Renderer>();

        renderer.material.color =
            new Color(0.90f, 0.25f, 0.15f);

        NavMeshObstacle navObstacle =
            obstacle.AddComponent<NavMeshObstacle>();

        navObstacle.shape = NavMeshObstacleShape.Box;
        navObstacle.size = Vector3.one;
        navObstacle.carving = true;

        activeObstacles.Add(obstacle);
    }

    private void RemoveCurrentObstacles()
    {
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }

        activeObstacles.Clear();
    }
}