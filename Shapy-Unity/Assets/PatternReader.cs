using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PatternReader : MonoBehaviour
{
    public string csvFileName = "/Patterns/pattern1.csv";
    private bool[,] gridArray;

    void Start()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        string filePath = Application.dataPath + csvFileName;

        if (File.Exists(filePath))
        {
            // Read all lines from the CSV file
            string[] lines = File.ReadAllLines(filePath);

            // Initialize the 2D bool array
            gridArray = new bool[lines.Length, lines[0].Split(',').Length];

            for (int i = 0; i < lines.Length; i++)
            {
                // Split the CSV row into individual values
                string[] values = lines[i].Split(',');

                for (int j = 0; j < values.Length; j++)
                {
                    // Parse the string values to bool and assign to the array
                    if (bool.TryParse(values[j], out bool result))
                    {
                        gridArray[i, j] = result;
                    }
                    else
                    {
                        Debug.LogError("Error parsing CSV value at position (" + i + ", " + j + ")");
                    }
                }
            }

            // Now the gridArray contains the loaded data
            Debug.Log("CSV Loaded Successfully");

            // Optionally, you can use the gridArray in your game logic
            // Example: Access the value at position (2, 3)
            bool valueAtPosition23 = gridArray[2, 3];
            Debug.Log("Value at position (2, 3): " + valueAtPosition23);
        }
        else
        {
            Debug.LogError("CSV file not found at path: " + filePath);
        }
    }
}

