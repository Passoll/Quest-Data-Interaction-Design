using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class Creatline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Add a Spline Computer component to this object
        SplineComputer spline = gameObject.AddComponent<SplineComputer>();
        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[5];
        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * i;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }
        //Write the points to the spline
        spline.SetPoints(points);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
