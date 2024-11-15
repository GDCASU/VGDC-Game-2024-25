using Gaskellgames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Manages the Item Dragging
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class MouseSlotFollower : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    private Canvas canvas;
    private InventorySlot slot;
    public InventorySlot selected;
    public static CanvasGroup canvasGroup;

    public void Awake()
    {
        canvas = this.transform.parent.parent.GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        slot = GetComponentInChildren<InventorySlot>();
        Toggle(false);
    }

    public void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, 
            Input.mousePosition, 
            canvas.worldCamera, 
            out position
        );

        this.transform.position = canvas.transform.TransformPoint(position);
    }

    public void SetSlot(InventorySlot s)
    {
        slot.ImitateSlot(s.data);
        slot.SetStackSize(s.stackSize);
    }

    public InventorySlot GetSlot() { return slot; }

    public void Toggle(bool toggle)
    {
        canvasGroup.blocksRaycasts = !toggle;
        transform.GetChild(0).gameObject.SetActive(toggle);
    }
}
