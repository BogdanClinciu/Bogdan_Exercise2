using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;
using System.Linq;

public class PolynomeGraphHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform graphRect;
    [SerializeField]
    private Slider iterationsSlider;
    [SerializeField]
    private Slider xBoundsSlider;
    [SerializeField]
    private Text iterationsText;
    [SerializeField]
    private Text xBoundsText;
    [SerializeField]
    private LineRenderer graphRenderer;

    private static Polynome polynome;

    private Vector3[] plotPoints;
    private Vector3 graphCenterPos;
    private float graphRectHeight;

    private const int BASE_POINT_COUNT = 10;
    private const int BASE_DIVISIONS = 25;
    private const int GRAPH_LIMITS = 10;


    private void Start()
    {
        SizeGraphRect();
        UpdateIterationsText();
        UpdateXBoundText();
    }

    private void Update()
    {
        Debug.DrawLine(graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min), Color.red);
    }

    public static void SetGraphPolynome(Polynome polynomeToPlot)
    {
        polynome = polynomeToPlot;
    }

    public void PlotGraph()
    {
        // use graph rect height to limit graph y
        plotPoints = new Vector3[GRAPH_LIMITS * BASE_DIVISIONS];

        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x = Mathf.Lerp(-GRAPH_LIMITS, GRAPH_LIMITS, (float)i/(plotPoints.Length - 1));
            plotPoints[i].y = FuncS.PVal(polynome, plotPoints[i].x);
        }

        float scaleXToFit = (plotPoints.Max(a => a.x) > Mathf.Abs(plotPoints.Min(a => a.x))) ? (graphRectHeight/2) / plotPoints.Max(a => a.x) : (graphRectHeight/2) / plotPoints.Min(a => a.x);
        float scaleYToFit = (plotPoints.Max(a => a.y) > Mathf.Abs(plotPoints.Min(a => a.y))) ? (graphRectHeight/2) / plotPoints.Max(a => a.y) : (graphRectHeight/2) / plotPoints.Min(a => a.y);
        //Debug.Log(plotPoints.Max(a => a.y) + ">" + graphRectHeight);

        //normalizeScale
        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x *= (-scaleXToFit);
            plotPoints[i].y *= (-scaleXToFit);
            plotPoints[i].z = 0;
            plotPoints[i] += graphCenterPos;
        }

        //remove out of bounds vectors graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min)
        List<Vector3> ajustedPoints = new List<Vector3>();

        for (int i = 0; i < plotPoints.Length; i++)
        {
            if(plotPoints[i].y < (graphRect.TransformPoint(graphRect.rect.max).y))
            {
                if(plotPoints[i].y > (graphRect.TransformPoint(graphRect.rect.min).y))
                {
                    ajustedPoints.Add(plotPoints[i]);
                }
            }
        }

        Debug.Log("max: " + graphRect.rect.max.y + "   min: " + graphRect.rect.min.y);

        // graphRenderer.positionCount = plotPoints.Length;
        // graphRenderer.SetPositions(plotPoints);

        graphRenderer.positionCount = ajustedPoints.Count;
        graphRenderer.SetPositions(ajustedPoints.ToArray());
    }

    public void UpdateIterationsText()
    {
        iterationsText.text = "Div" + "\n" + iterationsSlider.value + "\n"  + "*10";
    }

    public void UpdateXBoundText()
    {
        xBoundsText.text = xBoundsSlider.value + "\n" + "to" + "\n" + xBoundsSlider.value;
    }

    private void SizeGraphRect()
    {
        graphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphRect.rect.height);
        graphRectHeight = Vector3.Distance(graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min)) / Mathf.Sqrt(2);
        graphCenterPos = graphRect.TransformPoint(graphRect.rect.center);
    }
}
