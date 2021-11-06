using UnityEngine;

// Holds some colours to make debugging / console messages clear

public static class DebugLogManager
{
    public enum OutputType { NOT_MY_JOB, HALF_TODO_HALF_NOT_MY_JOB };

    static bool outputMessages;

    // Selects the correct colour based on outputType and calls PrintMessage to handle output
    public static void Print(string content, OutputType outputType)
    {
        if (!outputMessages) return;

        switch (outputType)
        {
            case OutputType.NOT_MY_JOB:
                PrintMessage(content, ColorUtility.ToHtmlStringRGBA(Color.cyan));
                break;
            case OutputType.HALF_TODO_HALF_NOT_MY_JOB:
                PrintMessage(content, ColorUtility.ToHtmlStringRGBA(Color.yellow));
                break;
            default:
                Debug.LogError("OutputType of type " + outputType.ToString() + " does not exist!");
                break;
        }
    }

    // Takes in a message and a RGBA colour and outputs the message to the console in that colour
    static void PrintMessage(string content, string colour)
    {
        Debug.Log("<color=#" + colour + ">" + content + "</color>");
    }

    // Called from SettingsManager. Toggles whether messages are outputted
    public static void SetOutputMessages(bool _outputMessages)
    {
        outputMessages = _outputMessages;
    }
}