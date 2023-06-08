using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    [SerializeField] List<Light> lights;

    public void SwitchLights(bool toggle)
    {
        foreach (Light light in lights)
        {
            light.enabled = toggle;
        }
    }
}
