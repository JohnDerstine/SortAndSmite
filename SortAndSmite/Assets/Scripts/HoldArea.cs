using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HoldArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    private UIDocument document;

    private VisualElement container;
    private VisualElement slot1;
    private VisualElement slot2;
    private VisualElement slot3;
    private VisualElement hoverItem;
    private Dictionary<VisualElement, GameObject> slots = new Dictionary<VisualElement, GameObject>();

    private EventCallback<MouseUpEvent> dropItemCallback;
    private EventCallback<MouseDownEvent> getItemCallback;
    private EventCallback<MouseEnterEvent> hideItemCallback;
    private EventCallback<MouseLeaveEvent> showItemCallback;

    // Start is called before the first frame update
    void Start()
    {
        document = gameObject.GetComponent<UIDocument>();
        container = document.rootVisualElement.Q<VisualElement>("HoldContainer");
        slot1 = container.Q<VisualElement>("Slot1");
        slot2 = container.Q<VisualElement>("Slot2");
        slot3 = container.Q<VisualElement>("Slot3");

        slots.Add(slot1, null);
        slots.Add(slot2, null);
        slots.Add(slot3, null);

        dropItemCallback = new EventCallback<MouseUpEvent>(DropItem);
        getItemCallback = new EventCallback<MouseDownEvent>(GetItem);
        hideItemCallback = new EventCallback<MouseEnterEvent>(HideItem);
        showItemCallback = new EventCallback<MouseLeaveEvent>(ShowItem);

        slot1.RegisterCallback(dropItemCallback);
        slot2.RegisterCallback(dropItemCallback);
        slot3.RegisterCallback(dropItemCallback);

        container.RegisterCallback(hideItemCallback);
        container.RegisterCallback(showItemCallback);
    }

    private void DropItem(MouseUpEvent e)
    {
        Debug.Log("dropped");
        VisualElement slot = e.currentTarget as VisualElement;
        if (player.HeldItem != null && slots[slot] == null)
        {
            slot.style.backgroundImage = new StyleBackground(player.HeldItem.gameObject.GetComponent<SpriteRenderer>().sprite);
            slots[slot] = player.HeldItem.gameObject;
            player.HeldItem.gameObject.SetActive(false);
            player.HeldItem = null;
            slot.UnregisterCallback<MouseUpEvent>(DropItem);
            slot.RegisterCallback(getItemCallback);
            ShowItem(null);
        }
    }

    private void GetItem(MouseDownEvent e)
    {
        VisualElement slot = e.currentTarget as VisualElement;
        if (player.HeldItem == null && slots[slot] != null)
        {
            slot.style.backgroundImage = null;
            slots[slot].SetActive(true);
            player.HeldItem = slots[slot].GetComponent<SortableItem>();
            player.HeldItem.retrieved = true;
            player.HeldItem.Released.AddListener(player.HeldItem.RetrieveRelease);
            player.HeldItem.GetComponent<Rigidbody2D>().gravityScale = 0;
            slots[slot] = null;
            slot.UnregisterCallback<MouseDownEvent>(GetItem);
            slot.RegisterCallback(dropItemCallback);
            HideItemHelper(e.localMousePosition);
        }
    }

    private void HideItem(MouseEnterEvent e)
    {
        HideItemHelper(e.localMousePosition);
    }

    private void HideItemHelper(Vector2 pos)
    {
        if (player.HeldItem != null)
        {
            SpriteRenderer sr = player.HeldItem.GetComponent<SpriteRenderer>();
            hoverItem = new VisualElement();
            hoverItem.style.backgroundImage = new StyleBackground(sr.sprite);
            hoverItem.style.position = Position.Absolute;
            hoverItem.pickingMode = PickingMode.Ignore;
            hoverItem.style.width = sr.bounds.size.x * 100;
            hoverItem.style.height = sr.bounds.size.y * 100;
            hoverItem.style.left = container.WorldToLocal(Input.mousePosition).x - hoverItem.resolvedStyle.width / 2f;
            hoverItem.style.top = ( Screen.height / 2 ) - container.WorldToLocal(Input.mousePosition).y + hoverItem.resolvedStyle.height / 2f;
            hoverItem.style.flexGrow = 0;
            container.Add(hoverItem);
            sr.enabled = false;
        }
    }

    public void ShowItem(MouseLeaveEvent e)
    {
        if (player.HeldItem != null)
        {
            player.HeldItem.GetComponent<SpriteRenderer>().enabled = true;

        }
        if (hoverItem != null)
        {
            container.Remove(hoverItem);
            hoverItem = null;
        }
    }

    void Update()
    {
        if (hoverItem == null)
            return;
        hoverItem.style.left = container.WorldToLocal(Input.mousePosition).x - hoverItem.resolvedStyle.width / 2f;
        hoverItem.style.top = ( Screen.height / 2 ) - container.WorldToLocal(Input.mousePosition).y + hoverItem.resolvedStyle.height / 2f;
    }
}
