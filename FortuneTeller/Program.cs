﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace FortuneTeller
{

    public class Globals
    {
        public List<string> namesList = new();
        public int numChanges = 0;
        public bool hasSaidOkay = false;
    }
    public class Program
    {
        static void Main()
        {
            Globals globalVar = new Globals();
            Console.WriteLine("Please enter your name: ");
            string name = Console.ReadLine();
            globalVar.namesList.Add(name);
            Start(name, globalVar);
        }

        static void Start(string name, Globals globalVar)
        {
            Console.WriteLine("\nOkay, " + name + ", what would you like to do?\n");
            if (globalVar.hasSaidOkay == false)
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
                    SaveLoadMenu(name, "joke", globalVar);
                }
                else if (resNum == 2)
                {
                    SaveLoadMenu(name, "fortune", globalVar);
                }
                else if (resNum == 3)
                {
                    NameChange(name, globalVar);

                }
                else if (resNum == 4)
                {
                    Console.WriteLine("\nGoodbye " + name + "!");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid response! Try again.");
                    Start(name, globalVar);
                }

            }
            else
            {
                Console.WriteLine("Invalid response! Try again.");
                Start(name, globalVar);
            }
        }

        static void SaveToFile(string fileName, string textToWrite)
        {
            if (!File.Exists(fileName))
            {
                StreamWriter file = File.CreateText(fileName);
                file.WriteLine(DateTime.Now.ToString() + ":");
                file.WriteLine(textToWrite);
                file.WriteLine("\n");
                file.Close();
            }
            else
            {
                StreamWriter file = File.AppendText(fileName);
                file.WriteLine(DateTime.Now.ToString() + ":");
                file.WriteLine(textToWrite);
                file.WriteLine("\n");
                file.Close();
            }
        }

        static void LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File " + fileName + " not found! Try saving first.\n");
            }
            else
            {
                Console.WriteLine("Listing contents of " + fileName + ":\n");
                try
                {
                    using (var file = new StreamReader(fileName))
                    {
                        Console.WriteLine(file.ReadToEnd());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }
        }

        static void DeleteFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File " + fileName + " not found! Try saving first.");
            }
            else
            {
                Console.WriteLine("Are you sure you would like to delete " + fileName + "?");
                string response = Console.ReadLine();
                if (response.StartsWith("y") || response.StartsWith("Y"))
                {
                    File.Delete(fileName);
                }
                else
                {
                    Console.WriteLine("Delete cancelled!");
                }
            }
        }

        static async void SaveLoadMenu(string name, string type, Globals globalVar)
        {
            Console.WriteLine("\nPlease choose from the below list of options, " + name + ":\n1. Generate a new " + type +"\n2. Display saved " + type + "s\n3. Delete saved " + type + "s\n4. Go back.");
            string apiResponse;
            string firstResponse = Console.ReadLine();
            if (int.TryParse(firstResponse, out int resNum) == true)
            {
                if (resNum == 1)
                {
                    if (type == "fortune")
                    {
                        try
                        {
                            apiResponse = await GetFortune();
                        }
                        catch (Exception ex)
                        {
                            apiResponse = "API call failed!";
                            Console.WriteLine(apiResponse + ex.Message);
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            apiResponse = await GetJoke();
                        }
                        catch (Exception ex)
                        {
                            apiResponse = "API call failed!";
                            Console.WriteLine(apiResponse + ex.Message);
                            throw;
                        }
                    }
                    Console.WriteLine("\nHere's your " + type +", " + name + ":\n");
                    Console.WriteLine(apiResponse);
                    Console.WriteLine("\nWould you like to save your " + type +"?");
                    firstResponse = Console.ReadLine();
                    if (firstResponse.StartsWith("y") || firstResponse.StartsWith("Y"))
                    {
                        if (type == "fortune")
                        {
                            SaveToFile("fortunelist.txt", apiResponse);
                        }
                        else
                        {
                            SaveToFile("jokelist.txt", apiResponse);
                        }
                        SaveLoadMenu(name, type, globalVar);
                    }
                    else
                    {
                        SaveLoadMenu(name, type, globalVar);
                    }
                }
                else if (resNum == 2)
                {
                    if (type == "fortune")
                    {
                        LoadFromFile("fortunelist.txt");
                    }
                    else
                    {
                        LoadFromFile("jokelist.txt");
                    }
                    SaveLoadMenu(name, type, globalVar);
                }
                else if (resNum == 3)
                {
                    if (type == "fortune")
                    {
                        DeleteFile("fortunelist.txt");
                    }
                    else
                    {
                        DeleteFile("jokelist.txt");
                    }
                    SaveLoadMenu(name, type, globalVar);
                }
                else
                {
                    Start(name, globalVar);
                }
            }
            else
            {
                Console.WriteLine("Invalid response! Try again.");
                SaveLoadMenu(name, type, globalVar);
            }
        }


        static void NameChange(string name, Globals globalVar)
        {
            if (globalVar.numChanges < 5 && globalVar.hasSaidOkay == false)
            {
                string previousName = name;
                Console.WriteLine("\nWhat would you like your new name to be? Enter code 0451 to access the secret menu.");
                name = Console.ReadLine();
                if (name == "0451")
                {
                    globalVar.hasSaidOkay = true;
                    NameChange(previousName, globalVar);
                }
                else
                {
                    globalVar.numChanges++;
                    globalVar.namesList.Add(name);
                    if (globalVar.numChanges == 1)
                    {
                        Console.WriteLine("\nThanks, " + name + ", you've changed your name 1 time.");
                        Start(name, globalVar);
                    }
                    else
                    {
                        Console.WriteLine("\nThanks, " + name + ", you've changed your name " + globalVar.numChanges + " times.");
                        Start(name, globalVar);
                    }
                }
            }
            else if (globalVar.numChanges >= 5 || globalVar.hasSaidOkay == true)
            {
                if (globalVar.hasSaidOkay == false)
                {
                    Console.WriteLine("\n" + name + ", you've changed your name 5 times... Are you okay?");
                    string okayResponse = Console.ReadLine();
                    if (okayResponse == "y" || okayResponse == "yes" || okayResponse == "Yes" || okayResponse == "Yes." || okayResponse == "yes.")
                    {
                        globalVar.hasSaidOkay = true;
                        Console.WriteLine("\nOkay, just checking.\n");
                        NameChange(name, globalVar);
                    }
                    else
                    {
                        Console.WriteLine("\nSorry, please come back when you're feeling better.");
                        Start(name, globalVar);
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
                        globalVar.namesList.Add(name);
                        globalVar.numChanges++;
                        if (globalVar.numChanges == 1)
                        {
                            Console.WriteLine("\nThanks, " + name + ", you've changed your name 1 time.");
                        }
                        else
                        {
                            Console.WriteLine("\nThanks, " + name + ", you've changed your name " + globalVar.numChanges + " times.");
                        }
                        NameChange(name, globalVar);

                    }
                    else if (nameResponse.StartsWith("view") || nameResponse.StartsWith("View") || nameResponse.StartsWith("List") || nameResponse.StartsWith("list") || nameResponse.StartsWith("2"))
                    {
                        if (globalVar.numChanges == 0)
                        {
                            Console.WriteLine("\nHere's your list of " + (globalVar.numChanges + 1) + " name, " + name + ":");
                        }
                        else
                        {
                            Console.WriteLine("\nHere's your list of " + (globalVar.numChanges + 1) + " names, " + name + ":");
                        }
                        globalVar.namesList.ForEach(i => Console.WriteLine(i));
                        NameChange(name, globalVar);
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
                                globalVar.namesList.ForEach(i => namesFile.WriteLine(i));
                                namesFile.Close();
                                saveWorked = true;
                            }
                            else
                            {
                                StreamWriter namesFile = File.AppendText("nameslist.txt");
                                globalVar.namesList.ForEach(i => namesFile.WriteLine(i));
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
                        NameChange(name, globalVar);
                    }
                    else if (nameResponse.StartsWith("load") || nameResponse.StartsWith("Load") || nameResponse.StartsWith("4"))
                    {
                        if (!File.Exists("nameslist.txt"))
                        {
                            Console.WriteLine("File 'nameslist.txt' not found! Try saving first.\n");
                            NameChange(name, globalVar);
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
                            NameChange(name, globalVar);
                        }
                    }
                    else if (nameResponse.StartsWith("Delete") || nameResponse.StartsWith("delete") || nameResponse.StartsWith("5"))
                    {
                        if (!File.Exists("nameslist.txt"))
                        {
                            Console.WriteLine("File 'nameslist.txt' not found! Try saving first.\n");
                            NameChange(name, globalVar);
                        }
                        else
                        {
                            Console.WriteLine("Are you sure?");
                            nameResponse = Console.ReadLine();
                            if (nameResponse.StartsWith("y") || nameResponse.StartsWith("Y") || nameResponse.StartsWith("1"))
                            {
                                File.Delete("nameslist.txt");
                                NameChange(name, globalVar);
                            }
                            else
                            {
                                NameChange(name, globalVar);
                            }
                        }
                    }
                    else if (nameResponse.StartsWith("back") || nameResponse.StartsWith("Back") || nameResponse.StartsWith("6"))
                    {
                        Start(name, globalVar);
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input! Please enter 1, 2, 3, or 4. You can also enter 'new','list','save', 'load', 'delete', or 'back'.");
                        NameChange(name, globalVar);
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
