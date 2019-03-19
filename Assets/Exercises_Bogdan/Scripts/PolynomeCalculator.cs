using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;

public class PolynomeCalculator : MonoBehaviour
{

    [SerializeField]
    private PolynomeHandler polyHandlerOne;
    [SerializeField]
    private PolynomeHandler polyHandlerTwo;

    #region UI_References
        [SerializeField]
        private Text resultText;

        [SerializeField]
        private GameObject simpleOperationButtonsParent;
        [SerializeField]
        private GameObject resultOperationButtonsParent;

        private Button[] simpleOperationButtons;
        private Button[] resultOperationButtons;
        private bool lastButtonsState = true;

        [SerializeField]
        private GameObject xInputParent;

        //Graph related UI elements
        [SerializeField]
        private Slider graphIterationSlider;
        [SerializeField]
        private RectTransform graphContainer;
    #endregion

    public static Polynome ResultPolynome;

    public delegate void UpdateUI(bool isIntegrate);
    public delegate void UpdateUIString(string value);
    public static UpdateUI OnUpdateUI {get; private set;}
    public static UpdateUIString OnUpdateUIString {get; private set;}

    private const string CONSTANT = "+ C";


    private void Awake()
    {
        ResultPolynome = new Polynome(true);
        polyHandlerOne.onToggleButtons = (ToggleSimpleOperationButtons);
        polyHandlerTwo.onToggleButtons = (ToggleSimpleOperationButtons);
        OnUpdateUI = UpdateResultText;
        OnUpdateUIString = UpdateResultText;
        xInputParent.SetActive(false);
    }

    private void Start()
    {
        ResultPolynome.elements.Clear();
        simpleOperationButtons = simpleOperationButtonsParent.GetComponentsInChildren<Button>();
        resultOperationButtons = resultOperationButtonsParent.GetComponentsInChildren<Button>();
        ToggleSimpleOperationButtons();
        ToggleResultButtons();
    }

    public void OnAddPolynomials()
    {
        ResultPolynome = FuncS.PAdd(polyHandlerOne.CurentPolynome, polyHandlerTwo.CurentPolynome);
        UpdateResultText(false);
    }

    public void OnSubtractPolynomials()
    {
        ResultPolynome = FuncS.PSub(polyHandlerOne.CurentPolynome, polyHandlerTwo.CurentPolynome);
        UpdateResultText(false);
    }

    public void OnMultiplyPolynomials()
    {
        ResultPolynome = FuncS.PMul(polyHandlerOne.CurentPolynome, polyHandlerTwo.CurentPolynome);
        UpdateResultText(false);
    }

    // public void OnAddPolynomials()
    // {
    //     resultPolynome = FuncS.PAdd(polyHandlerOne.CurentPolynome, polyHandlerTwo.CurentPolynome);
    //     UpdateResultText();
    // }

    public void OnDeriveResult()
    {
        ResultPolynome = FuncC.PDerive(ResultPolynome);
        UpdateResultText(false);
    }

    public void OnIntegrateResult()
    {
        ResultPolynome = FuncC.PIntegrate(ResultPolynome);
        UpdateResultText(true);
    }

    public void SetXInputPolynome()
    {
        PolynomeXInputHandler.SetActivePolynome(ResultPolynome);
    }

    public void SetGraphPolynome()
    {
        PolynomeGraphHandler.SetGraphPolynome(ResultPolynome);
    }

    private void ToggleSimpleOperationButtons()
    {
        bool buttonsState = (polyHandlerOne.PolynomeValid && polyHandlerTwo.PolynomeValid);
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            PolynomeUtils.UI_Utils.ToggleButtonInteractability(simpleOperationButtons, buttonsState);
        }
    }

    private void UpdateResultText(bool isIntegrate)
    {
        resultText.text = PolynomeUtils.Parser.ParsePolynome(ResultPolynome);
        if(isIntegrate)
        {
            resultText.text += CONSTANT;
        }
        ToggleResultButtons();
    }

    private void UpdateResultText(string value)
    {
        ResultPolynome.elements.Clear();
        resultText.text = value;
        PolynomeUtils.UI_Utils.ToggleButtonInteractability(resultOperationButtons, false);
    }

    private void ToggleResultButtons()
    {
        bool targetState = (ResultPolynome != null && ResultPolynome.elements.Count > 0);
        PolynomeUtils.UI_Utils.ToggleButtonInteractability(resultOperationButtons, targetState);
    }
}
