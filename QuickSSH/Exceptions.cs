public class Exceptions
{
    public class InvalidClientException : Exception {} // Thrown when the specified client path is invalid
    public class SaveFileCreationException : Exception {} // Thrown when the configuration save file cannot be created
    public class JsonObjectNullException : Exception {} // Thrown when the deserialized JSON object is null
    public class ConnectionNotFoundException : Exception {} // Thrown when a specified connection is not found in the configuration

}