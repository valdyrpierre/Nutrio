using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class machinelearning
{
    // this will keep the selected food text from the user
    public string userInput;

    // this will keep the generated meal plan
    public string mealPlan;

    // this is the simple file that acts like storage for now
    public string dataBaseFilePath;

    public machinelearning()
    {
        // default values so null does not break later logic
        userInput = string.Empty;
        mealPlan = string.Empty;
        dataBaseFilePath = "food_choices_db.txt";
    }

    public void setUserInput(string input)
    {
        // the user input would be the food that the user clicked on
        userInput = (input ?? string.Empty).Trim();
    }

    public string getUserInput()
    {
        // whatever the user clicked will be returned here
        return userInput;
    }

    public void sendUserInputToDataBase()
    {
        // save user input into a local text file as a simple database
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return;
        }

        string lineToSave = DateTime.Now.ToString("s") + " | " + userInput;
        File.AppendAllText(dataBaseFilePath, lineToSave + Environment.NewLine);
    }

    public string setMealPlan()
    {
        // generate meal plan using current user input plus recent saved choices
        if (string.IsNullOrWhiteSpace(userInput))
        {
            mealPlan = "No food selected yet.";
            return mealPlan;
        }

        List<string> allFoods = new List<string>();

        // include current selection first
        allFoods.AddRange(splitFoods(userInput));

        // include latest selections from saved file if it exists
        if (File.Exists(dataBaseFilePath))
        {
            string[] lines = File.ReadAllLines(dataBaseFilePath);
            foreach (string line in lines.Reverse().Take(20))
            {
                string parsed = parseFoodFromSavedLine(line);
                if (!string.IsNullOrWhiteSpace(parsed))
                {
                    allFoods.AddRange(splitFoods(parsed));
                }
            }
        }

        // remove duplicates and blanks
        List<string> uniqueFoods = allFoods
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (uniqueFoods.Count == 0)
        {
            mealPlan = "No food selected yet.";
            return mealPlan;
        }

        // basic meal split
        string breakfast = pickFood(uniqueFoods, 0, "Oatmeal");
        string lunch = pickFood(uniqueFoods, 1, "Chicken breast");
        string dinner = pickFood(uniqueFoods, 2, "Salmon");
        string snack = pickFood(uniqueFoods, 3, "Banana");

        mealPlan = "Breakfast: " + breakfast
            + " | Lunch: " + lunch
            + " | Dinner: " + dinner
            + " | Snack: " + snack;

        return mealPlan;
    }

    public string getMealPlan()
    {
        // returns the current meal plan string
        return mealPlan;
    }

    public string[] getSavedUserInputs()
    {
        // returns all saved lines from the local file database
        if (!File.Exists(dataBaseFilePath))
        {
            return Array.Empty<string>();
        }

        return File.ReadAllLines(dataBaseFilePath);
    }

    public void clearDataBase()
    {
        // clear saved file data
        if (File.Exists(dataBaseFilePath))
        {
            File.Delete(dataBaseFilePath);
        }
    }

    private string[] splitFoods(string foodsText)
    {
        // support comma-separated input like: chicken, rice, salmon
        return foodsText
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private string parseFoodFromSavedLine(string line)
    {
        // saved format: yyyy-mm-ddThh:mm:ss | user input
        int separatorIndex = line.IndexOf(" | ", StringComparison.Ordinal);
        if (separatorIndex < 0 || separatorIndex + 3 >= line.Length)
        {
            return string.Empty;
        }

        return line.Substring(separatorIndex + 3).Trim();
    }

    private string pickFood(List<string> foods, int index, string fallback)
    {
        // picks by index if available, otherwise fallback
        if (foods.Count > index)
        {
            return foods[index];
        }

        return fallback;
    }
}