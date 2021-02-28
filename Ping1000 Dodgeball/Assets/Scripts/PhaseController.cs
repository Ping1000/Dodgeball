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

    private bool canPlayRound;

    // Start is called before the first frame update
    void Start()
    {
        isPlayingRound = false;
        canPlayRound = true;
        RespawnBalls();
        _txt.redText.text = "Red: " + redTeam.members.Count;
        _txt.blueText.text = "Blue: " + blueTeam.members.Count;
    }

    // Update is called once per frame
    void Update() {
        if (canPlayRound && !isPlayingRound)
            StartCoroutine(PlayingRound());
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

        int redCount = redTeam.members.Count;
        int blueCount = blueTeam.members.Count;
        _txt.redText.text = "Red: " + redCount;
        _txt.blueText.text = "Blue: " + blueCount;
        if (redCount == 0 && blueCount == 0) {
            // draw
            Debug.Log("Draw");
            canPlayRound = false;
        } else if (redCount != 0 && blueCount == 0) {
            // red wins
            Debug.Log("Red Wins");
            canPlayRound = false;
        } else if (redCount == 0 && blueCount != 0) {
            // blue wins
            Debug.Log("Blue Wins");
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
            GameObject newBall = (GameObject)Instantiate(Resources.Load("Prefabs/Ball"));
            Vector3 newBallPos = new Vector3(Random.Range(ballSpawnA.position.x, ballSpawnB.position.x),
                ballSpawnA.position.y,
                Random.Range(ballSpawnA.position.z, ballSpawnB.position.z));
            newBall.transform.position = newBallPos;
            balls.Add(newBall.GetComponent<BallController>());
        }
    }
}
