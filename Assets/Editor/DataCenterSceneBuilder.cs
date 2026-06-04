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
        // MATERIALES
        // =====================================================

        Material pisoMaterial = CrearMaterial(
            "Piso",
            new Color(0.12f, 0.16f, 0.22f)
        );

        Material paredMaterial = CrearMaterial(
            "Pared",
            new Color(0.30f, 0.36f, 0.44f)
        );

        Material pasilloMaterial = CrearMaterial(
            "Pasillo",
            new Color(0.20f, 0.26f, 0.34f)
        );

        Material rackMaterial = CrearMaterial(
            "Rack",
            new Color(0.08f, 0.34f, 0.62f)
        );

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

        Material obstaculoMaterial = CrearMaterial(
            "Obstaculo",
            new Color(0.82f, 0.22f, 0.16f)
        );

        Material robotMaterial = CrearMaterial(
            "RobotPlaceholder",
            new Color(0.62f, 0.22f, 0.82f)
        );

        Material entradaMaterial = CrearMaterial(
            "Entrada",
            new Color(0.18f, 0.78f, 0.42f)
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
        // PASILLOS
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Central",
            new Vector3(0f, 0.02f, 0f),
            new Vector3(23f, 0.04f, 1.6f),
            pasilloMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Norte",
            new Vector3(0f, 0.02f, 5.4f),
            new Vector3(23f, 0.04f, 1.3f),
            pasilloMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Pasillo_Sur",
            new Vector3(0f, 0.02f, -5.4f),
            new Vector3(23f, 0.04f, 1.3f),
            pasilloMaterial
        );

        // =====================================================
        // ENTRADA
        // =====================================================

        CrearCubo(
            dataCenter.transform,
            "Zona_Entrada",
            new Vector3(-11.2f, 0.03f, 0f),
            new Vector3(2f, 0.06f, 2f),
            entradaMaterial
        );

        // =====================================================
        // RACKS DE SERVIDORES
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
            CrearRack(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 1.4f, 3.1f),
                rackMaterial
            );

            numeroRack++;

            CrearRack(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 1.4f, -3.1f),
                rackMaterial
            );

            numeroRack++;
        }

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

        // El NavMesh solamente considera objetos de la capa Default.
        // De esta forma, ignora los carteles 3D creados más abajo.
        surface.layerMask =
            LayerMask.GetMask("Default");

        surface.BuildNavMesh();

        // =====================================================
        // CARTELES FLOTANTES 3D
        // Se crean DESPUÉS del NavMesh para evitar errores de TMP.
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
            "LabCrearEtiel_RacksNorte",
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
            "Data center completo generado correctamente."
        );
    }

    // =====================================================
    // MÉTODOS AUXILIARES
    // =====================================================

    private static void CrearRack(
        Transform parent,
        string nombre,
        Vector3 posicion,
        Material material
    )
    {
        CrearCubo(
            parent,
            nombre,
            posicion,
            new Vector3(1.2f, 2.8f, 1.5f),
            material
        );
    }

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
            material
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

    private static GameObject CrearCubo(
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

        cubo.name = nombre;
        cubo.transform.SetParent(parent);
        cubo.transform.position = posicion;
        cubo.transform.localScale = escala;

        cubo
            .GetComponent<Renderer>()
            .sharedMaterial = material;

        return cubo;
    }

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

    // Evita que los carteles interfieran con el NavMesh.
    int ignoreRaycastLayer =
        LayerMask.NameToLayer("Ignore Raycast");

    if (ignoreRaycastLayer >= 0)
    {
        labelObject.layer =
            ignoreRaycastLayer;
    }

    labelObject.transform.SetParent(parent);
    labelObject.transform.position = position;

    TextMeshPro textComponent =
        labelObject.AddComponent<TextMeshPro>();

    // Carga explícitamente la fuente importada de TextMesh Pro.
    TMP_FontAsset fontAsset =
        AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"
        );

    // Respaldo por si la ruta cambia.
    if (fontAsset == null)
    {
        fontAsset =
            TMP_Settings.defaultFontAsset;
    }

    if (fontAsset == null)
    {
        Debug.LogError(
            "No se encontró LiberationSans SDF. " +
            "Importá TMP Essential Resources desde Window → TextMeshPro."
        );

        Object.DestroyImmediate(labelObject);
        return;
    }

    textComponent.font = fontAsset;
    textComponent.text = labelText;
    textComponent.fontSize = 7f;
    textComponent.color = color;

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

    labelObject.AddComponent<BillboardLabel>();

    textComponent.ForceMeshUpdate();
}

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
}