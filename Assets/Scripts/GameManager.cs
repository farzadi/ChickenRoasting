using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Chicken chicken;
    public Campfire campfire;
    public Slider fireIntensitySlider;
    public Slider chickenInsideTemperatureSlider;
    public TextMeshProUGUI winFailMessage;
    public float perfectInsideTemperatureRange = 50f;
    public GameObject restart;

    private bool gameInProgress = true;

    private void Update()
    {
        if (!gameInProgress) return;

        HandleInput();
        chicken.Cook(campfire.GetIntensity(), Time.deltaTime);
        //
        fireIntensitySlider.value = campfire.GetIntensity();
        chickenInsideTemperatureSlider.value = chicken.insideTemperature / chicken.maxInsideTemperature;
        CheckFailConditions();
    }

    private void CheckFailConditions()
    {
        if (chicken.sideCookStates.Any(state => state == Chicken.SideCookState.Burnt))
        {
            Fail("You burnt the chicken!");
        }
        else if (chicken.sideCookStates.All(state => state == Chicken.SideCookState.Cooked))
        {
            winFailMessage.text = "You cooked all sides!";
        }
        else if (campfire.GetIntensity() <= 0)
        {
            Fail("The fire went out!");
        }
    }

    private void Fail(string message)
    {
        gameInProgress = false;
        winFailMessage.text = message + "\nTry again!";
        winFailMessage.gameObject.SetActive(true);
        restart.SetActive(true);
        Invoke("Restart", 5f);
    }

    private void Win()
    {
        gameInProgress = false;
        winFailMessage.text = "You cooked the perfect chicken!";
        winFailMessage.gameObject.SetActive(true);
        restart.SetActive(true);
        Invoke("Restart", 5f);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator CheckWinCondition()
    {
        while (gameInProgress)
        {
            Debug.Log(Mathf.Abs(chicken.insideTemperature - chicken.maxInsideTemperature) + " " +perfectInsideTemperatureRange );
            if (Mathf.Abs(chicken.insideTemperature - chicken.maxInsideTemperature) <= perfectInsideTemperatureRange)
            {
                if (chicken.sideCookStates.All(state => state == Chicken.SideCookState.Cooked))
                {
                    Win();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckWinCondition());
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            chicken.Rotate(true);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            chicken.Rotate(false);
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            campfire.IncreaseIntensity(0.1f);
        }
    }
}