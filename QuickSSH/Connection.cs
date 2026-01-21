using System.Diagnostics;

public class Connection
{
    private static string ConvertArgsToString(string[] args)
    {
        /* Converts an array of arguments into a single string with spaces in between */
        
        string result = ""; // Initialize empty result string
        foreach (string arg in args) 
        {
            result += arg + " "; // Append each argument followed by a space
        }

        return result; // Return the concatenated string
    }
    
    private static void TestCommand(string fileName)
    {
        /* Tests if the given command-line client is valid by attempting to start a process */
        
        try // Try to start the process
        {
            Process testProcess = new Process();
            testProcess.StartInfo.FileName = fileName;
            testProcess.StartInfo.RedirectStandardError = true;
            testProcess.StartInfo.UseShellExecute = false;
            testProcess.StartInfo.CreateNoWindow = true;
            testProcess.Start();
        }
        catch // Catch any exceptions that occur during process start
        {
            throw new Exceptions.InvalidClientException(); // Throw custom exception for invalid client
        }
    }

    private static void RunCommand(string fileName, string value, string[] args)
    {
        /* Runs the specified command-line client with the given value and arguments */
        
        string arguments = ConvertArgsToString(args); // Convert arguments array to string
        
        Process sshProcess = new Process();
        sshProcess.StartInfo.FileName = fileName;
        sshProcess.StartInfo.Arguments = value + arguments;
        sshProcess.StartInfo.UseShellExecute = false;
        sshProcess.StartInfo.RedirectStandardInput = false;
        sshProcess.StartInfo.RedirectStandardOutput = false;
        sshProcess.StartInfo.RedirectStandardError = false;
        sshProcess.Start();
        sshProcess.WaitForExit();
    }

    public static void VerifyValidSshClient()
    {
        /* Verifies if the SSH client path is valid */
        TestCommand(Config.LoadSshPath());
    }
    
    public static void VerifyValidSftpClient()
    {
        /* Verifies if the SFTP client path is valid */
        TestCommand(Config.LoadSftpPath());
    }
    
    public static void VerifyValidScpClient()
    {
        /* Verifies if the SCP client path is valid */
        TestCommand(Config.LoadScpPath());
    }
    
    public static void ConnectToServerSsh(string value, string[] args)
    {
        /* Connects to a server using the SSH client */
        RunCommand(Config.LoadSshPath(), value, args);
    }
    
    public static void ConnectToServerSftp(string value, string[] args)
    {
        /* Connects to a server using the SFTP client */
        RunCommand(Config.LoadSftpPath(), value, args);
    }
    
    public static void ConnectToServerScp(string value, string[] args)
    {
        /* Connects to a server using the SCP client */
        RunCommand(Config.LoadScpPath(), value, args);
    }
}