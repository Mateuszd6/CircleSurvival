using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjSingleton<T> 
    where T : ScriptableObject
{
    
}

/// <summary> This class holds all singletons references. </summary>
public static class Singles
{
    // If asked for the singe one, and script loaded it, the reference is 
    // left here and can be found next time.
    private static Dictionary<Type, ScriptableObject> _scriptableSingles;
    private static Dictionary<Type, ScriptableObject> ScriptableSingles
    {
        get
        {
            if (_scriptableSingles == null)
                _scriptableSingles = new Dictionary<Type, ScriptableObject>();
            return _scriptableSingles;
        }
    }

    public static T GetScriptable<T>() where T : ScriptableObject
    {
        Type type = typeof(T);
        T result = null;

        if (ScriptableSingles.TryGetValue(type, out ScriptableObject lookupResult))
        {
            result = lookupResult as T;

            // The asset was removed and the reference that was stored in the dictionaty was obsolete.
            if (result == null)
                ScriptableSingles.Remove(type);
        }

        if (result == null) // Object of type T is not loaded yet.
        {
            result = Resources.Load<T>("ScriptableSingles/" + typeof(T).ToString());
            //  There is no asset of this type of SO, at least in the ScriptableSingles folder.
            if (result == null)
            {
                result = ScriptableObject.CreateInstance<T>();

                // We can create an asset if we are in the editor.
#if UNITY_EDITOR  
                result.name = typeof(T).Name;
                UnityEditor.AssetDatabase.CreateAsset(result, 
                                                      "Assets/Resources/ScriptableSingles/"
                                                        + typeof(T).ToString() + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            ScriptableSingles.Add(type, result);
        }

        return result;
    }
}
