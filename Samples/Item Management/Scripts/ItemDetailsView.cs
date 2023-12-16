using System;
using Gamepangin;
using TMPro;
using UnityEngine;

public class ItemDetailsView : MonoBehaviour, IEventListener<ItemSelectedEvent>
{
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCategory;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private void OnEnable()
    {
        EventManager.AddListener(this);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(this);
    }

    public void OnEvent(ItemSelectedEvent e)
    {
        content.SetActive(e.item != null);

        if (e.item == null) return;

        itemName.text = e.item.Definition.Name;
        itemCategory.text = e.item.Definition.Category.Name;
        itemDescription.text = e.item.Definition.Description;
    }
}
