using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class GraphicRaycasterUI : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public List<RaycastResult> Results = new List<RaycastResult>();
    public bool IsMouseOnGUI;

    public UIManager UIManager;

    void Start()
    {
        UIManager = GetComponent<UIManager>();
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    public bool RayCastToResults(Vector2 mousePosition)
    {
        Results = new List<RaycastResult>();
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = mousePosition
        };

        m_Raycaster.Raycast(m_PointerEventData, Results);

        IsMouseOnGUI = Results.Count > 0;
        UIManager.RaycasterUpdated(Results);

        return IsMouseOnGUI;
    }
}
