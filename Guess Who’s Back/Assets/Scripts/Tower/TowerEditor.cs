#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tower))]
public class TowerEditor : Editor
{
    private SerializedProperty modules;

    private void OnEnable()
    {
        modules = serializedObject.FindProperty("modules");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (modules == null) modules = serializedObject.FindProperty("modules");

        // Everything except the module list, drawn like normal
        DrawPropertiesExcluding(serializedObject, "m_Script", "modules");

        EditorGUILayout.Space(8);

        if (modules == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.LabelField($"On-hit effects ({modules.arraySize})", EditorStyles.boldLabel);

        for (int i = 0; i < modules.arraySize; i++)
        {
            SerializedProperty element = modules.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            string label = GetDisplayName(element.managedReferenceFullTypename);
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, label, true);

            bool removed = false;
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                int sizeBefore = modules.arraySize;
                modules.DeleteArrayElementAtIndex(i);
                if (modules.arraySize == sizeBefore) modules.DeleteArrayElementAtIndex(i);
                removed = true;
            }
            EditorGUILayout.EndHorizontal();

            if (removed)
            {
                EditorGUILayout.EndVertical();
                break;
            }

            if (string.IsNullOrEmpty(element.managedReferenceFullTypename))
            {
                EditorGUILayout.LabelField("Missing module (class renamed or deleted?)", EditorStyles.miniLabel);
            }
            else if (element.isExpanded)
            {
                EditorGUI.indentLevel++;
                SerializedProperty child = element.Copy();
                SerializedProperty end = element.GetEndProperty();
                bool enterChildren = true;
                while (child.NextVisible(enterChildren) && !SerializedProperty.EqualContents(child, end))
                {
                    EditorGUILayout.PropertyField(child, true);
                    enterChildren = false;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ Add effect", GUILayout.Height(24)))
        {
            ShowAddMenu();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowAddMenu()
    {
        // Types already on this tower get greyed out so you can't add two Freezes
        HashSet<string> existing = new HashSet<string>();
        for (int i = 0; i < modules.arraySize; i++)
            existing.Add(modules.GetArrayElementAtIndex(i).managedReferenceFullTypename);

        GenericMenu menu = new GenericMenu();
        IEnumerable<Type> types = TypeCache.GetTypesDerivedFrom<TowerModule>()
            .Where(t => !t.IsAbstract)
            .OrderBy(t => t.Name);

        foreach (Type type in types)
        {
            Type t = type;
            string fullName = $"{t.Assembly.GetName().Name} {t.FullName}";
            GUIContent content = new GUIContent(GetDisplayName(fullName));

            if (existing.Contains(fullName))
            {
                menu.AddDisabledItem(content);
                continue;
            }

            menu.AddItem(content, false, () =>
            {
                serializedObject.Update();
                int index = modules.arraySize;
                modules.arraySize++;
                SerializedProperty newElement = modules.GetArrayElementAtIndex(index);
                newElement.managedReferenceValue = Activator.CreateInstance(t);
                newElement.isExpanded = true;
                serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    // "Assembly-CSharp FreezeModule" -> "Freeze"
    private static string GetDisplayName(string managedTypeName)
    {
        if (string.IsNullOrEmpty(managedTypeName)) return "(missing)";
        string name = managedTypeName.Split(' ').Last().Split('.').Last();
        if (name.EndsWith("Module")) name = name.Substring(0, name.Length - "Module".Length);
        if (name == "Aoe") return "Area of effect";
        if (name == "Dot") return "Damage over time";
        return ObjectNames.NicifyVariableName(name);
    }
}
#endif