using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaface.Utilities;

namespace Metaface.Debug
{
    public class MidEyeGazeDebugger : MonoBehaviour
    {
        [SerializeField]
        private MidEyeGazeHelper eyeGazeHelper;

        private void Update()
        {
            EyeGazeTarget mid;
            if (eyeGazeHelper.TryGetEyeGazeRaycast(out mid))
            {
                //Hitting the same target
                if (mid)
                {
                    var leftIndicator = mid.GetComponent<EyeGazeIndicator>();
                    leftIndicator.SetColor(Color.green);
                }

            }
        }
    }
}