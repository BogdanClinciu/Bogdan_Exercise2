using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;
using PolynomeUtils;

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
    public bool PolynomeValid { get; private set; } = false;
    public UnityEngine.Events.UnityAction onToggleButtons;

    private bool lastButtonsState = true;

    private void Start()
    {
        polinomialButtons = polinomialButtonsParent.GetComponentsInChildren<Button>();

        CurentPolynome = new Polynome(true);

        polyProperText.text = Parser.NO_POLYNOME;

        TogglePolynomeButtons(string.Empty);

        polyInputField.onValidateInput += delegate(string input, int charIndex, char addedChar) { return Parser.PolynomeInputValidation(input, charIndex, addedChar); };

        polyInputField.onValueChanged.AddListener((string input) => Parser.ParsePolynome(input, polyProperText, CurentPolynome));
        polyInputField.onValueChanged.AddListener((string input) => TogglePolynomeButtons(input));
        if (onToggleButtons != null)
        {
            polyInputField.onValueChanged.AddListener((string input) => onToggleButtons());
        }
    }

    private void OnDestroy()
    {
        polyInputField.onValidateInput -= delegate(string input, int charIndex, char addedChar) { return Parser.PolynomeInputValidation(input, charIndex, addedChar); };

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

    private void TogglePolynomeButtons(string input)
    {
        bool buttonsState = input != string.Empty;
        if(lastButtonsState != buttonsState)
        {
            lastButtonsState = buttonsState;
            PolynomeValid = buttonsState;
            UI_Utils.ToggleButtonInteractability(polinomialButtons, buttonsState);
        }
    }
}
