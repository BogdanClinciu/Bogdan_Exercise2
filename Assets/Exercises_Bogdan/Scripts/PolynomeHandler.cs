using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;
using PolynomeUtils;


/// <summary>Handles polynome input, initialization, and single polynome operations.
/// </summary>
public class PolynomeHandler : MonoBehaviour
{

    #region UI_References
        //Input fields taking only the coeficients of the polinomial;
        [SerializeField]
        private InputField polyInputField;

        //Texts where polinomials are displayed with proper mathematical notation
        [SerializeField]
        private Text polyProperText;

        [SerializeField]
        private GameObject polinomialButtonsParent;

        private Button[] polinomialButtons;
    #endregion

    public Polynome CurentPolynome { get; private set; }
    public bool IsPolynomeValid { get; private set; } = false;
    public UnityEngine.Events.UnityAction OnToggleButtons;

    private bool lastButtonsState = true;



    private void Start()
    {
        polinomialButtons = polinomialButtonsParent.GetComponentsInChildren<Button>();

        CurentPolynome = new Polynome(true);

        polyProperText.text = PolynomeParser.NO_POLYNOME;

        TogglePolynomeButtons(string.Empty);

        //Here we subscribe out custom validation method to the onValidateInput funcition on the polynome input field
        polyInputField.onValidateInput += delegate(string input, int charIndex, char addedChar) { return PolynomeParser.PolynomeInputValidation(input, charIndex, addedChar); };

        //We add listeners to the onValueChanged event of the input field to check polynome validity and parse the polynome
        polyInputField.onValueChanged.AddListener((string input) => PolynomeParser.ParsePolynome(input, polyProperText, CurentPolynome));
        polyInputField.onValueChanged.AddListener((string input) => TogglePolynomeButtons(input));
        if (OnToggleButtons != null)
        {
            polyInputField.onValueChanged.AddListener((string input) => OnToggleButtons());
        }
    }

    private void OnDestroy()
    {
        polyInputField.onValidateInput -= delegate(string input, int charIndex, char addedChar) { return PolynomeParser.PolynomeInputValidation(input, charIndex, addedChar); };

        polyInputField.onValueChanged.RemoveAllListeners();
    }

    public void OnDerive()
    {
        PolynomeCalculator.ResultPolynome = FuncC.PDerive(CurentPolynome);
        PolynomeCalculator.OnUpdateUI(false);
    }

    public void OnIntegrate()
    {
        PolynomeCalculator.ResultPolynome = FuncC.PIntegrate(CurentPolynome);
        PolynomeCalculator.OnUpdateUI(true);
    }

    public void SetXInputPolynome()
    {
        PolynomeXInputHandler.SetActivePolynome(CurentPolynome);
    }

    public void SetGraphPolynome()
    {
        PolynomeGraphHandler.SetGraphPolynome(CurentPolynome);
    }

    //Toggles the operation buttons on if this handler has a valid polynomial
    private void TogglePolynomeButtons(string input)
    {
        bool buttonsState = input != string.Empty;
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            IsPolynomeValid = buttonsState;
            UI_Utils.ToggleButtonInteractability(polinomialButtons, buttonsState);
        }
    }
}
