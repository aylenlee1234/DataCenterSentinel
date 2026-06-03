using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EventLogger : MonoBehaviour
{
    public string missionId = "M0001";
    public float evaluationIntervalSeconds = 0.5f;

    private SensorSimulator sensors;
    private NavMeshAgent agent;

    private string filePath;
    private float elapsedTime;
    private float nextEvaluationTime;

    private bool temperatureLogged;
    private bool humidityLogged;
    private bool noiseLogged;
    private bool vibrationLogged;
    private bool arrivalLogged;

    private void Start()
    {
        sensors = GetComponent<SensorSimulator>();
        agent = GetComponent<NavMeshAgent>();

        if (sensors == null)
        {
            Debug.LogError("EventLogger: falta agregar SensorSimulator al robot.");
            enabled = false;
            return;
        }

        if (agent == null)
        {
            Debug.LogError("EventLogger: falta agregar NavMeshAgent al robot.");
            enabled = false;
            return;
        }

        string dataFolder = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        filePath = Path.Combine(dataFolder, "eventos_mision.csv");

        File.WriteAllText(
            filePath,
            "mission_id,elapsed_seconds,event_type,severity,value\n"
        );

        Debug.Log("Registrando eventos en: " + filePath);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= nextEvaluationTime)
        {
            EvaluarEventos();
            nextEvaluationTime = elapsedTime + evaluationIntervalSeconds;
        }
    }

    private void EvaluarEventos()
    {
        if (sensors.CurrentTemperature >= 32f && !temperatureLogged)
        {
            RegistrarEvento(
                "temperatura_elevada",
                sensors.CurrentTemperature >= 35f ? "alta" : "media",
                sensors.CurrentTemperature
            );

            temperatureLogged = true;
        }

        if (sensors.CurrentHumidity >= 58f && !humidityLogged)
        {
            RegistrarEvento(
                "humedad_elevada",
                sensors.CurrentHumidity >= 62f ? "alta" : "media",
                sensors.CurrentHumidity
            );

            humidityLogged = true;
        }

        if (sensors.CurrentNoise >= 68f && !noiseLogged)
        {
            RegistrarEvento(
                "ruido_anormal",
                sensors.CurrentNoise >= 73f ? "alta" : "media",
                sensors.CurrentNoise
            );

            noiseLogged = true;
        }

        if (sensors.CurrentVibration >= 4.5f && !vibrationLogged)
        {
            RegistrarEvento(
                "vibracion_anormal",
                sensors.CurrentVibration >= 5.3f ? "alta" : "media",
                sensors.CurrentVibration
            );

            vibrationLogged = true;
        }

        bool arrived =
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (arrived && !arrivalLogged)
        {
            RegistrarEvento(
                "destino_alcanzado",
                "informativa",
                agent.remainingDistance
            );

            arrivalLogged = true;

            Debug.Log("Eventos de misión generados correctamente.");
        }
    }

    private void RegistrarEvento(
        string eventType,
        string severity,
        float value
    )
    {
        string line = string.Join(",",
            missionId,
            elapsedTime.ToString("F2", CultureInfo.InvariantCulture),
            eventType,
            severity,
            value.ToString("F2", CultureInfo.InvariantCulture)
        );

        File.AppendAllText(filePath, line + "\n");
    }
}