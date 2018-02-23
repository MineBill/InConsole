using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InConsole : MonoBehaviour {
	public enum InType{Log,Warning,Error,None}
	
	public static InConsole instance;

    /// <summary>
    /// Returns whether or not the console is active or not
    /// </summary>
    public static bool isOpen;

	public string consoleWindowName;
    public KeyCode hotkey = KeyCode.F1;
	public Text container;


	private int maxLogLength = 1500;
	private string stdPrefix = "#";

	private List<string> consoleListings = new List<string>();
	private Dictionary<string,CommandFunction> baseCommands = new Dictionary<string, CommandFunction>();
	private Dictionary<string,string> kidParentPair = new Dictionary<string,string>();
	private Dictionary<string,CommandFunction> subCommands = new Dictionary<string, CommandFunction>();

    private GameObject parent;
    private InputField inputField;

	public int GetMaxLength{get{return maxLogLength;}}

	private void Awake() {
        parent = transform.GetChild(0).gameObject;
        inputField = parent.transform.Find("InputPanel").Find("InputField").GetComponent<InputField>();
        parent.SetActive(false);
        if(instance != null)
        {
            LogError("Another InConsole is active but did not get the instance.");
        }
        else
        {
            instance = this;
        }
		if(container == null){
			container = GameObject.Find("Container").GetComponent<Text>();
		}

		RegisterCommand("print",ConsolePrint);
		RegisterCommand("help",Help);
		RegisterCommand("log",Empty);

		RegisterSubCommand("log","clear",ClearLog);
		RegisterSubCommand("log","length",LogLength);
	}

    private void Update()
    {
        isOpen = parent.active;
        if (UnityEngine.Input.GetKeyDown(hotkey))
        {
            parent.SetActive(!parent.active);
            inputField.Select();
            if (!inputField.isFocused)
            {
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
    }

	#region StaticMethods
	///Use this function to display a message to the console
	public static void Log(string content,InType logType){
		if(content == null || content == ""){
			content = " Error, string was empty.";
		}
		InConsole.instance.CreateLog(content,logType);
	}

	///Display the desired message on the console.
	public static void LogWarning(string content){
		if(content == null || content == ""){
			content = " Error, string was empty.";
		}
		InConsole.instance.CreateLog(content,InType.Warning);
	}

	///Display the desired message on the console.
	public static void LogError(string content){
		if(content == null || content == ""){
			content = " Error, string was empty.";
		}
		InConsole.instance.CreateLog(content,InType.Error);
	}

	///Display the desired message on the console.
	public static void LogNone(string content){
		if(content == null || content == ""){
			content = " Error, string was empty.";
		}
		InConsole.instance.CreateLog(content,InType.None);
	}

	///Display the desired message on the console.
	public static void LogSimple(string content){
		if(content == null || content == ""){
			content = " Error, string was empty.";
		}
		InConsole.instance.CreateLog(content,InType.Log);
	}
	#endregion

	#region Command Registering

	public delegate void CommandFunction(string[] wholeParameters,string wholeCommand);

	///This registers a new command to the console.If it's just a base command for sub commands then create an Empty function for it.
	public void RegisterCommand(string command,CommandFunction function){
		baseCommands.Add(command,function);
	}

	///This registers a sub command to the console. Make sure to include the base command then the sub command.
	public void RegisterSubCommand(string parentCommand,string subCommand,CommandFunction function){
		kidParentPair.Add(subCommand,parentCommand);
		subCommands.Add(subCommand,function);
	}
    #endregion

    #region InputAndListCreating
    public void Input(string command) {
        if (command == "" || command == null)
        {
            return;
        }

		string[] input = command.Split(new char[]{'.'});
		string baseCommand;
		string arguments;
		bool hasSubCmd = false;

		if(input.Length == 1){
			hasSubCmd = false;
			input = command.Split(new char[]{' '});
			baseCommand = input[0];
			arguments = (input.Length == 1)?"":input.Join(" ",0);

		}else{
			hasSubCmd = true;
			baseCommand = input[0];
			arguments = (input.Length == 1)?"":input[1];
		}

		LogSimple(command);
		
		CommandFunction cmdToRun;
		if(baseCommands.TryGetValue(baseCommand,out cmdToRun)){
			if(!hasSubCmd){
                string[] arr = arguments.Split(new char[] { ' ' });
				cmdToRun(arr,command);
			}else{
				if(arguments != ""){
					string[] argumentArray = arguments.Split(new char[]{' '});
					string subCmd = argumentArray[0];
                    string[] subArguments = 
                        (argumentArray.Length == 1) ? null : argumentArray.Join(" ", 1).Split(new char[] { ' '});
                    
					string parentCmd;
					if(kidParentPair.TryGetValue(subCmd,out parentCmd)){
						if(parentCmd == baseCommand){
							CommandFunction subCmdToRun;
							if(subCommands.TryGetValue(subCmd,out subCmdToRun)){
								subCmdToRun(subArguments,command);
							}
						}
					}else{
						LogError(string.Format("Sub-Command '{0}' could not be found. Use 'help' for help.",subCmd));
					}
				}else{
					LogError("Error");
				}
			}
		}else{
			LogError(string.Format("Command '{0}' could not be found. Use 'help' for help.",baseCommand));
		}
	}

	void CreateLog(string content,InType logType){
		string prefix = "";
		switch(logType){
			case InType.Log:
				prefix = " > ";
			break;
			case InType.Warning:
				prefix = " WARNING: ";
			break;
			case InType.Error:
				prefix = " ERROR: ";
			break;
			case InType.None:
				prefix = " ";
			break;
		}

		if(container.text.Length > maxLogLength){
			string[] containerSplit = container.text.Split(stdPrefix.ToCharArray());
			string newContainer = "";

			for (int i = 0; i < containerSplit.Length; i++)
			{
				containerSplit[i] = stdPrefix + containerSplit[i];
				if(i > containerSplit.Length/2){
					newContainer += containerSplit[i];
				}
			}

			container.text = newContainer;
		}

		container.text += "\n" + stdPrefix + prefix + content;
		consoleListings.Add(content);
	}
	#endregion

	#region InternalCommands
	public void ConsolePrint(string[] text,string command){
		if(text.Length == 0){
			Log("Nothing to print.",InType.Error);
		}else{
			Log(text.Join(" ",1),InType.Log);
		}
	}

    public void ClearLog(string[] parameters,string command){
		container.text = "Logger(experimental), 'help' for help ";
		consoleListings.Clear();
	}

    public void Help(string[] parameters,string command){
        Log("Available commands:",InType.Log);
        foreach (KeyValuePair<string,InConsole.CommandFunction> item in baseCommands)
        {
            Log(item.Key,InType.None);
			foreach (KeyValuePair<string,string> subItem in kidParentPair)
			{
				if(subItem.Value == item.Key){
					Log("--<" + subItem.Key,InType.None);
				}
			}
        }
    }

	public void LogLength(string[] arguments,string command){
        if(arguments == null){
            Log("Displayed text length: " + container.text.Length.ToString(),InType.Log);
        }else{
            string mainAttribute = arguments[0];
            string value = (arguments.Length == 1)?"": arguments[1];

            switch(mainAttribute){
                case "max":{
                    if(value == ""){
                        Log(maxLogLength.ToString(),InType.Log);
                    }else{
                        maxLogLength = int.Parse(value);
                        Log(maxLogLength.ToString(),InType.Log);
                    }
                    break;
                }
            }
        }
    }

	public void Empty(string[] x1,string x2){
		ConsolePrint(new string[] { "Empty command" }, x2);
	}
	#endregion

	string[] RemoveFirst(string[] input){
		string[] returnArr = new string[]{};
		for (int i = 0; i < input.Length; i++)
		{
			if(i > 1){
				returnArr[i-1] = input[i];
			}
		}
		return returnArr;
	}
	
}

public static class Extensions{
	public static string Join(this string[] array,string joint,int startIndex){
		string output = "";
		bool done = false;
        if(startIndex > array.Length)
        {
            return null;
        }
		for (int i = startIndex; i < array.Length; i++)
		{
			if(!done){
				output += array[i];
				done = true;
			}else{
				output += joint + array[i];
			}
		}
		return output;
	}

    public static string[] SplitWithOffset(this string input,char[] spliter,int offset)
    {
        string[] array = input.Split(spliter);
        string output = "";
        for (int i = offset; i < array.Length; i++)
        {
            output += array[i] + spliter;
        }
        return output.Split(spliter);
    }
}
