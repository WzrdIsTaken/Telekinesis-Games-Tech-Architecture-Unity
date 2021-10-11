using System;
using UnityEngine;

// Allows for Interfaces to be viewed in the Inspector. Credit | Patryk Galach (https://bit.ly/3oSjPMU)

public class RequireInterfaceAttribute : PropertyAttribute
{
    public Type RequiredType { get; private set; }

    public RequireInterfaceAttribute(Type type)
    {
        RequiredType = type;
    }
}