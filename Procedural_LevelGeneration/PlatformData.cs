using System.Collections;
using UnityEngine;

public enum StartDirection { Forward, Left, Right }
public enum EndDirection   { Forward, Left, Right }
public class PlatformData : MonoBehaviour
{
    public StartDirection startDirection;
    public EndDirection   endDirection;

    [Header("PlatformSpecs")]
    public Transform   connectionPoint;
    public Transform[] obstaclesPositions;
    public Transform   obstaclesRotation;
    [Space]
    public Transform[] aiSpawnPositions;
    public Transform[] collectablesSpawnPositions;

    [Header("Collectables")]
    public GameObject coinsArray;
    public GameObject ammo;
    public GameObject healthPack;
    public GameObject powerUp;


    [Header("Spawnable AI")]
    public GameObject policeCar;
    public GameObject[] spawnableAI;

    [Header("SpawnVariables")]
    private bool _aiHasBeenSpawned  = false;
    public float policeCarSpawnTime = 0;

    public GameObject _aiCollector;


    private AI_DespawnTrigger _despawnTrigger;
    private AI_SpawnTrigger   _spawnTrigger;

    private void Start()
    {
        _spawnTrigger   = GetComponentInChildren<AI_SpawnTrigger>();
        _despawnTrigger = GetComponentInChildren<AI_DespawnTrigger>();

        SpawnCollectables();
    }

    private void Update()
    {
        if (_spawnTrigger)
        {
            if (_spawnTrigger.spawnAI == true && !_aiHasBeenSpawned)
                SpawnAI();
        }

        if(_despawnTrigger)
        {
            if (_despawnTrigger.despawn)
            { _aiCollector.SetActive(false); }
        }
    }

    //Spawn a fixed amount of collectables on the platform of random type.
    private void SpawnCollectables()
    {
        int healthPackLock = 0;
        int ammoLock       = 0;
        int powerUpLock    = 0;

        for (int i = 0; i < collectablesSpawnPositions.Length; i++)
        {
            float collectableRandomization = Random.Range(0, 100f);

            if (collectableRandomization > 0 && collectableRandomization < 10 && ammoLock < 3)
            {
               GameObject clone = Instantiate(ammo, collectablesSpawnPositions[i].transform.position, obstaclesRotation.rotation);

                clone.transform.parent = _aiCollector.transform.parent;

                ammoLock++;
            }
            else 
            if (collectableRandomization > 9 && collectableRandomization < 25 && healthPackLock < 3)
            {
                GameObject clone = Instantiate(healthPack, collectablesSpawnPositions[i].transform.position, obstaclesRotation.rotation);

                clone.transform.parent = _aiCollector.transform.parent;

                healthPackLock++;
            }
            else
            if (collectableRandomization > 24 && collectableRandomization < 80)
            {
                GameObject clone = Instantiate(coinsArray, collectablesSpawnPositions[i].transform.position, obstaclesRotation.rotation);

                clone.transform.parent = _aiCollector.transform.parent;
            }
            else
            if (collectableRandomization > 79 && collectableRandomization < 101 && powerUpLock < 2)
            {
                GameObject clone = Instantiate(powerUp, collectablesSpawnPositions[i].transform.position, obstaclesRotation.rotation);

               clone.transform.parent = _aiCollector.transform.parent;

                powerUpLock ++;
            }

            if (i == collectablesSpawnPositions.Length)
            {
                healthPackLock = 0;
                ammoLock = 0;
                powerUpLock = 0;
            }
        }
    }

    //Decide which AI to spawn based on the Platform Start and End Direction.
    private void SpawnAI()
    {
        _aiHasBeenSpawned = true;

        if (endDirection == EndDirection.Left || endDirection == EndDirection.Right)
        {
            StartCoroutine(PoliceCarsRoutineRight());
        }
        else if (endDirection == EndDirection.Forward)
        {
            SpawnBots();
        }
    }

    //"Car only" Spawn a new car in the opposite side of the building after time is expired.
    private IEnumerator PoliceCarsRoutineRight()
    {
        GameObject car = Instantiate(policeCar, aiSpawnPositions[0].transform.position, aiSpawnPositions[0].transform.rotation);

        car.transform.parent = _aiCollector.transform;

        yield return new WaitForSeconds(policeCarSpawnTime);

        if(!_despawnTrigger.despawn)
          StartCoroutine(PoliceCarsRoutineLeft());
    }

    private IEnumerator PoliceCarsRoutineLeft()
    {
        GameObject car = Instantiate(policeCar, aiSpawnPositions[1].transform.position, aiSpawnPositions[0].transform.rotation);

        car.transform.parent = _aiCollector.transform;

        yield return new WaitForSeconds(policeCarSpawnTime);

        if (!_despawnTrigger.despawn)
            StartCoroutine(PoliceCarsRoutineRight());
    }

    //"Robots only" Spawn a fixed amount of robots on the platform of random type.
    private void SpawnBots()
    {
        for (int i = 0; i < aiSpawnPositions.Length; i++)
        {
            GameObject AI = Instantiate(spawnableAI[Random.Range(0, 3)], aiSpawnPositions[i].transform.position, obstaclesRotation.rotation);

            AI.transform.parent = _aiCollector.transform;
        }

        
    }
}
