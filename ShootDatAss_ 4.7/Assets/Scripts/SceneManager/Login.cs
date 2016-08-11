using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	public GameObject login;
	public GameObject register;

	public InputField register_username;
	public InputField register_password;
	public InputField register_passwordConfirm;

	public InputField login_username;
	public InputField login_password;

	public Text loginMessageText;
	public Text registerMessageText;

	public bool isRegistering;
	public bool isLogingIn;

	public string[] perbodenString;
	public bool alreadyRegistered;

	public string registerMessege;
	public string loginMessage;

	public Button register_login;
	public Button register_confirm;
	public Button login_cencel;
	public Button login_login;

	// Use this for initialization
    void Start()
    {
        Toogle(true);
	}
		
	
	// Update is called once per frame
	void Update () {
		Filter (register_username);
		Filter (login_username);

		loginMessageText.text = loginMessage;
		registerMessageText.text = registerMessege;

		register_login.interactable = !isRegistering;
		register_confirm.interactable = !isRegistering;

		login_cencel.interactable = !isLogingIn;
		login_login.interactable = !isLogingIn;
	}

	void Filter(InputField inputField){
		//print (inputField.text);
		for(int i=0; i<perbodenString.Length; i++){
			if(inputField.text.Contains(perbodenString[i])){
				try{
					inputField.text = inputField.text.Replace(perbodenString[i],"");
					break;
				}catch{}
			}
		}
	}

	public void Toogle(bool condition){
		login.SetActive (!condition);
		register.SetActive (condition);
	}

	public void Register(){

		if(!isRegistering && !isLogingIn){
			if(register_username.text == ""){
				registerMessege = "Username cannot empty!";
				ResetRegister();
				return;
			}else if(register_password.text == "" || register_passwordConfirm.text == ""){
				registerMessege = "Password cannot empty";
				ResetRegister();
				return;
			}else if (register_password.text != register_passwordConfirm.text) {
				registerMessege = "Password didn't match!";
				ResetRegister();
				return;
			}else if(register_password.text.Length < 6){
				registerMessege = "Password minimal 6 character";
				ResetRegister();
				return;
			}
		}

		StartCoroutine(RegisterPlayer());
	}

	public IEnumerator RegisterPlayer(){
		print ("Registering");
		isRegistering = true;

		string url = GameData.instance.databseConnectorUrl + "register.php?name=" + register_username.text + "&pass=" + register_password.text;

		WWW www = new WWW (url);

		yield return www;

		if (www.text == "") {
			ResetRegister();
			registerMessege = www.error;
			print("Register error!");
		}

		if(www.text.Contains("exist")){
			ResetRegister();
			registerMessege = "Username is not availeble!";
		}else{
			registerMessege = "Register succes!";
			login_username.text = register_username.text;
			ResetRegister();
			Toogle(false);
		}

		isRegistering = false;

		print (registerMessege);
	}

	void ResetRegister(){
		isRegistering = false;
		register_username.text = "";
		register_password.text = "";
		register_passwordConfirm.text = "";
	}

	public void PlayerLogin(){
		if(!isLogingIn && !isRegistering){
			if (login_username.text == "") {
				loginMessage = "Username cannot empty!";
				return;
			}else if(login_password.text == ""){
				loginMessage = "Password cannot empty!";
				return;
			}
		}

		StartCoroutine(LoginPlayer());

	}

	public IEnumerator LoginPlayer(){
		print ("Loging in");
		isLogingIn = true;
		
		string url = GameData.instance.databseConnectorUrl + "login.php?name=" + login_username.text + "&pass=" + login_password.text;
        WWW www = new WWW (url);
		
		yield return www;

		if(www.text.Contains("success")){
			LoginSuccess();
		}else{
			loginMessage = "Username or password wrong!";
			ResetLogin();
		}

		isLogingIn = false;
	}

	void ResetLogin(){
		isLogingIn = false;
		login_password.text = "";
		login_username.text = "";
	}

	void LoginSuccess(){
		GameData.instance.SaveUsernameAndPassword(login_username.text, login_password.text);
		loginMessage = "";
		print("login success");
	}

}
