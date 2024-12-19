using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metaface.Utilities
{
    public class EyeGazeHelper : MonoBehaviour
    {

        [SerializeField]
        OVREyeGaze leftEye;

        [SerializeField]
        OVREyeGaze rightEye;

        [SerializeField]
        private bool showRays = false;

        [SerializeField]
        private float maxGazeDistance = 1000f;

        [SerializeField]
        private Material eyeGazeLineMaterial;

        private LineRenderer leftRay, rightRay, centerRay;


        private Dictionary<OVREyeGaze, EyeGazeTarget> eyeCache = new Dictionary<OVREyeGaze, EyeGazeTarget>();

        void Start()
        {
            leftRay = CreateRay(Color.red);
            rightRay = CreateRay(Color.green);

            centerRay = CreateRay(Color.blue);
        }

        private LineRenderer CreateRay(Color color)
        {
            var go = new GameObject("EyeRay");
            go.transform.SetParent(transform);
            var ray = go.AddComponent<LineRenderer>();
            ray.startWidth = ray.endWidth = 0.01f;
            ray.material = eyeGazeLineMaterial;
            ray.startColor = ray.endColor = color;
            ray.enabled = showRays;
            return ray;
        }

        void Update()
        {
            RaycastHit hitLeft, hitRight;
            UpdateEye(RaycastEye(leftEye, out hitLeft, leftRay, maxGazeDistance), leftEye, hitLeft);
            UpdateEye(RaycastEye(rightEye, out hitRight, rightRay, maxGazeDistance), rightEye, hitRight);


            if (showRays)
            {
                Vector3[] allPosL = new Vector3[2];
                leftRay.GetPositions(allPosL);

                Vector3[] allPosR = new Vector3[2];
                rightRay.GetPositions(allPosR);

                centerRay.SetPositions(new Vector3[] {(leftEye.transform.position + rightEye.transform.position) / 2, (allPosL[1] + allPosR[1]) / 2 });
            }
            

        }


 

        private void UpdateEye(bool didHit, OVREyeGaze eyeGaze, RaycastHit hit)
        {
            if (didHit)
            {
                EyeGazeTarget target = hit.transform.gameObject.GetComponent<EyeGazeTarget>();
                if (target)
                {
                    if (!eyeCache.ContainsKey(eyeGaze))
                        eyeCache.Add(eyeGaze, target);
                    else
                        eyeCache[eyeGaze] = target;
                    return;
                }
            }
            eyeCache.Remove(eyeGaze);
        }

        /// <summary>
        /// Public helper function to get the current 
        /// left and right RayCastHit for the CURRENT frame.
        /// </summary>
        /// <param name="leftTarget"></param>
        /// <param name="rightTarget"></param>
        /// <returns>True if either left or right hit, false if neither hit</returns>
        public bool TryGetEyeGazeRaycast(out EyeGazeTarget leftTarget, out EyeGazeTarget rightTarget)
        {
            bool hasHit = false;
            leftTarget = rightTarget = default(EyeGazeTarget);
            //Get the current eye hit from cache
            if (eyeCache.ContainsKey(leftEye))
            {
                leftTarget = eyeCache[leftEye];
                hasHit = true;
            }
            if (eyeCache.ContainsKey(rightEye))
            {
                rightTarget = eyeCache[rightEye];
                hasHit = true;
            }
            return hasHit;
        }


        /// <summary>
        /// Performs a raycast from the particular eye
        /// </summary>
        /// <param name="gaze"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool RaycastEye(OVREyeGaze gaze, out RaycastHit hit, LineRenderer visualRay, float distance = 1000f)
        {
            if (showRays)
            {
                visualRay.SetPositions(new Vector3[]
                {
                    gaze.transform.position,
                    gaze.transform.forward * distance
                });
            }
            return Physics.Raycast(gaze.transform.position, gaze.transform.forward, out hit, distance);
        }


        /// <summary>
        /// Performs a raycast from the particular eye
        /// </summary>
        /// <param name="gaze"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool RaycastMidEye(OVREyeGaze gazeL, OVREyeGaze gazeR, out RaycastHit hit, LineRenderer visualRay, float distance = 1000f)
        {
            Vector3[] allPosL = new Vector3[2];
            Vector3[] allPosR = new Vector3[2];

            if (showRays)
            {
                leftRay.GetPositions(allPosL);
                rightRay.GetPositions(allPosR);

                centerRay.SetPositions(new Vector3[] { (allPosL[0] + allPosR[0]) / 2, (allPosL[1] + allPosR[1]) / 2 });
            }

            return Physics.Raycast((allPosL[0] + allPosR[0]) / 2, ((allPosL[1] + allPosR[1]) / 2).normalized, out hit, distance);
        }


    }
}