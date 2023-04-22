using System.Text.Json.Serialization;

namespace dotnet_rpg.Models
{
    // Configure the JSON Converter to show keys instead of values of the RpgClass in Swagger.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    // An enum is a special "class" that represents a group of constants (unchangeable/read-only variables).
    public enum RpgClass
    {
        Knight = 1,
        Mage = 2,
        Cleric = 3
    }
}