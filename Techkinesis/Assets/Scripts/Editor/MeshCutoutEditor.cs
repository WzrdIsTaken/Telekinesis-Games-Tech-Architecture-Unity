using UnityEngine;
using UnityEditor;

// A custom editor to make the editing of mesh cutouts easier. Useful for finding out what ranges of values produce good results | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

[CustomEditor(typeof(MeshCutout))]
public class MeshCutoutEditor : Editor
{
    MeshCutout meshCutout;
    Editor cutoutEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                meshCutout.GenerateCutout();
            }
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            meshCutout.GenerateCutout();
        }

        DrawSettingsEditor(meshCutout.settings, meshCutout.OnShapeSettingsUpdated, ref meshCutout.meshCutoutSettingsFoldout, ref cutoutEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed) onSettingsUpdated?.Invoke();
                }
            }
        }
    }

    void OnEnable()
    {
        meshCutout = (MeshCutout)target;
    }
}