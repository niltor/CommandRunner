using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSDev.Tools.CommandRunner
{
  /// <summary>
  /// Use with Websocket
  /// </summary>
  public class WebSocketRunner
  {
    private readonly WebSocket _webSocket;
    readonly Dictionary<string, string> _taskMap = new Dictionary<string, string>()
    {
      ["test"] = "ls"
    };

    public WebSocketRunner(WebSocket webSocket)
    {
      _webSocket = webSocket;
    }

    public async Task Run(string command)
    {
      var myProcess = new Process();

      if (_taskMap.TryGetValue(command, out string value))
      {
        Console.WriteLine(value);
        command = value;
      }

      Console.WriteLine("command is :" + command);

      try
      {
        myProcess.StartInfo.UseShellExecute = false;
        //linux
        myProcess.StartInfo.FileName = "bash";
        myProcess.StartInfo.Arguments = "-c \"" + command + "\"";

        //windows
        //myProcess.StartInfo.FileName = "powershell.exe";
        //myProcess.StartInfo.Arguments = command;

        Console.WriteLine("command is :" + command);

        myProcess.StartInfo.CreateNoWindow = false;
        myProcess.StartInfo.RedirectStandardOutput = true;
        myProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;

        myProcess.Start();

        StreamReader reader = myProcess.StandardOutput;
				string line = reader.ReadLine();

        while (line != null)
        {
          await Echo(line);
          Console.WriteLine(line);
          line = reader.ReadLine();
        }
        await Echo("Done");
        myProcess.WaitForExit();
        myProcess.Dispose();

      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
    private async Task Echo(string message) => await _webSocket.SendAsync(
          new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
  }
}
