using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
namespace UI
{
    public class NodeRender : MonoBehaviour
    {
        bool is_enabled = false;
        bool is_visited = false;
        public Color currentColor = Color.blue;
        public Color reachableColor = Color.green;
        public Color notReachableColor = Color.grey;

        private float scaleHover = 1.2f;
        private float scaleNormal = 0.8f;

        private NodeGraph node;

        private bool hover = false;

        void OnMouseEnter()
        {
           if(is_enabled) hover = true;
        }

        void OnMouseExit()
        {
            if (is_enabled) hover = false;
        }

        void OnMouseDown()
        {
            if(is_enabled)
            {
                GameManager.Instance.NodeClicked(node);
            }
        }

        public void SetNode(NodeGraph n)
        {
            node = n;
        }


        void Update()
        {
            if (hover && transform.localScale.x < scaleHover)
            {
                transform.localScale = transform.localScale + Vector3.one * Time.deltaTime * (scaleHover - scaleNormal) * 4f;
                if (transform.localScale.x > scaleHover)
                {
                    transform.localScale = Vector3.one * scaleHover;
                }
            }
            else if (!hover && transform.localScale.x > scaleNormal)
            {
                transform.localScale = transform.localScale - Vector3.one * Time.deltaTime * (scaleHover - scaleNormal) * 4f;
                if (transform.localScale.x < scaleNormal)
                {
                    transform.localScale = Vector3.one * scaleNormal;
                }
            }

    }

        public void OnCurrentNodeChanged(NodeGraph currNode)
        {
            if (currNode == node)
            {
                is_visited = true;
                Color newColor = currentColor;
                newColor.a = GetComponent<SpriteRenderer>().color.a;
                GetComponent<SpriteRenderer>().color = newColor;
                is_enabled = false;
            }
            else if (currNode.neighbours.Contains(node))
            {
                Color newColor = reachableColor;
                newColor.a = GetComponent<SpriteRenderer>().color.a;
                GetComponent<SpriteRenderer>().color = newColor;
                is_enabled = true;
            }
            else
            {

                    Color newColor = notReachableColor;
                    newColor.a = GetComponent<SpriteRenderer>().color.a;
                    GetComponent<SpriteRenderer>().color = newColor;
                    is_enabled = false;
            }
        }

        public void OnUIStatechanged(UIState newState)
        {
            Color newColor = GetComponent<SpriteRenderer>().color;
            newColor.a = newState > UIState.Off ? 0f : (is_visited || is_enabled ? 1f : 0f);
            GetComponent<SpriteRenderer>().color = newColor;
        }



    }
}
