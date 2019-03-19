using UnityEngine;
using UnityEngine.UI;
using PolynomeMath;

public class PolynomeXInputHandler : MonoBehaviour
{
   [SerializeField]
   private GameObject xInputParent;
   [SerializeField]
   private InputField xValueInputField;
   [SerializeField]
   private Button xValueConfirmButton;

   private static Polynome ActivePolynome;

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

    public static void SetActivePolynome(Polynome p)
    {
        ActivePolynome = new Polynome(p);
    }

    public void ToggleXInputPanel()
    {
        xInputParent.SetActive(!xInputParent.activeSelf);
        ResetInputField();
    }

    public void OnConfirmXValue()
    {
        PolynomeCalculator.OnUpdateUIString(FuncS.PValString(ActivePolynome, validatedValue));
        ToggleXInputPanel();
    }

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
