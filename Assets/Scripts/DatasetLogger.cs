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
    private string currentZoneType;
    private string currentZoneCriticality;
    private string currentIncidentType;

    private int currentObstacleCount;
    private Vector3 currentDestinationPosition;

    private float missionStartTime;
    private float nextLogTime;
    private float batteryPct;

    private float anomalyFactor;
    private float temperatureBoost;
    private float humidityBoost;
    private float noiseBoost;
    private float vibrationBoost;

    private int incidentBaseScore;

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

        // Exporta los CSV afuera de Assets para que Unity
        // no intente reimportarlos mientras se modifican.
        dataFolder = Path.GetFullPath(
            Path.Combine(
                Application.dataPath,
                "..",
                "Exports"
            )
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
            "mission_id,destination,zone_type,zone_criticality,incident_type_ground_truth,obstacle_count,duration_seconds,completed,battery_initial_pct,battery_final_pct,max_temperature_c,max_humidity_pct,max_noise_db,max_vibration,event_count,requires_intervention,risk_level\n"
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

        ConfigurarZona();
        ConfigurarIncidenteOculto(missionNumber);

        // Hace reproducibles las pequeñas variaciones
        // ambientales de cada misión.
        UnityEngine.Random.InitState(
            missionNumber * 911
        );

        missionActive = true;

        RegistrarEvento(
            "mision_iniciada",
            "informativa",
            currentObstacleCount
        );

        Debug.Log(
            $"{currentMissionId}: escenario oculto → " +
            $"{currentIncidentType}. Zona → " +
            $"{currentZoneType} ({currentZoneCriticality})."
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

        int finalRiskScore =
            CalcularRiskScore(completed);

        string requiresIntervention =
            finalRiskScore >= 3
                ? "yes"
                : "no";

        string riskLevel;

        if (finalRiskScore <= 1)
        {
            riskLevel = "low";
        }
        else if (finalRiskScore <= 3)
        {
            riskLevel = "medium";
        }
        else
        {
            riskLevel = "high";
        }

        string line = string.Join(
            ",",
            currentMissionId,
            currentDestinationName,
            currentZoneType,
            currentZoneCriticality,
            currentIncidentType,
            currentObstacleCount.ToString(),
            Formatear(durationSeconds),
            completed ? "yes" : "no",
            Formatear(initialBatteryPct),
            Formatear(batteryPct),
            Formatear(maxTemperature),
            Formatear(maxHumidity),
            Formatear(maxNoise),
            Formatear(maxVibration),
            eventCount.ToString(),
            requiresIntervention,
            riskLevel
        );

        File.AppendAllText(
            missionsPath,
            line + "\n"
        );

        missionActive = false;

        Debug.Log(
            $"{currentMissionId}: dataset guardado. " +
            $"Intervención → {requiresIntervention}. " +
            $"Riesgo → {riskLevel}."
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

    private void ConfigurarZona()
    {
        switch (currentDestinationName)
        {
            case "Sala Red":
                currentZoneType =
                    "network";

                currentZoneCriticality =
                    "medium";

                break;

            case "Sala Energia":
                currentZoneType =
                    "electrical";

                currentZoneCriticality =
                    "high";

                break;

            case "Sala Cooling":
                currentZoneType =
                    "cooling";

                currentZoneCriticality =
                    "high";

                break;

            case "Sala UPS":
                currentZoneType =
                    "power_backup";

                currentZoneCriticality =
                    "high";

                break;

            case "Rack Norte":
            case "Rack Sur":
                currentZoneType =
                    "server_rack";

                currentZoneCriticality =
                    "medium";

                break;

            default:
                currentZoneType =
                    "general";

                currentZoneCriticality =
                    "low";

                break;
        }
    }

    private void ConfigurarIncidenteOculto(
        int missionNumber
    )
    {
        System.Random random =
            new System.Random(
                missionNumber * 733 +
                currentObstacleCount * 17
            );

        anomalyFactor =
            0.45f +
            (float) random.NextDouble() *
            0.45f;

        temperatureBoost = 0f;
        humidityBoost = 0f;
        noiseBoost = 0f;
        vibrationBoost = 0f;
        incidentBaseScore = 0;

        int normalThreshold =
            currentZoneCriticality == "high"
                ? 28
                : 40;

        int roll =
            random.Next(0, 100);

        if (roll < normalThreshold)
        {
            currentIncidentType =
                "normal_operation";

            return;
        }

        if (
            currentObstacleCount >= 4 &&
            roll % 4 == 0
        )
        {
            currentIncidentType =
                "access_obstruction";

            incidentBaseScore = 2;

            return;
        }

        if (roll >= 88)
        {
            currentIncidentType =
                "multiple_anomalies";

            temperatureBoost = 7f;
            humidityBoost = 8f;
            noiseBoost = 9f;
            vibrationBoost = 3.5f;

            incidentBaseScore = 5;

            return;
        }

        switch (currentZoneType)
        {
            case "cooling":
                currentIncidentType =
                    "cooling_failure";

                temperatureBoost = 6f;
                humidityBoost = 10f;
                noiseBoost = 4f;

                incidentBaseScore = 4;

                break;

            case "electrical":
            case "power_backup":
                currentIncidentType =
                    "electrical_instability";

                temperatureBoost = 4f;
                noiseBoost = 5f;
                vibrationBoost = 2.5f;

                incidentBaseScore = 3;

                break;

            case "network":
                currentIncidentType =
                    "network_overload";

                temperatureBoost = 3f;
                noiseBoost = 11f;

                incidentBaseScore = 2;

                break;

            case "server_rack":
                currentIncidentType =
                    "rack_overheating";

                temperatureBoost = 7f;
                noiseBoost = 3f;
                vibrationBoost = 1f;

                incidentBaseScore = 3;

                break;

            default:
                currentIncidentType =
                    "environmental_anomaly";

                temperatureBoost = 4f;
                humidityBoost = 5f;

                incidentBaseScore = 2;

                break;
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
                batteryPct -
                batteryDrain
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
            (
                10f *
                anomalyFactor +
                temperatureBoost
            ) +
            UnityEngine.Random.Range(
                -0.7f,
                0.7f
            );

        float humidity =
            45f +
            proximity *
            (
                16f *
                anomalyFactor +
                humidityBoost
            ) +
            UnityEngine.Random.Range(
                -1.8f,
                1.8f
            );

        float noise =
            52f +
            proximity *
            (
                20f *
                anomalyFactor +
                noiseBoost
            ) +
            UnityEngine.Random.Range(
                -2.5f,
                2.5f
            );

        float vibration =
            1.2f +
            proximity *
            (
                4.5f *
                anomalyFactor +
                vibrationBoost
            ) +
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

    private int CalcularRiskScore(
        bool completed
    )
    {
        int score =
            incidentBaseScore;

        if (
            currentZoneCriticality ==
            "high" &&
            currentIncidentType !=
            "normal_operation"
        )
        {
            score += 1;
        }

        if (currentObstacleCount >= 4)
        {
            score += 1;
        }

        if (!completed)
        {
            score += 4;
        }

        if (maxTemperature >= 35f)
        {
            score += 1;
        }

        if (maxVibration >= 5.3f)
        {
            score += 1;
        }

        return score;
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