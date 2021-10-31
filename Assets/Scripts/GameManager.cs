using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    //References
    public GameObject enemyPrefab;
    public GameObject[] objectPrefab;
    public Transform rightObjectSpawnPosition;
    public Transform leftObjectSpawnPosition;
    public GameObject[] scenarioImages;
    public GameObject Scenario;
    public AudioClip BGMusic, MainTittleMusic, EasterEgg;
    public AudioSource audioS;
    public Player player;

    public TextMeshProUGUI BestScore, CurrentScore;
    //Variables
    public int spawnCount = 0;
    private bool spawned = false, objectSpawn;
    public int enemyLife = 1;
    public float enemySpeed = 3;
    public float parallaxSpeed = 0.02f;
    public float minObjectSpawnTime = 10f;
    public float maxObjectSpawnTime = 20f;

    public int currentScore = 0;
    public int bestScore = 0;

    void Awake()
    {
        LoadBestScore();
        Debug.Log("GameManager Start");
        int eE = Random.Range(1, 51);
        if(eE == 1)
        {
            Debug.Log("Easter Egg");
            audioS.clip = EasterEgg;
            audioS.Play();
        }
        else
        {
            Debug.Log("BG Music");
            audioS.clip = BGMusic;
            audioS.Play();
        }

        audioS.volume = PlayerPrefs.GetFloat("Audio");
    }
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Get a array of gameobjects with the tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //if enemies is empty instantiate a new enemyPrefab after wait 1 second
        if (enemies.Length == 0 && !spawned)
        {
            spawned = true;
            Invoke("SpawnEnemy", 1f);
        }
        //if objectSpawn is false call spawnObject with a random time between 1 and 3 seconds
        if (!objectSpawn)
        {
            objectSpawn = true;
            //call spawnObject with a random time between 10 and 20 seconds with a random position between the left and right spawn position using coroutine
            StartCoroutine(SpawnObjects(objectPrefab[Random.Range(0,3)], (Random.Range(0, 2) == 0 ? leftObjectSpawnPosition.position : rightObjectSpawnPosition.position), Random.Range(minObjectSpawnTime, maxObjectSpawnTime)));
            //StartCoroutine(SpawnObjects(objectPrefab, objectSpawnPosition.position, Random.Range(10, 20)));
        }
        //call parallax
        Parallax();
        audioS.pitch = (float)player.health / 10;

        BestScore.text = "Best: " + bestScore;
        CurrentScore.text = "Current: "+ currentScore;

        SaveBestScore();
    }
    void SpawnEnemy()
    {
        //Instantiate a new enemyPrefab at random x position between -8 and 8, y position is 4.47 and z position is 0
        GameObject Enemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-8f, 8f), 3.631945f, 0f), enemyPrefab.transform.rotation);
        //Set the name of the enemy to "Enemy" + spawnCount
        Enemy.name = "Enemy" + spawnCount;
        //Change the sprite renderer color of the enemy to a random color different from black
        Enemy.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //Increase spawnCount by 1
        spawnCount++;
        currentScore++;
        //Call EnemyStats function and give it the enemy gameobject
        EnemyStats(Enemy);
        //Set spawned to false
        spawned = false;
    }

    //Function for enemy stats
    public void EnemyStats(GameObject enemy)
    {
        //if spawnCount is a even number, increase the life of the enemy by 1
        if (spawnCount % 2 == 0 && spawnCount % 10 != 0)
        {
            enemyLife += 1;
            //set Enemy IA to false
        }
        if(spawnCount % 10 == 0)
        {
            //set Enemy IA to true
            enemy.GetComponent<Enemy>().IA = true;
            //increase enemy speed by 1
            enemySpeed += 1;
        }
        else
        {
            //set Enemy IA to false
            enemy.GetComponent<Enemy>().IA = false;
        }
        //Set the enemy's life to the enemyLife variable
        enemy.GetComponent<Enemy>().life = enemyLife;
        //Set the enemy's speed to the enemySpeed variable
        enemy.GetComponent<Enemy>().speed = enemySpeed;
    }

    //coroutine for calling spawn object
    public IEnumerator SpawnObjects(GameObject objectToSpawn, Vector3 position, float time)
    {
        //Wait for time
        yield return new WaitForSeconds(time);
        //Instantiate a new objectToSpawn at position
        GameObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);
        //Give to the spawned object a parabolic movement
        //if the position is on the left side of the screen add a positive x force between 0 and 15 and a y force between 0 and 4
        if (position.x < 0)
        {
            spawnedObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(5, 15), Random.Range(0, 4)), ForceMode2D.Impulse);
        }
        else
        {
            spawnedObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15, -5), Random.Range(0, 4)), ForceMode2D.Impulse);
        }

        
        objectSpawn = false;
    }

    //Vertical movement of the scenario images, when the first image is out of the screen, move it to the end of the list
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

    //save best score to playerprefs
    public void SaveBestScore()
    {
        //if the current score is greater than the best score
        if (currentScore > bestScore)
        {
            //set the best score to the current score
            bestScore = currentScore;
            //save the best score to playerprefs
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
    }

    //load best score from playerprefs
    public void LoadBestScore()
    {
        //set the best score to the playerprefs best score
        bestScore = PlayerPrefs.GetInt("BestScore");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
    

}
