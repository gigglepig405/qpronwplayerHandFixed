using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metaface.Utilities
{
    public class MidEyeGazeHelper : MonoBehaviour
    {

        [SerializeField]
        OVREyeGaze leftEye;

        [SerializeField]
        OVREyeGaze rightEye;

        [SerializeField]
        GameObject midRayOB;

        [SerializeField]
        private bool showRays = false;

        [SerializeField]
        private float maxGazeDistance = 1000f;

        private LineRenderer midRay;

        private List<EyeGazeTarget> eyeCache = new List<EyeGazeTarget>();

        public string focusOBJ;

        void Start()
        {
            midRay = midRayOB.GetComponent<LineRenderer>();
        }


        void Update()
        {
            RaycastHit hitMid;
            UpdateMidEye(RaycastMidEye(leftEye, rightEye, out hitMid, midRay, maxGazeDistance), hitMid);
        }


        private void UpdateMidEye(bool didHit, RaycastHit hit)
        {
            if (didHit)
            {
                transform.GetChild(1).transform.position = hit.point;
                //EyeGazeTarget target = hit.transform.gameObject.GetComponent<EyeGazeTarget>();
                focusOBJ = hit.collider.tag;
                //print("Current gazing: " + hit.collider.name + " of tag: " + hit.collider.tag);
                    //print("Target hit: " + hit.collider.gameObject.name);

                    //if (!eyeCache.Contains(target))
                    //    eyeCache.Add(target);
                    //return;
           
            }
            //eyeCache.Clear();
        }


        /// <summary>
        /// Public helper function to get the current 
        /// left and right RayCastHit for the CURRENT frame.
        /// </summary>
        /// <param name="leftTarget"></param>
        /// <param name="rightTarget"></param>
        /// <returns>True if either left or right hit, false if neither hit</returns>
        public bool TryGetEyeGazeRaycast(out EyeGazeTarget midTarget)
        {
            bool hasHit = false;
            midTarget = default(EyeGazeTarget);
            //Get the current eye hit from cache
            if (eyeCache.Count > 0)
            {
                midTarget = eyeCache[0];
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
        private bool RaycastMidEye(OVREyeGaze gazeL, OVREyeGaze gazeR, out RaycastHit hit, LineRenderer visualRay, float distance = 1000f)
        {

            if (showRays)
            {
                midRay.SetPositions(new Vector3[] { (gazeL.transform.position + gazeR.transform.position) / 2, 
                                                       (gazeL.transform.forward * distance + gazeR.transform.forward * distance) / 2 });
            }

            return Physics.Raycast((gazeL.transform.position + gazeR.transform.position) / 2, 
                ((gazeL.transform.forward * distance + gazeR.transform.forward * distance) / 2).normalized, out hit, distance);
        }


    }
}