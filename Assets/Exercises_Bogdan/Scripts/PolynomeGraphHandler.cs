using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;
using System.Linq;

/// <summary>Plots and draws graph for x value range for a given polynomial.
/// </summary>
public class PolynomeGraphHandler : MonoBehaviour
{

    #region UI_References
        [SerializeField]
        private RectTransform graphRect;
        [SerializeField]
        private LineRenderer graphRenderer;
    #endregion

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
    }

    public static void SetGraphPolynome(Polynome polynomeToPlot)
    {
        polynome = polynomeToPlot;
    }

    public void PlotGraph()
    {
        //Create a new array with the desired amout of points
        plotPoints = new Vector3[GRAPH_LIMITS * BASE_DIVISIONS];

        //We then evaluate for evenly divided points within graph x limits
        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x = Mathf.Lerp(-GRAPH_LIMITS, GRAPH_LIMITS, (float)i/(plotPoints.Length - 1));
            plotPoints[i].y = FuncS.PVal(polynome, plotPoints[i].x);
        }

        //Based on min, and max values we determine the x axis scalar we need in order to limit the graph to out grid rect
        float scaleXToFit = (plotPoints.Max(a => a.x) > Mathf.Abs(plotPoints.Min(a => a.x))) ? (graphRectHeight/2) / plotPoints.Max(a => a.x) : (graphRectHeight/2) / plotPoints.Min(a => a.x);

        //We then normalize all points with respect to the grid rect height
        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x *= (-scaleXToFit);
            plotPoints[i].y *= (-scaleXToFit);
            plotPoints[i].z = 0;
            plotPoints[i] += graphCenterPos;
        }

        //Remove out of bounds vectors graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min)
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

        graphRenderer.positionCount = ajustedPoints.Count;
        graphRenderer.SetPositions(ajustedPoints.ToArray());
    }

    private void SizeGraphRect()
    {
        graphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphRect.rect.height);
        graphRectHeight = Vector3.Distance(graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min)) / Mathf.Sqrt(2);
        graphCenterPos = graphRect.TransformPoint(graphRect.rect.center);
    }
}
