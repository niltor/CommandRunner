using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSDev.Tools.CommandRunner
{
    public class Runner
    {
        //private readonly Dictionary<String, String> _taskMap = new Dictionary<String, String>() {
        //  ["bingnews"] = "cd /var/task/queue;sudo dotnet MSDev.Taskschd.dll"
        //};

        public Runner()
        {

        }

        public async Task<String> RunCommand(String command)
        {
            String result = command + "\r";
            var myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    myProcess.StartInfo.FileName = "bash";
                    myProcess.StartInfo.Arguments = $"-c \"{command}\"";
                } else
                {
                    myProcess.StartInfo.FileName = "powershell.exe";
                    myProcess.StartInfo.Arguments = command;
                }
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                myProcess.Start();

                StreamReader reader = myProcess.StandardOutput;
                String line = await reader.ReadLineAsync();

                while (line != null)
                {
                    result += line + "\r";
                    Console.WriteLine(line);
                    line = reader.ReadLine();
                }
                Console.WriteLine("Done!");
                result += "Done";

                myProcess.WaitForExit();
                myProcess.Dispose();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;

            }

            return result;
        }
    }
}
