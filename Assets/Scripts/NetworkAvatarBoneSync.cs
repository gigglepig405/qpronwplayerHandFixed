using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using Oculus.Movement.Effects;

public class NetworkAvatarBoneSync : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField, Range(1, 60)]
    private float recordFPS = 30f;
    float time = 0;
    float fraction;

    public Transform[] allChildTransforms;

    Transform camTrans;

    //public GameObject leftEye, rightEye;

    private void Start()
    {
        fraction = 1000 / recordFPS;

        if (photonView.IsMine)
        {
            //camAsset.SetActive(true);
            camTrans = GameObject.Find("CenterEyeAnchor").transform;
            this.transform.position = camTrans.position;
        }

    }

    private void Update()
    {
        time += Time.deltaTime * 1000;
    }


    // Method to retrieve the positions and rotations of all child objects
    private void GetChildPositionsAndRotations(out Vector3[] childPositions, out Quaternion[] childRotations)
    {
        int childCount = allChildTransforms.Length;
        childPositions = new Vector3[childCount];
        childRotations = new Quaternion[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childPositions[i] = allChildTransforms[i].position;
            childRotations[i] = allChildTransforms[i].rotation;
        }
    }

    // Method to apply positions and rotations to all child objects
    private void SetChildPositionsAndRotations(Vector3[] childPositions, Quaternion[] childRotations)
    {
        int childCount = allChildTransforms.Length;
        for (int i = 0; i < childCount; i++)
        {
            allChildTransforms[i].position = childPositions[i];
            allChildTransforms[i].rotation = childRotations[i];
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting && time >= fraction && photonView.IsMine)
        {
            Vector3[] childPositions;
            Quaternion[] childRotations;
            GetChildPositionsAndRotations(out childPositions, out childRotations);

            // Send data for the parent and all child objects
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(childPositions);
            stream.SendNext(childRotations);

            time = 0;        
        }

        if(stream.IsReading && time >= fraction && !photonView.IsMine)
        {
            Vector3 parentPosition = (Vector3)stream.ReceiveNext();
            Quaternion parentRotation = (Quaternion)stream.ReceiveNext();

            // Receive data for all child objects
            Vector3[] receivedChildPositions = (Vector3[])stream.ReceiveNext();
            Quaternion[] receivedChildRotations = (Quaternion[])stream.ReceiveNext();

            // Apply data to the parent
            transform.position = parentPosition;
            transform.rotation = parentRotation;

            // Apply data to child objects
            SetChildPositionsAndRotations(receivedChildPositions, receivedChildRotations);

            time = 0;
        }
    }
}
