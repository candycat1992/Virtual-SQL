using UnityEngine;
using System.Collections;

public class ControllableSphereComponent : MonoBehaviour
{
    public float Force;
	void Update()
    {
        if (Input.GetKey("left"))
        {
            this.rigidbody.AddForce(-Time.deltaTime * Force, 0, 0, ForceMode.Acceleration);
        }
        if (Input.GetKey("right"))
        {
            this.rigidbody.AddForce(Time.deltaTime * Force, 0, 0, ForceMode.Acceleration);
        }
        if (Input.GetKey("up"))
        {
            this.rigidbody.AddForce(0, 0, Time.deltaTime * Force, ForceMode.Acceleration);
        }
        if (Input.GetKey("down"))
        {
            this.rigidbody.AddForce(0, 0, -Time.deltaTime * Force, ForceMode.Acceleration);
        }
	}
}
