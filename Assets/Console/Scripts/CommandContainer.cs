using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandContainer : MonoBehaviour {

	private InConsole console;

    private void Awake() {
        console = InConsole.instance;
    }

    private void Start() {
        console.RegisterCommand("gameobject",Empty);
        console.RegisterSubCommand("gameobject","find",Find);
    }

    /* ALL COMMAND FUNCTIONS REQUIRE 2 STRING ARGUMENTS (arguments,command) */

    public void Empty(string arguments,string command){
        InConsole.Log("Nothing to run, this command possibly requires parameters.",InConsole.InType.Warning);
    }

    public void Find(string arguments,string command){
        InConsole.Log(arguments,InConsole.InType.None);
        GameObject obj = GameObject.Find(arguments);
        if(obj){
            InConsole.Log("Found this => " + obj.name,InConsole.InType.Log);
        }else{
            InConsole.Log("Could not find that gameobject.",InConsole.InType.Error);
        }
    }

    public void Disable(string arguments,string command){
        GameObject obj = GameObject.Find(arguments);
        if(obj){
            obj.SetActive(false);
        }else{
            InConsole.Log("Could not find that gameobject.",InConsole.InType.Error);
        }
    }


}