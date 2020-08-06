using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedural_Generation : MonoBehaviour
{
    [Header("Platforms")]
    public GameObject[] leftPlatforms;
    public GameObject[] rightPlatforms;

    [Header("Obstacles")]
    public GameObject   barrier;
    public GameObject[] obstacles;
    public int          obstaclesToSpawn;

    [Header("StageControlLists")]
    public List<GameObject>  stageToDelete = new List<GameObject>();
    public List<GameObject>  previousStage = new List<GameObject>();
    public List<GameObject>  currentStage  = new List<GameObject>();

    [Space]
    [Header("GenericVariables")]
    public GameObject lastPlatform;
    public float      platformExpTime;
    public int        maxPlatforms = 3;

    private GameObject _newPlatform = null;

    private int        _loopInt;
    private int        _platformIndex     = 0;
    private int        _randomizePlatform = 0;

    private PlatformSpawnTrigger _spawnTrigger;
    private PlatformData         _previousPlatform;

    void Start()
    {
        _spawnTrigger = GetComponentInChildren<PlatformSpawnTrigger>();

        InitializeStartingPlatforms();
    }

    //Spawn the first set of platforms.
    private void InitializeStartingPlatforms()
    {
        _spawnTrigger.spawnNewPlatforms = true;
        SpawnPlatforms();
    }
    
    void Update()
    {
        SpawnPlatforms();
    }

    //------------PlatformsGeneration_System------------\\

    private void SpawnPlatforms()
    {
        if (_spawnTrigger.spawnNewPlatforms)
        {
            _platformIndex = 0;
            _spawnTrigger.spawnNewPlatforms = false;

            //Move the last platforms into an array to be deactivated.
            if (previousStage[_platformIndex] != null)
            {
                for (int i = 0; i < stageToDelete.Count; i++)
                {
                    stageToDelete[i] = previousStage[i];
                }

                StartCoroutine(DeletePlatforms());
            }

            //Spawn a new set of platforms.
            for (int _loopInt = 0; _loopInt < maxPlatforms; _loopInt++)
            {
                float randomization = Random.Range(0, 100f);

                if (randomization < 49)
                { _randomizePlatform = 0; }
                else
                { _randomizePlatform = 1; }

                _platformIndex = _loopInt;

                _previousPlatform = lastPlatform.GetComponent<PlatformData>();

                //Always spawn platforms in the opposite direction to avoid crossing each other.
                switch (_previousPlatform.endDirection)
                {
                    case EndDirection.Forward:
                        ForwardPlatform();
                        break;
                    case EndDirection.Left:
                        LeftPlatform();
                        break;
                    case EndDirection.Right:
                        RightPlatform();
                        break;
                    default:
                        break;
                }

                //Move the spawnTrigger at the end of the current set of platforms.
                if (_loopInt == 0)
                    _spawnTrigger.gameObject.transform.position = _previousPlatform.connectionPoint.transform.position;
            }

            StartCoroutine(MoveBarrier());
        }
    }

    //Move the barrier behind the player to the last platform of the current set.
    //This barrier is needed to avoid that the player goes back in the old platforms.
    private IEnumerator MoveBarrier()
    {
        yield return new WaitForSeconds(60f);

        barrier.transform.position = previousStage[0].transform.position;
        barrier.transform.rotation = previousStage[0].transform.rotation;
    }

    //If the last platform end forward spawn a platform that start or end right.
    private void ForwardPlatform()
    {
        if (_previousPlatform.startDirection != StartDirection.Left)
        {
            _newPlatform = Instantiate(leftPlatforms[_randomizePlatform],
            _previousPlatform.connectionPoint.transform.position, _previousPlatform.connectionPoint.transform.rotation);

            previousStage[_platformIndex] = currentStage[_platformIndex];
            currentStage[_platformIndex]  = _newPlatform;

            lastPlatform = _newPlatform;
        }
        else if (_previousPlatform.startDirection != StartDirection.Right)
        {
            _newPlatform = Instantiate(rightPlatforms[_randomizePlatform],
            _previousPlatform.connectionPoint.transform.position, _previousPlatform.connectionPoint.transform.rotation);

            previousStage[_platformIndex] = currentStage[_platformIndex];
            currentStage[_platformIndex]  = _newPlatform;

            lastPlatform = _newPlatform;
        }

        PlatformData newPlatformData = _newPlatform.GetComponent<PlatformData>();

        ObstacleGeneration(newPlatformData);
    }

    //Spawn a platform that end or start left.
    private void LeftPlatform()
    {
        _newPlatform = Instantiate(rightPlatforms[_randomizePlatform],
            _previousPlatform.connectionPoint.transform.position, _previousPlatform.connectionPoint.transform.rotation);

        previousStage[_platformIndex] = currentStage[_platformIndex];
        currentStage[_platformIndex]  = _newPlatform;

        lastPlatform = _newPlatform;

        PlatformData newPlatformData = _newPlatform.GetComponent<PlatformData>();

        ObstacleGeneration(newPlatformData);
    }

    //Spawn a platform that end or start right.
    private void RightPlatform()
    {
        _newPlatform = Instantiate(leftPlatforms[_randomizePlatform],
            _previousPlatform.connectionPoint.transform.position, _previousPlatform.connectionPoint.transform.rotation);

        previousStage[_platformIndex] = currentStage[_platformIndex];
        currentStage[_platformIndex]  = _newPlatform;

        lastPlatform = _newPlatform;

        PlatformData newPlatformData = _newPlatform.GetComponent<PlatformData>();

        ObstacleGeneration(newPlatformData);
    }

    //Generate the obstacles on the platform in random position.
    private void ObstacleGeneration(PlatformData platform)
    {
        for (int i = 0; i < obstaclesToSpawn; i++)
        {
           GameObject obstacle = Instantiate(obstacles[0], platform.obstaclesPositions[Random.Range(0, platform.obstaclesPositions.Length)].transform.position, platform.obstaclesRotation.rotation);

            obstacle.transform.parent = platform.gameObject.transform;
        }
    }

    //Delete old platforms.
    private IEnumerator DeletePlatforms()
    {
        yield return new WaitForSeconds(platformExpTime);

        for (int i = 0; i < previousStage.Count; i++)
        {
            if (stageToDelete[i] != null)
                stageToDelete[i].SetActive(false);
            else
                break;
        }
    }
}
