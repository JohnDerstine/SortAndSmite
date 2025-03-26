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
    private Dictionary<VisualElement, GameObject> slots = new Dictionary<VisualElement, GameObject>();

    private EventCallback<MouseUpEvent> dropItemCallback;
    private EventCallback<MouseDownEvent> getItemCallback;

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

        slot1.RegisterCallback(dropItemCallback);
        slot2.RegisterCallback(dropItemCallback);
        slot3.RegisterCallback(dropItemCallback);
    }

    private void DropItem(MouseUpEvent e)
    {
        VisualElement slot = e.currentTarget as VisualElement;
        if (player.HeldItem != null && slots[slot] == null)
        {
            slot.style.backgroundImage = new StyleBackground(player.HeldItem.gameObject.GetComponent<SpriteRenderer>().sprite);
            slots[slot] = player.HeldItem.gameObject;
            player.HeldItem.gameObject.SetActive(false);
            player.HeldItem = null;
            slot.UnregisterCallback<MouseUpEvent>(DropItem);
            slot.RegisterCallback(getItemCallback);
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
        }
    }
}
