using System;
using UnityEngine;

// Allows for easy and nice looking optional variables within the inspector | Credit: aarthificial (https://bit.ly/3iTMCg7)

/**
    Usage:
         public/[SerializeField] Optional<variableType> variableName - Create an optional variable of type variableType
         public/[SerializeField] Optional<variableType> variableName = new Optional<variableType>(intialValue) - Create an optional variable of type variableType with the value of defaultValue
**/

[Serializable]
public class Optional<T>
{
    public bool Enabled => enabled;
    public T Value => value;

    [SerializeField] bool enabled;
    [SerializeField] T value;

    public Optional(T intialValue)
    {
        enabled = true;
        value = intialValue;
    }
}