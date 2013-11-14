using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(Saveable))]
class SaveableEditor : Editor {

    void OnEnable ()
    {
    }

    public override void OnInspectorGUI()
    {
        var saveable = (Saveable)serializedObject.targetObject;

        saveable.SaveChildren = EditorGUILayout.Toggle("Save Child GameObjects", saveable.SaveChildren);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Restoration Prefab");
        saveable.Prefab = (GameObject)EditorGUILayout.ObjectField(saveable.Prefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        var components = saveable.gameObject.GetComponents<Component>()
            .Where(component => component != null)
            .Select(component => component.GetType())
            .Distinct()
            .Except(new System.Type[]
                {
                    typeof(UniqueIdentifier),
                    typeof(Saveable)
                })
            .ToArray();
        EditorGUILayout.LabelField("Components to serialize:");

        for (int i = 0; i < components.Length; ++i)
        {
            var component = components[i];

            var isChecked = !saveable.ComponentsToIgnore.Contains(component);

            var newIsChecked = EditorGUILayout.Toggle(component.Name, isChecked);
            
            if (isChecked && !newIsChecked)
            {
                saveable.ComponentsToIgnore.Add(component);
            }
            else if (!isChecked && newIsChecked)
            {
                saveable.ComponentsToIgnore.Remove(component);
            }
            if (!newIsChecked)
                continue;

            var serializableMemberNames = SaveIt.ContextBase.FindSerializableMemberNames(component);
            var membersToIgnore = saveable.MembersToIgnore.FirstOrDefault(m => m.Key == component).Value;
            if (membersToIgnore == null)
            {
                membersToIgnore = new List<string>();
                saveable.MembersToIgnore.Add(new KeyValuePair<System.Type, List<string>>(component, membersToIgnore));
            }
            foreach (var serializableMemberName in serializableMemberNames)
            {
                EditorGUILayout.BeginHorizontal();
                var shouldSerialize = !membersToIgnore.Contains(serializableMemberName);
                var newShouldSerialize = EditorGUILayout.Toggle("    " + serializableMemberName, shouldSerialize);

                if (shouldSerialize && !newShouldSerialize)
                {
                    membersToIgnore.Add(serializableMemberName);
                }
                else if (!shouldSerialize && newShouldSerialize)
                {
                    membersToIgnore.Remove(serializableMemberName);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}