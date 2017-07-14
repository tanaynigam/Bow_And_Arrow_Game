using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    public class AdjustToSpace : MonoBehaviour {

	    // Use this for initialization
	    void Start () {
            Vector3 direction = (PlayerManager.Instance.hmdRig.transform.position - transform.position).normalized;
            transform.position += new Vector3(
                direction.x * (PlayerManager.BASE_MES - PlayerManager.Instance.PlaySpaceWidth),
                0f,
                direction.z * (PlayerManager.BASE_MES - PlayerManager.Instance.PlaySpaceLength)
            );
	    }
    }
}
