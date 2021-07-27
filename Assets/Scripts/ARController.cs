using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;
using GoogleARCore.Examples.Common;

public class ARController : MonoBehaviour
{
    
    // List of tracked planes
    //We will fill this list with the planes that ARCore detected in the current frame
   // private List<DetectedPlane> m_NewTrackedPlanes = new List<DetectedPlane>();
    public GameObject GridPrefab;
    public GameObject Portal;
    public GameObject ARCamera;

    private DetectedPlane detectedPlane;


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        // Check ARCore Seesion status is tracking anything
        if (Session.Status != SessionStatus.Tracking)
        {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        ProcessTouch();
        
        // Check for the plane being subsumed.
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (detectedPlane.SubsumedBy != null) 
        { 
            detectedPlane = detectedPlane.SubsumedBy; 
        }
                
        
        /*// The following funtion will fill m_NewTrackedPlane with the planes that ARCore detected in the current frame
        
        Session.GetTrackables(m_NewTrackedPlanes,TrackableQueryFilter.Updated);
        
        
        // Instantiate a Grid for each DetectedPlane in m_NewTrackedPlanes.
        for (int i = 0; i < m_NewTrackedPlanes.Count; i++)
        {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);
            
            // This function will set the position of grid and modify the vertices of the attached mesh
            grid.GetComponent<GridVisualiser>().Initialize(m_NewTrackedPlanes[i]);
        }
        */

        

    }

    void ProcessTouch()
    {
        /*// check if the user touches the screen
        Touch touch;

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        */
        
        Touch touch;
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        //Let's now check if the user touched any of the tracked planes
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
        
        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Let's now place the portal on top of the tracked plane thatwe touched
            
            // Enable the portal
            Portal.SetActive(true);
            
            // Create a new Anchor
            
            
            detectedPlane = hit.Trackable as DetectedPlane;
            CreateAnchor(detectedPlane);





        }
    }

    void CreateAnchor(DetectedPlane plane)
    {
        
        /*Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);*/

        Anchor anchor = plane.CreateAnchor(plane.CenterPose);

        // Set the position of the portal to be the same as the hit position
        /*Portal.transform.position = hit.Pose.position;
        Portal.transform.rotation = hit.Pose.rotation;*/

        Portal.transform.position = plane.CenterPose.position;
        Portal.transform.rotation = plane.CenterPose.rotation;
            
        // We want the portal to face the camera
        Vector3 cameraPosition = ARCamera.transform.position;
            
        // The Portal should only rotate around the Y axis
        cameraPosition.y = plane.CenterPose.position.y; /*hit.Pose.position.y;*/
            
        //Rotate the postal to face the camera
        Portal.transform.LookAt(cameraPosition, Portal.transform.up);

        // ARCore will keep understanding the world and update the anchors accordingly hence we need to attach our portal to the anchor
        /*Portal.transform.parent = anchor.transform;*/
        Portal.transform.SetParent(anchor.transform);
        
    }

   


}
