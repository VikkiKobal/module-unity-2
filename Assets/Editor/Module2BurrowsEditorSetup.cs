#if UNITY_EDITOR
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class Module2BurrowsEditorSetup
{
    const string ScenePath = "Assets/Scenes/module_2.unity";
    const string RootName = "Burrows_Module2_Auto";

    [MenuItem("Tools/Module 2/Build Burrows Scene")]
    public static void BuildBurrows()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(false))
            return;

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var old = GameObject.Find(RootName);
        if (old != null)
            Object.DestroyImmediate(old);

        var root = new GameObject(RootName);

        BuildEnvironment(root.transform);
        var defeat = BuildUi(root.transform);
        BuildShrew(root.transform);
        BuildSnake(root.transform, defeat);
        SetupCameraAndLight();
        EnsureEventSystem();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Module2] Burrows built in module_2. Press Play to test point-and-click and snake chase.");
    }

    static void BuildEnvironment(Transform parent)
    {
        var env = new GameObject("Environment");
        env.transform.SetParent(parent, false);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.tag = "Ground";
        floor.transform.SetParent(env.transform, false);
        floor.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        floor.transform.localScale = new Vector3(20f, 1f, 20f);
        MarkNavigationStatic(floor);

        Wall(env.transform, new Vector3(0f, 1f, 9.5f), new Vector3(20f, 2f, 1f));
        Wall(env.transform, new Vector3(0f, 1f, -9.5f), new Vector3(20f, 2f, 1f));
        Wall(env.transform, new Vector3(9.5f, 1f, 0f), new Vector3(1f, 2f, 20f));
        Wall(env.transform, new Vector3(-9.5f, 1f, 0f), new Vector3(1f, 2f, 20f));

        Wall(env.transform, new Vector3(0f, 1f, 0f), new Vector3(6f, 2f, 0.7f));
        Wall(env.transform, new Vector3(-4f, 1f, 3f), new Vector3(0.7f, 2f, 5f));
        Wall(env.transform, new Vector3(4f, 1f, -2.5f), new Vector3(0.7f, 2f, 7f));

        var surface = env.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.BuildNavMesh();
    }

    static void Wall(Transform envParent, Vector3 localPos, Vector3 scale)
    {
        var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
        w.name = "Wall";
        w.transform.SetParent(envParent, false);
        w.transform.localPosition = localPos;
        w.transform.localScale = scale;
        MarkNavigationStatic(w);
    }

    static void MarkNavigationStatic(GameObject go)
    {
        GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.NavigationStatic);
    }

    static DefeatScreenController BuildUi(Transform parent)
    {
        var hud = new GameObject("GameHUD");
        hud.transform.SetParent(parent, false);

        var canvasGo = new GameObject("Canvas");
        canvasGo.transform.SetParent(hud.transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("DefeatPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var panelRt = panel.AddComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;
        var img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.55f);
        panel.SetActive(false);

        var textGo = new GameObject("DefeatText");
        textGo.transform.SetParent(panel.transform, false);
        var textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.5f, 0.5f);
        textRt.anchorMax = new Vector2(0.5f, 0.5f);
        textRt.sizeDelta = new Vector2(640f, 120f);
        textRt.anchoredPosition = Vector2.zero;
        var text = textGo.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 36;
        text.color = Color.white;
        text.text = "Game Over / Defeat";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                     ?? Font.CreateDynamicFontFromOSFont("Arial", 16);

        var ctrl = hud.AddComponent<DefeatScreenController>();
        ctrl.BindPanel(panel, text);
        return ctrl;
    }

    static void BuildShrew(Transform parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "Shrew";
        go.tag = "Player";
        go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(new Vector3(3.5f, 0.7f, 3.5f), Quaternion.identity);
        go.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var agent = go.AddComponent<NavMeshAgent>();
        agent.speed = 4.2f;
        agent.angularSpeed = 540f;
        agent.acceleration = 24f;
        agent.radius = 0.35f;
        agent.height = 1.6f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        go.AddComponent<ShrewNavMeshClickMove>();
    }

    static void BuildSnake(Transform parent, DefeatScreenController defeat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "Snake";
        go.tag = "Enemy";
        go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(new Vector3(-3.5f, 0.45f, -3.5f), Quaternion.identity);
        go.transform.localScale = new Vector3(0.85f, 0.45f, 0.85f);

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var agent = go.AddComponent<NavMeshAgent>();
        agent.speed = 5.8f;
        agent.angularSpeed = 420f;
        agent.acceleration = 28f;
        agent.radius = 0.42f;
        agent.height = 1.2f;

        go.AddComponent<SnakeChaseNavAgent>();

        var triggerGo = new GameObject("CatchTrigger");
        triggerGo.transform.SetParent(go.transform, false);
        triggerGo.transform.localPosition = Vector3.zero;
        var sphere = triggerGo.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.75f;
        var trig = triggerGo.AddComponent<SnakePlayerDefeatTrigger>();
        trig.Bind(defeat);

        var r = go.GetComponent<Renderer>();
        if (r != null)
            r.material.color = new Color(0.35f, 0.55f, 0.2f);
    }

    static void SetupCameraAndLight()
    {
        var camGo = GameObject.FindGameObjectWithTag("MainCamera");
        if (camGo != null)
        {
            camGo.transform.SetPositionAndRotation(new Vector3(0f, 16f, -12f), Quaternion.Euler(52f, 0f, 0f));
            var cam = camGo.GetComponent<Camera>();
            if (cam != null)
                cam.nearClipPlane = 0.1f;
        }

        var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var light in lights)
        {
            if (light.type != LightType.Directional)
                continue;
            light.intensity = 0.55f;
            light.shadows = LightShadows.Soft;
            break;
        }
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
            return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<InputSystemUIInputModule>();
    }
}
#endif
