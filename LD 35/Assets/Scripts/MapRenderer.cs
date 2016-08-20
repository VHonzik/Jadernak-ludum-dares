using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI {

    [RequireComponent(typeof(SpriteRenderer))]
    public class MapRenderer : MonoBehaviour {

        public Texture2D empty_map;
        public Texture2D full_map;

        public GameObject ui_fparagraph_prefab;
        public GameObject ui_lparagraph_prefab;
        public GameObject ui_button_prefab;

        private GameObject panel;
        private GameObject fparagraph;
        private List<GameObject> choicebuttons = new List<GameObject>();

        public delegate void UIStateChangedHandler(UIState newState);
        public event UIStateChangedHandler UIStateChangedEvent;


        private static MapRenderer _instance;
        public static MapRenderer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MapRenderer>();
                }
                return _instance;
            }
        }
         
        // In screen pixels
        private float ui_squircle_r = 230f;
        private float ui_alpha = 0.0f;
        private float ui_alpha_speed = 1.0f;

        private int currentSize;
        private Vector2[] panelSizes;
        private int[] minParSizes;
        private int[] fontSizes;
        private float[] rSizes;


        private LinkedList<TextToDisplay> text_queue = new LinkedList<TextToDisplay>();
        private TextToDisplay current_text = null;

        UIState _ui_state;
        public UIState ui_state { get { return _ui_state;  } private set { _ui_state = value; OnUIStateChanged(); } }

        Vector4 last_camera_rect_in_uv_space = new Vector4();
        float squircle_size_in_uv_space;

        // Use this for initialization
        void Awake() {
            _instance = this;
            ui_state = UIState.Off;            

            var sprite_render = GetComponent<SpriteRenderer>() as SpriteRenderer;
            sprite_render.material.SetTexture("_BGTex", empty_map);
            sprite_render.material.SetTexture("_FullTex", full_map);


        }

        void Start()
        {
            panel = GameObject.FindGameObjectWithTag("uipanel");
            panelSizes = new Vector2[] { new Vector2(575, 345), new Vector2(875, 525), };
            fontSizes = new int[] { 20, 35 };
            rSizes = new float[] { 230f, 360f };
            minParSizes = new int[] { 120, 280 };
            currentSize = 0;

            //currentSize = 1;
            //ChangeeUIScale();

            fparagraph = GameObject.Instantiate(ui_fparagraph_prefab);
            fparagraph.transform.SetParent(panel.transform);
            ((Text)fparagraph.GetComponent<Text>()).text = "";
        }

        public Vector2 PixelPositionToWorldPosition(Vector2Int pixelPosition)
        {
            return new Vector2((pixelPosition.x - full_map.width / 2) / 100f, -(pixelPosition.y - full_map.height / 2) / 100f);
        }

        private void DisplayText()
        {
            MusicSoruce.Instance.PlayScribble();

            ((Text)fparagraph.GetComponent<Text>()).text = current_text.text;

            foreach (var choice in current_text.choices)
            {
                var choicebutton = GameObject.Instantiate(ui_button_prefab);
                choicebuttons.Add(choicebutton);
                choicebutton.GetComponentInChildren<Text>().text = choice.GetText();
                choicebutton.transform.SetParent(panel.transform);
                choicebutton.GetComponent<TextualButton>().resultEvent = choice;
            }

            if(!current_text.isStory)
            {
                var lparagraph = GameObject.Instantiate(ui_lparagraph_prefab);
                ((Text)lparagraph.GetComponent<Text>()).text = Events.ResourcesManager.Instance.CurrentState();
                lparagraph.transform.SetParent(panel.transform);
                choicebuttons.Add(lparagraph);
            }

        }

        private void UIChangedStateToOn()
        {
            if(current_text != null)
            {
                DisplayText();
            }
            
        }

        private void OnUIStateChanged()
        {
            if (UIStateChangedEvent != null)
                UIStateChangedEvent(ui_state);
        }

        private void UpdateUIAnim()
        {
            if( current_text != null && ui_state == UIState.Off)
            {
                ui_state = UIState.Revealing;
            }

            if( current_text == null && text_queue.Count == 0 && ui_state > UIState.Off)
            {
                ui_state = UIState.Hiding;
            }

            if(ui_state == UIState.Revealing)
            {
                ui_alpha += ui_alpha_speed * Time.deltaTime;

                if(ui_alpha >= 1f)
                {
                    ui_alpha = 1f;
                    ui_state = UIState.On;
                    UIChangedStateToOn();
                }
            }

            if (ui_state == UIState.Hiding)
            {
                ui_alpha -= ui_alpha_speed * Time.deltaTime;

                if (ui_alpha <= 0f)
                {
                    ui_alpha = 0f;
                    ui_state = UIState.Off;
                    UIChangedStateToOn();
                }
            }    
        }

        public void DeleteCurrentText()
        {
            ((Text)fparagraph.GetComponent<Text>()).text = "";
            current_text = null;
            foreach(var button in choicebuttons)
            {
                GameObject.Destroy(button);
            }
        }

        void ChangeeUIScale()
        {
            ui_squircle_r = rSizes[currentSize];
            panel.GetComponent<RectTransform>().sizeDelta = panelSizes[currentSize];
            ui_button_prefab.GetComponentInChildren<Text>().fontSize = fontSizes[currentSize];
            ui_button_prefab.GetComponent<LayoutElement>().minHeight = fontSizes[currentSize] + 10;
            ui_button_prefab.GetComponent<LayoutElement>().preferredHeight = fontSizes[currentSize] + 10;
            fparagraph.GetComponent<Text>().fontSize = fontSizes[currentSize];
            fparagraph.GetComponent<Text>().resizeTextForBestFit = false;
            fparagraph.GetComponent<LayoutElement>().minHeight = minParSizes[currentSize];
            fparagraph.GetComponent<LayoutElement>().preferredHeight = minParSizes[currentSize];
            ui_lparagraph_prefab.GetComponent<Text>().fontSize = fontSizes[currentSize];
            ui_lparagraph_prefab.GetComponent<LayoutElement>().minHeight = minParSizes[currentSize] / 2;
            ui_lparagraph_prefab.GetComponent<Text>().resizeTextForBestFit = false;
        }

        void UpdateUIScale()
        {
            if ((Screen.width < 1280 || Screen.height < 960) && currentSize != 0)
            {
                currentSize = 0;
                ChangeeUIScale();
            }
            else if ((Screen.width >= 1280 && Screen.height >= 960) && currentSize != 1)
            {
                currentSize = 1;
                ChangeeUIScale();
            }
        }


        void Update() {
            UpdateCameraAndUIProperties();
            UpdateUIAnim();
            UpdateUIScale();

            var sprite_render = GetComponent<SpriteRenderer>() as SpriteRenderer;
            sprite_render.material.SetVector("_CameraUVRect", last_camera_rect_in_uv_space);
            sprite_render.material.SetFloat("_UIAlpha", ui_alpha);
            sprite_render.material.SetFloat("_UISize", squircle_size_in_uv_space);


            if (text_queue.Count > 0 && current_text == null)
            {
                current_text = text_queue.First.Value;
                text_queue.RemoveFirst();
                if (current_text.task != null)
                {
                    current_text.task();
                    current_text = null;
                }
                else if (ui_state == UIState.On)
                {
                    DisplayText();
                }
            }

        }

        public void QueueText(TextToDisplay text)
        {
            text_queue.AddLast(text);
        }

        public void QueueResultText(TextToDisplay text)
        {
            text_queue.AddFirst(text);
        }

        private void UpdateCameraAndUIProperties()
        {
            var sprite_render = GetComponent<SpriteRenderer>() as SpriteRenderer;
            float one_world_unit_camera_space_pixels = Screen.height / (Camera.main.orthographicSize * 2f);
            Vector2 camera_center_offset_in_map_space = Camera.main.transform.position * sprite_render.sprite.pixelsPerUnit;
            Vector2 image_size = new Vector2(empty_map.width, empty_map.height);
            Vector2 camera_center_in_map_space = camera_center_offset_in_map_space + image_size / 2f;

            Vector2 camera_size_in_map_space = new Vector2(Screen.width, Screen.height) / one_world_unit_camera_space_pixels * sprite_render.sprite.pixelsPerUnit;

            var position = camera_center_in_map_space - (camera_size_in_map_space / 2f);
            var size = camera_size_in_map_space;

            //Convert to UV
            position.x /= image_size.x;
            position.y /= image_size.y;

            size.x /= image_size.x;
            size.y /= image_size.y;

            last_camera_rect_in_uv_space = new Vector4(position.x, position.y, size.x, size.y);

            squircle_size_in_uv_space = ((ui_squircle_r / Screen.height) * camera_size_in_map_space.y) / image_size.y;
        }
    }
}
