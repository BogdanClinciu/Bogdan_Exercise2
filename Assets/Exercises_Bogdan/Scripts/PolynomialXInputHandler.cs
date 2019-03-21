using UnityEngine;
using UnityEngine.UI;
using PolynomialMath;

/// <summary>Handles the x input for value/evaluate type operations.
/// </summary>
public class PolynomialXInputHandler : MonoBehaviour
{
   [SerializeField]
   private GameObject xInputParent;
   [SerializeField]
   private InputField xValueInputField;
   [SerializeField]
   private Button xValueConfirmButton;

   private static Polynomial ActivePolynomial;

   private bool hasValidValue = false;
   private float validatedValue = 0.0f;

    private void Start()
    {
        ResetInputField();
        if(xInputParent.activeSelf)
        {
            xInputParent.SetActive(false);
        }
    }

    public static void SetActivePolynomial(Polynomial p)
    {
        ActivePolynomial = new Polynomial(p);
    }

    public void ToggleXInputPanel()
    {
        xInputParent.SetActive(!xInputParent.activeSelf);
        ResetInputField();
    }

    public void ConfirmXValue()
    {
        PolynomialCalculator.OnUpdateUIString(SimpleOperations.PolynomialStringEvaluate(ActivePolynomial, validatedValue));
        ToggleXInputPanel();
    }


    //Toggles confirm button based on the contents of the x value input field
    public void ValidateValue()
    {
        if(!xValueInputField.text.Length.Equals(0))
        {
            if(float.TryParse(xValueInputField.text, out validatedValue))
            {
                if(!validatedValue.Equals(0.0f))
                {
                    hasValidValue = true;
                }
                else
                {
                    hasValidValue = false;
                }
            }
            else
            {
                hasValidValue = false;
            }

            ToggleConfirmButton(hasValidValue);
        }
    }

    private void ResetInputField()
    {
        ToggleConfirmButton(false);
        validatedValue = 0.0f;
        hasValidValue = false;
        xValueInputField.text = string.Empty;
    }

    private void ToggleConfirmButton(bool state)
    {
        xValueConfirmButton.interactable = state;
    }

}
