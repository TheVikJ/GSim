using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtils : MonoBehaviour
{
    private static int _uiLayer;

    private void Awake()
    {
        _uiLayer = LayerMask.NameToLayer("UI");
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private static bool IsPointerOverUIElement(IReadOnlyList<RaycastResult> eventSystemRaycastResults)
    {
        for (int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            var curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.layer == _uiLayer)
                return true;
        }

        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}