using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using SaveIt;

[InitializeOnLoad]
public class SaveItVerification
{
    static SaveItVerification()
    {
        EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
    }

    [MenuItem("Window/SaveIt/Setup Scene Assistent", true)]
    static bool ValidateActivateSceneAssistent()
    {
        return !CanAutoVerify;
    }

    [MenuItem("Window/SaveIt/Remove Scene Assistent", true)]
    static bool ValidateDeactivateSceneAssistent()
    {
        return CanAutoVerify;
    }

    [MenuItem("Window/SaveIt/Remove Scene Assistent")]
    static void DeactivateSceneAssistent()
    {
        if (EditorUtility.DisplayDialog("Disable SaveIt Scene Assistent", "Do you want to disable the assisted scene data load and save functionality?\n\nRemoving the assistent will makes it harder for you save the content of this scene.", "Yes", "No"))
        {
            GameObject.DestroyImmediate(GameObject.Find("[SaveIt]"));
            if (EditorUtility.DisplayDialog("Remove SaceIt UniqueIdentifier Components", "Should all SaveIt UniqueIdentifier Components be removed?", "Yes", "No"))
            {
                var uniqueIdentifiers = Resources.FindObjectsOfTypeAll(typeof(UniqueIdentifier)).ToArray();
                foreach (var uid in uniqueIdentifiers)
                {
                    Object.DestroyImmediate(uid);
                }
            }
            if (EditorUtility.DisplayDialog("Remove SaceIt Saveable Components", "Should all SaveIt Saveable Components be removed?", "Yes", "No"))
            {
                var saveables = Resources.FindObjectsOfTypeAll(typeof(Saveable)).ToArray();
                foreach (var saveable in saveables)
                {
                    Object.DestroyImmediate(saveable);
                }
            }
        }
    }

    [MenuItem("Window/SaveIt/Setup Scene Assistent")]
    static void ActivateSceneAssistent()
    {
        if (EditorUtility.DisplayDialog("Enable SaveIt Scene Assistent", "Do you want to enable the assisted scene data load and save functionality?\n\nThis assistent will automatically add dependend resources needed to load and save the content of this scene.", "Yes", "No"))
        {
            var saveItGameObject = new GameObject("[SaveIt]");
            saveItGameObject.AddComponent<SaveItComponent>();
            Verify();
        }
    }

    private static void OnPlaymodeStateChanged()
    {
        var isPlaying = UnityEditor.EditorApplication.isPlaying;
        var isChangingOrPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;

        if (!isPlaying && isChangingOrPlaying && CanAutoVerify)
        {
            Verify();
        }
    }

    private static void Verify()
    {
        VerifyUniqueIdentifiers();
        VerifyDependencies();
    }

    private static bool CanAutoVerify
    {
        get
        {
            return GameObject.Find("[SaveIt]") != null;
        }
    }

    private static void VerifyUniqueIdentifiers()
    {
        var gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(go => go.name != "[SaveIt]" && !string.IsNullOrEmpty(AssetDatabase.GetAssetOrScenePath(go))).ToArray();
        foreach (var go in gameObjects)
        {
            var gameObject = (GameObject)go;
            var uniqueId = gameObject.GetComponent<UniqueIdentifier>();

            // Prefabs dont need UniqueIds
            if (PrefabUtility.GetPrefabType(gameObject) != PrefabType.Prefab && PrefabUtility.GetPrefabType(gameObject) != PrefabType.ModelPrefab && uniqueId == null)
            {
                gameObject.AddComponent<UniqueIdentifier>();
            }
        }

        var uniqueIdentifiers = Resources.FindObjectsOfTypeAll(typeof(UniqueIdentifier)).ToArray();
        foreach (var uid in uniqueIdentifiers)
        {
            var uniqueIdentifier = (UniqueIdentifier)uid;
            if (PrefabUtility.GetPrefabType(uniqueIdentifier.gameObject) == PrefabType.Prefab)
            {
                Object.DestroyImmediate(uniqueIdentifier, true);
            }
            else
            {
                uniqueIdentifier.EnsureId();
            }
        }
    }

    private static void VerifyDependencies()
    {
        var gameObjects = GameObject.FindObjectsOfType(typeof(GameObject)).Where(go => go.name != "[SaveIt]").ToArray();
        var usableDependencyTypes = new System.Type[]
            {
                typeof(Mesh),
                typeof(Material),
                typeof(PhysicMaterial),
                typeof(Shader),
                typeof(Texture),
                typeof(Texture2D),
                typeof(AudioClip),
                typeof(ProceduralMaterial),
                typeof(AnimationClip),
                typeof(Font)
            };
        var dependencies = EditorUtility.CollectDependencies(gameObjects)
            .Where(dependency1 =>
            {
                if (dependency1 == null)
                {
                    return false;
                }
                var want = usableDependencyTypes.Contains(dependency1.GetType());
                return want;
            })
            .Union(new Object[] { new PhysicMaterial() { name = "Default" } })
            .Select(dependency2 =>
            {
                var path = AssetDatabase.GetAssetPath(dependency2);
                if (string.IsNullOrEmpty(path))
                {
                    path = "$" + dependency2.name;
                }
                return new ResourceEntry()
                {
                    Name = dependency2.name,
                    Resource = dependency2,
                    TypeName = dependency2.GetType().FullName
                };
            });

        dependencies = dependencies.Union(GameObject.FindObjectsOfType(typeof(Saveable))
            .Where(saveable => ((Saveable)saveable).Prefab != null)
            .Select(saveable => 
            {
                var prefab = ((Saveable)saveable).Prefab;
                return new ResourceEntry()
                {
                    Name = prefab.name,
                    Resource = prefab,
                    TypeName = prefab.GetType().FullName
                };
            }));

        var saveItGameObject = (GameObject)GameObject.Find("[SaveIt]");
        if (saveItGameObject == null)
        {
            saveItGameObject = new GameObject("[SaveIt]");
        }
        var saveItComponent = saveItGameObject.GetComponent<SaveItComponent>();
        if (saveItComponent == null)
        {
            saveItComponent = saveItGameObject.AddComponent<SaveItComponent>();
        }

        saveItComponent.ResourceEntries = dependencies.Distinct().ToArray();
    }

    private static bool HasUnserializableComponents(GameObject gameObject)
    {
        var components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            if (IsUnserializableComponent(component))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsUnserializableComponent(Component component)
    {
        var type = component.GetType();
        return type.IsAbstract || type == typeof(Behaviour) || type == typeof(Component);
    }
}