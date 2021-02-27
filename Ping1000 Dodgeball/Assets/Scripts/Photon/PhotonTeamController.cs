using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonTeamController : MonoBehaviour
{
    public PhotonView photonView;
    public GameObject playerCamera;

    public List<PhotonPlayerController> members;
    [HideInInspector]
    public int finishedActing;
    
    void Awake()
    {
        
    }

    void Start()
    {
        isPlanning = false;
        isActing = false;
        if (photonView.isMine)
        {
            GameObject.Find("Main Camera").SetActive(false);
            GameObject.Find("Canvas").SetActive(false);
            playerCamera.SetActive(true);

        }
    }

    public void BeginPlanPhase()
    {
        if (!photonView.isMine) { return; }
        StartCoroutine(PlanningPhase());
    }

    
    //[HideInInspector]
    public bool isPlanning;
    IEnumerator PlanningPhase()
    {
        //isPlanning = true;
        photonView.RPC(nameof(SetPlanning), PhotonTargets.AllBuffered, true);
        Debug.Log("Beginning planning phase..." + this.name);

        // TODO allow player to select characters individually 
        foreach (PhotonPlayerController m in members)
        {
            if (m != null)
            {
                m.SelectCharacter();
                yield return new WaitUntil(() => m.areActionsBuilt);
            }
        }


        Debug.Log("Planning phase complete." + this.name);
        photonView.RPC(nameof(SetPlanning), PhotonTargets.AllBuffered, false);
        //isPlanning = false;
    }
    public void BeginActionPhase()
    {
        if (!photonView.isMine) { return; }
        StartCoroutine(ActionPhase());
    }

   
    //[HideInInspector]
    public bool isActing;
    IEnumerator ActionPhase()
    {
        //isActing = true;
        photonView.RPC(nameof(SetActing), PhotonTargets.AllBuffered, true);

        Debug.Log("Beginning action phase..." + this.name);
        foreach (PhotonPlayerController m in members)
        {
            m.ExecuteActions();
        }
        finishedActing = 0;
        yield return new WaitForEndOfFrame();
        // surely there's a better way to manage this
        //foreach (ActionController m in members) {
        //    yield return new WaitUntil(() => !(m.isActing));
        //}
        yield return new WaitUntil(() => finishedActing >= members.Count);
        Debug.Log("Action phase complete." + this.name);
        photonView.RPC(nameof(SetActing), PhotonTargets.AllBuffered, false);
        //isActing = false;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
           /* stream.SendNext(isActing);
            stream.SendNext(isPlanning);*/
        }
        else
        {
            /*isActing = (bool)stream.ReceiveNext();
            isPlanning = (bool)stream.ReceiveNext();*/
        }
    }

    [PunRPC]
    void SetActing(bool val)
    {
        isActing = val;
        Debug.Log(this.name + " acting set to " + val);
    }

    [PunRPC]
    void SetPlanning(bool val)
    {
        isPlanning = val;
        Debug.Log(this.name + " planning set to " + val);
    }
}
