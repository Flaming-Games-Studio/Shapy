using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitializeUGS : MonoBehaviour
{

    public string environment = "development";  //"production" - for after launch

    async void Start()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
            AnalyticsService.Instance.StartDataCollection();
        }
        catch (Exception exception)
        {
            print(exception.Message);
        }
    }
}