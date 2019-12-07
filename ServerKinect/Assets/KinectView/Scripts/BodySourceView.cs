using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityEngine.UI;
using System;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject BodyPlayers;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    private PlayerSource players;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    // Kinect Gesture

    public Text GestureTextLeftGameObject;
    public Text ConfidenceTextLeftGameObject;

    public Text GestureTextRightGameObject;
    public Text ConfidenceTextRightGameObject;

    public Text GestureProgressThrowGameObject;
    public Text GestureThrowGameObject;

    public Text GestureTextJumpGameObject;
    public Text ConfidenceTextJumpGameObject;

    private string leanLeftGestureName = "lean_Right";
    private string leanRightGestureName = "lean_Left";
    private string ThrowProgressName = "throw2Progress";
    private string JumpGestureName = "jump";

    private Dictionary<ulong, GestureDetector> _Gestures = new Dictionary<ulong, GestureDetector>();
    private List<GestureDetector> gestureDetectorList = null;

    // End Kinect Gesture

    private void Start()
    {
        gestureDetectorList = new List<GestureDetector>();
    }

    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        players = BodyPlayers.GetComponent<PlayerSource>();

        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                GestureTextLeftGameObject.text = "None";
                GestureTextRightGameObject.text = "None";
                GestureThrowGameObject.text = "None";

                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                _Gestures.Remove(trackingId);
                players.RemovePlayer(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    GestureTextLeftGameObject.text = "None";
                    GestureTextRightGameObject.text = "None";
                    GestureThrowGameObject.text = "None";
                    players.AddPlayer(body.TrackingId);
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    _Gestures[body.TrackingId] = new GestureDetector(_BodyManager.GetSensor());
                    _Gestures[body.TrackingId].TrackingId = body.TrackingId;
                    _Gestures[body.TrackingId].IsPaused = (body.TrackingId == 0);
                    _Gestures[body.TrackingId].OnGestureDetected += CreateOnGestureHandler(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        /////// test Canvas
        GameObject gameObject = new GameObject("Canvas");
        gameObject.transform.SetParent(body.transform);
        gameObject.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        gameObject.AddComponent<CanvasScaler>();
       
        GameObject text = new GameObject("Text");
        text.AddComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 1.0f);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, 60.0f);
        text.transform.SetParent(gameObject.transform);
        text.AddComponent<Text>().text = "Player " + players.GetPlayer(id);
        text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        //Debug.Log("player1 " + players.getPlayer1());
        //Debug.Log("player2 " + players.getPlayer2());

        //gameObject.transform.parent = body.transform;

        //////  End Test

        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        /////// test Canvas
        
        //////  End Test

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            if (jt.ToString().Equals("Head"))
            {
                Transform canvas = bodyObject.transform.Find("Canvas").Find("Text");
                //tranform text = canvas.transform.Find("Text")
                canvas.localPosition = GetVector3FromJoint(sourceJoint);
            }
            
            /*LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }*/
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }


    //Gesture Kinect Functions
    private EventHandler<GestureEventArgs> CreateOnGestureHandler(ulong bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, ulong bodyIndex)
    {
        var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;
        float progressGesture = e.progressGesture;
        
        if (e.GestureID == leanLeftGestureName && bodyIndex == players.getPlayer2())
        {
            GestureTextLeftGameObject.text = "Gesture Left Detected: " + isDetected;
            ConfidenceTextLeftGameObject.text = "Confidence Left: " + e.DetectionConfidence;
        }

        if (e.GestureID == leanRightGestureName && bodyIndex == players.getPlayer2())
        {
            GestureTextRightGameObject.text = "Gesture Right Detected: " + isDetected; 
            ConfidenceTextRightGameObject.text = "Confidence Right: " + e.DetectionConfidence;
        }

        if (e.GestureID == ThrowProgressName && bodyIndex == players.getPlayer1())
        {
            GestureProgressThrowGameObject.text = "ThrowProgress: " + progressGesture;

            if (progressGesture > 0.75f)
            {
                GestureThrowGameObject.text = "Gesture Throw Detected: True";
            }
            else
            {
                GestureThrowGameObject.text = "Gesture Throw Detected: False";
            }
        }

        if (e.GestureID == JumpGestureName && bodyIndex == players.getPlayer1())
        {
            GestureTextJumpGameObject.text = "Gesture Jump Detected: " + isDetected;
            ConfidenceTextJumpGameObject.text = "Confidence Jump: " + e.DetectionConfidence;
        }

    }

}
