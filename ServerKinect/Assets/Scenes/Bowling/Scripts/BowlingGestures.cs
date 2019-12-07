using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingGestures : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BodySourceManager;
    public GameObject BodyPlayers;
    public GameObject BallPlayer;
    private Turning turnScript;
    private MoveForward moveScript;
    private Jump jumpScript;

    public Dictionary<ulong, ulong> Bodies = new Dictionary<ulong, ulong>();
    private BodySourceManager _BodyManager;
    private PlayerSource players;

    private string leanLeftGestureName = "lean_Right";
    private string leanRightGestureName = "lean_Left";
    private string ThrowProgressName = "throw2Progress";
    private string JumpGestureName = "jump";

    private Dictionary<ulong, GestureDetector> _Gestures = new Dictionary<ulong, GestureDetector>();
    void Start()
    {
        turnScript = BallPlayer.GetComponent<Turning>();
        moveScript = BallPlayer.GetComponent<MoveForward>();
        jumpScript = BallPlayer.GetComponent<Jump>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BodySourceManager == null)
            return;

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        players = BodyPlayers.GetComponent<PlayerSource>();

        if (_BodyManager == null)
            return;

        Windows.Kinect.Body[] data = _BodyManager.GetData();

        if (data == null)
            return;

        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
                continue;

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }    
        }

        List<ulong> knowsIds = new List<ulong>(Bodies.Keys);
        foreach(ulong trakingId in knowsIds)
        {
            if (!trackedIds.Contains(trakingId))
            {
                Bodies.Remove(trakingId);
                _Gestures.Remove(trakingId);
                players.RemovePlayer(trakingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
                continue;

            if (body.IsTracked)
            {
                if (!Bodies.ContainsKey(body.TrackingId))
                {
                    players.AddPlayer(body.TrackingId);
                    Bodies[body.TrackingId] = body.TrackingId;
                    _Gestures[body.TrackingId] = new GestureDetector(_BodyManager.GetSensor());
                    _Gestures[body.TrackingId].TrackingId = body.TrackingId;
                    _Gestures[body.TrackingId].IsPaused = (body.TrackingId == 0);
                    _Gestures[body.TrackingId].OnGestureDetected += CreateOnGestureHandler(body.TrackingId);
                }
            }
        }

    }

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(ulong bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, ulong bodyIndex)
    {
        var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;
        float progressGesture = e.progressGesture;
        Debug.Log("Progress: " + progressGesture);
        Debug.Log("Player1: " + players.getPlayer1());
        Debug.Log("Player2: " + players.getPlayer2());

        if (e.GestureID == leanLeftGestureName && bodyIndex == players.getPlayer2())
        {
            if (e.DetectionConfidence > 0.2f)
            {
                turnScript.turnLeft = true;
                turnScript.GetXAxis(e.DetectionConfidence);
            }
            else
            {
                turnScript.turnLeft = false;
            }
        }

        if (e.GestureID == leanRightGestureName && bodyIndex == players.getPlayer2())
        {
            if (e.DetectionConfidence > 0.2f)
            {
                turnScript.turnRight = true;
                turnScript.GetXAxis(e.DetectionConfidence);
            }
            else
            {
                turnScript.turnRight = false;
            }
        }

        if (e.GestureID == ThrowProgressName && bodyIndex == players.getPlayer1())
        {
            if (progressGesture > 0.75f)
            {
                moveScript.ShootBall();
            }
        }

        if (e.GestureID == JumpGestureName && bodyIndex == players.getPlayer1())
        {
            if (e.DetectionConfidence > 0.30f)
            {
                moveScript.Jump();
            }
        }
    }
}
