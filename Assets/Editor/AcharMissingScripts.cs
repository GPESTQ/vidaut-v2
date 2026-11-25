#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AcharMissingScripts : EditorWindow
{
    [MenuItem("Tools/Achar Missing Scripts")]
    static void Achar()
    {
        int total = 0;

        foreach (var go in FindObjectsOfType<GameObject>(true))
        {
            if (AcharNaHierarquia(go))
                total++;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && AcharNaHierarquia(prefab))
                total++;
        }
    }

    static bool AcharNaHierarquia(GameObject go)
    {
        bool achou = false;
        var components = go.GetComponents<Component>();
        foreach (var c in components)
        {
            if (c == null)
                achou = true;
        }

        foreach (Transform filho in go.transform)
        {
            if (AcharNaHierarquia(filho.gameObject))
                achou = true;
        }

        return achou;
    }
}
#endif
