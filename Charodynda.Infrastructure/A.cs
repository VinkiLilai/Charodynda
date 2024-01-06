using Newtonsoft.Json;

namespace Charodynda.Infrastructure;

public class Car
{
    [JsonProperty("Model")]
    public string Model { get; }
    [JsonProperty("Colour")]
    public string Colour { get; }

    public Car(string model, string colour)
    {
        Model = model;
        Colour = colour;
    }
}

public static class A
{
    public static void Program()
    {
        Console.WriteLine(JsonConvert.SerializeObject(new int[] { 1, 2, 3 }));
        var serializedArrayOfCars = JsonConvert.SerializeObject(new Car[]
        {
            new Car("Lada Sedan", "Baklazhan"), new Car("Toyota Corolla", "Silver")
        });

        var deserialisedCarArray = JsonConvert.DeserializeObject<Car[]>(serializedArrayOfCars);
        
    }
}