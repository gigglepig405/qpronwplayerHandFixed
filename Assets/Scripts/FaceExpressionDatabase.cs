using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Records face express 
/// </summary>
public class FaceExpressionDatabase : MonoBehaviour
{

    [Header("Settings")]

    [SerializeField]
    private OVRFaceExpressions ovrFaceExpressions;

    [SerializeField, Range(1, 60)]
    private float recordFPS = 60f;

    public bool facialTracker = false;

    public float[] facialParameters;

    bool isRecording = false;

    private void Start()
    {
        facialParameters = new float[63];
    }

    private void Update()
    {
        if (facialTracker) { 
            if (isRecording == true)  
                StopRecording(); 
            else StartRecording();  
        }
    }


    void StartRecording()
    {
        facialTracker = false;
        isRecording = true;
        StartCoroutine(RecordRoutine());

    }

    public void StopRecording()
    {
        isRecording = false;
    }


    private IEnumerator RecordRoutine()
    {

        float time = 0;
        float timeTotal = 0;
        float fraction = 1000 / recordFPS;
        while (isRecording)
        {
            time += Time.deltaTime * 1000;
            timeTotal += Time.deltaTime;
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

        for (var expressionIndex = 0; expressionIndex < (int)OVRFaceExpressions.FaceExpression.Max; //63 = max
                ++expressionIndex)
        {
            //Try get the weight
            float weight;
            if (ovrFaceExpressions.TryGetFaceExpressionWeight((OVRFaceExpressions.FaceExpression)expressionIndex, out weight))
                facialParameters[expressionIndex] = weight;
            else
                facialParameters[expressionIndex] = 0;
        }
    }
}