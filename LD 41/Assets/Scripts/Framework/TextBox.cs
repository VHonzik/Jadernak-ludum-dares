using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

    public Sprite ContinueSymbol;
    public Sprite EndSymbol;

    private Image Symbol;
    private Text Text;
    private RectTransform Panel;
    private CharacterMovement CharacterMovement;

    private Queue<string> Messages;

    private bool Visible;
    private bool JustChangedState;

    private float InputDelayTimer;

    // Use this for initialization
    void Start () {
        Text = transform.GetChild(0).GetComponent<Text>();
        Symbol = transform.GetChild(2).GetComponent<Image>();
        Panel = transform.GetComponent<RectTransform>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Messages = new Queue<string>();

        JustChangedState = false;
        Hide();
    }

	// Update is called once per frame
	void Update () {
        InputDelayTimer = Mathf.Max(InputDelayTimer - Time.deltaTime, 0);
        if (Input.GetButtonDown("InspectItem") && Visible && !JustChangedState && InputDelayTimer <= 0.0f)
        {
            if (Messages.Count > 0)
            {
                Text.text = Messages.Dequeue();
                Symbol.sprite = Messages.Count > 0 ? ContinueSymbol : EndSymbol;
            }
            else
            {
                Hide();
                CharacterMovement.UnblockInput(this);
            }
        }

        JustChangedState = false;
    }

    void Hide()
    {
        Panel.position = Vector3.up * -42;
        Visible = false;
        JustChangedState = true;
    }

    void Show()
    {
        Visible = true;
        Panel.position = Vector3.up * 40;
        JustChangedState = true;
    }

    public void AddMessage(string msg)
    {
        Messages.Enqueue(msg);
        if (Visible == false)
        {
            CharacterMovement.BlockInput(this);
            Text.text = Messages.Dequeue();
            Show();
            InputDelayTimer = 0.5f;
        }
        Symbol.sprite = Messages.Count > 0 ? ContinueSymbol : EndSymbol;
    }

    public IEnumerator Wait()
    {
        while(Visible)
        {
            yield return null;
        }

        yield return null;
    }
}
