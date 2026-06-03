using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class DataCenterSceneBuilder
{
    [MenuItem("Tools/DataCenter Sentinel/Generar escenario base")]
    public static void GenerarEscenario()
    {
        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects,
            NewSceneMode.Single
        );

        Material pisoMaterial = CrearMaterial(
            "Piso",
            new Color(0.16f, 0.20f, 0.26f)
        );

        Material paredMaterial = CrearMaterial(
            "Pared",
            new Color(0.30f, 0.36f, 0.44f)
        );

        Material rackMaterial = CrearMaterial(
            "Rack",
            new Color(0.10f, 0.35f, 0.60f)
        );

        Material obstaculoMaterial = CrearMaterial(
            "Obstaculo",
            new Color(0.80f, 0.22f, 0.16f)
        );

        Material destinoMaterial = CrearMaterial(
            "Destino",
            new Color(0.15f, 0.80f, 0.32f)
        );

        Material robotMaterial = CrearMaterial(
            "RobotPlaceholder",
            new Color(0.60f, 0.20f, 0.80f)
        );

        GameObject dataCenter = new GameObject("DataCenter");

        // Piso
        CrearCubo(
            dataCenter.transform,
            "Floor",
            new Vector3(0f, -0.1f, 0f),
            new Vector3(20f, 0.2f, 14f),
            pisoMaterial
        );

        // Paredes exteriores
        CrearCubo(
            dataCenter.transform,
            "Wall_North",
            new Vector3(0f, 1.5f, 6.9f),
            new Vector3(20f, 3f, 0.2f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_South",
            new Vector3(0f, 1.5f, -6.9f),
            new Vector3(20f, 3f, 0.2f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_East",
            new Vector3(9.9f, 1.5f, 0f),
            new Vector3(0.2f, 3f, 14f),
            paredMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Wall_West",
            new Vector3(-9.9f, 1.5f, 0f),
            new Vector3(0.2f, 3f, 14f),
            paredMaterial
        );

        // Racks
        float[] posicionesX = { -4.5f, -1.5f, 1.5f, 4.5f };
        int numeroRack = 1;

        foreach (float x in posicionesX)
        {
            CrearCubo(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 1.25f, 3.2f),
                new Vector3(1.2f, 2.5f, 1.5f),
                rackMaterial
            );

            numeroRack++;

            CrearCubo(
                dataCenter.transform,
                $"Rack_{numeroRack:00}",
                new Vector3(x, 1.25f, -3.2f),
                new Vector3(1.2f, 2.5f, 1.5f),
                rackMaterial
            );

            numeroRack++;
        }

        // Obstáculos iniciales
        CrearCubo(
            dataCenter.transform,
            "Obstacle_01",
            new Vector3(0f, 0.5f, 0f),
            new Vector3(1f, 1f, 1f),
            obstaculoMaterial
        );

        CrearCubo(
            dataCenter.transform,
            "Obstacle_02",
            new Vector3(3f, 0.4f, 1.2f),
            new Vector3(0.8f, 0.8f, 0.8f),
            obstaculoMaterial
        );

        // Punto de destino
        GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.name = "Target_SalaCooling";
        target.transform.position = new Vector3(7.5f, 0.35f, 5.3f);
        target.transform.localScale = Vector3.one * 0.7f;
        target.GetComponent<Renderer>().sharedMaterial = destinoMaterial;

        Object.DestroyImmediate(target.GetComponent<Collider>());

        // Robot provisional
        GameObject robot = new GameObject("UnitreeGo2_Placeholder");
        robot.transform.position = new Vector3(-8f, 0f, 0f);

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "Visual";
        visual.transform.SetParent(robot.transform);
        visual.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        visual.transform.localScale = new Vector3(0.65f, 0.5f, 0.65f);
        visual.GetComponent<Renderer>().sharedMaterial = robotMaterial;

        Object.DestroyImmediate(visual.GetComponent<Collider>());

        // Navegación del robot
        NavMeshAgent agent = robot.AddComponent<NavMeshAgent>();
        agent.speed = 3f;
        agent.angularSpeed = 240f;
        agent.acceleration = 8f;
        agent.radius = 0.35f;
        agent.height = 1.1f;

        RobotMover mover = robot.AddComponent<RobotMover>();
        mover.target = target.transform;

        // Superficie navegable
        GameObject navigation = new GameObject("Navigation");
        NavMeshSurface surface = navigation.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        // Cámara
        Camera camera = Camera.main;

        if (camera != null)
        {
            camera.transform.position = new Vector3(0f, 15f, -18f);
            camera.transform.LookAt(Vector3.zero);
        }

        EditorSceneManager.SaveScene(
            scene,
            "Assets/Scenes/DataCenterScene.unity"
        );

        Selection.activeGameObject = robot;

        Debug.Log(
            "Escenario generado correctamente. Tocá Play para iniciar el recorrido."
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
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cubo.name = nombre;
        cubo.transform.SetParent(parent);
        cubo.transform.position = posicion;
        cubo.transform.localScale = escala;
        cubo.GetComponent<Renderer>().sharedMaterial = material;

        return cubo;
    }

    private static Material CrearMaterial(
        string nombre,
        Color color
    )
    {
        string ruta = $"Assets/Materials/{nombre}.mat";

        Material material = AssetDatabase.LoadAssetAtPath<Material>(ruta);

        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            material = new Material(shader);
            material.color = color;

            AssetDatabase.CreateAsset(material, ruta);
        }
        else
        {
            material.color = color;
            EditorUtility.SetDirty(material);
        }

        return material;
    }
}