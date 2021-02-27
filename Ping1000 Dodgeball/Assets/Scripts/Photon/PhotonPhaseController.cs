using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonPhaseController : MonoBehaviour
{
    [HideInInspector]
    public PhotonTeamController redTeam;
    [HideInInspector]
    public PhotonTeamController blueTeam;
    [HideInInspector]
    public TextManager _txt;

    public int maxBalls;
    public List<PhotonBallController> balls;

    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private Transform ballSpawnA;
    [SerializeField]
    private Transform ballSpawnB;

    private bool canPlayRound;

    private PhotonView photonView;

    // Start is called before the first frame update
    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        isPlayingRound = false;
        canPlayRound = true;
        RespawnBalls();
        //_txt = GameObject.Find("Text").GetComponent<TextManager>(); // TODO Do this properly
        //_txt.redText.text = "Red: " + redTeam.members.Count;
        //_txt.blueText.text = "Blue: " + blueTeam.members.Count;
    }


    void Update()
    {

        if (canPlayRound && !isPlayingRound && redTeam != null && blueTeam != null)
        {
            StartCoroutine(PlayingRound());
        }

    }

    // Don't use these RPC's I think
    [PunRPC]
    void RpcBeginPlanPhase()
    { // Possible source of bugs here, these might need their own RPC, using [TargetRpc]
        // Do I need to include _txt or is updating on server good enough?
        //_txt.phaseText.text = "Current Phase: Planning";
        /*Debug.Log("Is red team null? " + redTeam == null);
        Debug.Log(redTeam.name);

        Debug.Log("Does photonView exist? " + redTeam.gameObject.GetPhotonView());
        Debug.Log("I control red team: " + redTeam.gameObject.GetPhotonView().isMine)*/;
        blueTeam.BeginPlanPhase();

        redTeam.BeginPlanPhase();
    }

    [PunRPC]
    void RpcBeginActionPhase()
    {
        //_txt.phaseText.text = "Current Phase: Action";
        redTeam.BeginActionPhase();
        blueTeam.BeginActionPhase();
    }

    [HideInInspector]
    public bool isPlayingRound;
    IEnumerator PlayingRound()
    {
        isPlayingRound = true;

        // plan phase
        //photonView.RPC(nameof(RpcBeginPlanPhase), PhotonTargets.AllBuffered);
        //RpcBeginPlanPhase();
        redTeam.BeginPlanPhase();
        blueTeam.BeginPlanPhase();
        yield return new WaitUntil(() => !(redTeam.isPlanning) && !(blueTeam.isPlanning));
        /*redTeam.BeginPlanPhase();
        yield return new WaitUntil(() => !(redTeam.isPlanning));
        blueTeam.BeginPlanPhase();
        yield return new WaitUntil(() => !(blueTeam.isPlanning));*/

        // action phase
        //RpcBeginActionPhase();
        //photonView.RPC(nameof(RpcBeginActionPhase), PhotonTargets.AllBuffered);
        redTeam.BeginActionPhase();
        blueTeam.BeginActionPhase();
        yield return new WaitUntil(() => !(redTeam.isActing) && !(blueTeam.isActing));
        /*redTeam.BeginActionPhase();
        blueTeam.BeginActionPhase();
        yield return new WaitUntil(() => !(redTeam.isActing));
        yield return new WaitUntil(() => !(blueTeam.isActing));*/

        int redCount = redTeam.members.Count;
        int blueCount = blueTeam.members.Count;
        //_txt.redText.text = "Red: " + redCount;
        //_txt.blueText.text = "Blue: " + blueCount;
        if (redCount == 0 && blueCount == 0)
        {
            // draw
            Debug.Log("Draw");
            canPlayRound = false;
        }
        else if (redCount != 0 && blueCount == 0)
        {
            // red wins
            Debug.Log("Red Wins");
            canPlayRound = false;
        }
        else if (redCount == 0 && blueCount != 0)
        {
            // blue wins
            Debug.Log("Blue Wins");
            canPlayRound = false;
        }

        // do other stuff (music, camera, etc)
        // delay?
        // respawn balls
        RespawnBalls();
        isPlayingRound = false;
    }

    void RespawnBalls()
    {
        while (balls.Count < maxBalls)
        { // TODO add avoidance to spawn so they don't stack on eachother as much
          //GameObject newBall = (GameObject)Instantiate(Resources.Load("Prefabs/Ball"));

            Vector3 newBallPos = new Vector3(Random.Range(ballSpawnA.position.x, ballSpawnB.position.x),
                ballSpawnA.position.y,
                Random.Range(ballSpawnA.position.z, ballSpawnB.position.z));
            GameObject newBall = PhotonNetwork.Instantiate(ballPrefab.name, newBallPos, Quaternion.identity, 0);
            balls.Add(newBall.GetComponent<PhotonBallController>());

            
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}
