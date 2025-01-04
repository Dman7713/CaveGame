using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OutlineBoxGenerator))]
public class OutlineBoxGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector for all fields
        DrawDefaultInspector();

        // Get reference to the OutlineBoxGenerator instance
        OutlineBoxGenerator generator = (OutlineBoxGenerator)target;

        // Add a button that will call GenerateBox() when clicked
        if (GUILayout.Button("Generate Box"))
        {
            generator.GenerateBox(); // Call the GenerateBox method from the OutlineBoxGenerator
        }
    }
}
