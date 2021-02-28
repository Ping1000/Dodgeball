using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLevelController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject sceneCamera;
    public GameObject redTeamPrefab;
    public GameObject blueTeamPrefab;
    public GameObject phaseControllerPrefab;

    public Transform redSpawn;
    public Transform blueSpawn;

    public bool red;
    public bool blue;
    public bool phase;

    private PhotonView photonView;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        canvas.SetActive(true);
        red = false;
        blue = false;
        phase = false;
        
    }

    public void SpawnRed()
    { // TODO make sure button that is already pressed, nonpressable
        if (red) { return; }
        GameObject player = PhotonNetwork.Instantiate(redTeamPrefab.name, redSpawn.position, redSpawn.rotation, 0);
        Debug.Log(player.name + " is owned by me?: " + player.GetPhotonView().isMine);
        photonView.RPC(nameof(SetRed), PhotonTargets.AllBuffered);

        //red = true;
        
        
    }

    public void SpawnBlue()
    {
        if (blue) { return; }
        GameObject player = PhotonNetwork.Instantiate(blueTeamPrefab.name, blueSpawn.position, blueSpawn.rotation, 0);
        Debug.Log(player.name + " is owned by me?: " + player.GetPhotonView().isMine);
        photonView.RPC(nameof(SetBlue), PhotonTargets.AllBuffered);

        //blue = true;

        
    }

    void Update()
    {
        if (!phase && red && blue)
        {
            
            //phase = true;
            photonView.RPC(nameof(SetPhase), PhotonTargets.AllBuffered);
            GameObject phaseControllerObject = PhotonNetwork.Instantiate(phaseControllerPrefab.name, Vector3.zero, Quaternion.identity, 0);
            PhotonPhaseController phaseController = phaseControllerObject.GetComponent<PhotonPhaseController>();
            phaseController.redTeam = GameObject.FindGameObjectWithTag("Red Team").GetComponent<PhotonTeamController>();
            phaseController.blueTeam = GameObject.FindGameObjectWithTag("Blue Team").GetComponent<PhotonTeamController>();
            //phaseController._txt = GameObject.Find("Text").GetComponent<TextManager>();
            photonView.RPC(nameof(PhaseAnnounce), PhotonTargets.AllBuffered);

        }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            /*stream.SendNext(red);
            stream.SendNext(blue);
            stream.SendNext(phase);*/
        }
        else
        {
            /*red = (bool)stream.ReceiveNext();
            blue = (bool)stream.ReceiveNext();
            phase = (bool)stream.ReceiveNext();*/

        }
    }

    [PunRPC]
    void SetRed()
    {
        red = true;
        Debug.Log("Red player built");
    }

    [PunRPC]
    void SetBlue()
    {
        blue = true;
        Debug.Log("Blue player built");

    }

    [PunRPC]
    void SetPhase()
    {
        phase = true;
        Debug.Log("Phase Controller building");
    }

    [PunRPC]
    void PhaseAnnounce()
    {
        Debug.Log("Phase Controller Built");
        //Debug.Log("Phase is controlled by me? ");
    }


}
