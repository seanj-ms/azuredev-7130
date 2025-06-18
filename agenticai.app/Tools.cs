using AutoGen.Core;

namespace AgenticAI.App;

public partial class Tools
{
    [Function(
        functionName: "UpperCase",
        description: "Converts the input string to uppercase.")]
    public async Task<string> UpperCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        }

        // Simulate some processing delay
        await Task.Delay(100);

        // Convert the input string to uppercase
        return input.ToUpperInvariant();
    }

    [Function(
        functionName: "ConcatenateStrings",
        description: "Concatenates an array of strings with a specified separator.")]
    public async Task<string> ConcatenateStrings(string[] inputs, string separator = ", ")
    {
        if (inputs == null || inputs.Length == 0)
        {
            throw new ArgumentException("Input array cannot be null or empty.", nameof(inputs));
        }

        // Simulate some processing delay
        await Task.Delay(100);

        // Concatenate the strings with the specified separator
        return string.Join(separator, inputs);
    }

    [Function(
        functionName: "CalculateTax",
        description: "Calculates the tax for a given amount based on the country.")]
    public async Task<string> CalculateTax(
        decimal amount,
        string country = "USA")
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        var countryTaxRates = new Dictionary<string, decimal>
        {
            { "USA", 0.2m },
            { "UK", 0.2m },
            { "Germany", 0.19m },
            { "France", 0.2m }
        };

        // Simulate some processing delay
        await Task.Delay(100);

        if (!countryTaxRates.TryGetValue(country, out var effectiveTaxRate))
        {
            throw new ArgumentException($"Tax rate for country '{country}' is not defined.", nameof(country));
        }

        // Calculate the tax based on the amount and the effective tax rate
        var tax = amount * effectiveTaxRate;

        return $"The tax for an amount of {amount:C} in {country} is {tax:C}.";
    }
}