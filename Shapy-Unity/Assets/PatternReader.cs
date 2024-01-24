using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class PatternReader : MonoBehaviour
{
    public string csvFileName = "/Patterns/pattern1.csv";
    private bool[,] gridArray;
    public GameObject prefab;

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
            CreateGrid();
        }
        else
        {
            Debug.LogError("CSV file not found at path: " + filePath);
        }
    }

    void CreateGrid()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        int _column = gridArray.GetLength(0);
        int _row = gridArray.GetLength(1);
        print(_row + " i " + _column);

        float cellWidth = screenSize.x / _column;  // Corrected order
        float cellHeight = screenSize.y / _row;    // Corrected order

        for (int col = 0; col < _column; col++)
        {
            for (int row = 0; row < _row; row++)
            {
                if (!gridArray[col, row]) continue;  // Adjusted indices and used '!' for clarity

                // Calculate the center of each grid cell
                float centerX = col * cellWidth + cellWidth / 2;
                float centerY = row * cellHeight + cellHeight / 2;

                // Convert screen coordinates to world coordinates
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(centerX, centerY, 0));
                worldPosition.z = 0;

                // Optionally, instantiate an object at the center of each grid cell
                // For example, you can instantiate a prefab or perform other actions
                Instantiate(prefab, worldPosition, Quaternion.identity);

                Debug.Log("Center of cell (" + col + ", " + row + "): " + worldPosition);
            }
        }
    }


    //void CreateGrid()
    //{
    //    Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    //    int _column = gridArray.GetLength(0);
    //    int _row = gridArray.GetLength(1);
    //    print(_row + " i " + _column);

    //    float cellWidth = screenSize.x / _row;
    //    float cellHeight = screenSize.y / _column;

    //    for (int col = 0; col < _column; col++)
    //    {
    //        for (int row = 0; row < _row; row++)
    //        {
    //            if (gridArray[row, col] == false) continue;
    //            // Calculate the center of each grid cell
    //            float centerX = col * cellWidth + cellWidth / 2;
    //            float centerY = row * cellHeight + cellHeight / 2;

    //            // Convert screen coordinates to world coordinates
    //            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(centerX, centerY, 0));
    //            worldPosition.z = 0;

    //            // Optionally, instantiate an object at the center of each grid cell
    //            // For example, you can instantiate a prefab or perform other actions
    //            Instantiate(prefab, worldPosition, Quaternion.identity);

    //            Debug.Log("Center of cell (" + row + ", " + col + "): " + worldPosition);
    //        }
    //    }
    //}
}

