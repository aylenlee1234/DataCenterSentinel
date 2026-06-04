using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class DatasetLogger : MonoBehaviour
{
    // =====================================================
    // CONFIGURACIÓN
    // =====================================================

    public float logIntervalSeconds = 0.5f;
    public float initialBatteryPct = 100f;
    public bool clearFilesWhenPlayStarts = true;

    public string robotId = "R01";

    // =====================================================
    // COMPONENTES Y ARCHIVOS
    // =====================================================

    private NavMeshAgent agent;

    private string dataFolder;
    private string missionsPath;
    private string telemetryPath;
    private string sensorsPath;
    private string eventsPath;
    private string iotAlertsPath;

    // =====================================================
    // MISIÓN ACTUAL
    // =====================================================

    private bool missionActive;

    private string currentMissionId;
    private string currentDestinationName;
    private string currentZoneId;
    private string currentZoneType;
    private string currentZoneCriticality;
    private string currentIncidentType;

    private int currentObstacleCount;
    private Vector3 currentDestinationPosition;

    // =====================================================
    // ALERTA IOT ACTUAL
    // =====================================================

    private string currentAlertId;
    private string currentAlertType;
    private string currentAlertSeverity;

    private float currentAlertSensorValue;
    private float currentAlertThreshold;

    // =====================================================
    // TIEMPO, BATERÍA E INCIDENTE
    // =====================================================

    private float missionStartTime;
    private float nextLogTime;
    private float batteryPct;

    private float anomalyFactor;
    private float temperatureBoost;
    private float humidityBoost;
    private float noiseBoost;
    private float vibrationBoost;

    private int incidentBaseScore;

    // =====================================================
    // VALORES MÁXIMOS
    // =====================================================

    private float maxTemperature;
    private float maxHumidity;
    private float maxNoise;
    private float maxVibration;

    // =====================================================
    // EVENTOS
    // =====================================================

    private int eventCount;

    private bool temperatureEventLogged;
    private bool humidityEventLogged;
    private bool noiseEventLogged;
    private bool vibrationEventLogged;
    private bool batteryEventLogged;

    // =====================================================
    // INICIALIZACIÓN
    // =====================================================

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

        // Los CSV se guardan afuera de Assets.
        // Esto evita que Unity intente reimportarlos mientras cambian.
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

        iotAlertsPath = Path.Combine(
            dataFolder,
            "alertas_iot.csv"
        );

        PrepararArchivos();
    }

    // =====================================================
    // PREPARAR CSV
    // =====================================================

    private void PrepararArchivos()
    {
        PrepararArchivo(
            missionsPath,
            "mission_id,alert_id,robot_id,zone_id,destination,zone_type,zone_criticality,incident_type_ground_truth,obstacle_count,duration_seconds,completed,battery_initial_pct,battery_final_pct,max_temperature_c,max_humidity_pct,max_noise_db,max_vibration,event_count,requires_intervention,risk_level\n"
        );

        PrepararArchivo(
            telemetryPath,
            "mission_id,robot_id,elapsed_seconds,pos_x,pos_y,pos_z,speed,remaining_distance,battery_pct\n"
        );

        PrepararArchivo(
            sensorsPath,
            "mission_id,robot_id,elapsed_seconds,destination,hotspot_distance,temperature_c,humidity_pct,noise_db,vibration\n"
        );

        PrepararArchivo(
            eventsPath,
            "mission_id,robot_id,elapsed_seconds,event_type,severity,value\n"
        );

        PrepararArchivo(
            iotAlertsPath,
            "alert_id,mission_id,robot_id,zone_id,alert_type,sensor_value,threshold,alert_severity,robot_dispatched,validation_result\n"
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

    // =====================================================
    // INICIO DE MISIÓN
    // =====================================================

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
        ConfigurarAlertaIoT(missionNumber);

        // Hace reproducibles las variaciones ambientales.
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
            $"{currentMissionId}: alerta IoT → " +
            $"{currentAlertId}, {currentAlertType}. " +
            $"Robot enviado a {currentDestinationName}."
        );
    }

    // =====================================================
    // FIN DE MISIÓN
    // =====================================================

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
            Time.time -
            missionStartTime;

        int finalRiskScore =
            CalcularRiskScore(
                completed
            );

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

        // La alerta IoT se valida con la inspección física.
        string validationResult =
            currentIncidentType ==
            "normal_operation"
                ? "false_positive"
                : "confirmed";

        GuardarAlertaIoT(
            validationResult
        );

        string line = string.Join(
            ",",
            currentMissionId,
            currentAlertId,
            robotId,
            currentZoneId,
            currentDestinationName,
            currentZoneType,
            currentZoneCriticality,
            currentIncidentType,
            currentObstacleCount.ToString(),
            Formatear(durationSeconds),
            completed
                ? "yes"
                : "no",
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
            $"{currentMissionId}: alerta IoT → " +
            $"{validationResult}. " +
            $"Intervención técnica → {requiresIntervention}. " +
            $"Riesgo → {riskLevel}."
        );
    }

    // =====================================================
    // REGISTRO PERIÓDICO
    // =====================================================

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

    // =====================================================
    // CONFIGURAR ZONA
    // =====================================================

    private void ConfigurarZona()
    {
        switch (currentDestinationName)
        {
            case "Sala Red":
                currentZoneId = "Z01";
                currentZoneType = "network";
                currentZoneCriticality = "medium";
                break;

            case "Sala Energia":
                currentZoneId = "Z02";
                currentZoneType = "electrical";
                currentZoneCriticality = "high";
                break;

            case "Sala Cooling":
                currentZoneId = "Z03";
                currentZoneType = "cooling";
                currentZoneCriticality = "high";
                break;

            case "Sala UPS":
                currentZoneId = "Z04";
                currentZoneType = "power_backup";
                currentZoneCriticality = "high";
                break;

            case "Rack Norte":
                currentZoneId = "Z05";
                currentZoneType = "server_rack";
                currentZoneCriticality = "medium";
                break;

            case "Rack Sur":
                currentZoneId = "Z06";
                currentZoneType = "server_rack";
                currentZoneCriticality = "medium";
                break;

            default:
                currentZoneId = "Z00";
                currentZoneType = "general";
                currentZoneCriticality = "low";
                break;
        }
    }

    // =====================================================
    // INCIDENTE OCULTO
    // =====================================================

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
            random.Next(
                0,
                100
            );

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

    // =====================================================
    // ALERTA IOT PREVIA
    // =====================================================

    private void ConfigurarAlertaIoT(
        int missionNumber
    )
    {
        System.Random random =
            new System.Random(
                missionNumber * 1291 +
                currentObstacleCount * 29
            );

        currentAlertId =
            $"AL{missionNumber:0000}";

        switch (currentIncidentType)
        {
            case "cooling_failure":
                currentAlertType =
                    ElegirValor(
                        random,
                        "temperatura_elevada",
                        "humedad_elevada"
                    );

                break;

            case "electrical_instability":
                currentAlertType =
                    ElegirValor(
                        random,
                        "vibracion_anormal",
                        "temperatura_elevada"
                    );

                break;

            case "rack_overheating":
                currentAlertType =
                    "temperatura_elevada";

                break;

            case "network_overload":
                currentAlertType =
                    "ruido_anormal";

                break;

            case "access_obstruction":
                currentAlertType =
                    "obstruccion_detectada";

                break;

            case "multiple_anomalies":
                currentAlertType =
                    ElegirValor(
                        random,
                        "temperatura_elevada",
                        "vibracion_anormal",
                        "ruido_anormal"
                    );

                break;

            default:
                // Cuando la operación es normal, se simula
                // una alerta IoT levemente elevada:
                // después el robot la descarta como falso positivo.
                currentAlertType =
                    ElegirValor(
                        random,
                        "temperatura_elevada",
                        "humedad_elevada",
                        "ruido_anormal",
                        "vibracion_anormal"
                    );

                break;
        }

        currentAlertThreshold =
            ObtenerUmbral(
                currentAlertType
            );

        if (
            currentAlertType ==
            "obstruccion_detectada"
        )
        {
            currentAlertSensorValue =
                Mathf.Max(
                    1,
                    currentObstacleCount
                );
        }
        else
        {
            bool isFalsePositive =
                currentIncidentType ==
                "normal_operation";

            float excess =
                CalcularExcesoAlerta(
                    random,
                    currentAlertType,
                    isFalsePositive
                );

            currentAlertSensorValue =
                currentAlertThreshold +
                excess;
        }

        currentAlertSeverity =
            CalcularSeveridadAlerta(
                currentAlertType,
                currentAlertSensorValue,
                currentAlertThreshold
            );
    }

    private string ElegirValor(
        System.Random random,
        params string[] values
    )
    {
        return values[
            random.Next(
                0,
                values.Length
            )
        ];
    }

    private float ObtenerUmbral(
        string alertType
    )
    {
        switch (alertType)
        {
            case "temperatura_elevada":
                return 32f;

            case "humedad_elevada":
                return 58f;

            case "ruido_anormal":
                return 68f;

            case "vibracion_anormal":
                return 4.5f;

            case "obstruccion_detectada":
                return 1f;

            default:
                return 0f;
        }
    }

    private float CalcularExcesoAlerta(
        System.Random random,
        string alertType,
        bool isFalsePositive
    )
    {
        if (isFalsePositive)
        {
            return RandomEntre(
                random,
                0.15f,
                1.20f
            );
        }

        switch (alertType)
        {
            case "temperatura_elevada":
                return RandomEntre(
                    random,
                    2.0f,
                    7.5f
                );

            case "humedad_elevada":
                return RandomEntre(
                    random,
                    2.0f,
                    10.0f
                );

            case "ruido_anormal":
                return RandomEntre(
                    random,
                    2.0f,
                    12.0f
                );

            case "vibracion_anormal":
                return RandomEntre(
                    random,
                    0.6f,
                    3.0f
                );

            default:
                return 1f;
        }
    }

    private string CalcularSeveridadAlerta(
        string alertType,
        float sensorValue,
        float threshold
    )
    {
        if (
            alertType ==
            "obstruccion_detectada"
        )
        {
            return sensorValue >= 4f
                ? "high"
                : "medium";
        }

        float difference =
            sensorValue -
            threshold;

        switch (alertType)
        {
            case "temperatura_elevada":
                return difference >= 3f
                    ? "high"
                    : "medium";

            case "humedad_elevada":
                return difference >= 5f
                    ? "high"
                    : "medium";

            case "ruido_anormal":
                return difference >= 6f
                    ? "high"
                    : "medium";

            case "vibracion_anormal":
                return difference >= 1.5f
                    ? "high"
                    : "medium";

            default:
                return "medium";
        }
    }

    // =====================================================
    // CREAR LECTURA
    // =====================================================

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

        Vector3 robotPosition =
            new Vector3(
                transform.position.x,
                0f,
                transform.position.z
            );

        Vector3 destinationPosition =
            new Vector3(
                currentDestinationPosition.x,
                0f,
                currentDestinationPosition.z
            );

        float hotspotDistance =
            Vector3.Distance(
                robotPosition,
                destinationPosition
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

    // =====================================================
    // TELEMETRÍA, SENSORES Y EVENTOS
    // =====================================================

    private void GuardarTelemetria(
        float elapsedSeconds,
        float speed,
        float remainingDistance
    )
    {
        string line = string.Join(
            ",",
            currentMissionId,
            robotId,
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
            robotId,
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
            robotId,
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

    // =====================================================
    // ALERTAS IOT
    // =====================================================

    private void GuardarAlertaIoT(
        string validationResult
    )
    {
        string line = string.Join(
            ",",
            currentAlertId,
            currentMissionId,
            robotId,
            currentZoneId,
            currentAlertType,
            Formatear(currentAlertSensorValue),
            Formatear(currentAlertThreshold),
            currentAlertSeverity,
            "yes",
            validationResult
        );

        File.AppendAllText(
            iotAlertsPath,
            line + "\n"
        );
    }

    // =====================================================
    // RIESGO
    // =====================================================

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

        if (
            currentObstacleCount >= 4
        )
        {
            score += 1;
        }

        if (!completed)
        {
            score += 4;
        }

        if (
            maxTemperature >= 35f
        )
        {
            score += 1;
        }

        if (
            maxVibration >= 5.3f
        )
        {
            score += 1;
        }

        return score;
    }

    // =====================================================
    // AUXILIARES
    // =====================================================

    private float RandomEntre(
        System.Random random,
        float min,
        float max
    )
    {
        return min +
            (float) random.NextDouble() *
            (max - min);
    }

    private string Formatear(
        float value
    )
    {
        return value.ToString(
            "F3",
            CultureInfo.InvariantCulture
        );
    }
}