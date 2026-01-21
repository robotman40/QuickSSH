using System.Diagnostics;
using System.Runtime.InteropServices;

class QuickSsh
{
    private const string Version = "0.1.0";
    
    static void Main(string[] args)
    {
        // Try to create the configuration directory during startup (if it doesn't exist)
        try
        {
            Config.CreateSaveFile();
        }
        catch (Exceptions.SaveFileCreationException)
        {
            Console.WriteLine("Failed to create configuration file. Please ensure the application has the necessary permissions.");
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred during initialization: {e.Message}");
            Environment.Exit(1);
        }
        
        // If the initialization is successful, proceed with the main application logic
        
        // Load existing connections from configuration
        Dictionary<string, string> connections = new Dictionary<string, string>();
        
        try
        {
            connections = Config.LoadConnections(); // Load existing connections from configuration
        }
        catch (Exceptions.JsonObjectNullException) // Catch deserialization errors
        {
            Console.WriteLine("Configuration file is corrupted. Please delete the configuration file and restart the application.");
            Environment.Exit(1);
        }
        catch (FileNotFoundException) // Catch missing file errors
        {
            Console.WriteLine("Configuration file not found. Please ensure the application has been initialized correctly.");
            Environment.Exit(1);
        }
        catch (Exception e) // Catch any other unexpected errors
        {
            Console.WriteLine($"An unexpected error occurred while loading the configuration: {e.Message}");
        }

        if (args.Length == 0) // No arguments provided
        {
            Console.WriteLine("No arguments provided. Exiting...");
        } 
        else if (args.Length == 3 && args[0] == "create") // Create new connection
        {
            Config.AddConnection(args[2], args[1]); // Add new connection to configuration
            Console.WriteLine("Connection created successfully.");
        } 
        else if (args.Length == 2 && args[0] == "delete") // Delete existing connection
        {
            try
            {
                Config.RemoveConnection(args[1]); // Remove connection from configuration
                Console.WriteLine("Connection deleted successfully.");
            }
            catch (Exceptions.ConnectionNotFoundException) // Catch if connection does not exist
            {
                Console.WriteLine("Connection not found. Cannot delete non-existent connection.");
                Environment.Exit(1);
            }
            catch (Exception e) // Catch any other unexpected errors
            {
                Console.WriteLine($"An unexpected error occurred while deleting the connection: {e.Message}");
                Environment.Exit(1);
            }
        } 
        else if (args.Length >= 2 && args[0] == "ssh")
        {
            string client = string.Empty; // Get the SSH address from the connections
            try
            {
                client = connections[args[1]];
            }
            catch (KeyNotFoundException) // Catch if connection does not exist
            {
                Console.WriteLine("Connection not found. Cannot connect to non-existent connection.");
                Environment.Exit(1);
            }
            
            string[] additionalArgs = args[2..]; // Get any additional arguments
            
            try
            {
                Connection.VerifyValidSshClient(); // Verify the SSH client path is valid
            }
            catch (Exceptions.InvalidClientException)
            {
                Console.WriteLine("Invalid SSH client path. Please update the configuration.");
                Environment.Exit(1);
            }
            
            Connection.ConnectToServerSsh(client, additionalArgs);
        }
        else if (args.Length >= 2 && args[0] == "sftp")
        {
            string client = string.Empty; // Get the SFTP address from the connections
            try
            {
                client = connections[args[1]];
            }
            catch (KeyNotFoundException) // Catch if connection does not exist
            {
                Console.WriteLine("Connection not found. Cannot connect to non-existent connection.");
                Environment.Exit(1);
            }
            
            string[] additionalArgs = args[2..]; // Get any additional arguments
            
            try
            {
                Connection.VerifyValidSftpClient(); // Verify the SFTP client path is valid
            }
            catch (Exceptions.InvalidClientException)
            {
                Console.WriteLine("Invalid SFTP client path. Please update the configuration.");
                Environment.Exit(1);
            }
            
            Connection.ConnectToServerSftp(client, additionalArgs);
        }
        else if (args.Length >= 2 && args[0] == "scp")
        {
            string client = string.Empty; // Get the SCP address from the connections
            try
            {
                client = connections[args[1]];
            }
            catch (KeyNotFoundException) // Catch if connection does not exist
            {
                Console.WriteLine("Connection not found. Cannot connect to non-existent connection.");
                Environment.Exit(1);
            }
            
            string[] additionalArgs = args[2..]; // Get any additional arguments
            
            try
            {
                Connection.VerifyValidScpClient(); // Verify the SCP client path is valid
            }
            catch (Exceptions.InvalidClientException)
            {
                Console.WriteLine("Invalid SCP client path. Please update the configuration.");
                Environment.Exit(1);
            }
            
            Connection.ConnectToServerScp(client, additionalArgs);
        }
        else if (args.Length == 2 && args[0] == "config") // No action for config without parameters
        {
            string client = args[1]; // Client type (ssh, sftp, scp)
            
            if (client == "ssh") // SSH client
            {
                string path = Config.LoadSshPath();
                Console.WriteLine($"SSH client path: {path}");
            } 
            else if (client == "sftp") // SFTP client
            {
                string path = Config.LoadSftpPath();
                Console.WriteLine($"SFTP client path: {path}");
            }
            else if (client == "scp") // SCP client
            {
                string path = Config.LoadScpPath();
                Console.WriteLine($"SCP client path: {path}");
            }
            else // Invalid client specified
            {
                Console.WriteLine("Invalid client specified. Use 'ssh', 'sftp', or 'scp'.");
                Environment.Exit(1);
            }
        }
        else if (args.Length == 3 && args[0] == "config") // Update the configuration
        {
            string client = args[1]; // Client type (ssh, sftp, scp)
            string path = args[2]; // New path to set
            
            if (client == "ssh") // SSH client
            {
                Config.UpdateSshPath(path);
                Console.WriteLine("SSH client path updated successfully.");
            } 
            else if (client == "sftp") // SFTP client
            {
                Config.UpdateSftpPath(path);
                Console.WriteLine("SFTP client path updated successfully.");
            }
            else if (client == "scp") // SCP client
            {
                Config.UpdateScpPath(path);
                Console.WriteLine("SCP client path updated successfully.");
            }
            else // Invalid client specified
            {
                Console.WriteLine("Invalid client specified. Use 'ssh', 'sftp', or 'scp'.");
                Environment.Exit(1);
            }
        }
        else if (args.Length == 1 && args[0] == "list") // List all saved connections
        {
            Console.WriteLine("Saved SSH connections:");
            foreach (var pair in connections)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}"); // Display each connection name and address
            }
        }
        else if (args.Length == 1 && args[0] == "version") // Display version information
        {
            string os = Environment.OSVersion.VersionString; // Get operating system version
            string arch = RuntimeInformation.OSArchitecture.ToString().ToLower(); // Get system architecture
            
            /* Small easter egg for detecting
             if running on Wine. This can be done
             by checking if winlogon.exe is running
             on a platform reporting as Windows */
            if (OperatingSystem.IsWindows() && Process.GetProcessesByName("winlogon").Length == 0)
            {
                os += " on Wine"; // Append Wine to OS string if detected
            }
            
            Console.WriteLine($"QuickSSH {Version} on {os} {arch}"); // Display version and system information
        }
        else if (args.Length == 1 && args[0] == "help") // Display help information
        {
            Console.WriteLine("QuickSsh Help:");
            Console.WriteLine("create <address> <name> - Create a new SSH connection with the given name and address.");
            Console.WriteLine("delete <name> - Delete the SSH connection with the given name.");
            Console.WriteLine("ssh <name> [additional ssh args] - Connect to the SSH server with the given name, passing any additional arguments to SSH.");
            Console.WriteLine("sftp <name> [additional sftp args] - Connect to the SSH server via SFTP with the given name, passing any additional arguments to SFTP.");
            Console.WriteLine("scp <name> [additional scp args] - Connect to the SSH server via SCP with the given name, passing any additional arguments to SCP.");
            Console.WriteLine("config ssh - Print the currently set path to the SSH client.");
            Console.WriteLine("config ssh <path> - Set the path to the SSH client.");
            Console.WriteLine("config sftp - Print the currently set path to the SFTP client.");
            Console.WriteLine("config sftp <path> - Set the path to the SFTP client.");
            Console.WriteLine("config scp - Print the currently set path to the SCP client.");
            Console.WriteLine("config scp <path> - Set the path to the SCP client.");
            Console.WriteLine("list - List all saved SSH connections.");
            Console.WriteLine("version - Display the current version of QuickSSH.");
            Console.WriteLine("help - Display this help message.");
        }
        else // Invalid arguments provided
        {
            Console.WriteLine("Invalid arguments provided. Exiting...");
        }

        Environment.Exit(0); // Exit the application successfully
    }
}