using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public TeamController redTeam;
    public TeamController blueTeam;
    public TextManager _txt;
    public MusicManager _music;

    public int maxBalls;
    public List<BallController> balls;

    [SerializeField]
    private Transform ballSpawnA;
    [SerializeField]
    private Transform ballSpawnB;

    private WinScreen winScreen;

    private bool canPlayRound;
    [HideInInspector]
    public float ballTimer;

    private string[] ballPrefabList = {
        "Prefabs/Foods/Food Balls/turkey",
        "Prefabs/Foods/Food Balls/pineapple",
        "Prefabs/Foods/Food Balls/tomato",
        "Prefabs/Foods/Food Balls/cake"
        // "Prefabs/Ball"
    };

    // Start is called before the first frame update
    void Start()
    {
        isPlayingRound = false;
        canPlayRound = true;
        RespawnBalls();
        _txt.redText.text = "Red: " + redTeam.members.Count;
        _txt.blueText.text = "Blue: " + blueTeam.members.Count;
        ballTimer = 0;
        winScreen = GetComponent<WinScreen>();
    }

    // Update is called once per frame
    void Update() {
        if (canPlayRound && !isPlayingRound)
            StartCoroutine(PlayingRound());
        if (ballTimer > 0)
            ballTimer -= Time.deltaTime;
    }

    [HideInInspector]
    public bool isPlayingRound;
    IEnumerator PlayingRound() {
        isPlayingRound = true;

        // plan phase
        _txt.phaseText.text = "Current Phase: Planning";
        redTeam.BeginPlanPhase();
        yield return new WaitUntil(() => !(redTeam.isPlanning));
        blueTeam.BeginPlanPhase();
        yield return new WaitUntil(() => !(blueTeam.isPlanning));

        // action phase
        _txt.phaseText.text = "Current Phase: Action";
        redTeam.BeginActionPhase();
        blueTeam.BeginActionPhase();
        yield return new WaitUntil(() => !(redTeam.isActing));
        yield return new WaitUntil(() => !(blueTeam.isActing));
        yield return new WaitUntil(() => ballTimer <= 0);

        int redCount = redTeam.members.Count;
        int blueCount = blueTeam.members.Count;
        _txt.redText.text = "Red: " + redCount;
        _txt.blueText.text = "Yellow: " + blueCount;
        if (redCount == 0 && blueCount == 0) {
            // draw
            winScreen.Show("Draw!");
            canPlayRound = false;
        } else if (redCount != 0 && blueCount == 0) {
            // red wins
            winScreen.Show("Red Wins!");
            canPlayRound = false;
        } else if (redCount == 0 && blueCount != 0) {
            // blue wins
            winScreen.Show("Yellow Wins!");
            canPlayRound = false;
        }

        // do other stuff (music, camera, etc)
        // delay?
        // respawn balls
        _music.UpdateSounds(redCount, blueCount);
        RespawnBalls();
        isPlayingRound = false;
    }

    void RespawnBalls() {
        while (balls.Count < maxBalls) {
            int i = Random.Range(0, ballPrefabList.Length);
            GameObject newBall = (GameObject)Instantiate(Resources.Load(ballPrefabList[i]));
            Vector3 newBallPos = new Vector3(Random.Range(ballSpawnA.position.x, ballSpawnB.position.x),
                ballSpawnA.position.y,
                Random.Range(ballSpawnA.position.z, ballSpawnB.position.z));
            newBall.transform.position = newBallPos;
            balls.Add(newBall.GetComponent<BallController>());
        }
    }
}
