using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnLights : MonoBehaviour
{
    public Light lightObj;

    public float lightLifeInSec = 300f;
    public float lightPercentage = 0f;

    private void Update()
    {
        lightObj.intensity = Mathf.Lerp(lightObj.intensity, lightPercentage, Time.deltaTime);
        lightPercentage += Time.deltaTime * (50 / lightLifeInSec);

        if (lightObj.intensity >= 1)
            lightObj.intensity = 1;
    }
}