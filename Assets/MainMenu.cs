using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public bool starting = false;

    public GameObject[] scenarioImages;

    public TextMeshProUGUI StartText;

    public Slider sliderSound;

    public AudioSource audioSource;
    public Rigidbody2D spaceShip;

    public GameObject Options;

    public float time = 3;
    public float parallaxSpeed = 0.02f;

    void start()
    {
        Screen.fullScreen = true;
    }
    void Update()
    {
        if(starting)
        {
            spaceShip.velocity = new Vector2(0, 5);
            time -= Time.deltaTime;
            StartText.text = Mathf.RoundToInt(time).ToString();
        }

        if(time <= 0)
        {
            time = 0;
            SceneManager.LoadScene("Game");
        }
        Parallax();

        audioSource.volume = sliderSound.value;

        PlayerPrefs.SetFloat("Audio", sliderSound.value);

        if(Input.GetKeyDown(KeyCode.Escape) && Options.activeInHierarchy)
        {
            Options.SetActive(false);
        }
    }

    public void StartButton()
    {
        starting = true;
    }

    public void Parallax()
    {
        //Get a array of gameobjects with the tag "Scenario"
        //For each scenario in the array
        foreach (GameObject scenario in scenarioImages)
        {
            //Move the scenario image up by speed
            scenario.transform.Translate(Vector3.up * parallaxSpeed);
            if (scenario.transform.position.y > 10.1f)
            {
                scenario.transform.position = new Vector3(scenario.transform.position.x, -10.5f, scenario.transform.position.z);
            }
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void OptionsButton()
    {
        if(Options.activeInHierarchy)
        {
            Options.SetActive(false);
        }
        else
        {
            Options.SetActive(true);
        }
    }
}
