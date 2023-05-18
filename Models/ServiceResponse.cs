namespace dotnet_rpg.Models
{
    // Goal: Return a wrapper object to the client with every service call. 
    /// Advantage: you can add additional information to the result like a success or exception message.
    /// The frontend is able to react to this additional info and read the additional data with the help for for example HTTP interceptors.
    /// Use the generics to use the correct types.
    /// T is the actual type of data we want to return.
    public class ServiceResponse<T>
    {
        // The actual data like the Character.
        public T? Data { get; set; }
        // Use this to tell the frontend that all went well.
        public bool IsSuccess { get; set; } = true;
        // Send a message to the frontend like the error message.
        public string Message { get; set; } = string.Empty;
    }
}