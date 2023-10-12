using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function
{
    // Fill this transform to parent.
    public static void SetRect(RectTransform _transform)
    {  
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(1, 1);

        _transform.offsetMin = new Vector2(0, 0);
        _transform.offsetMax = new Vector2(0, 0);

        _transform.localScale = new Vector3(1f, 1f, 1f);
    }

    // Copy component.
    public static Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
    public static Component CopyComponentFields(Component original, Component target)
    {
        System.Type type = original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(target, field.GetValue(original));
        }
        return target;
    }
}
