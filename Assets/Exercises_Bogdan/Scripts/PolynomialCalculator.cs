using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomialMath;
using PolynomialUtils;


/// <summary>Handles two polynomial and single polynomial operations and result panel UI interactability.
/// </summary>
public class PolynomialCalculator : MonoBehaviour
{

    [SerializeField]
    private PolynomialHandler polyHandlerOne;
    [SerializeField]
    private PolynomialHandler polyHandlerTwo;

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
    #endregion

    public static Polynomial ResultPolynomial;

    public delegate void UpdateUI(bool isIntegrate);
    public delegate void UpdateUIString(string value);
    public static UpdateUI OnUpdateUI {get; private set;}
    public static UpdateUIString OnUpdateUIString {get; private set;}

    private const string CONSTANT = "+ C";


    private void Awake()
    {
        ResultPolynomial = new Polynomial(true);

        //We set ToggleOperationButtons method to polyHandler one and two, in order to check and update buttons on polynomial validation
        polyHandlerOne.OnToggleButtons = ToggleOperationButtons;
        polyHandlerTwo.OnToggleButtons = ToggleOperationButtons;

        //For single plolynome operations we set the OnUpdateUI methods and trigger them from the script containing the single polynomial
        OnUpdateUI = UpdateResultText;
        OnUpdateUIString = UpdateResultText;

        xInputParent.SetActive(false);
    }

    private void Start()
    {
        ResultPolynomial.elements.Clear();
        simpleOperationButtons = simpleOperationButtonsParent.GetComponentsInChildren<Button>();
        resultOperationButtons = resultOperationButtonsParent.GetComponentsInChildren<Button>();
        ToggleOperationButtons();
        ToggleResultButtons();
    }

    #region SimpleOperations
        public void AddPolynomials()
        {
            ResultPolynomial = SimpleOperations.PolynomialAddition(polyHandlerOne.CurentPolynomial, polyHandlerTwo.CurentPolynomial);
            UpdateResultText(false);
        }

        public void SubtractPolynomials()
        {
            ResultPolynomial = SimpleOperations.PolynomialSubtraction(polyHandlerOne.CurentPolynomial, polyHandlerTwo.CurentPolynomial);
            UpdateResultText(false);
        }

        public void MultiplyPolynomials()
        {
            ResultPolynomial = SimpleOperations.PolynomialMultiplication(polyHandlerOne.CurentPolynomial, polyHandlerTwo.CurentPolynomial);
            UpdateResultText(false);
        }

        public void DividePolynomials()
        {
            KeyValuePair<Polynomial, Polynomial> result = SimpleOperations.PolynomialDivision(polyHandlerOne.CurentPolynomial, polyHandlerTwo.CurentPolynomial);
            ResultPolynomial = result.Key;
            UpdateResultText(false);
            resultText.text += " R: " + PolynomialParser.FormatMathNotationPolynomial(result.Value);
        }
    #endregion

    #region ComplexOperations
        public void DerivativeResult()
        {
            ResultPolynomial = ComplexOperations.PlynomialDerivative(ResultPolynomial);
            UpdateResultText(false);
        }

        public void IntegrateResult()
        {
            ResultPolynomial = ComplexOperations.PlynomialIntegrate(ResultPolynomial);
            UpdateResultText(true);
        }
    #endregion

    public void SetXInputPolynomial()
    {
        PolynomialXInputHandler.SetActivePolynomial(ResultPolynomial);
    }

    public void SetGraphPolynomial()
    {
        PolynomialGraphHandler.SetGraphPolynomial(ResultPolynomial);
    }

    //Toggles the operation buttons on if both PolynomialHandlers curently have a valid polynomial
    private void ToggleOperationButtons()
    {
        bool buttonsState = (polyHandlerOne.IsPolynomialValid && polyHandlerTwo.IsPolynomialValid);
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            UI_Utils.ToggleButtonInteractability(simpleOperationButtons, buttonsState);
        }
    }

    private void UpdateResultText(bool isIntegrate)
    {
        resultText.text = PolynomialParser.FormatMathNotationPolynomial(ResultPolynomial);
        if(isIntegrate)
        {
            resultText.text += CONSTANT;
        }
        ToggleResultButtons();
    }

    private void UpdateResultText(string value)
    {
        ResultPolynomial.elements.Clear();
        resultText.text = value;
        UI_Utils.ToggleButtonInteractability(resultOperationButtons, false);
    }

    private void ToggleResultButtons()
    {
        bool targetState = (ResultPolynomial != null && ResultPolynomial.elements.Count > 0);
        UI_Utils.ToggleButtonInteractability(resultOperationButtons, targetState);
    }
}
