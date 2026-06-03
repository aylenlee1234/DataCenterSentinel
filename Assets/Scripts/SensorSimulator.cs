using System.Globalization;
using System.IO;
using UnityEngine;

public class SensorSimulator : MonoBehaviour
{
    public string missionId = "M0001";
    public float logIntervalSeconds = 0.5f;

    public Vector3 hotspotPosition = new Vector3(7.5f, 0f, 5.3f);
    public float hotspotRadius = 4f;

    public float CurrentTemperature { get; private set; }
    public float CurrentHumidity { get; private set; }
    public float CurrentNoise { get; private set; }
    public float CurrentVibration { get; private set; }
    public float CurrentHotspotDistance { get; private set; }

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

        CurrentHotspotDistance = Vector3.Distance(
            robotPosition,
            hotspotPosition
        );

        float intensity = Mathf.Clamp01(
            1f - CurrentHotspotDistance / hotspotRadius
        );

        CurrentTemperature =
            24f +
            intensity * 12f +
            Random.Range(-0.8f, 0.8f);

        CurrentHumidity =
            45f +
            intensity * 18f +
            Random.Range(-2f, 2f);

        CurrentNoise =
            52f +
            intensity * 22f +
            Random.Range(-3f, 3f);

        CurrentVibration =
            1.2f +
            intensity * 4.5f +
            Random.Range(-0.3f, 0.3f);

        string line = string.Join(",",
            missionId,
            elapsedTime.ToString("F2", CultureInfo.InvariantCulture),
            transform.position.x.ToString("F3", CultureInfo.InvariantCulture),
            transform.position.z.ToString("F3", CultureInfo.InvariantCulture),
            CurrentHotspotDistance.ToString("F3", CultureInfo.InvariantCulture),
            CurrentTemperature.ToString("F2", CultureInfo.InvariantCulture),
            CurrentHumidity.ToString("F2", CultureInfo.InvariantCulture),
            CurrentNoise.ToString("F2", CultureInfo.InvariantCulture),
            CurrentVibration.ToString("F2", CultureInfo.InvariantCulture)
        );

        File.AppendAllText(filePath, line + "\n");
    }
}