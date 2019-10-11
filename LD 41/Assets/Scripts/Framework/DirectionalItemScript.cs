using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InventoryItem))]
public class DirectionalItemScript : MonoBehaviour
{
    private CharacterMovement CharacterMovement;

    public GameObject SelectionArrowAsset;
    private GameObject[] SelectionArrows;

    private Inventory Inventory;

    private bool Selecting;
    private bool Done;

    // Use this for initialization
    void Start()
    {
        CharacterMovement =  GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();
        Selecting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Selecting)
        {
            bool Selected = false;
            Vector3 direction = Vector3.zero;

            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");

            if (Mathf.Approximately(Mathf.Abs(vertical), 1.0f))
            {
                Selected = true;
                direction = Mathf.Sign(Input.GetAxis("Vertical")) * Vector3.up;

            }
            else if (Mathf.Approximately(Mathf.Abs(horizontal), 1.0f))
            {
                Selected = true;
                direction = Mathf.Sign(Input.GetAxis("Horizontal")) * Vector3.right;
            }

            if (Selected)
            {
                Selecting = false;
                CharacterMovement.UnblockInput(this);
                Inventory.UnblockOpening(this);

                for (var i = 0; i < 4; i++)
                {
                    Destroy(SelectionArrows[i]);
                }

                StartCoroutine(Apply(direction));
            }
        }

    }

    private IEnumerator Apply(Vector3 direction)
    {
        if (GetComponent<TeleportItemScript>())
        {
            yield return StartCoroutine(GetComponent<TeleportItemScript>().DirectionSelected(direction));
        }

        Done = true;
    }

    public IEnumerator Use()
    {
        CharacterMovement.BlockInput(this);
        Inventory.BlockOpening(this);

        SelectionArrows = new GameObject[4];
        for (var i = 0; i < 2; i++)
        {
            SelectionArrows[i] = GameObject.Instantiate(SelectionArrowAsset);
            SelectionArrows[i].transform.SetParent(CharacterMovement.transform);
            Vector3 left = Vector3.left + Vector3.right * i * 2;
            SelectionArrows[i].transform.position = CharacterMovement.transform.position + left;
            SelectionArrows[i].transform.Rotate(new Vector3(0, 0, 90 + i * 180));
        }

        for (var j = 0; j < 2; j++)
        {
            SelectionArrows[2 + j] = GameObject.Instantiate(SelectionArrowAsset);
            SelectionArrows[2 + j].transform.SetParent(CharacterMovement.transform);
            Vector3 down = Vector3.down + Vector3.up * j * 2;
            SelectionArrows[2 + j].transform.position = CharacterMovement.transform.position + down;
            SelectionArrows[2 + j].transform.Rotate(new Vector3(0, 0, 180 - 180 * j));
        }

        Done = false;
        Selecting = true;

        while(!Done)
        {
            yield return null;
        }
    }
}
