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
            new Color(0.10f, 0.14f, 0.20f)
        );

        Material paredMaterial = CrearMaterial(
            "Pared",
            new Color(0.28f, 0.34f, 0.42f)
        );

        Material pasilloMaterial = CrearMaterial(
            "Pasillo",
            new Color(0.18f, 0.23f, 0.31f)
        );

        Material entradaMaterial = CrearMaterial(
            "Entrada",
            new Color(0.18f, 0.78f, 0.42f)
        );

        Material obstaculoMaterial = CrearMaterial(
            "Obstaculo",
            new Color(0.82f, 0.22f, 0.16f)
        );

        Material robotMaterial = CrearMaterial(
            "RobotPlaceholder",
            new Color(0.62f, 0.22f, 0.82f)
        );

        // =====================================================
        // MATERIALES DE RACKS
        // =====================================================

        Material rackBaseMaterial = CrearMaterial(
            "RackBase",
            new Color(0.06f, 0.09f, 0.13f)
        );

        Material rackFrameMaterial = CrearMaterial(
            "RackFrame",
            new Color(0.03f, 0.05f, 0.08f)
        );

        Material serverMaterial = CrearMaterial(
            "ServerUnit",
            new Color(0.12f, 0.17f, 0.24f)
        );

        Material ledMaterial = CrearMaterialEmisivo(
            "RackLed",
            new Color(0.20f, 1.00f, 0.55f),
            4f
        );

        Material bandejaCableadoMaterial = CrearMaterial(
            "BandejaCableado",
            new Color(0.13f, 0.16f, 0.21f)
        );

        // =====================================================
        // MATERIALES DE ZONAS
        // =====================================================

        Material coolingMaterial = CrearMaterial(
            "Cooling",
            new Color(0.10f, 0.72f, 0.82f)
        );

        Material upsMaterial = CrearMaterial(
            "UPS",
            new Color(0.88f, 0.60f, 0.10f)
        );

        Material energiaMaterial = CrearMaterial(
            "Energia",
            new Color(0.92f, 0.30f, 0.18f)
        );

        Material redMaterial = CrearMaterial(
            "Red",
            new Color(0.28f, 0.72f, 0.32f)
        );

        Material pasilloFrioMaterial = CrearMaterialEmisivo(
            "PasilloFrio",
            new Color(0.08f, 0.42f, 0.88f),
            1.7f
        );

        Material pasilloCalienteMaterial = CrearMaterialEmisivo(
            "PasilloCaliente",
            new Color(0.88f, 0.20f, 0.12f),
            1.7f
        );

        Material luzTechoMaterial = CrearMaterialEmisivo(
            "LuzTecho",
            new Color(0.90f, 0.96f, 1.00f),
            2.5f
        );

        // =====================================================
        // CONTENEDOR PRINCIPAL
        // =====================================================

        GameObject dataCenter =
            new GameObject("DataCenter");

        // =====================================================
        // PISO Y PAREDES
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Floor",
            new Vector3(0f, -0.1f, 0f),
            new Vector3(26f, 0.2f, 18f),
            pisoMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_North",
            new Vector3(0f, 1.6f, 8.9f),
            new Vector3(26f, 3.2f, 0.2f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_South",
            new Vector3(0f, 1.6f, -8.9f),
            new Vector3(26f, 3.2f, 0.2f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_East",
            new Vector3(12.9f, 1.6f, 0f),
            new Vector3(0.2f, 3.2f, 18f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_West",
            new Vector3(-12.9f, 1.6f, 0f),
            new Vector3(0.2f, 3.2f, 18f),
            paredMaterial
        );

        // =====================================================
        // PASILLOS GENERALES
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Central",
            new Vector3(0f, 0.02f, 0f),
            new Vector3(23f, 0.04f, 1.6f),
            pasilloMaterial,
            true
        );

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Norte",
            new Vector3(0f, 0.02f, 5.4f),
            new Vector3(23f, 0.04f, 1.3f),
            pasilloMaterial,
            true
        );

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Sur",
            new Vector3(0f, 0.02f, -5.4f),
            new Vector3(23f, 0.04f, 1.3f),
            pasilloMaterial,
            true
        );

        // =====================================================
        // PASILLOS FRÍOS Y CALIENTES
        // =====================================================

        // Frente de racks: líneas azules.
        CrearCubo(
            dataCenter.transform,
            "Linea_Fria_Norte",
            new Vector3(0f, 0.055f, 4.25f),
            new Vector3(18.5f, 0.025f, 0.20f),
            pasilloFrioMaterial,
            true
        );

        CrearCubo(
            dataCenter.transform,
            "Linea_Fria_Sur",
            new Vector3(0f, 0.055f, -4.25f),
            new Vector3(18.5f, 0.025f, 0.20f),
            pasilloFrioMaterial,
            true
        );

        // Parte trasera de racks: líneas rojas.
        CrearCubo(
            dataCenter.transform,
            "Linea_Caliente_Norte",
            new Vector3(0f, 0.055f, 2.05f),
            new Vector3(18.5f, 0.025f, 0.20f),
            pasilloCalienteMaterial,
            true
        );

        CrearCubo(
            dataCenter.transform,
            "Linea_Caliente_Sur",
            new Vector3(0f, 0.055f, -2.05f),
            new Vector3(18.5f, 0.025f, 0.20f),
            pasilloCalienteMaterial,
            true
        );

        // =====================================================
        // ENTRADA
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Zona_Entrada",
            new Vector3(-11.2f, 0.03f, 0f),
            new Vector3(2f, 0.06f, 2f),
            entradaMaterial,
            true
        );

        // =====================================================
        // RACKS REALISTAS
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

        int numeroRack = 1;

        foreach (float x in posicionesX)
        {
            // Fila norte: frente hacia el pasillo frío exterior.
            CrearRack(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 0f, 3.1f),
                1f,
                rackBaseMaterial,
                rackFrameMaterial,
                serverMaterial,
                ledMaterial
            );

            numeroRack++;

            // Fila sur: frente hacia el pasillo frío exterior.
            CrearRack(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 0f, -3.1f),
                -1f,
                rackBaseMaterial,
                rackFrameMaterial,
                serverMaterial,
                ledMaterial
            );

            numeroRack++;
        }

        // =====================================================
        // BANDEJAS SUPERIORES DE CABLEADO
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Bandeja_Cableado_Norte",
            new Vector3(0f, 3.80f, 3.1f),
            new Vector3(19f, 0.16f, 0.35f),
            bandejaCableadoMaterial,
            true
        );

        CrearCubo(
            dataCenter.transform,
            "Bandeja_Cableado_Sur",
            new Vector3(0f, 3.80f, -3.1f),
            new Vector3(19f, 0.16f, 0.35f),
            bandejaCableadoMaterial,
            true
        );

        // =====================================================
        // SALA DE REFRIGERACIÓN
        // =====================================================

        CrearZonaPiso(
            dataCenter.transform,
            "Zona_Cooling",
            new Vector3(10.1f, 0.03f, 5.8f),
            new Vector3(4.2f, 0.06f, 4.2f),
            coolingMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Cooling_Unit_01",
            new Vector3(9.3f, 0.85f, 6.1f),
            new Vector3(1.2f, 1.7f, 1.0f),
            coolingMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Cooling_Unit_02",
            new Vector3(11.0f, 0.85f, 6.1f),
            new Vector3(1.2f, 1.7f, 1.0f),
            coolingMaterial
        );

        // =====================================================
        // SALA UPS
        // =====================================================

        CrearZonaPiso(
            dataCenter.transform,
            "Zona_UPS",
            new Vector3(10.1f, 0.03f, -5.8f),
            new Vector3(4.2f, 0.06f, 4.2f),
            upsMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "UPS_Unit_01",
            new Vector3(9.4f, 0.85f, -6.1f),
            new Vector3(1.2f, 1.7f, 1.0f),
            upsMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "UPS_Unit_02",
            new Vector3(11.0f, 0.85f, -6.1f),
            new Vector3(1.2f, 1.7f, 1.0f),
            upsMaterial
        );

        // =====================================================
        // SALA DE ENERGÍA
        // =====================================================

        CrearZonaPiso(
            dataCenter.transform,
            "Zona_Energia",
            new Vector3(-9.5f, 0.03f, -5.8f),
            new Vector3(4.6f, 0.06f, 4.2f),
            energiaMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Tablero_Electrico_01",
            new Vector3(-10.2f, 0.95f, -6.0f),
            new Vector3(1.0f, 1.9f, 0.8f),
            energiaMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Tablero_Electrico_02",
            new Vector3(-8.7f, 0.95f, -6.0f),
            new Vector3(1.0f, 1.9f, 0.8f),
            energiaMaterial
        );

        // =====================================================
        // SALA DE RED
        // =====================================================

        CrearZonaPiso(
            dataCenter.transform,
            "Zona_Red",
            new Vector3(-9.5f, 0.03f, 5.8f),
            new Vector3(4.6f, 0.06f, 4.2f),
            redMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Network_Cabinet_01",
            new Vector3(-10.2f, 1.0f, 6.0f),
            new Vector3(1.0f, 2.0f, 0.8f),
            redMaterial
        );

        CrearEquipoTecnico(
            dataCenter.transform,
            "Network_Cabinet_02",
            new Vector3(-8.7f, 1.0f, 6.0f),
            new Vector3(1.0f, 2.0f, 0.8f),
            redMaterial
        );

        // =====================================================
        // OBSTÁCULOS FIJOS
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Caja_Tecnica_Fija",
            new Vector3(-1.0f, 0.40f, 1.0f),
            new Vector3(0.9f, 0.8f, 0.9f),
            obstaculoMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Herramientas_Mantenimiento",
            new Vector3(5.5f, 0.35f, -1.1f),
            new Vector3(1.1f, 0.7f, 0.8f),
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

        visual.name = "Visual";
        visual.transform.SetParent(robot.transform);

        visual.transform.localPosition =
            new Vector3(0f, 0.5f, 0f);

        visual.transform.localScale =
            new Vector3(0.65f, 0.5f, 0.65f);

        visual
            .GetComponent<Renderer>()
            .sharedMaterial = robotMaterial;

        Object.DestroyImmediate(
            visual.GetComponent<Collider>()
        );

        // =====================================================
        // COMPONENTES DEL ROBOT
        // =====================================================

        NavMeshAgent agent =
            robot.AddComponent<NavMeshAgent>();

        agent.speed = 4f;
        agent.angularSpeed = 260f;
        agent.acceleration = 10f;
        agent.radius = 0.35f;
        agent.height = 1.1f;
        agent.stoppingDistance = 0.25f;

        robot.AddComponent<RobotMover>();
        robot.AddComponent<DynamicObstacleManager>();
        robot.AddComponent<DatasetLogger>();
        robot.AddComponent<MissionManager>();

        // =====================================================
        // NAVEGACIÓN
        // =====================================================

        GameObject navigation =
            new GameObject("Navigation");

        NavMeshSurface surface =
            navigation.AddComponent<NavMeshSurface>();

        // Solo los objetos Default participan del cálculo.
        surface.layerMask =
            LayerMask.GetMask("Default");

        surface.BuildNavMesh();

        // =====================================================
        // LUCES DE TECHO
        // =====================================================

        float[] posicionesLuces =
        {
            -10f,
            -6f,
            -2f,
            2f,
            6f,
            10f
        };

        foreach (float x in posicionesLuces)
        {
            CrearLuzTecho(
                dataCenter.transform,
                $"Luz_Techo_{x}",
                new Vector3(x, 4.7f, 0f),
                luzTechoMaterial
            );
        }

        // =====================================================
        // CARTELES FLOTANTES 3D
        // =====================================================

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_Entrada",
            "ENTRADA",
            new Vector3(-11.2f, 2.0f, 0f),
            Color.white
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaRed",
            "SALA DE RED",
            new Vector3(-9.5f, 2.8f, 5.8f),
            new Color(0.55f, 1f, 0.60f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaEnergia",
            "SALA DE ENERGIA",
            new Vector3(-9.5f, 2.8f, -5.8f),
            new Color(1f, 0.55f, 0.45f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaCooling",
            "SALA COOLING",
            new Vector3(10.1f, 2.8f, 5.8f),
            new Color(0.45f, 0.95f, 1f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_SalaUPS",
            "SALA UPS",
            new Vector3(10.1f, 2.8f, -5.8f),
            new Color(1f, 0.85f, 0.35f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_RacksNorte",
            "RACKS NORTE",
            new Vector3(1f, 3.4f, 3.1f),
            new Color(0.55f, 0.80f, 1f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_RacksSur",
            "RACKS SUR",
            new Vector3(1f, 3.4f, -3.1f),
            new Color(0.55f, 0.80f, 1f)
        );

        CrearEtiqueta3D(
            dataCenter.transform,
            "Label_PasilloTecnico",
            "PASILLO TECNICO",
            new Vector3(0f, 1.0f, 0f),
            new Color(0.80f, 0.85f, 0.95f)
        );

        // =====================================================
        // CÁMARA
        // =====================================================

        Camera camera = Camera.main;

        if (camera != null)
        {
            camera.transform.position =
                new Vector3(0f, 22f, -25f);

            camera.transform.LookAt(
                Vector3.zero
            );
        }

        // =====================================================
        // GUARDAR ESCENA
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
    // RACK REALISTA
    // =====================================================

    private static void CrearRack(
        Transform parent,
        string nombre,
        Vector3 posicion,
        float direccionFrenteZ,
        Material baseMaterial,
        Material frameMaterial,
        Material serverMaterial,
        Material ledMaterial
    )
    {
        GameObject rack =
            new GameObject(nombre);

        rack.transform.SetParent(parent);
        rack.transform.position = posicion;

        // Base inferior.
        CrearCubo(
            rack.transform,
            "Base",
            posicion + new Vector3(0f, 0.10f, 0f),
            new Vector3(1.30f, 0.20f, 1.55f),
            baseMaterial
        );

        // Columnas del marco.
        Vector3[] columnas =
        {
            new Vector3(-0.58f, 1.55f, -0.68f),
            new Vector3( 0.58f, 1.55f, -0.68f),
            new Vector3(-0.58f, 1.55f,  0.68f),
            new Vector3( 0.58f, 1.55f,  0.68f)
        };

        foreach (Vector3 offset in columnas)
        {
            CrearCubo(
                rack.transform,
                "FramePost",
                posicion + offset,
                new Vector3(0.10f, 2.90f, 0.10f),
                frameMaterial
            );
        }

        // Techo y parte inferior del marco.
        CrearCubo(
            rack.transform,
            "FrameTop",
            posicion + new Vector3(0f, 3.00f, 0f),
            new Vector3(1.30f, 0.12f, 1.55f),
            frameMaterial
        );

        CrearCubo(
            rack.transform,
            "FrameBottom",
            posicion + new Vector3(0f, 0.20f, 0f),
            new Vector3(1.30f, 0.12f, 1.55f),
            frameMaterial
        );

        // Servidores horizontales y LEDs.
        for (int index = 0; index < 8; index++)
        {
            float y =
                0.45f +
                index * 0.30f;

            float servidorZ =
                0.72f *
                direccionFrenteZ;

            float ledZ =
                0.795f *
                direccionFrenteZ;

            CrearCubo(
                rack.transform,
                $"Server_{index + 1:00}",
                posicion +
                new Vector3(0f, y, servidorZ),
                new Vector3(1.08f, 0.20f, 0.12f),
                serverMaterial
            );

            CrearCubo(
                rack.transform,
                $"Led_{index + 1:00}",
                posicion +
                new Vector3(0.42f, y, ledZ),
                new Vector3(0.08f, 0.055f, 0.025f),
                ledMaterial,
                true
            );
        }

        // Paneles laterales.
        CrearCubo(
            rack.transform,
            "SidePanel_Left",
            posicion + new Vector3(-0.63f, 1.55f, 0f),
            new Vector3(0.06f, 2.75f, 1.40f),
            frameMaterial
        );

        CrearCubo(
            rack.transform,
            "SidePanel_Right",
            posicion + new Vector3(0.63f, 1.55f, 0f),
            new Vector3(0.06f, 2.75f, 1.40f),
            frameMaterial
        );
    }

    // =====================================================
    // LUCES
    // =====================================================

    private static void CrearLuzTecho(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material panelMaterial
    )
    {
        // Panel luminoso visible.
        CrearCubo(
            parent,
            nombre + "_Panel",
            posicion,
            new Vector3(2.20f, 0.10f, 0.42f),
            panelMaterial,
            true
        );

        // Luz real que ilumina la escena.
        GameObject lightObject =
            new GameObject(
                nombre + "_Light"
            );

        lightObject.transform.SetParent(
            parent
        );

        lightObject.transform.position =
            posicion +
            new Vector3(0f, -0.20f, 0f);

        Light lightComponent =
            lightObject.AddComponent<Light>();

        lightComponent.type =
            LightType.Point;

        lightComponent.color =
            new Color(
                0.90f,
                0.96f,
                1.00f
            );

        lightComponent.intensity =
            2.8f;

        lightComponent.range =
            7f;

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
            new GameObject(objectName);

        // Impide que los textos interfieran con el NavMesh.
        int ignoreRaycastLayer =
            LayerMask.NameToLayer(
                "Ignore Raycast"
            );

        if (ignoreRaycastLayer >= 0)
        {
            labelObject.layer =
                ignoreRaycastLayer;
        }

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
                "Importá TMP Essential Resources desde " +
                "Window → TextMeshPro."
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
    // ZONAS Y EQUIPOS
    // =====================================================

    private static void CrearZonaPiso(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Material material
    )
    {
        CrearCubo(
            parent,
            nombre,
            posicion,
            escala,
            material,
            true
        );
    }

    private static void CrearEquipoTecnico(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Material material
    )
    {
        CrearCubo(
            parent,
            nombre,
            posicion,
            escala,
            material
        );
    }

    // =====================================================
    // CUBOS AUXILIARES
    // =====================================================

    private static GameObject CrearCubo(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Vector3 escala,
        Material material,
        bool eliminarCollider = false
    )
    {
        GameObject cubo =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        cubo.name = nombre;

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

        if (eliminarCollider)
        {
            Collider collider =
                cubo.GetComponent<Collider>();

            if (collider != null)
            {
                Object.DestroyImmediate(
                    collider
                );
            }
        }

        return cubo;
    }

    // =====================================================
    // CARPETAS
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

    // =====================================================
    // MATERIALES
    // =====================================================

    private static Material CrearMaterial(
        string nombre,
        Color color
    )
    {
        string ruta =
            $"Assets/Materials/{nombre}.mat";

        Material material =
            AssetDatabase.LoadAssetAtPath<
                Material
            >(
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
                new Material(shader);

            AssetDatabase.CreateAsset(
                material,
                ruta
            );
        }

        material.color =
            color;

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
        string ruta =
            $"Assets/Materials/{nombre}.mat";

        Material material =
            AssetDatabase.LoadAssetAtPath<
                Material
            >(
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
                new Material(shader);

            AssetDatabase.CreateAsset(
                material,
                ruta
            );
        }

        material.color =
            color;

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