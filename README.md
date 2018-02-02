# InConsole

InConsole is an in-game console that you can use to debug your code or even input commands. It's easy to use and with a command system, you can register commands and sub-commands even at run time.

  - Debug your code
  - Register your own commands
  - Drag n' drop console prefab
  - Design you own console if you want
 
### Installation

The whole console is included in a .unitypackage file but you can take a look at the code yourself.

### Default Commands

| Command | Action |
| ------- | ------ |
| help | Displays a list of ALL the available commands |
| log.length | Displays the total character count of the console |
| log.clear | Clears the console text |
| print | Print the inputed text |

### Registering Commands

The function to be executed when creating a new command needs two strings as parameters -input parameters and the whole ipnut-

Example:
```
public void CommandFunction(string parameters,string command){
    //Do stuff here
}

public void SubCommandFunction(string parameters,string command){
    //Do stuff here
}
```

You can register a new command using:
```
InConsole.instance.RegisterCommand(commandName,CommandFunction);
```

Or a new sub-command using:
```
InConsole.instance.RegisterSubCommand(baseCommandName,subCommandName,CommandFunction);
```


### Development

Want to contribute? Great!
Make sure to let me know if you have any suggestions!


License
----

MIT


**Free Software, Hell Yeah!**
