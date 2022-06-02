using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FortuneTeller
{

    static class Globals
    {
        public static List<string> namesList = new();
        public static int numChanges = 0;
        public static bool hasSaidOkay = false;
    }
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Please enter your name: ");
            string name = Console.ReadLine();
            Globals.namesList.Add(name);
            Start(name);
        }

        static async void Start(string name)
        {
            Console.WriteLine("\nOkay, " + name + ", what would you like me to do?\n");
            if (Globals.hasSaidOkay == false)
            {
                Console.WriteLine("1. Tell me a joke.\n2. Tell me my fortune.\n3. Change my name.\n4. Exit");
            }
            else
            {
                Console.WriteLine("1. Tell me a joke.\n2. Tell me my fortune.\n3. Change my name, or view a list of them.\n4. Exit");
            }
            String firstResponse = Console.ReadLine();
            if (int.TryParse(firstResponse, out int resNum) == true)
            {
                if (resNum == 1)
                {
                    //Tell a joke by calling the GetJoke method
                    string joke;
                    try
                    {
                        joke = await GetJoke();
                    }
                    catch (Exception ex)
                    {
                        joke = "API call failed!";
                        Console.WriteLine(joke + ex.Message);
                        throw;
                    }
                    Console.WriteLine("\nHere's your joke, " + name + ":\n");
                    Console.WriteLine(joke);
                    Start(name);
                }
                else if (resNum == 2)
                {
                    //Give a fortune by calling the GetFortune method
                    string fortune;
                    try
                    {
                        fortune = await GetFortune();
                    }
                    catch (Exception ex)
                    {
                        fortune = "API call failed!";
                        Console.WriteLine(fortune + ex.Message);
                        throw;
                    }
                    Console.WriteLine("\nHere's your fortune, " + name + ":\n");
                    Console.WriteLine(fortune);
                    Start(name);
                }
                else if (resNum == 3)
                {
                    NameChange(name);

                }
                else if (resNum == 4)
                {
                    Console.WriteLine("\nGoodbye " + name + "!");
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

        static void NameChange(string name)
        {
            if (Globals.numChanges < 5)
            {
                Console.WriteLine("\nWhat would you like your new name to be?");
                name = Console.ReadLine();
                Globals.namesList.Add(name);
                Globals.numChanges++;
                if (Globals.numChanges == 1)
                {
                    Console.WriteLine("\nThanks, " + name + ", you've changed your name 1 time.");
                    Start(name);
                }
                else
                {
                    Console.WriteLine("\nThanks, " + name + ", you've changed your name " + Globals.numChanges + " times.");
                    Start(name);
                }
            }
            else if (Globals.numChanges >= 5)
            {
                if (Globals.hasSaidOkay == false)
                {
                    Console.WriteLine("\n" + name + "...You've changed your name 5 times...\nAre you okay?");
                    string okayResponse = Console.ReadLine();
                    if (okayResponse == "y" || okayResponse == "yes" || okayResponse == "Yes" || okayResponse == "Yes." || okayResponse == "yes.")
                    {
                        Globals.hasSaidOkay = true;
                        Console.WriteLine("\nOkay, just making sure...\n");
                        NameChange(name);
                    }
                    else
                    {
                        Console.WriteLine("\nWell, I'm just a computer program. Please come back when you're feeling better.");
                        Start(name);
                    }
                }
                else
                {
                    Console.WriteLine("\nWould you like to view the names you've entered, or enter a new one?");
                    string nameResponse = Console.ReadLine();
                    if (nameResponse.StartsWith("new") || nameResponse.StartsWith("New") || nameResponse.StartsWith("enter") || nameResponse.StartsWith("Enter"))
                    {
                        Console.WriteLine("\nWhat would you like your new name to be?");
                        name = Console.ReadLine();
                        Globals.namesList.Add(name);
                        Globals.numChanges++;
                        Console.WriteLine("\nThanks, " + name + ", you've changed your name " + Globals.numChanges + " times.");
                        Start(name);

                    }
                    else if (nameResponse.StartsWith("view") || nameResponse.StartsWith("View") || nameResponse.StartsWith("List") || nameResponse.StartsWith("list"))
                    {
                        Console.WriteLine("\nHere's your list of " + (Globals.numChanges + 1) + " names, " + name + ":");
                        Globals.namesList.ForEach(i => Console.WriteLine(i));
                        Start(name);
                    }
                    else
                    {
                        Console.WriteLine("\nI didn't understand! Please try using simple words or phrases like 'enter a new name' or 'view'.");
                        NameChange(name);
                    }
                }
            }
        }
        static async Task<String> GetFortune()
        {
            var baseAddress = "https://api.justyy.workers.dev/api/fortune";
            using var client = new HttpClient();
            using var response = client.GetAsync(baseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                resultString = resultString.Replace("\"", "");
                resultString = resultString.Replace("\\n", "");
                resultString = resultString.Replace("\\t", "");
                return resultString;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return "API Call Failed!";
            }
        }

        static async Task<String> GetJoke()
        {
            var baseAddress = "https://v2.jokeapi.dev/joke/Programming?blacklistFlags=nsfw,religious,political,racist,sexist,explicit&format=txt";
            using var client = new HttpClient();
            using var response = client.GetAsync(baseAddress).Result;
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
