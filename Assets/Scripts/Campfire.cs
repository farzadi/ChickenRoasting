using UnityEngine;

public class Campfire : MonoBehaviour
{
    public ParticleSystem ps;
    public float intensityDecayRate = 1f;

    private float intensity = 1;


    private void Update()
    {
        intensity -= Time.deltaTime * intensityDecayRate * 0.1f;
        intensity = Mathf.Clamp(intensity, 0f, 1f);
        var main = ps.main;
        main.startLifetime = intensity;
    }

    public void IncreaseIntensity(float amount)
    {
        intensity += amount;
        intensity = Mathf.Clamp(intensity, 0f, 1f);
    }

    public float GetIntensity()
    {
        return intensity;
    }
}