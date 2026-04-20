using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{

    [SerializeField] private RectTransform selectionAreaRectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnitSelectionManager.Instance.OnSelectionStart += Instance_OnSelectionStart;
        UnitSelectionManager.Instance.OnSelectionEnd += Instance_OnSelectionEnd;

        selectionAreaRectTransform.gameObject.SetActive(false);
    }


    private void Instance_OnSelectionStart(object sender, System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }

    private void Instance_OnSelectionEnd(object sender, System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    

    // Update is called once per frame
    void Update()
    {
        if(selectionAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    public void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

        float canvasScale = canvas.transform.localScale.x;

        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x,selectionAreaRect.y) / canvasScale;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width,selectionAreaRect.height) / canvasScale;
    }
}
