using System.Linq;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    [SerializeField] private Renderer[] chickenRenderers;
    [SerializeField] private float cookSpeedMultiplier = 1f;

    [SerializeField] private string cookedColorRaw = "#FFB251";
    [SerializeField] private string cookedColorBurnt = "#5A2000";

    private int currentSide = 0;

    public float insideTemperature;
    public float[] sideCookCompletion;
    public SideCookState[] sideCookStates;
    public float maxInsideTemperature;
    public float[] sideCookTimes;

    public enum SideCookState
    {
        Raw,
        Cooked,
        Burnt
    }

    private void Start()
    {
        sideCookCompletion = new float[4];
        sideCookStates = Enumerable.Repeat(SideCookState.Raw, chickenRenderers.Length).ToArray();
        sideCookTimes = Enumerable.Repeat(0f, chickenRenderers.Length).ToArray();
    }

    public void Rotate(bool clockwise)
    {
        int rotationDirection = clockwise ? 1 : -1;
        currentSide = (currentSide + rotationDirection + 4) % 4;
        transform.Rotate(Vector3.right, 90f * rotationDirection);
    }

    public void Cook(float fireIntensity, float deltaTime)
    {
        float cookSpeed = fireIntensity * deltaTime * cookSpeedMultiplier;
        sideCookCompletion[currentSide] += cookSpeed;
        sideCookTimes[currentSide] = sideCookCompletion[currentSide];
        UpdateChickenColor();

        if (fireIntensity > 0.5f)
        {
            insideTemperature = Mathf.Clamp(insideTemperature + cookSpeed, 0f, maxInsideTemperature);
        }
    }

    private void UpdateChickenColor()
    {
        if (TryParseHtmlColors(out Color colorRaw, out Color colorBurnt))
        {
            float cookCompletion = sideCookTimes[currentSide] / 10;
            chickenRenderers[currentSide].material.SetColor("_Color", Color.Lerp(colorRaw, colorBurnt, cookCompletion));
            UpdateSideCookState();
        }
    }

    private bool TryParseHtmlColors(out Color colorRaw, out Color colorBurnt)
    {
        colorBurnt = default;
        return ColorUtility.TryParseHtmlString(cookedColorRaw, out colorRaw) &&
               ColorUtility.TryParseHtmlString(cookedColorBurnt, out colorBurnt);
    }

    private void UpdateSideCookState()
    {
        sideCookStates[currentSide] = sideCookTimes[currentSide] switch
        {
            < 10 => SideCookState.Raw,
            >= 10 and < 20 => SideCookState.Cooked,
            _ => SideCookState.Burnt
        };
    }

    public bool IsFullyCooked(float targetCookCompletion)
    {
        return sideCookCompletion.All(completion => completion / 10 >= targetCookCompletion);
    }
}