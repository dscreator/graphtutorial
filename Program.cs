using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using GraphTutorial;

namespace graphtutorial
{
    class Program
    {
        static IConfigurationRoot LoadAppSettings()
            {
                var appConfig = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();

                // Check for required settings
                if (string.IsNullOrEmpty(appConfig["appId"]) ||
                    // Make sure there's at least one value in the scopes array
                    string.IsNullOrEmpty(appConfig["scopes:0"]))
                {
                    return null;
                }

                return appConfig;
            }
        
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Core Tutorial for Graph API");

            var appConfig = LoadAppSettings();

            if (appConfig == null)
            {
                Console.WriteLine("Missing or invalid appsettings.json...exiting");
                return;
            }

            var appId = appConfig["appId"];
            var scopes = appConfig.GetSection("scopes").Get<string[]>();

            // Initialize the auth provider with values from appsettings.json
            var authProvider = new DeviceCodeAuthProvider(appId, scopes);

            // Request a token to sign in the user
            var accessToken = authProvider.GetAccessToken().Result;
//***********************************************************************

            int choice = -1;

            while (choice != 0)
            {
                Console.WriteLine("0. To exit");
                Console.WriteLine("1. Display access token");
                Console.WriteLine("2. To exit");
                choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 0:
                        break;
                    case 1:
                        //display access token
                        Console.WriteLine($"Access token: {accessToken}\n");

                        break;
                    case 2:
                        // display calendar events
                        Console.WriteLine("The Calendar events are: ");
                        break;


                }

            }
            

        }
    }
}
