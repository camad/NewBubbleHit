using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public bool IsProcessed;
    public bool Updateable;
    public List<modules> values = new List<modules>();
    private void OnEnable()
    {
        if(IsProcessed)
            if (values.Count > 0)
                ModuleService.M.AddValues(transform, values);
    }
    private void OnDisable()
    {
        if(IsProcessed)
            if (values.Count > 0)
                ModuleService.M.RemoveValues(transform.GetInstanceID());
    }
}
