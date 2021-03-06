﻿using UnityEngine;
using UnityEngine.UI;
using PolynomialMath;
using System.Linq;

/// <summary>Plots and draws graph for x value range for a given polynomial.
/// </summary>
public class PolynomialGraphHandler : MonoBehaviour
{
    #region UI_References
        [SerializeField]
        private RectTransform graphDisplayRect;
        [SerializeField]
        private RectTransform graphRect;
        [SerializeField]
        private LineRenderer graphRenderer;

        [SerializeField]
        private Slider scaleSlider;
        [SerializeField]
        private Text scaleText;
    #endregion

    private static Polynomial polynomial;

    private Vector3[] plotPoints;
    private float graphRectHeight;

    private const int BASE_DIVISIONS = 250;
    public const int GRAPH_LIMITS = 10;

    private int scale = 1;
    private const string SCALE_LABEL = "Scale: 1×";
    private Vector3[] graphPointsCache;

    private void Start()
    {
        SizeGraphRect();
        scaleText.text = SCALE_LABEL + scale;
    }

    public static void SetGraphPolynomial(Polynomial polynomialToPlot)
    {
        polynomial = polynomialToPlot;
    }

    public void PlotGraph()
    {
        if(polynomial == null)
        {
            return;
        }
        //Create a new array with the desired amout of points
        plotPoints = new Vector3[GRAPH_LIMITS * BASE_DIVISIONS];

        //We then evaluate for evenly divided points within graph x limits
        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x = Mathf.Lerp(-GRAPH_LIMITS * scale, GRAPH_LIMITS * scale, (float)i/(plotPoints.Length - 1));
            plotPoints[i].y = SimpleOperations.PlynomialEvaluate(polynomial, plotPoints[i].x);
        }

        //Based on min, and max values we determine the x axis scalar we need in order to limit the graph to out grid rect
        float scaleXToFit = (plotPoints.Max(a => a.x) > Mathf.Abs(plotPoints.Min(a => a.x))) ? (graphRectHeight/2) / plotPoints.Max(a => a.x) : (graphRectHeight/2) / plotPoints.Min(a => a.x);

        //We then normalize all points with respect to the grid rect height
        for (int i = 0; i < plotPoints.Length; i++)
        {
            plotPoints[i].x *= (-scaleXToFit) * graphRect.localScale.x;
            plotPoints[i].y *= (-scaleXToFit) * graphRect.localScale.y;
            plotPoints[i].z = 0;
            plotPoints[i] += graphRect.TransformPoint(graphRect.rect.center);
        }

        graphPointsCache = new Vector3[plotPoints.Length];
        System.Array.Copy(plotPoints, graphPointsCache, plotPoints.Length);
        graphRenderer.positionCount = plotPoints.Length;
        graphRenderer.SetPositions(plotPoints);
    }

    public void ChangeScaleValue()
    {
        scale = (int)scaleSlider.value;
        scaleText.text = SCALE_LABEL + scale;
    }

    public void OffsetGraphPoints(Vector3 offset, float scale)
    {
        if(graphRenderer.positionCount > 0)
        {
            for (int i = 0; i < graphRenderer.positionCount; i++)
            {
                plotPoints[i] = graphPointsCache[i] * scale + offset;
            }
            graphRenderer.SetPositions(plotPoints);
        }
    }

    private void SizeGraphRect()
    {
        graphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphRect.rect.height);
        graphRectHeight = Vector3.Distance(graphRect.TransformPoint(graphRect.rect.max), graphRect.TransformPoint(graphRect.rect.min)) / Mathf.Sqrt(2);

        graphDisplayRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphDisplayRect.rect.height);
    }
}
