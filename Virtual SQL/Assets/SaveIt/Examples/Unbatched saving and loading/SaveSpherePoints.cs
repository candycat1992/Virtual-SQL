using UnityEngine;
using System.Collections;
using SaveIt;

public class SaveSpherePoints : MonoBehaviour
{
    public string FileName = "spherepoints.dat";
    public string Category = "Category1";

    private PointsComponent PointsComponent;
    private Color baseColor = Color.red;

    void Start()
    {
        baseColor = this.renderer.material.color;
        PointsComponent = this.GetComponent<PointsComponent>();
    }

    void OnGUI()
    {
        var coordinates = Camera.main.WorldToScreenPoint(this.transform.position);
        coordinates.y = Screen.height - coordinates.y;
        GUI.Label(new Rect(coordinates.x - 50, coordinates.y, 100, 20), PointsComponent.Points.ToString() + " Points");
    }

    void OnMouseEnter()
    {
        this.renderer.material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        this.renderer.material.color = baseColor;
    }

    void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            this.renderer.material.color = Color.yellow;
        else
            this.renderer.material.color = Color.cyan;
    }

    void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            var loadContext = LoadContext.FromUnbufferedFile(FileName);
            if (loadContext.Exists(Category))
                PointsComponent.Points = loadContext.Load<int>(Category);
            else
                PointsComponent.Points = 0;
        }
        else
        {
            var saveContext = SaveContext.ToUnbufferedFile(FileName);
            saveContext.Save(PointsComponent.Points, Category);

            /*
            This is not needed when using unbuffered files:
            
            saveContext.Flush();
            */
        }
    }
}
