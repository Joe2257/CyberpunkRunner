using UnityEngine;

public class GameManager : MonoBehaviour
{

    ScreenOrientation screenOrientation;

    private void Awake()
    {
        screenOrientation = ScreenOrientation.LandscapeRight;

        Screen.orientation = screenOrientation;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;

        DontDestroyOnLoad(gameObject);
    }
}
