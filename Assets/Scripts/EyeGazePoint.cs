using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Rendering;
using Metaface.Utilities;
using System.Linq;
using UnityEngine.AI;
using Oculus.Interaction.Input;

public class EyeGazePoint : MonoBehaviour
{
    [SerializeField]
    OVREyeGaze leftEye;

    [SerializeField]
    OVREyeGaze rightEye;

    [SerializeField]
    BlinkHelper blinkHelper;

    [SerializeField]
    private TextMeshProUGUI leftEyePosition, rightEyePosition, rightEyeRot, leftEyeRot, headPos, headRot, blinkCounter, blinkTime, EyeTime, FaceTime, eyeCount, faceCount;

    private Vector3 headPosition;
    private Vector3 headrotationEulerAngles;

    private int blinkCount, hitEyeCounter, hitFaceCounter;
    private float lastBlinkTime, hitEyeTimer, hitFaceTimer;

    private OVRPlugin.BodyState _bodyState;

    public GameObject eyeCursor;
    private LineRenderer combinedRay;

    private Vector3 combineEyePos;
    private bool onFace, onEye;
    Vector3[] buffer = new Vector3[5];
    int bufferIdx = 0; 


    private void Start()
    {
        blinkHelper.OnBlink.AddListener((blink) =>
        {
            lastBlinkTime = blink.EyesClosedTime;
            blinkCount++;
            blinkCounter.text = blinkCount.ToString();
            blinkTime.text = lastBlinkTime.ToString();
        });

        combinedRay = Instantiate(new GameObject("CombinedRay")).AddComponent<LineRenderer>();
        combinedRay.startWidth = combinedRay.endWidth = 0.01f;
        combinedRay.startColor = combinedRay.endColor = Color.green;

    }


    void Update()
    {
        leftEyePosition.text = leftEye.transform.position.ToString();
        rightEyePosition.text = rightEye.transform.position.ToString();
        leftEyeRot.text = leftEye.transform.eulerAngles.ToString();
        rightEyeRot.text = rightEye.transform.eulerAngles.ToString();

        //combining eye gaze position
        Vector3 rawComboEyePos = (leftEye.transform.position + rightEye.transform.position) / 2;
        //combine eye direction
        Vector3 normEyeDirection = (leftEye.transform.forward.normalized + rightEye.transform.forward.normalized).normalized;

        //smooth out gaze points add timer to collider and counter.
        
        buffer[bufferIdx] = rawComboEyePos;
        bufferIdx = (bufferIdx + 1) % 5;
        Vector3 sum = Vector3.zero;
        for(int i = 0; i < 5; i++)
        {
            sum += buffer[i];
        }

        combineEyePos = sum / 5;

        //lerp
        combineEyePos = Vector3.Lerp(combineEyePos, rawComboEyePos, 0.1f);

        raycastEye(combineEyePos, normEyeDirection);


        if (OVRPlugin.GetBodyState(OVRPlugin.Step.Render, ref _bodyState))
        {
            var tempV = _bodyState.JointLocations[(int)OVRPlugin.BoneId.Body_Head].Pose.Position;
            headPosition = new Vector3(tempV.x, tempV.y, tempV.z);
            var tempQ = _bodyState.JointLocations[(int)OVRPlugin.BoneId.Body_Head].Pose.Orientation;

            headrotationEulerAngles = new Quaternion(tempQ.x, tempQ.y, tempQ.z, tempQ.w).eulerAngles;
            headPos.text = headPosition.ToString();
            headRot.text = headrotationEulerAngles.ToString();
        }
    }

    public void raycastEye(Vector3 position, Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, dir, out hit))
        {
            eyeCursor.transform.position = hit.point;
            Debug.Log("combine" + combineEyePos + "cursor" + eyeCursor.transform.position + "hit object" + hit.transform.name); //this is where it collides add a timer and counter write into csv file

            //add counter and timer for the hit.transform.name
            if (hit.transform.name == "Eyes")
            {
                if (!onEye)
                {
                    hitEyeCounter++;
                    eyeCount.text = hitEyeCounter.ToString();
                }
                onEye=true;
                hitEyeTimer += Time.deltaTime;
                EyeTime.text = hitEyeTimer.ToString();
            }
            else
            {
                onEye = false;
                //hitEyeTimer=0f;
            }



            if (hit.transform.name == "WholeFace")
            {
                if (!onFace)
                {
                    faceCount.text = hitFaceCounter.ToString();
                    hitFaceCounter++;
                }
                onFace = true;
                hitFaceTimer += Time.deltaTime;
                FaceTime.text = hitFaceTimer.ToString();
            }
            else
            {
                onFace = false;
                //hitFaceTimer=0f;
            }

        }
    }
}
