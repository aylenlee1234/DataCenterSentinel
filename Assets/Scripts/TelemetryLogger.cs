using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class TelemetryLogger : MonoBehaviour
{
    public string missionId = "M0001";
    public float logIntervalSeconds = 0.5f;

    private NavMeshAgent agent;
    private string filePath;
    private float elapsedTime;
    private float nextLogTime;
    private bool arrivalLogged;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("TelemetryLogger: el robot no tiene NavMeshAgent.");
            enabled = false;
            return;
        }

        string dataFolder = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        filePath = Path.Combine(dataFolder, "telemetria_robot.csv");

        File.WriteAllText(
            filePath,
            "mission_id,elapsed_seconds,pos_x,pos_y,pos_z,speed,remaining_distance,arrived\n"
        );

        Debug.Log("Registrando telemetría en: " + filePath);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= nextLogTime)
        {
            RegistrarLectura();
            nextLogTime = elapsedTime + logIntervalSeconds;
        }
    }

    private void RegistrarLectura()
    {
        float remainingDistance = agent.hasPath
            ? agent.remainingDistance
            : 0f;

        bool arrived =
            !agent.pathPending &&
            agent.hasPath &&
            remainingDistance <= agent.stoppingDistance + 0.1f;

        string line = string.Join(",",
            missionId,
            elapsedTime.ToString("F2", CultureInfo.InvariantCulture),
            transform.position.x.ToString("F3", CultureInfo.InvariantCulture),
            transform.position.y.ToString("F3", CultureInfo.InvariantCulture),
            transform.position.z.ToString("F3", CultureInfo.InvariantCulture),
            agent.velocity.magnitude.ToString("F3", CultureInfo.InvariantCulture),
            remainingDistance.ToString("F3", CultureInfo.InvariantCulture),
            arrived ? "yes" : "no"
        );

        File.AppendAllText(filePath, line + "\n");

        if (arrived && !arrivalLogged)
        {
            arrivalLogged = true;
            Debug.Log("Misión completada. CSV generado correctamente.");
        }
    }
}
