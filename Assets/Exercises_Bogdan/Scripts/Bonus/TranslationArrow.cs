using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TranslationArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private LineRenderer translationLine;
    [SerializeField]
    private RectTransform graphRectTransform;
    [SerializeField]
    private RectTransform graphDisplayRectTransform;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private PolynomialGraphHandler graphHandler;
    [SerializeField]
    private RawImage infiniteGrid;

    private Vector3 startPosition = Vector2.one;
    private Vector3 endPosition = Vector2.one;
    private Vector3 graphRectInitialPosition = Vector2.one;
    private Vector3 graphRectStartPosition = Vector2.one;
    private float scaleFactor = 0;
    private Rect targetRect;

    bool drawLine = false;


    private void Start()
    {
        graphRectStartPosition = graphRectTransform.position;
        scaleFactor = (
            (graphRectTransform.TransformPoint(graphRectTransform.rect.max).y - graphRectTransform.TransformPoint(graphRectTransform.rect.min).y )/
            (graphDisplayRectTransform.TransformPoint(graphDisplayRectTransform.rect.max).y - graphDisplayRectTransform.TransformPoint(graphDisplayRectTransform.rect.min).y)
            );

        translationLine.positionCount = 2;
        targetRect = new Rect(infiniteGrid.uvRect);
    }

    private void Update()
    {
        if(drawLine)
        {
            UpdateLine();
            targetRect.position = -graphRectTransform.position/(graphRectTransform.TransformPoint(graphRectTransform.rect.max).y - graphRectTransform.TransformPoint(graphRectTransform.rect.min).y);
        }
        else if (translationLine.enabled)
        {
            EndLineUpdate();
            targetRect.position = -graphRectTransform.position/(graphRectTransform.TransformPoint(graphRectTransform.rect.max).y - graphRectTransform.TransformPoint(graphRectTransform.rect.min).y);
        }
    }


    private void OnGUI()
    {
        if(drawLine)
        {
            infiniteGrid.uvRect = targetRect;
        }
        else if (infiniteGrid.uvRect != targetRect)
        {
            infiniteGrid.uvRect = targetRect;
        }
    }

    public void ResetGraphRectPosition()
    {
        graphRectTransform.position = graphRectStartPosition;
        graphHandler.PlotGraph();
    }

    private void EndLineUpdate()
    {
        translationLine.enabled = false;
        resetButton.interactable = (graphRectTransform.position != startPosition);
    }

    private void UpdateLine()
    {
        endPosition = (Input.mousePosition - graphDisplayRectTransform.position) * scaleFactor;
        endPosition.z = translationLine.transform.position.z;
        translationLine.SetPosition(1, endPosition);

        graphRectTransform.position = graphRectInitialPosition + (endPosition - startPosition);
        graphHandler.OffsetGraphPoints((endPosition - startPosition), false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        graphRectInitialPosition = graphRectTransform.position;
        drawLine = true;
        translationLine.enabled = true;
        startPosition = (Input.mousePosition - graphDisplayRectTransform.position) * scaleFactor;
        startPosition.z = translationLine.transform.position.z;
        translationLine.SetPosition(0, startPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        drawLine = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        drawLine = false;
        graphHandler.OffsetGraphPoints((endPosition - startPosition), true);
    }
}
