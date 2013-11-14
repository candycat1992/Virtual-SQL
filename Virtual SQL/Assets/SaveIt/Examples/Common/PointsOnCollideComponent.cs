using UnityEngine;
using System.Collections;

public class PointsOnCollideComponent : MonoBehaviour
{
    public PointsComponent PointsComponent;

    void Start()
    {
        if (PointsComponent == null)
            PointsComponent = this.GetComponent<PointsComponent>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PointsComponent == null)
            return;

        PointsComponent.AddPoints(1);
    }
}
