using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject timerBonus;

    [Range(0,1)]
    public float setVolume = 1f;
    float volume;

    public GameObject[] platform;
    public GameObject platformParent;
    public int maxPlatforms = 50;

    public GameObject gameUI;
    public GameObject startUI;
    public GameObject pauseUI;

    public GameObject player;

    public float width;

    [Range(1f, 4f)]
    public float gap;
    public float height;
    float interval = 1;

    float triggerPoint;

    public float setTimer;
    [HideInInspector]
    public float timer;
    public Text timerText;
    public Text heightText;
    public Text highScoreText;
    public int initialTimerBonusFreq = 10;
    int timerBonusFreq;

    bool alive;

    PlatformController platformController;

    public float intitialLightIntensity;
    public Light playerLight;

    float highscore;

    public GameObject soundSources;
    public GameObject cam;

    public GameObject[] music;

    AudioSource musicSource;

    public float topHeight;

    private void Start()
    {
        alive = false;
        player.SetActive(false);
        pauseUI.SetActive(false);
        startUI.SetActive(true);
        music = GameObject.FindGameObjectsWithTag("Music");
    }

    private void Update()
    {
        //gameplay
        if (alive == true)
        {
            //Build
            if (player.transform.position.y > triggerPoint)
            {
                Build();
                triggerPoint = triggerPoint + interval;
            }

            //timer
            if (Mathf.Round(timer) > -1)
            {
                Timer();
            }
            if (Mathf.Round(timer) == -1)
            {
                GameOver();
            }

            //Light dim
            if (timer < 10f)
            {
                playerLight.intensity = intitialLightIntensity * (timer / 10f);
            }
            if (timer > 10f)
            {
                playerLight.intensity = intitialLightIntensity;
            }

            UpdateHeight();
            Music();

            soundSources.transform.position = cam.transform.position;
        }

        //Auto Retry
        if(player.transform.position.y < 0 && Input.GetButtonDown("Jump") && !alive)
        {      
            Retry();
        }

        //Pause
        if(Input.GetButtonDown("Cancel"))
        {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            Time.timeScale = 0f;
        }   
    }

    //UI~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Begin()
    {
        Setup();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        Setup();
    }

    //GAME~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Setup()
    {
        gameUI.SetActive(true);
        player.SetActive(true);
        pauseUI.SetActive(false);
        startUI.SetActive(false);
        alive = true;
        timer = setTimer;
        Time.timeScale = 1f;
        triggerPoint = 1;
        timerBonusFreq = initialTimerBonusFreq;
        playerLight.intensity = intitialLightIntensity;
        if(float.Parse(heightText.text) > highscore)
        {
            highscore = float.Parse(heightText.text);
            highScoreText.text = "highscore: " + highscore;
        }
        heightText.text = "0";

        for(int i =0; i < music.Length-1; i++)
        {
            musicSource = music[i].GetComponent<AudioSource>();
            musicSource.volume = 0f;
            musicSource.Play();
            
        }
        volume = setVolume;
    }

    void Build()
    {
        float playerX = player.transform.position.x;
        if (Mathf.Round(timer) > 0)
        {
            for (int i = 0; i < interval; i++)
            { 
                Instantiate(platform[Random.Range(0, platform.Length - 1)], new Vector3(Random.Range(-width+playerX, width+playerX), triggerPoint + i * Random.Range(0f, 2f) * gap, 0), Quaternion.identity, platformParent.transform);
            }
        }

        if(Mathf.Round(player.transform.position.y % timerBonusFreq) == 1)
        {
            Instantiate(timerBonus, new Vector3(Random.Range(-(width+2)+playerX, width +2 + playerX), player.transform.position.y+5, 0), Quaternion.identity);
        }        
    }

    void UpdateHeight()
    {
        height = Mathf.Round(player.transform.position.y);       
        if(height > float.Parse(heightText.text))
        {
            heightText.text = height.ToString();
        }
    }

    void Timer()
    {
            float timerRounded = Mathf.Round(timer);
            timerText.text = timerRounded.ToString();
            timer = timer - Time.deltaTime;
    }

    void GameOver()
    {
        alive = false;
        GameObject[] placedPlatforms = (GameObject.FindGameObjectsWithTag("platforms"));
        if(placedPlatforms.Length > maxPlatforms)
        {
            for(int i = 0; i < (placedPlatforms.Length-maxPlatforms); i++)
            {
                Destroy(placedPlatforms[i]);
            }
        }
        for (int i = 0; i < placedPlatforms.Length; i++)
        {
            Rigidbody2D rb = placedPlatforms[i].GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.None;
        }

        timerText.text = "";
    }

    void Music()
    {    
        if (float.Parse(heightText.text) < (topHeight / 5))
        {
            musicSource = music[0].GetComponent<AudioSource>();
            musicSource.volume = volume*(float.Parse(heightText.text) / (topHeight*2));
        }
        if (float.Parse(heightText.text) > (topHeight / 5) && float.Parse(heightText.text) < (topHeight / 5)*2)
        {
            musicSource = music[1].GetComponent<AudioSource>();
            musicSource.volume = volume*((float.Parse(heightText.text)-(topHeight/5)) / (topHeight * 2));
        }
        if (float.Parse(heightText.text) > (topHeight / 5)*2 && float.Parse(heightText.text) < (topHeight / 5) * 3)
        {
            musicSource = music[2].GetComponent<AudioSource>();
            musicSource.volume = volume * ((float.Parse(heightText.text) - (topHeight / 5)*2) / (topHeight * 2));
        }
        if (float.Parse(heightText.text) > (topHeight / 5) * 3 && float.Parse(heightText.text) < (topHeight / 5) * 4)
        {
            musicSource = music[3].GetComponent<AudioSource>();
            musicSource.volume = volume * ((float.Parse(heightText.text) - (topHeight / 5) * 3) / (topHeight * 2));
        }
        if (float.Parse(heightText.text) > (topHeight / 5) * 4 && float.Parse(heightText.text) < topHeight)
        {
            musicSource = music[4].GetComponent<AudioSource>();
            musicSource.volume = volume * ((float.Parse(heightText.text) - (topHeight / 5) * 4) / (topHeight * 2));
        }
    }
}