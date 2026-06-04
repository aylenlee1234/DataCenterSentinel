using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;

public static class DataCenterSceneBuilder
{
    [MenuItem("Tools/DataCenter Sentinel/Generar data center completo")]
    public static void GenerarEscenario()
    {
        // =====================================================
        // ESCENA NUEVA
        // =====================================================

        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects,
            NewSceneMode.Single
        );

        CrearCarpetaSiNoExiste("Assets", "Scenes");
        CrearCarpetaSiNoExiste("Assets", "Materials");

        // =====================================================
        // MATERIALES GENERALES
        // =====================================================

        Material pisoMaterial = CrearMaterial(
            "Piso",
            new Color(0.08f, 0.11f, 0.16f)
        );

        Material paredMaterial = CrearMaterial(
            "Pared",
            new Color(0.25f, 0.30f, 0.38f)
        );

        Material pasilloMaterial = CrearMaterial(
            "Pasillo",
            new Color(0.15f, 0.20f, 0.28f)
        );

        Material entradaMaterial = CrearMaterial(
            "Entrada",
            new Color(0.16f, 0.70f, 0.38f)
        );

        Material obstaculoMaterial = CrearMaterial(
            "Obstaculo",
            new Color(0.82f, 0.22f, 0.16f)
        );

        Material robotMaterial = CrearMaterial(
            "RobotPlaceholder",
            new Color(0.62f, 0.22f, 0.82f)
        );

        Material metalOscuroMaterial = CrearMaterial(
            "MetalOscuro",
            new Color(0.06f, 0.08f, 0.12f)
        );

        Material metalMedioMaterial = CrearMaterial(
            "MetalMedio",
            new Color(0.15f, 0.19f, 0.25f)
        );

        Material negroMaterial = CrearMaterial(
            "Negro",
            new Color(0.02f, 0.03f, 0.05f)
        );

        Material blancoMaterial = CrearMaterial(
            "Blanco",
            new Color(0.88f, 0.92f, 0.96f)
        );

        Material grisClaroMaterial = CrearMaterial(
            "GrisClaro",
            new Color(0.46f, 0.52f, 0.60f)
        );

        // =====================================================
        // MATERIALES EMISIVOS
        // =====================================================

        Material ledVerdeMaterial = CrearMaterialEmisivo(
            "LedVerde",
            new Color(0.15f, 1.00f, 0.45f),
            4f
        );

        Material ledAzulMaterial = CrearMaterialEmisivo(
            "LedAzul",
            new Color(0.10f, 0.58f, 1.00f),
            4f
        );

        Material ledRojoMaterial = CrearMaterialEmisivo(
            "LedRojo",
            new Color(1.00f, 0.14f, 0.10f),
            4f
        );

        Material ledAmarilloMaterial = CrearMaterialEmisivo(
            "LedAmarillo",
            new Color(1.00f, 0.72f, 0.12f),
            4f
        );

        Material luzTechoMaterial = CrearMaterialEmisivo(
            "LuzTecho",
            new Color(0.90f, 0.96f, 1.00f),
            2.8f
        );

        // =====================================================
        // MATERIALES DE ZONAS
        // =====================================================

        Material coolingMaterial = CrearMaterial(
            "Cooling",
            new Color(0.10f, 0.62f, 0.78f)
        );

        Material upsMaterial = CrearMaterial(
            "UPS",
            new Color(0.84f, 0.56f, 0.10f)
        );

        Material energiaMaterial = CrearMaterial(
            "Energia",
            new Color(0.82f, 0.22f, 0.14f)
        );

        Material redMaterial = CrearMaterial(
            "Red",
            new Color(0.22f, 0.66f, 0.30f)
        );

        Material pasilloFrioMaterial = CrearMaterialEmisivo(
            "PasilloFrio",
            new Color(0.06f, 0.38f, 0.92f),
            1.8f
        );

        Material pasilloCalienteMaterial = CrearMaterialEmisivo(
            "PasilloCaliente",
            new Color(0.92f, 0.16f, 0.08f),
            1.8f
        );

        Material bordeSeguridadMaterial = CrearMaterialEmisivo(
            "BordeSeguridad",
            new Color(1.00f, 0.68f, 0.08f),
            1.7f
        );

        // =====================================================
        // CONTENEDOR PRINCIPAL
        // =====================================================

        GameObject dataCenter =
            new GameObject("DataCenter");

        // =====================================================
        // GEOMETRÍA FÍSICA PARA NAVMESH
        // Solo piso, paredes, obstáculos y bloqueadores invisibles.
        // =====================================================

        CrearCuboFisico(
            dataCenter.transform,
            "Floor",
            new Vector3(0f, -0.10f, 0f),
            new Vector3(26f, 0.20f, 18f),
            pisoMaterial
        );

        CrearCuboFisico(
            dataCenter.transform,
            "Wall_North",
            new Vector3(0f, 1.60f, 8.90f),
            new Vector3(26f, 3.20f, 0.20f),
            paredMaterial
        );

        CrearCuboFisico(
            dataCenter.transform,
            "Wall_South",
            new Vector3(0f, 1.60f, -8.90f),
            new Vector3(26f, 3.20f, 0.20f),
            paredMaterial
        );

        CrearCuboFisico(
            dataCenter.transform,
            "Wall_East",
            new Vector3(12.90f, 1.60f, 0f),
            new Vector3(0.20f, 3.20f, 18f),
            paredMaterial
        );

        CrearCuboFisico(
            dataCenter.transform,
            "Wall_West",
            new Vector3(-12.90f, 1.60f, 0f),
            new Vector3(0.20f, 3.20f, 18f),
            paredMaterial
        );

        // =====================================================
        // BLOQUEADORES INVISIBLES DE RACKS
        // Le indican al NavMesh que el robot no puede atravesarlos.
        // =====================================================

        float[] posicionesX =
        {
            -5.8f,
            -3.2f,
            -0.6f,
            2.0f,
            4.6f,
            7.2f
        };

        foreach (float x in posicionesX)
        {
            CrearBloqueadorInvisible(
                dataCenter.transform,
                $"Blocker_Rack_Norte_{x}",
                new Vector3(x, 1.50f, 3.10f),
                new Vector3(1.35f, 3.00f, 1.60f)
            );

            CrearBloqueadorInvisible(
                dataCenter.transform,
                $"Blocker_Rack_Sur_{x}",
                new Vector3(x, 1.50f, -3.10f),
                new Vector3(1.35f, 3.00f, 1.60f)
            );
        }

        // =====================================================
        // BLOQUEADORES INVISIBLES DE SALAS TÉCNICAS
        // =====================================================

        // Sala de red.
        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Network_01",
            new Vector3(-10.2f, 1.05f, 6.15f),
            new Vector3(1.15f, 2.10f, 1.00f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Network_02",
            new Vector3(-8.70f, 1.05f, 6.15f),
            new Vector3(1.15f, 2.10f, 1.00f)
        );

        // Sala de energía.
        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Energia_01",
            new Vector3(-10.2f, 1.00f, -6.15f),
            new Vector3(1.15f, 2.00f, 1.00f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Energia_02",
            new Vector3(-8.70f, 1.00f, -6.15f),
            new Vector3(1.15f, 2.00f, 1.00f)
        );

        // Sala Cooling.
        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Cooling_01",
            new Vector3(9.00f, 1.00f, 6.30f),
            new Vector3(1.00f, 2.00f, 1.10f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Cooling_02",
            new Vector3(10.35f, 1.00f, 6.30f),
            new Vector3(1.00f, 2.00f, 1.10f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_Cooling_03",
            new Vector3(11.70f, 1.00f, 6.30f),
            new Vector3(1.00f, 2.00f, 1.10f)
        );

        // Sala UPS.
        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_UPS_01",
            new Vector3(9.30f, 1.05f, -6.20f),
            new Vector3(1.10f, 2.10f, 1.00f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_UPS_02",
            new Vector3(10.75f, 1.05f, -6.20f),
            new Vector3(1.10f, 2.10f, 1.00f)
        );

        CrearBloqueadorInvisible(
            dataCenter.transform,
            "Blocker_UPS_03",
            new Vector3(11.85f, 1.05f, -6.20f),
            new Vector3(0.75f, 2.10f, 1.00f)
        );

        // =====================================================
        // OBSTÁCULOS FIJOS
        // =====================================================

        CrearCuboFisico(
            dataCenter.transform,
            "Caja_Tecnica_Fija",
            new Vector3(-1.0f, 0.40f, 1.00f),
            new Vector3(0.90f, 0.80f, 0.90f),
            obstaculoMaterial
        );

        CrearCuboFisico(
            dataCenter.transform,
            "Herramientas_Mantenimiento",
            new Vector3(5.50f, 0.35f, -1.10f),
            new Vector3(1.10f, 0.70f, 0.80f),
            obstaculoMaterial
        );

        // =====================================================
        // ROBOT PROVISIONAL
        // =====================================================

        GameObject robot =
            new GameObject("UnitreeGo2_Placeholder");

        robot.transform.position =
            new Vector3(-11f, 0f, 0f);

        GameObject visual =
            GameObject.CreatePrimitive(
                PrimitiveType.Capsule
            );

        visual.name =
            "Visual";

        visual.transform.SetParent(
            robot.transform
        );

        visual.transform.localPosition =
            new Vector3(0f, 0.50f, 0f);

        visual.transform.localScale =
            new Vector3(0.65f, 0.50f, 0.65f);

        visual
            .GetComponent<Renderer>()
            .sharedMaterial =
            robotMaterial;

        Object.DestroyImmediate(
            visual.GetComponent<Collider>()
        );

        NavMeshAgent agent =
            robot.AddComponent<NavMeshAgent>();

        agent.speed = 4f;
        agent.angularSpeed = 260f;
        agent.acceleration = 10f;
        agent.radius = 0.35f;
        agent.height = 1.10f;
        agent.stoppingDistance = 0.25f;

        robot.AddComponent<RobotMover>();
        robot.AddComponent<DynamicObstacleManager>();
        robot.AddComponent<DatasetLogger>();
        robot.AddComponent<MissionManager>();

        // =====================================================
        // NAVMESH
        // =====================================================

        GameObject navigation =
            new GameObject("Navigation");

        NavMeshSurface surface =
    navigation.AddComponent<NavMeshSurface>();

// Solo se usa la capa Default.
// La decoración queda ignorada.
surface.layerMask =
    LayerMask.GetMask("Default");

// IMPORTANTE:
// Usa los colliders físicos para reconocer los
// bloqueadores invisibles de racks y salas técnicas.
surface.useGeometry =
    NavMeshCollectGeometry.PhysicsColliders;

surface.BuildNavMesh();

        // =====================================================
        // DECORACIÓN GENERAL
        // Se crea después del NavMesh para no interferir.
        // =====================================================

        CrearCuboDecorativo(
            dataCenter.transform,
            "Pasillo_Central",
            new Vector3(0f, 0.02f, 0f),
            new Vector3(23f, 0.04f, 1.60f),
            pasilloMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Pasillo_Norte",
            new Vector3(0f, 0.02f, 5.40f),
            new Vector3(23f, 0.04f, 1.30f),
            pasilloMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Pasillo_Sur",
            new Vector3(0f, 0.02f, -5.40f),
            new Vector3(23f, 0.04f, 1.30f),
            pasilloMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Zona_Entrada",
            new Vector3(-11.20f, 0.03f, 0f),
            new Vector3(2.00f, 0.06f, 2.00f),
            entradaMaterial
        );

        // Pasillos fríos.
        CrearCuboDecorativo(
            dataCenter.transform,
            "Linea_Fria_Norte",
            new Vector3(0f, 0.055f, 4.25f),
            new Vector3(18.50f, 0.025f, 0.20f),
            pasilloFrioMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Linea_Fria_Sur",
            new Vector3(0f, 0.055f, -4.25f),
            new Vector3(18.50f, 0.025f, 0.20f),
            pasilloFrioMaterial
        );

        // Pasillos calientes.
        CrearCuboDecorativo(
            dataCenter.transform,
            "Linea_Caliente_Norte",
            new Vector3(0f, 0.055f, 2.05f),
            new Vector3(18.50f, 0.025f, 0.20f),
            pasilloCalienteMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Linea_Caliente_Sur",
            new Vector3(0f, 0.055f, -2.05f),
            new Vector3(18.50f, 0.025f, 0.20f),
            pasilloCalienteMaterial
        );

        // =====================================================
        // RACKS REALISTAS
        // =====================================================

        int numeroRack = 1;

        foreach (float x in posicionesX)
        {
            CrearRackVisual(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 0f, 3.10f),
                1f,
                metalOscuroMaterial,
                metalMedioMaterial,
                ledVerdeMaterial
            );

            numeroRack++;

            CrearRackVisual(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 0f, -3.10f),
                -1f,
                metalOscuroMaterial,
                metalMedioMaterial,
                ledVerdeMaterial
            );

            numeroRack++;
        }

        // Bandejas superiores.
        CrearCuboDecorativo(
            dataCenter.transform,
            "Bandeja_Cableado_Norte",
            new Vector3(0f, 3.80f, 3.10f),
            new Vector3(19f, 0.16f, 0.35f),
            metalOscuroMaterial
        );

        CrearCuboDecorativo(
            dataCenter.transform,
            "Bandeja_Cableado_Sur",
            new Vector3(0f, 3.80f, -3.10f),
            new Vector3(19f, 0.16f, 0.35f),
            metalOscuroMaterial
        );

        // =====================================================
        // SALAS TÉCNICAS REALISTAS
        // =====================================================

        CrearSalaRed(
            dataCenter.transform,
            redMaterial,
            metalOscuroMaterial,
            metalMedioMaterial,
            ledVerdeMaterial,
            ledAzulMaterial
        );

        CrearSalaEnergia(
            dataCenter.transform,
            energiaMaterial,
            metalOscuroMaterial,
            metalMedioMaterial,
            ledRojoMaterial,
            bordeSeguridadMaterial
        );

        CrearSalaCooling(
            dataCenter.transform,
            coolingMaterial,
            metalOscuroMaterial,
            grisClaroMaterial,
            ledAzulMaterial
        );

        CrearSalaUPS(
            dataCenter.transform,
            upsMaterial,
            metalOscuroMaterial,
            metalMedioMaterial,
            ledAmarilloMaterial,
            bordeSeguridadMaterial
        );

        // =====================================================
        // DETALLES EXTRA
        // =====================================================

        CrearMarcoPuerta(
            dataCenter.transform,
            "Puerta_Entrada",
            new Vector3(-12.72f, 0f, 0f),
            metalMedioMaterial,
            ledVerdeMaterial
        );

        CrearMatafuego(
            dataCenter.transform,
            "Matafuego_01",
            new Vector3(-12.35f, 0.75f, 2.10f),
            energiaMaterial,
            negroMaterial
        );

        CrearCamaraSeguridad(
            dataCenter.transform,
            "Camara_01",
            new Vector3(-11.90f, 2.65f, 7.90f),
            metalOscuroMaterial,
            ledRojoMaterial
        );

        CrearLucesDeTecho(
            dataCenter.transform,
            luzTechoMaterial
        );

        // =====================================================
        // CARTELES FLOTANTES
        // =====================================================

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_Entrada",
            "ENTRADA",
            new Vector3(-11.20f, 2.20f, 0f),
            Color.white
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaRed",
            "SALA DE RED",
            new Vector3(-9.50f, 3.05f, 5.80f),
            new Color(0.55f, 1.00f, 0.60f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaEnergia",
            "SALA DE ENERGIA",
            new Vector3(-9.50f, 3.05f, -5.80f),
            new Color(1.00f, 0.55f, 0.45f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaCooling",
            "SALA COOLING",
            new Vector3(10.10f, 3.05f, 5.80f),
            new Color(0.45f, 0.95f, 1.00f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaUPS",
            "SALA UPS",
            new Vector3(10.10f, 3.05f, -5.80f),
            new Color(1.00f, 0.85f, 0.35f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_RacksNorte",
            "RACKS NORTE",
            new Vector3(1.00f, 4.30f, 3.10f),
            new Color(0.55f, 0.80f, 1.00f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_RacksSur",
            "RACKS SUR",
            new Vector3(1.00f, 4.30f, -3.10f),
            new Color(0.55f, 0.80f, 1.00f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_PasilloTecnico",
            "PASILLO TECNICO",
            new Vector3(0f, 1.10f, 0f),
            new Color(0.80f, 0.85f, 0.95f)
        );

        // =====================================================
        // CÁMARA
        // =====================================================

        Camera camera =
            Camera.main;

        if (camera != null)
        {
            camera.transform.position =
                new Vector3(0f, 23f, -26f);

            camera.transform.LookAt(
                Vector3.zero
            );

            if (
                camera.GetComponent<
                    CameraOrbitController
                >() == null
            )
            {
                camera.gameObject.AddComponent<
                    CameraOrbitController
                >();
            }
        }

        RenderSettings.ambientLight =
            new Color(0.34f, 0.38f, 0.46f);

        // =====================================================
        // GUARDAR
        // =====================================================

        AssetDatabase.SaveAssets();

        EditorSceneManager.SaveScene(
            scene,
            "Assets/Scenes/DataCenterScene.unity"
        );

        Selection.activeGameObject =
            robot;

        Debug.Log(
            "Data center realista generado correctamente."
        );
    }

    // =====================================================
    // SALA DE RED
    // =====================================================

    private static void CrearSalaRed(
        Transform parent,
        Material zonaMaterial,
        Material gabineteMaterial,
        Material moduloMaterial,
        Material ledVerde,
        Material ledAzul
    )
    {
        CrearCuboDecorativo(
            parent,
            "Zona_Red",
            new Vector3(-9.50f, 0.03f, 5.80f),
            new Vector3(4.60f, 0.06f, 4.20f),
            zonaMaterial
        );

        CrearBordeZona(
            parent,
            "Borde_Red",
            new Vector3(-9.50f, 0.07f, 5.80f),
            4.60f,
            4.20f,
            ledVerde
        );

        CrearGabineteRed(
            parent,
            "Network_Cabinet_01",
            new Vector3(-10.20f, 0f, 6.15f),
            gabineteMaterial,
            moduloMaterial,
            ledVerde,
            ledAzul
        );

        CrearGabineteRed(
            parent,
            "Network_Cabinet_02",
            new Vector3(-8.70f, 0f, 6.15f),
            gabineteMaterial,
            moduloMaterial,
            ledVerde,
            ledAzul
        );

        CrearCuboDecorativo(
            parent,
            "Bandeja_Red",
            new Vector3(-9.45f, 2.65f, 6.15f),
            new Vector3(3.00f, 0.16f, 0.30f),
            gabineteMaterial
        );
    }

    private static void CrearGabineteRed(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material gabineteMaterial,
        Material moduloMaterial,
        Material ledVerde,
        Material ledAzul
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Gabinete",
            posicion + new Vector3(0f, 1.05f, 0f),
            new Vector3(1.15f, 2.10f, 1.00f),
            gabineteMaterial
        );

        for (int index = 0; index < 7; index++)
        {
            float y =
                0.35f +
                index * 0.24f;

            CrearCuboDecorativo(
                parent,
                nombre + $"_Switch_{index + 1:00}",
                posicion + new Vector3(0f, y, -0.52f),
                new Vector3(0.92f, 0.14f, 0.10f),
                moduloMaterial
            );

            CrearCuboDecorativo(
                parent,
                nombre + $"_LedA_{index + 1:00}",
                posicion + new Vector3(0.30f, y, -0.59f),
                new Vector3(0.05f, 0.045f, 0.025f),
                index % 2 == 0
                    ? ledVerde
                    : ledAzul
            );

            CrearCuboDecorativo(
                parent,
                nombre + $"_LedB_{index + 1:00}",
                posicion + new Vector3(0.40f, y, -0.59f),
                new Vector3(0.05f, 0.045f, 0.025f),
                ledVerde
            );
        }
    }

    // =====================================================
    // SALA DE ENERGÍA
    // =====================================================

    private static void CrearSalaEnergia(
        Transform parent,
        Material zonaMaterial,
        Material gabineteMaterial,
        Material panelMaterial,
        Material ledRojo,
        Material bordeMaterial
    )
    {
        CrearCuboDecorativo(
            parent,
            "Zona_Energia",
            new Vector3(-9.50f, 0.03f, -5.80f),
            new Vector3(4.60f, 0.06f, 4.20f),
            zonaMaterial
        );

        CrearBordeZona(
            parent,
            "Borde_Energia",
            new Vector3(-9.50f, 0.07f, -5.80f),
            4.60f,
            4.20f,
            bordeMaterial
        );

        CrearTableroElectrico(
            parent,
            "Tablero_01",
            new Vector3(-10.20f, 0f, -6.15f),
            gabineteMaterial,
            panelMaterial,
            ledRojo
        );

        CrearTableroElectrico(
            parent,
            "Tablero_02",
            new Vector3(-8.70f, 0f, -6.15f),
            gabineteMaterial,
            panelMaterial,
            ledRojo
        );

        CrearTransformador(
            parent,
            "Transformador_01",
            new Vector3(-11.00f, 0f, -4.75f),
            gabineteMaterial,
            ledRojo
        );
    }

    private static void CrearTableroElectrico(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material gabineteMaterial,
        Material panelMaterial,
        Material ledRojo
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Gabinete",
            posicion + new Vector3(0f, 1.00f, 0f),
            new Vector3(1.15f, 2.00f, 1.00f),
            gabineteMaterial
        );

        for (int index = 0; index < 4; index++)
        {
            float y =
                0.42f +
                index * 0.36f;

            CrearCuboDecorativo(
                parent,
                nombre + $"_Interruptor_{index + 1:00}",
                posicion + new Vector3(0f, y, -0.53f),
                new Vector3(0.78f, 0.18f, 0.08f),
                panelMaterial
            );
        }

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0.36f, 1.72f, -0.58f),
            new Vector3(0.09f, 0.09f, 0.025f),
            ledRojo
        );
    }

    private static void CrearTransformador(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material cuerpoMaterial,
        Material ledRojo
    )
    {
        CrearCilindroDecorativo(
            parent,
            nombre + "_Cuerpo",
            posicion + new Vector3(0f, 0.55f, 0f),
            new Vector3(0.52f, 0.55f, 0.52f),
            Vector3.zero,
            cuerpoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0f, 1.18f, -0.48f),
            new Vector3(0.12f, 0.12f, 0.04f),
            ledRojo
        );
    }

    // =====================================================
    // SALA COOLING
    // =====================================================

    private static void CrearSalaCooling(
        Transform parent,
        Material zonaMaterial,
        Material gabineteMaterial,
        Material rejillaMaterial,
        Material ledAzul
    )
    {
        CrearCuboDecorativo(
            parent,
            "Zona_Cooling",
            new Vector3(10.10f, 0.03f, 5.80f),
            new Vector3(4.20f, 0.06f, 4.20f),
            zonaMaterial
        );

        CrearBordeZona(
            parent,
            "Borde_Cooling",
            new Vector3(10.10f, 0.07f, 5.80f),
            4.20f,
            4.20f,
            ledAzul
        );

        float[] posicionesX =
        {
            9.00f,
            10.35f,
            11.70f
        };

        for (int index = 0; index < posicionesX.Length; index++)
        {
            CrearUnidadCooling(
                parent,
                $"CRAC_{index + 1:00}",
                new Vector3(posicionesX[index], 0f, 6.30f),
                gabineteMaterial,
                rejillaMaterial,
                ledAzul
            );
        }

        CrearCuboDecorativo(
            parent,
            "Conducto_Cooling",
            new Vector3(10.35f, 2.70f, 6.30f),
            new Vector3(4.10f, 0.30f, 0.55f),
            rejillaMaterial
        );
    }

    private static void CrearUnidadCooling(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material gabineteMaterial,
        Material rejillaMaterial,
        Material ledAzul
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Gabinete",
            posicion + new Vector3(0f, 1.00f, 0f),
            new Vector3(1.00f, 2.00f, 1.10f),
            gabineteMaterial
        );

        for (int index = 0; index < 5; index++)
        {
            float y =
                0.42f +
                index * 0.27f;

            CrearCuboDecorativo(
                parent,
                nombre + $"_Rejilla_{index + 1:00}",
                posicion + new Vector3(0f, y, -0.58f),
                new Vector3(0.76f, 0.08f, 0.05f),
                rejillaMaterial
            );
        }

        CrearCilindroDecorativo(
            parent,
            nombre + "_Ventilador",
            posicion + new Vector3(0f, 1.62f, -0.60f),
            new Vector3(0.32f, 0.06f, 0.32f),
            new Vector3(90f, 0f, 0f),
            rejillaMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0.36f, 1.88f, -0.63f),
            new Vector3(0.10f, 0.08f, 0.025f),
            ledAzul
        );
    }

    // =====================================================
    // SALA UPS
    // =====================================================

    private static void CrearSalaUPS(
        Transform parent,
        Material zonaMaterial,
        Material gabineteMaterial,
        Material moduloMaterial,
        Material ledAmarillo,
        Material bordeMaterial
    )
    {
        CrearCuboDecorativo(
            parent,
            "Zona_UPS",
            new Vector3(10.10f, 0.03f, -5.80f),
            new Vector3(4.20f, 0.06f, 4.20f),
            zonaMaterial
        );

        CrearBordeZona(
            parent,
            "Borde_UPS",
            new Vector3(10.10f, 0.07f, -5.80f),
            4.20f,
            4.20f,
            bordeMaterial
        );

        CrearGabineteUPS(
            parent,
            "UPS_Main",
            new Vector3(9.30f, 0f, -6.20f),
            gabineteMaterial,
            moduloMaterial,
            ledAmarillo
        );

        CrearGabineteUPS(
            parent,
            "Battery_Module_01",
            new Vector3(10.75f, 0f, -6.20f),
            gabineteMaterial,
            moduloMaterial,
            ledAmarillo
        );

        CrearGabineteUPS(
            parent,
            "Battery_Module_02",
            new Vector3(11.85f, 0f, -6.20f),
            gabineteMaterial,
            moduloMaterial,
            ledAmarillo
        );
    }

    private static void CrearGabineteUPS(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material gabineteMaterial,
        Material moduloMaterial,
        Material ledAmarillo
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Gabinete",
            posicion + new Vector3(0f, 1.05f, 0f),
            new Vector3(1.10f, 2.10f, 1.00f),
            gabineteMaterial
        );

        for (int index = 0; index < 5; index++)
        {
            float y =
                0.40f +
                index * 0.31f;

            CrearCuboDecorativo(
                parent,
                nombre + $"_Bateria_{index + 1:00}",
                posicion + new Vector3(0f, y, -0.53f),
                new Vector3(0.84f, 0.20f, 0.08f),
                moduloMaterial
            );
        }

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0.36f, 1.88f, -0.58f),
            new Vector3(0.10f, 0.08f, 0.025f),
            ledAmarillo
        );
    }

    // =====================================================
    // RACKS
    // =====================================================

private static void CrearRackVisual(
    Transform parent,
    string nombre,
    Vector3 posicion,
    float direccionFrenteZ,
    Material frameMaterial,
    Material serverMaterial,
    Material ledMaterial
)
{
    GameObject rack =
        new GameObject(nombre);

    AplicarCapaIgnorada(rack);

    rack.transform.SetParent(parent);
    rack.transform.position = posicion;

    float frenteZ =
        0.76f *
        direccionFrenteZ;

    float fondoZ =
        -0.72f *
        direccionFrenteZ;

    // =====================================================
    // ESTRUCTURA EXTERIOR CERRADA
    // =====================================================

    // Base inferior.
    CrearCuboDecorativo(
        rack.transform,
        "Base",
        posicion + new Vector3(0f, 0.12f, 0f),
        new Vector3(1.38f, 0.24f, 1.58f),
        frameMaterial
    );

    // Techo.
    CrearCuboDecorativo(
        rack.transform,
        "Top",
        posicion + new Vector3(0f, 3.02f, 0f),
        new Vector3(1.38f, 0.18f, 1.58f),
        frameMaterial
    );

    // Panel lateral izquierdo.
    CrearCuboDecorativo(
        rack.transform,
        "SidePanel_Left",
        posicion + new Vector3(-0.66f, 1.56f, 0f),
        new Vector3(0.10f, 2.90f, 1.50f),
        frameMaterial
    );

    // Panel lateral derecho.
    CrearCuboDecorativo(
        rack.transform,
        "SidePanel_Right",
        posicion + new Vector3(0.66f, 1.56f, 0f),
        new Vector3(0.10f, 2.90f, 1.50f),
        frameMaterial
    );

    // Panel trasero.
    CrearCuboDecorativo(
        rack.transform,
        "BackPanel",
        posicion + new Vector3(0f, 1.56f, fondoZ),
        new Vector3(1.30f, 2.90f, 0.10f),
        frameMaterial
    );

    // Bordes verticales frontales.
    CrearCuboDecorativo(
        rack.transform,
        "FrontFrame_Left",
        posicion + new Vector3(-0.60f, 1.56f, frenteZ),
        new Vector3(0.10f, 2.90f, 0.10f),
        frameMaterial
    );

    CrearCuboDecorativo(
        rack.transform,
        "FrontFrame_Right",
        posicion + new Vector3(0.60f, 1.56f, frenteZ),
        new Vector3(0.10f, 2.90f, 0.10f),
        frameMaterial
    );

    // =====================================================
    // SERVIDORES FRONTALES
    // =====================================================

    for (int index = 0; index < 9; index++)
    {
        float y =
            0.43f +
            index * 0.28f;

        CrearCuboDecorativo(
            rack.transform,
            $"Server_{index + 1:00}",
            posicion + new Vector3(0f, y, frenteZ + 0.015f * direccionFrenteZ),
            new Vector3(1.08f, 0.20f, 0.10f),
            serverMaterial
        );

        // LEDs verdes pequeños.
        CrearCuboDecorativo(
            rack.transform,
            $"Led_Green_{index + 1:00}",
            posicion + new Vector3(
                0.39f,
                y,
                frenteZ + 0.075f * direccionFrenteZ
            ),
            new Vector3(0.07f, 0.045f, 0.025f),
            ledMaterial
        );

        // Segundo LED para que parezca más realista.
        CrearCuboDecorativo(
            rack.transform,
            $"Led_Status_{index + 1:00}",
            posicion + new Vector3(
                0.49f,
                y,
                frenteZ + 0.075f * direccionFrenteZ
            ),
            new Vector3(0.045f, 0.045f, 0.025f),
            ledMaterial
        );
    }
}

    // =====================================================
    // BORDES DE ZONAS
    // =====================================================

    private static void CrearBordeZona(
        Transform parent,
        string nombre,
        Vector3 centro,
        float ancho,
        float profundidad,
        Material material
    )
    {
        float espesor =
            0.10f;

        CrearCuboDecorativo(
            parent,
            nombre + "_Norte",
            centro + new Vector3(0f, 0f, profundidad / 2f),
            new Vector3(ancho, 0.025f, espesor),
            material
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Sur",
            centro + new Vector3(0f, 0f, -profundidad / 2f),
            new Vector3(ancho, 0.025f, espesor),
            material
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Este",
            centro + new Vector3(ancho / 2f, 0f, 0f),
            new Vector3(espesor, 0.025f, profundidad),
            material
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Oeste",
            centro + new Vector3(-ancho / 2f, 0f, 0f),
            new Vector3(espesor, 0.025f, profundidad),
            material
        );
    }

    // =====================================================
    // PUERTA, MATAFUEGO Y CÁMARA
    // =====================================================

    private static void CrearMarcoPuerta(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material marcoMaterial,
        Material ledMaterial
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Izquierda",
            posicion + new Vector3(0f, 1.30f, -0.90f),
            new Vector3(0.22f, 2.60f, 0.22f),
            marcoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Derecha",
            posicion + new Vector3(0f, 1.30f, 0.90f),
            new Vector3(0.22f, 2.60f, 0.22f),
            marcoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Superior",
            posicion + new Vector3(0f, 2.60f, 0f),
            new Vector3(0.22f, 0.22f, 2.00f),
            marcoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0.12f, 2.35f, 0.67f),
            new Vector3(0.06f, 0.15f, 0.15f),
            ledMaterial
        );
    }

    private static void CrearMatafuego(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material rojoMaterial,
        Material oscuroMaterial
    )
    {
        CrearCilindroDecorativo(
            parent,
            nombre + "_Cuerpo",
            posicion,
            new Vector3(0.20f, 0.45f, 0.20f),
            Vector3.zero,
            rojoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Manguera",
            posicion + new Vector3(0.20f, 0.35f, 0f),
            new Vector3(0.24f, 0.05f, 0.05f),
            oscuroMaterial
        );
    }

    private static void CrearCamaraSeguridad(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material cuerpoMaterial,
        Material ledMaterial
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Soporte",
            posicion,
            new Vector3(0.16f, 0.16f, 0.50f),
            cuerpoMaterial
        );

        CrearCilindroDecorativo(
            parent,
            nombre + "_Lente",
            posicion + new Vector3(0f, -0.08f, -0.30f),
            new Vector3(0.14f, 0.12f, 0.14f),
            new Vector3(90f, 0f, 0f),
            cuerpoMaterial
        );

        CrearCuboDecorativo(
            parent,
            nombre + "_Led",
            posicion + new Vector3(0.11f, -0.04f, -0.36f),
            new Vector3(0.05f, 0.05f, 0.03f),
            ledMaterial
        );
    }

    // =====================================================
    // LUCES
    // =====================================================

    private static void CrearLucesDeTecho(
        Transform parent,
        Material panelMaterial
    )
    {
        float[] posicionesX =
        {
            -10f,
            -6f,
            -2f,
            2f,
            6f,
            10f
        };

        foreach (float x in posicionesX)
        {
            CrearLuzTecho(
                parent,
                $"Luz_Techo_{x}",
                new Vector3(x, 4.70f, 0f),
                panelMaterial
            );
        }
    }

    private static void CrearLuzTecho(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material panelMaterial
    )
    {
        CrearCuboDecorativo(
            parent,
            nombre + "_Panel",
            posicion,
            new Vector3(2.20f, 0.10f, 0.42f),
            panelMaterial
        );

        GameObject lightObject =
            new GameObject(
                nombre + "_Spot"
            );

        lightObject.transform.SetParent(
            parent
        );

        lightObject.transform.position =
            posicion;

        lightObject.transform.rotation =
            Quaternion.Euler(
                90f,
                0f,
                0f
            );

        Light lightComponent =
            lightObject.AddComponent<Light>();

        lightComponent.type =
            LightType.Spot;

        lightComponent.color =
            new Color(
                0.90f,
                0.96f,
                1.00f
            );

        lightComponent.intensity =
            3.20f;

        lightComponent.range =
            10f;

        lightComponent.spotAngle =
            95f;

        lightComponent.shadows =
            LightShadows.None;
    }

    // =====================================================
    // ETIQUETAS 3D
    // =====================================================

    private static void CrearEtiqueta3D(
        Transform parent,
        string objectName,
        string labelText,
        Vector3 position,
        Color color
    )
    {
        GameObject labelObject =
            new GameObject(
                objectName
            );

        AplicarCapaIgnorada(
            labelObject
        );

        labelObject.transform.SetParent(
            parent
        );

        labelObject.transform.position =
            position;

        TextMeshPro textComponent =
            labelObject.AddComponent<TextMeshPro>();

        TMP_FontAsset fontAsset =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"
            );

        if (fontAsset == null)
        {
            fontAsset =
                TMP_Settings.defaultFontAsset;
        }

        if (fontAsset == null)
        {
            Debug.LogError(
                "No se encontró LiberationSans SDF. " +
                "Importá TMP Essential Resources."
            );

            Object.DestroyImmediate(
                labelObject
            );

            return;
        }

        textComponent.font =
            fontAsset;

        textComponent.text =
            labelText;

        textComponent.fontSize =
            7f;

        textComponent.color =
            color;

        textComponent.alignment =
            TextAlignmentOptions.Center;

        textComponent.textWrappingMode =
            TextWrappingModes.NoWrap;

        textComponent.fontStyle =
            FontStyles.Bold;

        textComponent.rectTransform.sizeDelta =
            new Vector2(30f, 6f);

        labelObject.transform.localScale =
            Vector3.one * 0.60f;

        labelObject.AddComponent<
            BillboardLabel
        >();

        textComponent.ForceMeshUpdate();
    }

    // =====================================================
    // OBJETOS AUXILIARES
    // =====================================================

    private static GameObject CrearCuboFisico(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Material material
    )
    {
        GameObject cubo =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        cubo.name =
            nombre;

        cubo.transform.SetParent(
            parent
        );

        cubo.transform.position =
            posicion;

        cubo.transform.localScale =
            escala;

        cubo
            .GetComponent<Renderer>()
            .sharedMaterial =
            material;

        return cubo;
    }

    private static void CrearBloqueadorInvisible(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala
    )
    {
        GameObject blocker =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        blocker.name =
            nombre;

        blocker.transform.SetParent(
            parent
        );

        blocker.transform.position =
            posicion;

        blocker.transform.localScale =
            escala;

        Renderer renderer =
            blocker.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.enabled =
                false;
        }
    }

    private static GameObject CrearCuboDecorativo(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Material material
    )
    {
        GameObject cubo =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        cubo.name =
            nombre;

        AplicarCapaIgnorada(
            cubo
        );

        cubo.transform.SetParent(
            parent
        );

        cubo.transform.position =
            posicion;

        cubo.transform.localScale =
            escala;

        cubo
            .GetComponent<Renderer>()
            .sharedMaterial =
            material;

        Collider collider =
            cubo.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(
                collider
            );
        }

        return cubo;
    }

    private static GameObject CrearCilindroDecorativo(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Vector3 rotacionEuler,
        Material material
    )
    {
        GameObject cilindro =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder
            );

        cilindro.name =
            nombre;

        AplicarCapaIgnorada(
            cilindro
        );

        cilindro.transform.SetParent(
            parent
        );

        cilindro.transform.position =
            posicion;

        cilindro.transform.localScale =
            escala;

        cilindro.transform.rotation =
            Quaternion.Euler(
                rotacionEuler
            );

        cilindro
            .GetComponent<Renderer>()
            .sharedMaterial =
            material;

        Collider collider =
            cilindro.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(
                collider
            );
        }

        return cilindro;
    }

    private static void AplicarCapaIgnorada(
        GameObject gameObject
    )
    {
        int ignoreRaycastLayer =
            LayerMask.NameToLayer(
                "Ignore Raycast"
            );

        if (ignoreRaycastLayer >= 0)
        {
            gameObject.layer =
                ignoreRaycastLayer;
        }
    }

    // =====================================================
    // CARPETAS Y MATERIALES
    // =====================================================

    private static void CrearCarpetaSiNoExiste(
        string carpetaPadre,
        string nuevaCarpeta
    )
    {
        string ruta =
            $"{carpetaPadre}/{nuevaCarpeta}";

        if (
            !AssetDatabase.IsValidFolder(
                ruta
            )
        )
        {
            AssetDatabase.CreateFolder(
                carpetaPadre,
                nuevaCarpeta
            );
        }
    }

    private static Material CrearMaterial(
        string nombre,
        Color color
    )
    {
        string ruta =
            $"Assets/Materials/{nombre}.mat";

        Material material =
            AssetDatabase.LoadAssetAtPath<Material>(
                ruta
            );

        if (material == null)
        {
            Shader shader =
                Shader.Find(
                    "Universal Render Pipeline/Lit"
                );

            if (shader == null)
            {
                shader =
                    Shader.Find("Standard");
            }

            material =
                new Material(
                    shader
                );

            AssetDatabase.CreateAsset(
                material,
                ruta
            );
        }

        material.color =
            color;

        if (
            material.HasProperty(
                "_BaseColor"
            )
        )
        {
            material.SetColor(
                "_BaseColor",
                color
            );
        }

        EditorUtility.SetDirty(
            material
        );

        return material;
    }

    private static Material CrearMaterialEmisivo(
        string nombre,
        Color color,
        float intensidad
    )
    {
        Material material =
            CrearMaterial(
                nombre,
                color
            );

        material.EnableKeyword(
            "_EMISSION"
        );

        if (
            material.HasProperty(
                "_EmissionColor"
            )
        )
        {
            material.SetColor(
                "_EmissionColor",
                color * intensidad
            );
        }

        material.globalIlluminationFlags =
            MaterialGlobalIlluminationFlags
                .RealtimeEmissive;

        EditorUtility.SetDirty(
            material
        );

        return material;
   }
}