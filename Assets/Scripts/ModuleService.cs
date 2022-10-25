using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class value
{
    public variable variable;
    public float f;
    public Color color;
    public string key;
    public Transform obj;
}
[System.Serializable]
public struct modules
{
    public Module Module;
    public module module;
    public List<Values> Values;
}
[System.Serializable]
public class Values
{
    public variable Var;
    public value Val;
}
public enum module
{
    transforms,
    bubble,
    data
}
public enum variable
{
    transform = 0,
    bubbleData = 1,
    color = 2,
}
public class ModuleService : MonoBehaviour
{
    public static ModuleService M;
    public Dictionary<int, Dictionary<module, Dictionary<variable, value>>> Modules = new Dictionary<int, Dictionary<module, Dictionary<variable, value>>>();

    public List<Color> Colors = new List<Color>();
    private int GetId(Transform transform) { return transform.GetInstanceID(); }

    private void Awake()
    {
        M = this;
    }

    public void AddValues(Transform trans, List<modules> modules)
    {
        int id = GetId(trans);
        Dictionary<variable, value> value = new Dictionary<variable, value>();
        Dictionary<module, Dictionary<variable, value>> value1 = new Dictionary<module, Dictionary<variable, value>>();

        value.Add(variable.transform, new value { obj = trans});
        value1.Add(module.transforms, value);
        foreach (var item in modules)
        {
            value.Clear();
            foreach (var item1 in item.Values)
            {
                value.Add(item1.Var, item1.Val);
            }
            value1.Add(item.module, value);
        }
        Modules[id] = value1;
    }
    public void RemoveValues(int id)
    {
        Modules.Remove(id);
    }
    
    public void SetRandomColor(Transform trans)
    {
        Modules[GetId(trans)][module.bubble][variable.color].color = Colors[Random.Range(0, Colors.Count)];
        trans.GetComponent<SpriteRenderer>().color = Modules[GetId(trans)][module.bubble][variable.color].color;
    }

}
