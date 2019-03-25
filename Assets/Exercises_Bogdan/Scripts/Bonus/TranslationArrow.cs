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
    [SerializeField]
    private RectTransform infiniteGridRect;

    private Vector3 startPosition = Vector2.one;
    private Vector3 endPosition = Vector2.one;
    private Vector3 graphRectInitialPosition = Vector2.one;
    private Vector3 graphRectStartPosition = Vector2.one;
    private float scaleFactor = 0;
    private Rect targetRect;

    private bool doPan = false;
    private bool doPinchScale = false;

    private Vector3 initialPinchScale;
    private Vector2 initialTargetRectSize;
    private Vector3 rescale = Vector2.one;
    private float pinchScale = 1.0f;
    private float graphRectWidth = 0.0f;
    private const float maxScaleOffset = 3.9f;
    private const float minScaleOffset = 0.1f;


    private void Start()
    {
        initialPinchScale = graphRectTransform.localScale;
        initialTargetRectSize = infiniteGrid.uvRect.size;
        graphRectWidth = (graphRectTransform.TransformPoint(graphRectTransform.rect.max).y - graphRectTransform.TransformPoint(graphRectTransform.rect.min).y );
        graphRectStartPosition = graphRectTransform.position;
        scaleFactor = (
            graphRectWidth/
            (graphDisplayRectTransform.TransformPoint(graphDisplayRectTransform.rect.max).y - graphDisplayRectTransform.TransformPoint(graphDisplayRectTransform.rect.min).y)
            );

        translationLine.positionCount = 2;
        targetRect = new Rect(infiniteGrid.uvRect);
    }

    private void Update()
    {
        if(doPan)
        {
            PanScaleGraph(true);
            CalculateInfiniteRectPosition();
        }
        else if(doPinchScale)
        {
            PanScaleGraph(false);
        }
        else if (translationLine.enabled)
        {
            EndPanScale();
            CalculateInfiniteRectPosition();
        }
        Debug.DrawLine(graphRectTransform.position, Vector3.zero, Color.red);
    }

    private void OnGUI()
    {
        if(doPan)
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

    private void EndPanScale()
    {
        translationLine.enabled = false;
        resetButton.interactable = (graphRectTransform.position != startPosition);
        initialPinchScale = graphRectTransform.localScale;
        initialTargetRectSize = infiniteGrid.uvRect.size;
    }

    private void PanScaleGraph(bool isPan)
    {
        endPosition = (Input.mousePosition - graphDisplayRectTransform.position) * scaleFactor;
        endPosition.z = translationLine.transform.position.z;

        if(!isPan)
        {
            endPosition.y = startPosition.y;

            if(startPosition.x - endPosition.x >= 0)
            {
                pinchScale = Mathf.Lerp(1, minScaleOffset, (startPosition.x - endPosition.x)/graphRectWidth);
            }
            else
            {
                pinchScale = Mathf.Lerp(1, maxScaleOffset, -(startPosition.x - endPosition.x)/graphRectWidth);
            }

            rescale = initialPinchScale * pinchScale;
            graphRectTransform.localScale = rescale;
            targetRect.size = Vector2.one/rescale.y;
            CalculateInfiniteRectPosition();
            graphHandler.OffsetGraphPoints((Vector2)graphRectTransform.position, graphRectTransform.localScale.y);
        }
        else
        {
            graphHandler.OffsetGraphPoints((endPosition - startPosition), graphRectTransform.localScale.y);
            graphRectTransform.position = graphRectInitialPosition + (endPosition - startPosition);
        }

        translationLine.SetPosition(1, endPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        graphRectInitialPosition = graphRectTransform.position;
        doPan = eventData.pointerId.Equals(-1);
        doPinchScale = eventData.pointerId.Equals(-2);
        translationLine.enabled = true;
        startPosition = (Input.mousePosition - graphDisplayRectTransform.position) * scaleFactor;
        startPosition.z = translationLine.transform.position.z;
        translationLine.SetPosition(0, startPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        doPan = doPinchScale = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        doPan = doPinchScale = false;
        graphHandler.PlotGraph();
    }

    private void CalculateInfiniteRectPosition()
    {
        targetRect.position = Vector2.one - Vector2.one/rescale.y/2
        -(Vector2)graphRectTransform.position/(graphRectTransform.TransformPoint(graphRectTransform.rect.max).y - graphRectTransform.TransformPoint(graphRectTransform.rect.min).y);
    }
}
