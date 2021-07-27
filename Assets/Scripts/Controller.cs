using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    
    public GameObject Portal;
    public GameObject ARCamera;
    public Camera FirstPersonCamera;
    public GameObject PlaneGenerator;

    private DetectedPlane detectedPlane;

    public Button BackButton;
    public Button UpButton;
    public Button DownButton;
    public Button CancelPortal;
    
    //Button listener boolean
    private bool isUpClicked;
    private bool isDownClicked;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {

        isUpClicked = false;
        isDownClicked = false;
        
        BackButton.onClick.AddListener(() => { Application.Quit(); });
        UpButton.onClick.AddListener(() => { isUpClicked = true; });
        DownButton.onClick.AddListener(() => { isDownClicked = true; });
        CancelPortal.onClick.AddListener(() => {CancelPortalMethod(); });
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Check ARCore Seesion status is tracking anything
        if (Session.Status != SessionStatus.Tracking)
        {
           // int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
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
        
        //Implement thhe up and down movement button
        if (isUpClicked)
        {
            UpMove();
        }else if (isDownClicked)
        {
            DownMove();
        }

    }

    private void ProcessTouch()
    {
        Touch touch;
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        
        /*// Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }*/
        
        bool foundHit = false;
        
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
        raycastFilter |= TrackableHitFlags.PlaneWithinBounds;
        
        foundHit = Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit);
        
       

        if (foundHit)
        {
            // Use hit pose and camera pose to check if hit test is from the
            // back of the plane, if it is, there is no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                ShowAndroidToastMessage("Hit at back of the current DetectedPlane");
                DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                CreateAnchor(detectedPlane);
            }
            else
            {
                
                ShowAndroidToastMessage("Hit NORMAL current DetectedPlane");
                
                if (hit.Trackable is DetectedPlane)
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    if (detectedPlane.PlaneType != DetectedPlaneType.Vertical)
                    {
                        CreateAnchor(detectedPlane);
                        ShowAndroidToastMessage("It is Horizontal");
                    }
                    else
                    {
                        CreateAnchor(detectedPlane);
                    }
                }
                else
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    if (detectedPlane.PlaneType != DetectedPlaneType.Vertical)
                    {
                        CreateAnchor(detectedPlane);
                    }
                    else
                    {
                        CreateAnchor(detectedPlane);
                    }
                }
            }
            
            
            
            
            
        }

    }
    
    
    void CreateAnchor(DetectedPlane plane)
    {
        //Activate Portal
        Portal.SetActive(true);

        Anchor anchor = plane.CreateAnchor(plane.CenterPose);

        Portal.transform.position = plane.CenterPose.position;
        Portal.transform.rotation = plane.CenterPose.rotation;
            
        // We want the portal to face the camera
        Vector3 cameraPosition = ARCamera.transform.position;
            
        // The Portal should only rotate around the Y axis
        cameraPosition.y = plane.CenterPose.position.y; /*hit.Pose.position.y;*/
            
        //Rotate the postal to face the camera
        Portal.transform.LookAt(cameraPosition, Portal.transform.up);

        // ARCore will keep understanding the world and update the anchors accordingly hence we need to attach our portal to the anchor
        Portal.transform.SetParent(anchor.transform);
        
       
        
        //Deactivate Plane Generator
        PlaneGenerator.SetActive(false);
        
        
        
    }
    

    void UpMove()
    {
        ARCamera.transform.position = Vector3.Lerp(ARCamera.transform.position, Vector3.forward * 5, 0.5f * Time.deltaTime);
        isUpClicked = false;
    }

    void DownMove()
    {
        ARCamera.transform.position =
            Vector3.Lerp(ARCamera.transform.position, Vector3.back * 5, 0.5f * Time.deltaTime);
        isDownClicked = false;
    }

    void CancelPortalMethod()
    {
        Portal.SetActive(false);
        PlaneGenerator.SetActive(true);
    }
    
    private void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
