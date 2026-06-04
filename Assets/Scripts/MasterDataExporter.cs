using System.IO;
using UnityEditor;
using UnityEngine;

public static class MasterDataExporter
{
    [MenuItem("Tools/DataCenter Sentinel/Generar datasets maestros")]
    public static void GenerarDatasetsMaestros()
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

        GenerarRobots(exportFolder);
        GenerarZonas(exportFolder);
        GenerarActivos(exportFolder);
        GenerarCatalogoIncidentes(exportFolder);

        Debug.Log(
            "Datasets maestros generados correctamente en Exports."
        );
    }

    private static void GenerarRobots(
        string exportFolder
    )
    {
        string contenido =
            "robot_id,model,simulation_speed_m_s,initial_battery_pct,status\n" +
            "R01,Unitree Go2 Virtual Twin,4.0,100,operational\n";

        File.WriteAllText(
            Path.Combine(
                exportFolder,
                "robots.csv"
            ),
            contenido
        );
    }

    private static void GenerarZonas(
        string exportFolder
    )
    {
        string contenido =
            "zone_id,destination,zone_type,criticality,coord_x,coord_z,asset_count\n" +
            "Z01,Sala Red,network,medium,-9.2,4.7,2\n" +
            "Z02,Sala Energia,electrical,high,-9.2,-4.7,3\n" +
            "Z03,Sala Cooling,cooling,high,9.2,4.7,3\n" +
            "Z04,Sala UPS,power_backup,high,9.2,-4.7,3\n" +
            "Z05,Rack Norte,server_rack,medium,4.6,5.3,6\n" +
            "Z06,Rack Sur,server_rack,medium,4.6,-5.3,6\n";

        File.WriteAllText(
            Path.Combine(
                exportFolder,
                "zonas_datacenter.csv"
            ),
            contenido
        );
    }

    private static void GenerarActivos(
        string exportFolder
    )
    {
        string contenido =
            "asset_id,zone_id,asset_name,asset_type,criticality\n" +
            "A001,Z01,Network_Cabinet_01,network_cabinet,medium\n" +
            "A002,Z01,Network_Cabinet_02,network_cabinet,medium\n" +
            "A003,Z02,Tablero_01,electrical_panel,high\n" +
            "A004,Z02,Tablero_02,electrical_panel,high\n" +
            "A005,Z02,Transformador_01,transformer,high\n" +
            "A006,Z03,CRAC_01,cooling_unit,high\n" +
            "A007,Z03,CRAC_02,cooling_unit,high\n" +
            "A008,Z03,CRAC_03,cooling_unit,high\n" +
            "A009,Z04,UPS_Main,ups,high\n" +
            "A010,Z04,Battery_Module_01,battery_module,high\n" +
            "A011,Z04,Battery_Module_02,battery_module,high\n" +
            "A012,Z05,Racks_Norte,server_rack_group,medium\n" +
            "A013,Z06,Racks_Sur,server_rack_group,medium\n";

        File.WriteAllText(
            Path.Combine(
                exportFolder,
                "activos_datacenter.csv"
            ),
            contenido
        );
    }

    private static void GenerarCatalogoIncidentes(
        string exportFolder
    )
    {
        string contenido =
            "incident_type_ground_truth,description,base_severity,primary_signal\n" +
            "normal_operation,Operacion normal,low,none\n" +
            "cooling_failure,Falla de refrigeracion,high,temperature_humidity\n" +
            "electrical_instability,Inestabilidad electrica,high,vibration_temperature\n" +
            "rack_overheating,Sobrecalentamiento de rack,medium,temperature\n" +
            "network_overload,Sobrecarga de red,medium,noise_temperature\n" +
            "access_obstruction,Bloqueo de acceso,medium,obstacles\n" +
            "multiple_anomalies,Varias anomalias simultaneas,critical,multiple\n";

        File.WriteAllText(
            Path.Combine(
                exportFolder,
                "catalogo_incidentes.csv"
            ),
            contenido
        );
    }
}