using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ѕодразумеваетс€, что этот презентер будет на рутовом объекте радиального колеса
public class RadialMenuPresenter : MonoBehaviour
{
    // Ёто должно быть не здесь, а скорее всего прив€зыватьс€ к какому-то контейнеру
    [SerializeField]
    private List<RadialMenuItem> menuItems = new List<RadialMenuItem>();
    public List<RadialMenuItem> MenuItems => menuItems;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;


    [Header("Input")]
    [SerializeField]
    private KeyCode openKeyCode = KeyCode.Tab;


    [Header("Internal References")]
    [SerializeField]
    private GameObject mainContent;


    [Header("Selection Rotator")]
    [SerializeField]
    private bool isSnapping = false;

    [SerializeField]
    private float smoothSnappingSpeed = 10f;

    [SerializeField]
    private float selectionMouseSmoothInput = 0.1f;

    [SerializeField]
    private Transform selectionRotator;


    [Header("Divison Lines")]
    [SerializeField]
    private bool drawDivisionLines = true;

    [SerializeField]
    private Transform divisionLinesContentParent;

    [SerializeField]
    private GameObject divisionLinePrefab;


    [Header("Items")]
    [SerializeField]
    [Range(0, 1)]
    private float itemPlacement = 0.6f;

    [SerializeField]
    private Transform itemsContentParent;

    [SerializeField]
    private RadialMenuItemView radialItemViewPrefab;

    private RectTransform thisRect => this.GetComponent<RectTransform>();

    private List<Transform> divisionLines = new List<Transform>();
    private List<RadialMenuItemView> radialItemViews = new List<RadialMenuItemView>();


    private float itemAngleRange => (360 / menuItems.Count);
    private float divisionLineStartAngleOffset => itemAngleRange / 2;


    private Vector2 selectionMouseInput;
    private bool IsOpened { get; set; }
    

    private void Start()
    {
        InstantiateRadialMenu();
        HideMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKeyCode))
        {
            if (IsOpened)
                HideMenu();
            else
                ShowMenu();
        }

        if (!IsOpened) return;

        CalculateSelectionInput();
    }


    public void ShowMenu()
    {
        mainContent.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsOpened = true;
    }

    public void HideMenu()
    {
        mainContent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsOpened = false;
    }

    /// <summary>
    ///     »нициализируем и прорисовываем радиальное меню с новыми предметами
    /// </summary>
    [ContextMenu("Manual Instantiation")]
    private void InstantiateRadialMenu()
    {
        DrawDivisionLines();
        DrawItems();
    }

    private void DrawDivisionLines()
    {
        if (!drawDivisionLines || 
            menuItems.Count <= 1 || 
            divisionLinePrefab == null || 
            divisionLinesContentParent == null) return;

        for (int i = 0; i < divisionLines.Count; i++)
        {
            Destroy(divisionLines[i].gameObject);
        }
        divisionLines.Clear();

        for (int i = 0; i < menuItems.Count; i++)
        {
            GameObject divisionLine = Instantiate(divisionLinePrefab, divisionLinesContentParent);

            divisionLine.GetComponent<RectTransform>().sizeDelta = new Vector2(5, thisRect.rect.height / 2);
            divisionLine.transform.localEulerAngles = new Vector3(0, 0, divisionLineStartAngleOffset + i * itemAngleRange);

            divisionLines.Add(divisionLine.transform);
        }
    }

    private void DrawItems()
    {
        if (radialItemViewPrefab == null ||
            itemsContentParent == null) return;

        for (int i = 0; i < radialItemViews.Count; i++)
        {
            Destroy(radialItemViews[i].gameObject); 
        }
        radialItemViews.Clear();

        float separationRadians = (Mathf.PI * 2) / menuItems.Count;
        float radius = (this.GetComponent<RectTransform>().rect.width / 2) * itemPlacement;
        float x, y;

        for (int i = 0; i < menuItems.Count; i++)
        {
            RadialMenuItemView itemView = Instantiate(radialItemViewPrefab, itemsContentParent);

            x = Mathf.Sin(separationRadians * i) * radius;
            y = Mathf.Cos(separationRadians * i) * radius;

            itemView.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);

            radialItemViews.Add(itemView);
        }
    }


    private void CalculateSelectionInput()
    {
        Vector2 rawMouseInput = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

        selectionMouseInput += rawMouseInput * selectionMouseSmoothInput;
        selectionMouseInput.Normalize();

        Debug.Log(selectionMouseInput);

        //selectionMouseInput.x =  /*- (Screen.width / 2f)*/;
        //selectionMouseInput.y =  /*- (Screen.height / 2f)*/;
        //selectionMouseInput.Normalize();

        if (selectionMouseInput != Vector2.zero)
        {
            float selectionAngle = Mathf.Atan2(selectionMouseInput.y, -selectionMouseInput.x) / Mathf.PI;
            selectionAngle *= 180;
            selectionAngle -= 90;
            selectionAngle += divisionLineStartAngleOffset;

            if (selectionAngle < 0)
            {
                selectionAngle += 360;
            }

            if (selectionRotator != null && !isSnapping)
            {
                selectionRotator.transform.localEulerAngles = new Vector3(0, 0, -(selectionAngle - divisionLineStartAngleOffset));
            }

            if (showDebugLog)
                Debug.Log($"Mouse selection angle => {selectionAngle}");

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (selectionAngle >= i * itemAngleRange &&
                    selectionAngle <= (i + 1) * itemAngleRange)
                {
                    if (selectionRotator != null && isSnapping)
                    {
                        selectionRotator.transform.localRotation = Quaternion.Lerp(
                            selectionRotator.localRotation,  
                            Quaternion.Euler(new Vector3(0, 0, -(i * itemAngleRange))),
                            Time.deltaTime * smoothSnappingSpeed
                        );
                    }

                    if (showDebugLog)
                        Debug.Log($"Selecting => {menuItems[i].Title}");
                }
            }
        }
    }
}

[System.Serializable]
public class RadialMenuItem
{
    public Sprite Icon;
    public string Title;
    public string Description;
}