using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;
using PolynomeUtils;


/// <summary>Handles two polynomial and single polynomial operations and result panel UI interactability.
/// </summary>
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

        //We set ToggleOperationButtons method to polyHandler one and two, in order to check and update buttons on polynome validation
        polyHandlerOne.OnToggleButtons = ToggleOperationButtons;
        polyHandlerTwo.OnToggleButtons = ToggleOperationButtons;

        //For single plolynome operations we set the OnUpdateUI methods and trigger them from the script containing the single polynomial
        OnUpdateUI = UpdateResultText;
        OnUpdateUIString = UpdateResultText;

        xInputParent.SetActive(false);
    }

    private void Start()
    {
        ResultPolynome.elements.Clear();
        simpleOperationButtons = simpleOperationButtonsParent.GetComponentsInChildren<Button>();
        resultOperationButtons = resultOperationButtonsParent.GetComponentsInChildren<Button>();
        ToggleOperationButtons();
        ToggleResultButtons();
    }

    #region SimpleOperations
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

        public void OnDividePolynomials()
        {
            KeyValuePair<Polynome, Polynome> result = FuncS.PDiv(polyHandlerOne.CurentPolynome, polyHandlerTwo.CurentPolynome);
            ResultPolynome = result.Key;
            UpdateResultText(false);
            resultText.text += " R: " + PolynomeParser.FormatMathNotationPolynome(result.Value);
        }
    #endregion

    #region ComplexOperations
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
    #endregion

    public void SetXInputPolynome()
    {
        PolynomeXInputHandler.SetActivePolynome(ResultPolynome);
    }

    public void SetGraphPolynome()
    {
        PolynomeGraphHandler.SetGraphPolynome(ResultPolynome);
    }

    //Toggles the operation buttons on if both PolynomeHandlers curently have a valid polynomial
    private void ToggleOperationButtons()
    {
        bool buttonsState = (polyHandlerOne.IsPolynomeValid && polyHandlerTwo.IsPolynomeValid);
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            UI_Utils.ToggleButtonInteractability(simpleOperationButtons, buttonsState);
        }
    }

    private void UpdateResultText(bool isIntegrate)
    {
        resultText.text = PolynomeParser.FormatMathNotationPolynome(ResultPolynome);
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
        UI_Utils.ToggleButtonInteractability(resultOperationButtons, false);
    }

    private void ToggleResultButtons()
    {
        bool targetState = (ResultPolynome != null && ResultPolynome.elements.Count > 0);
        UI_Utils.ToggleButtonInteractability(resultOperationButtons, targetState);
    }
}
