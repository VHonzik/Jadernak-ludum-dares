using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

    private NodeGraph current_node;
    static System.Random rnd = new System.Random();

    private List<Events.RewardEvent> brothers;

    private float panning_factor;
    private Vector3 pan_camera_to;
    private Vector3 start_camera_pan;
    private NodeGraph wanted_node;

    public delegate void CurrentNodeChangedHandler(NodeGraph newNode);
    public event CurrentNodeChangedHandler CurrentNodeChangedEvent;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
	// Use this for initialization
	void Awake () {
        _instance = this;
        panning_factor = -1f;
        GenerateBrothers();
    }

    private void GenerateBrothers()
    {
        brothers = new List<Events.RewardEvent>();
        for (int i = 0; i < 8; i++)
        {
            string name = Events.StringTable.GetRandomText("PARTY_MEMBERS");
            
            int diplomacy = rnd.Next(0, 4);
            int strength = rnd.Next(0, 4);
            int tracking = rnd.Next(0, 4);

            string description = String.Format("Brother {0} ({1} diplomacy, {2} strength ,{3} tracking)", 
                name, diplomacy.ToString("+#;-#"), strength.ToString("+#;-#"), tracking.ToString("+#;-#"));

            brothers.Add(new Events.RewardEvent( new Events.RandomReward[] {
                new Events.RandomReward(Events.ResourceType.Diplomacy, new Events.RandomValue[] { new Events.RandomValue(1, diplomacy) }, "You have picked"+ name+".", ""),
                new Events.RandomReward(Events.ResourceType.Strenght, new Events.RandomValue[] { new Events.RandomValue(1, strength) }, "", ""),
                new Events.RandomReward(Events.ResourceType.Tracking, new Events.RandomValue[] { new Events.RandomValue(1, tracking) }, "", "")
               }, description, new Events.EventCost()));
        }
    }

    void Start()
    {
        Events.EventManager.Instance.StoryText(
    "\"Brother, a raven from Farreach just arrived. I am afraid you must depart immediately.\"",
    "Ask about your mission.");

        Events.EventManager.Instance.StoryText(
@"""Your are to find and hunt down a group of heretics. They were discovered north-west of Farreach. Here is the letter.""",
    "Glance over the letter.");

        Events.EventManager.Instance.StoryText(
@"The letter describes the heretics as highly dangerous savages rumored to posses an ability to shapeshift into animals. Their current whereabouts are unknown.",
    "Embark on journey.");

        UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(() => OnCurrentNodeChanged()));


        current_node = MapGraph.Instance.GetStartNode();

        Vector3 wantedPos = UI.MapRenderer.Instance.PixelPositionToWorldPosition(current_node.PixelPosition);
        Camera.main.transform.position = new Vector3(wantedPos.x, wantedPos.y, Camera.main.transform.position.z);
    }

    void Update()
    {
        if(panning_factor >= 0f)
        {
            panning_factor += Time.deltaTime;

            Vector3 wantedPos = Vector3.Lerp(start_camera_pan, pan_camera_to, panning_factor);
            Camera.main.transform.position = new Vector3(wantedPos.x, wantedPos.y, Camera.main.transform.position.z);
        }

        if(panning_factor >= 1f)
        {
            current_node = wanted_node;            
            Events.EventManager.Instance.EnteredNode(current_node);
            OnCurrentNodeChanged();

            Vector3 wantedPos = UI.MapRenderer.Instance.PixelPositionToWorldPosition(current_node.PixelPosition);
            Camera.main.transform.position = new Vector3(wantedPos.x, wantedPos.y, Camera.main.transform.position.z);

            panning_factor = -1f;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
	
    public void NodeClicked(NodeGraph node)
    {
        panning_factor = 0f;
        pan_camera_to = UI.MapRenderer.Instance.PixelPositionToWorldPosition(node.PixelPosition);
        start_camera_pan = UI.MapRenderer.Instance.PixelPositionToWorldPosition(current_node.PixelPosition);
        wanted_node = node;
    }

    public NodeGraph GetCurrentNode() { return current_node; }

    private void OnCurrentNodeChanged()
    {
        if (CurrentNodeChangedEvent != null)
            CurrentNodeChangedEvent(current_node);
    }


}
