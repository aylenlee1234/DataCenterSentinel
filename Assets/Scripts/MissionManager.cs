using System.Collections;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public int totalMissions = 12;
    public float pauseBetweenMissions = 1.5f;
    public float maxMissionDuration = 30f;

    private RobotMover robotMover;
    private DynamicObstacleManager obstacleManager;
    private DatasetLogger datasetLogger;

    private GameObject destinationMarker;
    private Renderer destinationRenderer;

    private readonly Vector3 startPosition =
        new Vector3(-8f, 0f, 0f);

    private readonly Vector3[] destinationPositions =
    {
        new Vector3(-4.5f, 0.35f, 5.2f),
        new Vector3(1.5f, 0.35f, -5.2f),
        new Vector3(7.5f, 0.35f, 5.3f),
        new Vector3(4.5f, 0.35f, -5.2f)
    };

    private readonly string[] destinationNames =
    {
        "Rack Norte",
        "Sala Energia",
        "Sala Cooling",
        "Rack Sur"
    };

    private readonly Color[] destinationColors =
    {
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };

    private void Start()
    {
        robotMover =
            GetComponent<RobotMover>();

        obstacleManager =
            GetComponent<
                DynamicObstacleManager
            >();

        datasetLogger =
            GetComponent<DatasetLogger>();

        if (robotMover == null)
        {
            Debug.LogError(
                "MissionManager: falta RobotMover."
            );

            enabled = false;
            return;
        }

        if (obstacleManager == null)
        {
            Debug.LogError(
                "MissionManager: falta DynamicObstacleManager."
            );

            enabled = false;
            return;
        }

        if (datasetLogger == null)
        {
            Debug.LogError(
                "MissionManager: falta DatasetLogger."
            );

            enabled = false;
            return;
        }

        CrearBolaDestino();

        StartCoroutine(
            EjecutarMisiones()
        );
    }

    private void CrearBolaDestino()
    {
        destinationMarker =
            GameObject.CreatePrimitive(
                PrimitiveType.Sphere
            );

        destinationMarker.name =
            "ActiveDestinationMarker";

        destinationMarker
            .transform
            .localScale =
            Vector3.one * 0.75f;

        destinationRenderer =
            destinationMarker
                .GetComponent<Renderer>();

        Collider markerCollider =
            destinationMarker
                .GetComponent<Collider>();

        if (markerCollider != null)
        {
            Destroy(markerCollider);
        }
    }

    private void MoverBolaDestino(
        int destinationIndex
    )
    {
        destinationMarker
            .transform
            .position =
            destinationPositions[
                destinationIndex
            ];

        destinationRenderer
            .material
            .color =
            destinationColors[
                destinationIndex
            ];

        destinationMarker.name =
            "ActiveDestinationMarker_" +
            destinationNames[
                destinationIndex
            ].Replace(" ", "");
    }

    private IEnumerator EjecutarMisiones()
    {
        for (
            int missionIndex = 0;
            missionIndex <
                totalMissions;
            missionIndex++
        )
        {
            int missionNumber =
                missionIndex + 1;

            string missionId =
                $"M{missionNumber:0000}";

            int destinationIndex =
                missionIndex %
                destinationPositions.Length;

            string destinationName =
                destinationNames[
                    destinationIndex
                ];

            MoverBolaDestino(
                destinationIndex
            );

            robotMover.ResetRobot(
                startPosition
            );

            obstacleManager
                .GenerateObstaclesForMission(
                    missionNumber
                );

            yield return
                new WaitForSeconds(
                    0.7f
                );

            robotMover.MoveTo(
                destinationMarker.transform
            );

            datasetLogger.BeginMission(
                missionId,
                destinationName,
                obstacleManager
                    .GetCurrentObstacleCount(),
                destinationMarker
                    .transform
                    .position,
                missionNumber
            );

            Debug.Log(
                $"{missionId}: destino → " +
                $"{destinationName}. " +
                $"Obstáculos dinámicos → " +
                obstacleManager
                    .GetCurrentObstacleCount()
            );

            float elapsedTime = 0f;

            while (
                !robotMover.HasArrived &&
                elapsedTime <
                    maxMissionDuration
            )
            {
                elapsedTime +=
                    Time.deltaTime;

                yield return null;
            }

            bool completed =
                robotMover.HasArrived;

            datasetLogger.EndMission(
                completed
            );

            if (completed)
            {
                Debug.Log(
                    $"{missionId}: completada en " +
                    $"{elapsedTime:F2} segundos."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"{missionId}: timeout."
                );
            }

            yield return
                new WaitForSeconds(
                    pauseBetweenMissions
                );
        }

        Debug.Log(
            "Todas las misiones finalizaron."
        );
    }
}