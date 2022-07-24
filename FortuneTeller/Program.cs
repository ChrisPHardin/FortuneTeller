using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

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
            Console.WriteLine("\nOkay, " + name + ", what would you like to do?\n");
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
            if (Globals.numChanges < 5 && Globals.hasSaidOkay == false)
            {
                string previousName = name;
                Console.WriteLine("\nWhat would you like your new name to be? Enter code 0451 to access the secret menu.");
                name = Console.ReadLine();
                if (name == "0451")
                {
                    Globals.hasSaidOkay = true;
                    NameChange(previousName);
                }
                else
                {
                    Globals.numChanges++;
                    Globals.namesList.Add(name);
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
            }
            else if (Globals.numChanges >= 5 || Globals.hasSaidOkay == true)
            {
                if (Globals.hasSaidOkay == false)
                {
                    Console.WriteLine("\n" + name + ", you've changed your name 5 times... Are you okay?");
                    string okayResponse = Console.ReadLine();
                    if (okayResponse == "y" || okayResponse == "yes" || okayResponse == "Yes" || okayResponse == "Yes." || okayResponse == "yes.")
                    {
                        Globals.hasSaidOkay = true;
                        Console.WriteLine("\nOkay, just checking.\n");
                        NameChange(name);
                    }
                    else
                    {
                        Console.WriteLine("\nSorry, please come back when you're feeling better.");
                        Start(name);
                    }
                }
                else
                {
                    Console.WriteLine("\nPlease choose from the below list of options, " + name + ".\n1. Enter a new name\n2. View list of names entered this session\n3. Save names from this session to nameslist.txt\n4. Load list of names from nameslist.txt\n5. Delete nameslist.txt\n6. Go Back");
                    string nameResponse = Console.ReadLine();
                    if (nameResponse.StartsWith("new") || nameResponse.StartsWith("New") || nameResponse.StartsWith("enter") || nameResponse.StartsWith("Enter") || nameResponse.StartsWith("1"))
                    {
                        Console.WriteLine("\nWhat would you like your new name to be?");
                        name = Console.ReadLine();
                        Globals.namesList.Add(name);
                        Globals.numChanges++;
                        if (Globals.numChanges == 1)
                        {
                            Console.WriteLine("\nThanks, " + name + ", you've changed your name 1 time.");
                        }
                        else
                        {
                            Console.WriteLine("\nThanks, " + name + ", you've changed your name " + Globals.numChanges + " times.");
                        }
                        NameChange(name);

                    }
                    else if (nameResponse.StartsWith("view") || nameResponse.StartsWith("View") || nameResponse.StartsWith("List") || nameResponse.StartsWith("list") || nameResponse.StartsWith("2"))
                    {
                        if (Globals.numChanges == 0)
                        {
                            Console.WriteLine("\nHere's your list of " + (Globals.numChanges + 1) + " name, " + name + ":");
                        }
                        else
                        {
                            Console.WriteLine("\nHere's your list of " + (Globals.numChanges + 1) + " names, " + name + ":");
                        }
                        Globals.namesList.ForEach(i => Console.WriteLine(i));
                        NameChange(name);
                    }
                    else if (nameResponse.StartsWith("save") || nameResponse.StartsWith("Save") || nameResponse.StartsWith("3"))
                    {
                        Boolean saveWorked = false;
                        //var filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location,"nameslist.txt");
                        try
                        {
                            if (!File.Exists("nameslist.txt"))
                            {
                                StreamWriter namesFile = File.CreateText("nameslist.txt");
                                Globals.namesList.ForEach(i => namesFile.WriteLine(i));
                                namesFile.Close();
                                saveWorked = true;
                            }
                            else
                            {
                                StreamWriter namesFile = File.AppendText("nameslist.txt");
                                Globals.namesList.ForEach(i => namesFile.WriteLine(i));
                                namesFile.Close();
                                saveWorked = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: " + e.Message);
                        }
                        finally
                        {
                            if (saveWorked){Console.WriteLine("Saved to nameslist.txt!");}
                        }
                        NameChange(name);
                    }
                    else if (nameResponse.StartsWith("load") || nameResponse.StartsWith("Load") || nameResponse.StartsWith("4"))
                    {
                        if (!File.Exists("nameslist.txt"))
                        {
                            Console.WriteLine("File 'nameslist.txt' not found! Try saving first.\n");
                            NameChange(name);
                        }
                        else
                        {
                            Console.WriteLine("Listing names from file nameslist.txt:");
                            try
                            {
                                using (var namesFile = new StreamReader("nameslist.txt"))
                                {
                                    Console.WriteLine(namesFile.ReadToEnd());
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception: " + e.Message);
                            }
                            NameChange(name);
                        }
                    }
                    else if (nameResponse.StartsWith("Delete") || nameResponse.StartsWith("delete") || nameResponse.StartsWith("5"))
                    {
                        if (!File.Exists("nameslist.txt"))
                        {
                            Console.WriteLine("File 'nameslist.txt' not found! Try saving first.\n");
                            NameChange(name);
                        }
                        else
                        {
                            Console.WriteLine("Are you sure?");
                            nameResponse = Console.ReadLine();
                            if (nameResponse.StartsWith("y") || nameResponse.StartsWith("Y") || nameResponse.StartsWith("1"))
                            {
                                File.Delete("nameslist.txt");
                                NameChange(name);
                            }
                            else
                            {
                                NameChange(name);
                            }
                        }
                    }
                    else if (nameResponse.StartsWith("back") || nameResponse.StartsWith("Back") || nameResponse.StartsWith("6"))
                    {
                        Start(name);
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input! Please enter 1, 2, 3, or 4. You can also enter 'new','list','save', 'load', 'delete', or 'back'.");
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
                Console.WriteLine("Received error response: ", (int)response.StatusCode, response.ReasonPhrase);
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
                Console.WriteLine("Received error response: ", (int)response.StatusCode, response.ReasonPhrase);
                return "API Call Failed!";
            }
        }
    }
}
