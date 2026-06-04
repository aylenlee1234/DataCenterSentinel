using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class DynamicObstacleManager : MonoBehaviour
{
    public int minObstacles = 1;
    public int maxObstacles = 5;

    public bool clearFileWhenPlayStarts = true;

    private string obstacleFilePath;

    private readonly List<GameObject> activeObstacles =
        new List<GameObject>();

    private readonly List<Vector3> candidatePositions =
        new List<Vector3>
        {
            new Vector3(-8.0f, 0.5f, 0f),
            new Vector3(-5.5f, 0.5f, 0f),
            new Vector3(-3.0f, 0.5f, 0f),
            new Vector3(-0.5f, 0.5f, 0f),
            new Vector3(2.0f, 0.5f, 0f),
            new Vector3(4.5f, 0.5f, 0f),
            new Vector3(7.0f, 0.5f, 0f),

            new Vector3(-6.8f, 0.5f, 5.3f),
            new Vector3(-6.8f, 0.5f, -5.3f),
            new Vector3(0f, 0.5f, 5.3f),
            new Vector3(0f, 0.5f, -5.3f),
            new Vector3(6.8f, 0.5f, 5.3f),
            new Vector3(6.8f, 0.5f, -5.3f)
        };

    private readonly List<string> obstacleTypes =
        new List<string>
        {
            "caja_tecnica",
            "carrito_herramientas",
            "pallet",
            "cableado_temporal"
        };

    private void Awake()
    {
        string exportFolder =
            Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "Exports"
                )
            );

        Directory.CreateDirectory(
            exportFolder
        );

        obstacleFilePath =
            Path.Combine(
                exportFolder,
                "obstaculos_mision.csv"
            );

        if (
            clearFileWhenPlayStarts ||
            !File.Exists(obstacleFilePath)
        )
        {
            File.WriteAllText(
                obstacleFilePath,
                "obstacle_id,mission_id,obstacle_type,pos_x,pos_z,blocks_route,generates_detour\n"
            );
        }
    }

    public void GenerateObstaclesForMission(
        int missionNumber
    )
    {
        RemoveCurrentObstacles();

        // La misma misión siempre genera la misma
        // configuración para que sea reproducible.
        Random.InitState(
            missionNumber * 100
        );

        int obstacleCount =
            Random.Range(
                minObstacles,
                maxObstacles + 1
            );

        List<Vector3> availablePositions =
            new List<Vector3>(
                candidatePositions
            );

        for (
            int index = 0;
            index < obstacleCount;
            index++
        )
        {
            if (
                availablePositions.Count == 0
            )
            {
                break;
            }

            int selectedIndex =
                Random.Range(
                    0,
                    availablePositions.Count
                );

            Vector3 position =
                availablePositions[
                    selectedIndex
                ];

            availablePositions.RemoveAt(
                selectedIndex
            );

            CreateObstacle(
                missionNumber,
                index + 1,
                position
            );
        }

        Debug.Log(
            $"Obstáculos generados para misión " +
            $"{missionNumber}: " +
            activeObstacles.Count
        );
    }

    public int GetCurrentObstacleCount()
    {
        return activeObstacles.Count;
    }

    private void CreateObstacle(
        int missionNumber,
        int obstacleNumber,
        Vector3 position
    )
    {
        string obstacleType =
            obstacleTypes[
                Random.Range(
                    0,
                    obstacleTypes.Count
                )
            ];

        GameObject obstacle =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        obstacle.name =
            $"DynamicObstacle_" +
            $"{obstacleNumber:00}_" +
            obstacleType;

        obstacle.transform.position =
            position;

        obstacle.transform.localScale =
            GetObstacleScale(
                obstacleType
            );

        Renderer renderer =
            obstacle.GetComponent<Renderer>();

        renderer.material.color =
            GetObstacleColor(
                obstacleType
            );

        NavMeshObstacle navObstacle =
            obstacle.AddComponent<
                NavMeshObstacle
            >();

        navObstacle.shape =
            NavMeshObstacleShape.Box;

        navObstacle.size =
            Vector3.one;

        navObstacle.carving =
            true;

        activeObstacles.Add(
            obstacle
        );

        SaveObstacleRow(
            missionNumber,
            obstacleNumber,
            obstacleType,
            position
        );
    }

    private Vector3 GetObstacleScale(
        string obstacleType
    )
    {
        switch (obstacleType)
        {
            case "carrito_herramientas":
                return new Vector3(
                    1.30f,
                    0.85f,
                    0.85f
                );

            case "pallet":
                return new Vector3(
                    1.40f,
                    0.55f,
                    1.15f
                );

            case "cableado_temporal":
                return new Vector3(
                    1.60f,
                    0.25f,
                    0.65f
                );

            default:
                return new Vector3(
                    1.10f,
                    1.00f,
                    1.10f
                );
        }
    }

    private Color GetObstacleColor(
        string obstacleType
    )
    {
        switch (obstacleType)
        {
            case "carrito_herramientas":
                return new Color(
                    0.95f,
                    0.52f,
                    0.12f
                );

            case "pallet":
                return new Color(
                    0.60f,
                    0.32f,
                    0.14f
                );

            case "cableado_temporal":
                return new Color(
                    0.90f,
                    0.78f,
                    0.10f
                );

            default:
                return new Color(
                    0.90f,
                    0.25f,
                    0.15f
                );
        }
    }

    private void SaveObstacleRow(
        int missionNumber,
        int obstacleNumber,
        string obstacleType,
        Vector3 position
    )
    {
        string missionId =
            $"M{missionNumber:0000}";

        string obstacleId =
            $"O{missionNumber:0000}_" +
            $"{obstacleNumber:00}";

        bool blocksRoute =
            Mathf.Abs(position.z) <
            0.75f;

        bool generatesDetour =
            blocksRoute ||
            Mathf.Abs(position.z) >
            5f;

        string line =
            string.Join(
                ",",
                obstacleId,
                missionId,
                obstacleType,
                FormatNumber(position.x),
                FormatNumber(position.z),
                blocksRoute
                    ? "yes"
                    : "no",
                generatesDetour
                    ? "yes"
                    : "no"
            );

        File.AppendAllText(
            obstacleFilePath,
            line + "\n"
        );
    }

    private string FormatNumber(
        float value
    )
    {
        return value.ToString(
            "F3",
            CultureInfo.InvariantCulture
        );
    }

    private void RemoveCurrentObstacles()
    {
        foreach (
            GameObject obstacle
            in activeObstacles
        )
        {
            if (obstacle != null)
            {
                Destroy(
                    obstacle
                );
            }
        }

        activeObstacles.Clear();
    }
}