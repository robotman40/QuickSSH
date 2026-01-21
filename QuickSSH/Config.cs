using System.Text.Json;

public class Config
{
    private const string ConfigFileName = "quickssh_config.json"; // Name of the configuration file
    
    protected class SshConfig
    {
        /* Represents the structure of the SSH configuration JSON file */
        public string ssh_path { set; get; } = "ssh";
        public string sftp_path { set; get; } = "sftp";
        public string scp_path { set; get; } = "scp";
        public Dictionary<string, string> connections { set; get; } = new Dictionary<string, string>();
    }

    private static SshConfig LoadConfig()
    {
        /* Loads the configuration from the JSON file and returns an SshConfig object */
        
        string savePath = GetSavePath(); // Get the path to the configuration directory
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        if (!File.Exists(savePath)) // Check if the config file exists
        {
            throw new FileNotFoundException("Save file not found.");
        }

        string jsonString = File.ReadAllText(savePath); // Read the contents of the config file
        SshConfig jsonFile = JsonSerializer.Deserialize<SshConfig>(jsonString); // Deserialize JSON into SshConfig object

        if (jsonFile == null) // Check if deserialization was successful
        {
            throw new Exceptions.JsonObjectNullException();
        }
        
        return jsonFile;                                 
    }

    private static void UpdateConfig(SshConfig config)
    {
        /* Updates the configuration file with the provided SshConfig object */
        
        string jsonString = JsonSerializer.Serialize(config); // Serialize the updated config

        string savePath = GetSavePath(); // Get the path to the configuration directory
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        File.WriteAllText(savePath, jsonString); // Write the updated config back to the file
    }
    
    public static string GetSavePath()
    {
        /* Returns the path to the configuration directory */
        
        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        savePath = System.IO.Path.Combine(savePath, "QuickSSH");

        return savePath;
    }
        
        
    public static void CreateSaveFile()
    {
        /* Creates the configuration file if it does not already exist */
        
        string savePath = GetSavePath(); // Get the path to the configuration directory
        if (!Directory.Exists(savePath)) // Check if the directory exists
        {
            Directory.CreateDirectory(savePath);
        }
        
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        if (!File.Exists(savePath)) // Check if the config file exists
        {
            SshConfig emptyConfig = new SshConfig();
            string jsonString = JsonSerializer.Serialize(emptyConfig);
            File.WriteAllText(savePath, jsonString);
        }
    }
    
    public static Dictionary<string, string> LoadConnections()
    {
        /* Loads the connections from the configuration file and returns them as a dictionary */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        
        return jsonFile.connections;
    }
    
    public static string LoadSshPath()
    {
        /* Loads the SSH client path from the configuration file */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        
        return jsonFile.ssh_path;
    }
    
    public static string LoadSftpPath()
    {
        /* Loads the SFTP client path from the configuration file */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        
        return jsonFile.sftp_path;
    }
    
    public static string LoadScpPath()
    {
        /* Loads the SCP client path from the configuration file */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        
        return jsonFile.scp_path;
    }
    
    public static void UpdateSshPath(string newPath)
    {
        /* Updates the SSH client path in the configuration file */
        
        // Updates the SSH client path in the configuration file
        SshConfig jsonFile = LoadConfig();
        jsonFile.ssh_path = newPath;

        string jsonString = JsonSerializer.Serialize(jsonFile); // Serialize the updated config

        string savePath = GetSavePath(); // Get the path to the configuration directory
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        File.WriteAllText(savePath, jsonString); // Write the updated config back to the file
    }

    public static void UpdateSftpPath(string newPath)
    {
        /* Updates the SFTP client path in the configuration file */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        jsonFile.sftp_path = newPath; // Update the SFTP path

        string jsonString = JsonSerializer.Serialize(jsonFile); // Serialize the updated config

        string savePath = GetSavePath(); // Get the path to the configuration directory
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        File.WriteAllText(savePath, jsonString); // Write the updated config back to the file
    }
    
    public static void UpdateScpPath(string newPath)
    {
        /* Updates the SCP client path in the configuration file */
        
        SshConfig jsonFile = LoadConfig(); // Load the configuration
        jsonFile.scp_path = newPath; // Update the SCP path

        string jsonString = JsonSerializer.Serialize(jsonFile); // Serialize the updated config

        string savePath = GetSavePath(); // Get the path to the configuration directory
        savePath = Path.Combine(savePath, ConfigFileName); // Combine to get full path to config file

        File.WriteAllText(savePath, jsonString); // Write the updated config back to the file
    }
    
    public static void AddConnection(string key, string value)
    {
        /* Adds a new connection to the configuration file */
        
        Dictionary<string, string> connections = LoadConnections(); // Load existing connections
        connections[key] = value; // Add or update the connection

        SshConfig updatedConfig = new SshConfig(); // Create a new SshConfig object
        updatedConfig.connections = connections; // Set the updated connections

        UpdateConfig(updatedConfig);
    }
    
    public static void RemoveConnection(string key)
    {
        /* Removes a connection from the configuration file */
        
        Dictionary<string, string> connections = LoadConnections(); // Load existing connections
        if (connections.ContainsKey(key)) // Check if the connection exists
        {
            connections.Remove(key); // Remove the connection
        }
        else // If the connection does not exist, throw an exception
        {
            throw new Exceptions.ConnectionNotFoundException();
        }

        SshConfig updatedConfig = new SshConfig(); // Create a new SshConfig object
        updatedConfig.connections = connections; // Set the updated connections

        UpdateConfig(updatedConfig);
    }
}