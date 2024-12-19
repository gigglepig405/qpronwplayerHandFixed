using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerGestureSync : MonoBehaviourPunCallbacks
{
//*********************************    
    [Header("General Config")]

    [SerializeField, Range(1, 60)]
    private float recordFPS = 30f;

    public GameObject PuppetPlayer; //To check if 3DHead is naturally active or non (Self or Networked)

//*********************************
    [Header("Input (Main) Avatar")]

    [SerializeField]
    private OVRFaceExpressions ovrFaceExpressions;

    public float[] facialParameters;

    public bool isRecording = false;
    public bool bootFaceSync = false;

//*********************************
    [Header("Output (Puppet) Avatar")]

    [SerializeField]
    private FacePlaybackSystem self_facePlaybackSystem;

    [SerializeField]
    private FacePlaybackSystem facePlaybackSystem;

    private bool isPlayback = false;
    public bool startPlayback;

//*********************************

    private void Start()
    {
       // Initialize and modify the array only on the local client       
        facialParameters = new float[70];               
        bootFaceSync = true;
    }

    private void Update()
    {
        if (photonView.IsMine && bootFaceSync && PhotonNetwork.CurrentRoom.PlayerCount > 1) // CHANGE TO >=1 FOR 1P-ONLY TEST MODE
        {
            isRecording = true;
            bootFaceSync = false;
            StartCoroutine(RecordRoutine());
        }

        if (startPlayback && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            startPlayback = false;
            isPlayback = true;
            StartCoroutine(PlaybackRoutine());
        }
    }


    private IEnumerator RecordRoutine()
    {
        float time = 0;
        float fraction = 1000 / recordFPS;
        while (isRecording)
        {
            time += Time.deltaTime * 1000;
            if (time >= fraction)
            {
                UpdateFaceData();
                time = 0;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void UpdateFaceData()
    {

        for (var expressionIndex = 0; expressionIndex < (int)OVRFaceExpressions.FaceExpression.Max; //70 = max
                ++expressionIndex)
        {
            float weight;
            if (ovrFaceExpressions.TryGetFaceExpressionWeight((OVRFaceExpressions.FaceExpression)expressionIndex, out weight))
                facialParameters[expressionIndex] = weight;
            else
                facialParameters[expressionIndex] = 0;
        }

        photonView.RPC("UpdateFacialParameters", RpcTarget.Others, facialParameters);
    }

    [PunRPC]
    public void UpdateFacialParameters(float[] updatedParameters)
    {
        // Receive and apply the updated facialParameters from the network
        this.facialParameters = updatedParameters;
    }

    private IEnumerator PlaybackRoutine()
    {

        float time = 0;
        float fraction = 1000 / recordFPS;

        int lineIdx = 0;
        while (isPlayback)
        {
            time += Time.deltaTime * 1000;
            if (time >= fraction)
            {

                if (photonView.IsMine)
                {
                    self_facePlaybackSystem?.ApplyFaceWeight(facialParameters);
                }
                else
                {
                    facePlaybackSystem?.ApplyFaceWeight(facialParameters);
                }
                
                time = 0;
                lineIdx++;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}