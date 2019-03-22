using UnityEngine;
using UnityEngine.UI;
using PolynomialMath;
using PolynomialUtils;


/// <summary>Handles polynomial input, initialization, and single polynomial operations.
/// </summary>
public class PolynomialHandler : MonoBehaviour
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

    public Polynomial CurentPolynomial { get; private set; }
    public bool IsPolynomialValid { get; private set; } = false;
    public UnityEngine.Events.UnityAction OnToggleButtons;

    private bool lastButtonsState = true;



    private void Start()
    {
        polinomialButtons = polinomialButtonsParent.GetComponentsInChildren<Button>();

        CurentPolynomial = new Polynomial(true);

        polyProperText.text = PolynomialParser.NO_POLYNOMIAL;

        TogglePolynomialButtons(string.Empty);

        //Here we subscribe out custom validation method to the onValidateInput funcition on the polynomial input field
        polyInputField.onValidateInput += delegate(string input, int charIndex, char addedChar) { return PolynomialParser.PolynomialInputValidation(input, charIndex, addedChar); };

        //We add listeners to the onValueChanged event of the input field to check polynomial validity and parse the polynomial
        polyInputField.onValueChanged.AddListener((string input) => PolynomialParser.ParsePolynomial(input, polyProperText, CurentPolynomial));
        polyInputField.onValueChanged.AddListener((string input) => TogglePolynomialButtons(input));
        if (OnToggleButtons != null)
        {
            polyInputField.onValueChanged.AddListener((string input) => OnToggleButtons());
        }
    }

    private void OnDestroy()
    {
        polyInputField.onValidateInput -= delegate(string input, int charIndex, char addedChar) { return PolynomialParser.PolynomialInputValidation(input, charIndex, addedChar); };

        polyInputField.onValueChanged.RemoveAllListeners();
    }

    public void Derivative()
    {
        PolynomialCalculator.ResultPolynomial = ComplexOperations.PlynomialDerivative(CurentPolynomial);
        PolynomialCalculator.OnUpdateUI(false);
    }

    public void Integrate()
    {
        PolynomialCalculator.ResultPolynomial = ComplexOperations.PlynomialIntegrate(CurentPolynomial);
        PolynomialCalculator.OnUpdateUI(true);
    }

    public void SetXInputPolynomial()
    {
        PolynomialXInputHandler.SetActivePolynomial(CurentPolynomial);
    }

    public void SetGraphPolynomial()
    {
        PolynomialGraphHandler.SetGraphPolynomial(CurentPolynomial);
    }

    //Toggles the operation buttons on if this handler has a valid polynomial
    private void TogglePolynomialButtons(string input)
    {
        bool buttonsState = input != string.Empty;
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            IsPolynomialValid = buttonsState;
            UI_Utils.ToggleButtonInteractability(polinomialButtons, buttonsState);
        }
    }
}
