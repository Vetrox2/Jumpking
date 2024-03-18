using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LoginPanel : MonoBehaviour
{
    [SerializeField]
    private SignPanel SignUpPanel;
    [SerializeField]
    private SignPanel SignInPanel;

    public void PressedEnter()
    {
        if (SignUpPanel.SignPanelObject.activeInHierarchy)
            SignUp();
        else
            SignIn();
    }

    public void SwapToLoginPanel()
    {
        SignUpPanel.SignPanelObject.SetActive(false);
        SignInPanel.SignPanelObject.SetActive(true);
    }

    public void SwapToSignUpPanel()
    {
        SignUpPanel.SignPanelObject.SetActive(true);
        SignInPanel.SignPanelObject.SetActive(false);
    }

    public void SignUp()
    {
        SignUpPanel.ErrorMessage.text = string.Empty;
        SignUpPanel.ErrorMessage.gameObject.SetActive(true);
        string passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$";
        string usernamePattern = @"^(?=.{3,12}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$";

        if (!Regex.Match(SignUpPanel.UserName.text, usernamePattern).Success)
        {
            SignUpPanel.ErrorMessage.text = SignUpPanel.WrongLoginMessage;
            return;
        }
        if (!Regex.Match(SignUpPanel.Password.text, passwordPattern).Success)
        {
            SignUpPanel.ErrorMessage.text = SignUpPanel.WrongPasswordMessage;
            return;
        }
        if (SignUpPanel.Password.text != SignUpPanel.ConfirmPassword.text)
        {
            SignUpPanel.ErrorMessage.text = SignUpPanel.DifferentPasswordsMessage;
            return;
        }

        SignUpPanel.ErrorMessage.gameObject.SetActive(false);
        SignUpPanel.Button.interactable = false;

        RealmController<Users> realmController = new(SignUpCallback);
    }

    public void SignIn()
    {
        SignInPanel.ErrorMessage.text = string.Empty;
        SignInPanel.ErrorMessage.gameObject.SetActive(false);

        if (SignInPanel.UserName.text.Length == 0 || SignInPanel.Password.text.Length == 0)
            return;

        SignInPanel.Button.interactable = false;

        RealmController<Users> realmController = new(SignInCallback);
    }

    void SignUpCallback(RealmController<Users> realmController)
    {
        if (!realmController.SignUp(SignUpPanel.UserName.text, SignUpPanel.Password.text))
        {
            SignUpPanel.ErrorMessage.text = "Login already exists";
            SignUpPanel.ErrorMessage.gameObject.SetActive(true);
            SignUpPanel.Button.interactable = true;
        }
        else
        {
            PlayerPrefs.SetString("UserName", SignUpPanel.UserName.text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    void SignInCallback(RealmController<Users> realmController)
    {
        if (realmController.SignIn(SignInPanel.UserName.text, SignInPanel.Password.text))
        {
            PlayerPrefs.SetString("UserName", SignInPanel.UserName.text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SignInPanel.ErrorMessage.text = SignInPanel.WrongPasswordMessage;
            SignInPanel.ErrorMessage.gameObject.SetActive(true);
            SignInPanel.Button.interactable = true;
        }
    }
}

[Serializable]
internal class SignPanel
{
    public GameObject SignPanelObject;
    public TMP_InputField UserName;
    public TMP_InputField Password;
    public TMP_InputField ConfirmPassword;
    public TextMeshProUGUI ErrorMessage;
    public Button Button;
    public string WrongLoginMessage;
    public string WrongPasswordMessage;
    public string DifferentPasswordsMessage;
}
