using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is for DIS/ENABLING the mirror if user is LOOKING at their partner / not
public class GazeColliderManager : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Puppet")
        {
            //GameObject.Find("EmpathicMirror").GetComponent<EmpathicMirrorBehaviour>().enableMirrorUpdate(); //disableMirrorUpdate(); //for debug use
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Puppet")
        {
           // GameObject.Find("EmpathicMirror").GetComponent<EmpathicMirrorBehaviour>().enableMirrorUpdate();
        }
    }

}
