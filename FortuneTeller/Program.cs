﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FortuneTeller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your name: ");
            string name = Console.ReadLine();
            Start(name);
        }

        static async void Start(string name)
        {
            Console.WriteLine("Okay, " + name + ", what would you like to do?");
            Console.WriteLine("1. Tell me a joke.\n2. Tell me my fortune.\n3. Change my name.\n4. Exit");
            String firstResponse = Console.ReadLine();
            if (int.TryParse(firstResponse, out int resNum) == true)
            {
                if (resNum == 1)
                {
                    // Do something here.
                    Console.WriteLine("You. You're the joke.");
                    Start(name);
                }
                else if (resNum == 2)
                {
                    // Do something here.
                    string fortune;
                    try
                    { 
                        fortune = await GetFortune();
                    }
                    catch (Exception ex)
                    {
                        fortune = "API call failed!";
                    }
                    Console.WriteLine(fortune);
                    Start(name);
                }
                else if (resNum == 3)
                {
                    Console.WriteLine("What would you like your new name to be?");
                    name = Console.ReadLine();
                    Start(name);

                }
                else if (resNum == 4)
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid response! Try again.");
                    Start(name);
                }

            }
            else
            {
                Console.WriteLine("Invalid response! Try again.");
                Start(name);
            }
        }

        static async Task<string> GetFortune()
        {
            var baseAddress = "https://api.justyy.workers.dev/api/fortune";
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(baseAddress).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        return resultString;
                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                        return "API Call Failed!";
                    }
                }
            }
        }
    }
}
