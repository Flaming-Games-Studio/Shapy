using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System.Linq;

public class PatternReader : MonoBehaviour
{
    public TextAsset[] patterns;
    private bool[,] gridArray;
    private List<Vector3> freeCells = new List<Vector3>();
    public GameObject prefab;
    public GameObject ball;
    public int maxBalls;
    private int ballCount;

    void Start()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        if (patterns != null)
        {
            // Load CSV content from TextAsset
            int ran = Random.Range(0, patterns.Length);
            string csvText = patterns[ran].text;
            print(patterns[ran].name);

            // Split CSV content into rows
            string[] rowsDirty = csvText.Split('\n');
            string[] columns = rowsDirty[0].Split(',');
            List<string> cc = rowsDirty.ToList<string>();
            cc.Remove("");
            string[] rows = cc.ToArray();
            gridArray = new bool[rows.Length, columns.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                print(rows[i]);
                // Split the CSV row into individual values
                string[] values = rows[i].Split(',');

                for (int j = 0; j < values.Length; j++)
                {
                    // Parse the string values to bool and assign to the array
                    if (bool.TryParse(values[j], out bool result))
                    {
                        gridArray[i, j] = result;
                    }
                    else
                    {
                        Debug.LogWarning("Error parsing CSV value at position (" + i + ", " + j + ")");
                    }
                }
            }

            // Now the gridArray contains the loaded data
            Debug.Log("CSV Loaded Successfully");

            // Optionally, you can use the gridArray in your game logic
            // Example: Access the value at position (2, 3)
            CreateGrid();
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

                // Calculate the center of each grid cell
                float centerX = col * cellWidth + cellWidth / 2;
                float centerY = row * cellHeight + cellHeight / 2;

                // Convert screen coordinates to world coordinates
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(centerX, centerY, 0));
                worldPosition.z = 0;

                if (!gridArray[col, row])
                {
                    freeCells.Add(worldPosition);
                }
                else
                {
                    // Optionally, instantiate an object at the center of each grid cell
                    // For example, you can instantiate a prefab or perform other actions
                    Instantiate(prefab, worldPosition, Quaternion.identity);

                    //Debug.Log("Center of cell (" + col + ", " + row + "): " + worldPosition);
                }
            }
        }

        SpawnBalls();
    }

    private void SpawnBalls()
    {
        for (int i = 0; i < maxBalls - ballCount; i++)
        {
            int where = Random.Range(0, freeCells.Count);
            Instantiate(ball, freeCells[where], Quaternion.identity);
            freeCells.RemoveAt(where);
        }
    }

}

