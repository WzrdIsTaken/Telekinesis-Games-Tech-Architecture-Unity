using UnityEngine;
using UnityEditor;

// Drawer for the RequireInterface attribute | Credit: Patryk Galach (https://bit.ly/3oSjPMU)

[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    // Overrides GUI drawing for the attribute

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if this is reference type property
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Get attribute parameters
            RequireInterfaceAttribute requiredAttribute = attribute as RequireInterfaceAttribute;

            // Begin drawing property field
            EditorGUI.BeginProperty(position, label, property);

            // Draw property field
            Object reference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.RequiredType, true);

            if (reference is null)  // Allows for the drag / dropping of objects in the inspector | Credit: Joel (https://bit.ly/3n01qv9)
            {
                Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);
                if (obj is GameObject g) reference = g.GetComponent(requiredAttribute.RequiredType);
            }

            // Finish drawing property field
            EditorGUI.EndProperty();
        }
        else
        {
            // If field is not reference, show error message

            // Save previous color and change GUI to red
            Color previousColor = GUI.color;
            GUI.color = Color.red;

            // Display label with error message
            EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));

            // Revert color change
            GUI.color = previousColor;
        }
    }
}

/*
    Ok dude I have literally no idea why this happens but for some unknown reason if this isn't here then 'RequireInterfaceAttribute' in this script cannot be found.
    However, if I then remove the RequireInterfaceAttribute class from its unique script then it cannot be found anywhere else. On top of this,I swear I haven't touched 
    it and it was working 5 minutes ago. Just Unity things.

    Update: Now it works fine again ??? Comment this back in if Unity forgets RequireInterfaceAttribute exists...

    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public System.Type RequiredType { get; private set; }

        public RequireInterfaceAttribute(System.Type type)
        {
           RequiredType = type;
        }
    }
*/