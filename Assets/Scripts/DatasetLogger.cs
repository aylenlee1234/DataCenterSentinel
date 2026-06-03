using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class DatasetLogger : MonoBehaviour
{
    public float logIntervalSeconds = 0.5f;
    public float initialBatteryPct = 100f;
    public bool clearFilesWhenPlayStarts = true;

    private NavMeshAgent agent;

    private string dataFolder;
    private string missionsPath;
    private string telemetryPath;
    private string sensorsPath;
    private string eventsPath;

    private bool missionActive;
    private string currentMissionId;
    private string currentDestinationName;
    private int currentObstacleCount;
    private int currentMissionNumber;

    private Vector3 currentDestinationPosition;

    private float missionStartTime;
    private float nextLogTime;
    private float batteryPct;
    private float anomalyFactor;

    private float maxTemperature;
    private float maxHumidity;
    private float maxNoise;
    private float maxVibration;

    private int eventCount;

    private bool temperatureEventLogged;
    private bool humidityEventLogged;
    private bool noiseEventLogged;
    private bool vibrationEventLogged;
    private bool batteryEventLogged;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(
                "DatasetLogger: falta NavMeshAgent en el robot."
            );

            enabled = false;
            return;
        }

        dataFolder = Path.Combine(
            Application.dataPath,
            "Data"
        );

        Directory.CreateDirectory(dataFolder);

        missionsPath = Path.Combine(
            dataFolder,
            "misiones_robot.csv"
        );

        telemetryPath = Path.Combine(
            dataFolder,
            "telemetria_robot.csv"
        );

        sensorsPath = Path.Combine(
            dataFolder,
            "lecturas_ambientales.csv"
        );

        eventsPath = Path.Combine(
            dataFolder,
            "eventos_mision.csv"
        );

        PrepararArchivos();
    }

    private void PrepararArchivos()
    {
        PrepararArchivo(
            missionsPath,
            "mission_id,destination,obstacle_count,duration_seconds,completed,battery_initial_pct,battery_final_pct,max_temperature_c,max_humidity_pct,max_noise_db,max_vibration,event_count\n"
        );

        PrepararArchivo(
            telemetryPath,
            "mission_id,elapsed_seconds,pos_x,pos_y,pos_z,speed,remaining_distance,battery_pct\n"
        );

        PrepararArchivo(
            sensorsPath,
            "mission_id,elapsed_seconds,destination,hotspot_distance,temperature_c,humidity_pct,noise_db,vibration\n"
        );

        PrepararArchivo(
            eventsPath,
            "mission_id,elapsed_seconds,event_type,severity,value\n"
        );
    }

    private void PrepararArchivo(
        string filePath,
        string header
    )
    {
        if (
            clearFilesWhenPlayStarts ||
            !File.Exists(filePath)
        )
        {
            File.WriteAllText(
                filePath,
                header
            );
        }
    }

    public void BeginMission(
        string missionId,
        string destinationName,
        int obstacleCount,
        Vector3 destinationPosition,
        int missionNumber
    )
    {
        currentMissionId = missionId;
        currentDestinationName = destinationName;
        currentObstacleCount = obstacleCount;
        currentDestinationPosition = destinationPosition;
        currentMissionNumber = missionNumber;

        missionStartTime = Time.time;
        nextLogTime = Time.time;

        batteryPct = initialBatteryPct;

        maxTemperature = 0f;
        maxHumidity = 0f;
        maxNoise = 0f;
        maxVibration = 0f;

        eventCount = 0;

        temperatureEventLogged = false;
        humidityEventLogged = false;
        noiseEventLogged = false;
        vibrationEventLogged = false;
        batteryEventLogged = false;

        // Cada misión tiene una intensidad diferente.
        // Así no todas generan exactamente las mismas lecturas.
        System.Random random =
            new System.Random(
                missionNumber * 733
            );

        anomalyFactor =
            0.45f +
            (float) random.NextDouble() * 0.95f;

        missionActive = true;

        RegistrarEvento(
            "mision_iniciada",
            "informativa",
            obstacleCount
        );

        Debug.Log(
            $"{missionId}: comenzó el registro del dataset."
        );
    }

    public void EndMission(bool completed)
    {
        if (!missionActive)
        {
            return;
        }

        RegistrarLectura();

        if (completed)
        {
            RegistrarEvento(
                "destino_alcanzado",
                "informativa",
                1f
            );
        }
        else
        {
            RegistrarEvento(
                "mision_interrumpida",
                "alta",
                1f
            );
        }

        float durationSeconds =
            Time.time - missionStartTime;

        string line = string.Join(
            ",",
            currentMissionId,
            currentDestinationName,
            currentObstacleCount.ToString(),
            Formatear(durationSeconds),
            completed ? "yes" : "no",
            Formatear(initialBatteryPct),
            Formatear(batteryPct),
            Formatear(maxTemperature),
            Formatear(maxHumidity),
            Formatear(maxNoise),
            Formatear(maxVibration),
            eventCount.ToString()
        );

        File.AppendAllText(
            missionsPath,
            line + "\n"
        );

        missionActive = false;

        Debug.Log(
            $"{currentMissionId}: resumen guardado en misiones_robot.csv."
        );
    }

    private void Update()
    {
        if (!missionActive)
        {
            return;
        }

        if (Time.time >= nextLogTime)
        {
            RegistrarLectura();

            nextLogTime =
                Time.time +
                logIntervalSeconds;
        }
    }

    private void RegistrarLectura()
    {
        float elapsedSeconds =
            Time.time -
            missionStartTime;

        float remainingDistance =
            agent.hasPath
                ? agent.remainingDistance
                : Vector3.Distance(
                    transform.position,
                    currentDestinationPosition
                );

        float speed =
            agent.velocity.magnitude;

        float batteryDrain =
            (
                speed * 0.020f +
                currentObstacleCount * 0.003f
            ) *
            logIntervalSeconds;

        batteryPct =
            Mathf.Max(
                0f,
                batteryPct - batteryDrain
            );

        Vector3 planarRobotPosition =
            new Vector3(
                transform.position.x,
                0f,
                transform.position.z
            );

        Vector3 planarDestinationPosition =
            new Vector3(
                currentDestinationPosition.x,
                0f,
                currentDestinationPosition.z
            );

        float hotspotDistance =
            Vector3.Distance(
                planarRobotPosition,
                planarDestinationPosition
            );

        float proximity =
            Mathf.Clamp01(
                1f -
                hotspotDistance /
                4.5f
            );

        float temperature =
            24f +
            proximity *
            10f *
            anomalyFactor +
            UnityEngine.Random.Range(
                -0.7f,
                0.7f
            );

        float humidity =
            45f +
            proximity *
            16f *
            anomalyFactor +
            UnityEngine.Random.Range(
                -1.8f,
                1.8f
            );

        float noise =
            52f +
            proximity *
            20f *
            anomalyFactor +
            UnityEngine.Random.Range(
                -2.5f,
                2.5f
            );

        float vibration =
            1.2f +
            proximity *
            4.5f *
            anomalyFactor +
            UnityEngine.Random.Range(
                -0.25f,
                0.25f
            );

        maxTemperature =
            Mathf.Max(
                maxTemperature,
                temperature
            );

        maxHumidity =
            Mathf.Max(
                maxHumidity,
                humidity
            );

        maxNoise =
            Mathf.Max(
                maxNoise,
                noise
            );

        maxVibration =
            Mathf.Max(
                maxVibration,
                vibration
            );

        GuardarTelemetria(
            elapsedSeconds,
            speed,
            remainingDistance
        );

        GuardarLecturaAmbiental(
            elapsedSeconds,
            hotspotDistance,
            temperature,
            humidity,
            noise,
            vibration
        );

        EvaluarEventos(
            temperature,
            humidity,
            noise,
            vibration
        );
    }

    private void GuardarTelemetria(
        float elapsedSeconds,
        float speed,
        float remainingDistance
    )
    {
        string line = string.Join(
            ",",
            currentMissionId,
            Formatear(elapsedSeconds),
            Formatear(transform.position.x),
            Formatear(transform.position.y),
            Formatear(transform.position.z),
            Formatear(speed),
            Formatear(remainingDistance),
            Formatear(batteryPct)
        );

        File.AppendAllText(
            telemetryPath,
            line + "\n"
        );
    }

    private void GuardarLecturaAmbiental(
        float elapsedSeconds,
        float hotspotDistance,
        float temperature,
        float humidity,
        float noise,
        float vibration
    )
    {
        string line = string.Join(
            ",",
            currentMissionId,
            Formatear(elapsedSeconds),
            currentDestinationName,
            Formatear(hotspotDistance),
            Formatear(temperature),
            Formatear(humidity),
            Formatear(noise),
            Formatear(vibration)
        );

        File.AppendAllText(
            sensorsPath,
            line + "\n"
        );
    }

    private void EvaluarEventos(
        float temperature,
        float humidity,
        float noise,
        float vibration
    )
    {
        if (
            temperature >= 32f &&
            !temperatureEventLogged
        )
        {
            RegistrarEvento(
                "temperatura_elevada",
                temperature >= 35f
                    ? "alta"
                    : "media",
                temperature
            );

            temperatureEventLogged = true;
        }

        if (
            humidity >= 58f &&
            !humidityEventLogged
        )
        {
            RegistrarEvento(
                "humedad_elevada",
                humidity >= 62f
                    ? "alta"
                    : "media",
                humidity
            );

            humidityEventLogged = true;
        }

        if (
            noise >= 68f &&
            !noiseEventLogged
        )
        {
            RegistrarEvento(
                "ruido_anormal",
                noise >= 73f
                    ? "alta"
                    : "media",
                noise
            );

            noiseEventLogged = true;
        }

        if (
            vibration >= 4.5f &&
            !vibrationEventLogged
        )
        {
            RegistrarEvento(
                "vibracion_anormal",
                vibration >= 5.3f
                    ? "alta"
                    : "media",
                vibration
            );

            vibrationEventLogged = true;
        }

        if (
            batteryPct <= 25f &&
            !batteryEventLogged
        )
        {
            RegistrarEvento(
                "bateria_baja",
                "alta",
                batteryPct
            );

            batteryEventLogged = true;
        }
    }

    private void RegistrarEvento(
        string eventType,
        string severity,
        float value
    )
    {
        float elapsedSeconds =
            Time.time -
            missionStartTime;

        string line = string.Join(
            ",",
            currentMissionId,
            Formatear(elapsedSeconds),
            eventType,
            severity,
            Formatear(value)
        );

        File.AppendAllText(
            eventsPath,
            line + "\n"
        );

        eventCount++;
    }

    private string Formatear(float value)
    {
        return value.ToString(
            "F3",
            CultureInfo.InvariantCulture
        );
    }
}