using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    private RectTransform InvetoryTransform;

    private RectTransform SelectorTransform;
    private Image SelectorImage;

    private CharacterMovement CharacterMovement;

    private List<Image> Slots;
    private List<RectTransform> SlotsTransforms;
    private List<GameObject> Items;

    private bool Visible;
    private int SelectedItem;
    private float SelectCooldown;

    private List<MonoBehaviour> BlockingInvetoryOpen;
    private bool InvetoryOpenBlocked;

    // Use this for initialization
    void Start () {
        InvetoryTransform = transform.GetComponent<RectTransform>();

        Slots = new List<Image>();
        SlotsTransforms = new List<RectTransform>();
        for (int i = 3; i < transform.childCount; i++)
        {
            Slots.Add(transform.GetChild(i).GetComponent<Image>());
            SlotsTransforms.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }

        foreach(var slotImage in Slots)
        {
            slotImage.enabled = false;
        }

        SelectorTransform = transform.GetChild(0).GetComponent<RectTransform>();
        SelectorImage = transform.GetChild(0).GetComponent<Image>();

        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();

        Items = new List<GameObject>();

        ClampAndUpdateSelectedItem();

        BlockingInvetoryOpen = new List<MonoBehaviour>();
        InvetoryOpenBlocked = false;

        Hide();
    }

    void Hide()
    {
        InvetoryTransform.anchoredPosition = Vector3.up * 45;
        Visible = false;
    }

    void Show()
    {
        Visible = true;
        InvetoryTransform.anchoredPosition = Vector3.up * -40;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Inventory") && !InvetoryOpenBlocked)
        {
            if (Visible)
            {
                Hide();
                CharacterMovement.UnblockInput(this);
            }
            else
            {
                Show();
                CharacterMovement.BlockInput(this);
            }
        }

        if (Visible)
        {
            SelectCooldown = Mathf.Max(SelectCooldown-Time.deltaTime,0.0f);
            if (!Mathf.Approximately(Input.GetAxis("Horizontal"), 0.0f) && Items.Count >= 2 && SelectCooldown <= 0.0f)
            {
                SelectCooldown = 0.25f;
                SelectedItem += Mathf.FloorToInt(Mathf.Sign(Input.GetAxis("Horizontal")));
                ClampAndUpdateSelectedItem();
            }

            if (Input.GetButtonDown("InspectItem") && Items.Count > 0)
            {
                StartCoroutine(UseItem(Items[SelectedItem]));

                Hide();
                CharacterMovement.UnblockInput(this);
            }
        }
    }

    public void AddItem(GameObject item)
    {

        Items.Add(item);
        var index = Items.Count - 1;

        ClampAndUpdateSelectedItem();

        item.transform.SetParent(CharacterMovement.transform);
        item.transform.position = Vector3.zero;

        if (item.GetComponent<BoxCollider2D>()) item.GetComponent<BoxCollider2D>().enabled = false;
        var itemSpriteRender = Items[index].GetComponent<SpriteRenderer>();
        Slots[index].sprite = itemSpriteRender.sprite;
        itemSpriteRender.enabled = false;
        Slots[index].enabled = true;
    }

    public IEnumerator UseItem(GameObject item)
    {
        var invetoryItem = item.GetComponent<InventoryItem>();

        if (invetoryItem)
        {
            var remove = invetoryItem.Use();

            var itemScript = item.GetComponent<DirectionalItemScript>();

            if (itemScript)
            {
                yield return StartCoroutine(itemScript.Use());
            }

            if (remove)
            {
                RemoveItemWithIndex(SelectedItem);
            }
        }
    }

    private void ClampAndUpdateSelectedItem()
    {
        if(Items.Count == 0)
        {
            SelectorImage.enabled = false;
            SelectedItem = 0;

            return;
        }

        if (Items.Count == 1)
        {
            SelectorImage.enabled = true;
            SelectedItem = 0;
        }

        if (SelectedItem >= Items.Count)
        {
            SelectedItem -= Items.Count;
        }
        else if (SelectedItem < 0)
        {
            SelectedItem += Items.Count;
        }
        SelectorTransform.anchoredPosition = new Vector3(SlotsTransforms[SelectedItem].anchoredPosition.x - 4, 0, 0);
    }

    public void BlockOpening(MonoBehaviour who)
    {
        if (!BlockingInvetoryOpen.Contains(who))
        {
            BlockingInvetoryOpen.Add(who);
            if (!InvetoryOpenBlocked)
            {
                InvetoryOpenBlocked = true;
            }
        }
    }

    public void UnblockOpening(MonoBehaviour who)
    {
        if (BlockingInvetoryOpen.Contains(who))
        {
            BlockingInvetoryOpen.Remove(who);
            if (InvetoryOpenBlocked && BlockingInvetoryOpen.Count == 0)
            {
                InvetoryOpenBlocked = false;
            }
        }
    }

    public bool HasItemWithName(string name)
    {
        return Items.Find(go => go.name == name);
    }

    public void RemoveItemWithName(string name)
    {
        var index = Items.FindIndex(go => go.name == name);
        if (index >= 0)
        {
            RemoveItemWithIndex(index);
        }
    }

    private void RemoveItemWithIndex(int index)
    {
        var item = Items[index];
        Items.RemoveAt(index);

        for (var i = index; i < Slots.Count - 1; i++)
        {
            Slots[i].sprite = Slots[i + 1].sprite;
            Slots[i].enabled = Slots[i + 1].enabled;
        }

        ClampAndUpdateSelectedItem();

        GameObject.Destroy(item);
    }
}
