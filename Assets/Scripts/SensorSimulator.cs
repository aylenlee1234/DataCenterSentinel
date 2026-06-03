using System.Globalization;
using System.IO;
using UnityEngine;

public class SensorSimulator : MonoBehaviour
{
    public string missionId = "M0001";
    public float logIntervalSeconds = 0.5f;

    // Zona crítica simulada: cerca de la esfera verde
    public Vector3 hotspotPosition = new Vector3(7.5f, 0f, 5.3f);
    public float hotspotRadius = 4f;

    private string filePath;
    private float elapsedTime;
    private float nextLogTime;

    private void Start()
    {
        string dataFolder = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        filePath = Path.Combine(dataFolder, "lecturas_ambientales.csv");

        File.WriteAllText(
            filePath,
            "mission_id,elapsed_seconds,pos_x,pos_z,hotspot_distance,temperature_c,humidity_pct,noise_db,vibration\n"
        );

        Debug.Log("Registrando lecturas ambientales en: " + filePath);
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
        Vector3 robotPosition = new Vector3(
            transform.position.x,
            0f,
            transform.position.z
        );

        float distanceToHotspot = Vector3.Distance(
            robotPosition,
            hotspotPosition
        );

        float intensity = Mathf.Clamp01(
            1f - distanceToHotspot / hotspotRadius
        );

        float temperature =
            24f +
            intensity * 12f +
            Random.Range(-0.8f, 0.8f);

        float humidity =
            45f +
            intensity * 18f +
            Random.Range(-2f, 2f);

        float noise =
            52f +
            intensity * 22f +
            Random.Range(-3f, 3f);

        float vibration =
            1.2f +
            intensity * 4.5f +
            Random.Range(-0.3f, 0.3f);

        string line = string.Join(",",
            missionId,
            elapsedTime.ToString("F2", CultureInfo.InvariantCulture),
            transform.position.x.ToString("F3", CultureInfo.InvariantCulture),
            transform.position.z.ToString("F3", CultureInfo.InvariantCulture),
            distanceToHotspot.ToString("F3", CultureInfo.InvariantCulture),
            temperature.ToString("F2", CultureInfo.InvariantCulture),
            humidity.ToString("F2", CultureInfo.InvariantCulture),
            noise.ToString("F2", CultureInfo.InvariantCulture),
            vibration.ToString("F2", CultureInfo.InvariantCulture)
        );

        File.AppendAllText(filePath, line + "\n");
    }
}