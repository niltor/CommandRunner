using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandRunner.Models;
using Newtonsoft.Json;

namespace CommandRunner.Services
{
  /// <summary>
  /// 每个文件代表一个任务命令
  /// </summary>
  public class JsonFileHelper
  {
    public const String DirPath = "./commandTasks";
    public JsonFileHelper()
    {

    }

    /// <summary>
    /// 读取任务列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<TaskModel>> ReadAllAsync()
    {

      List<TaskModel> taskList = new List<TaskModel>();
      if (Directory.Exists(DirPath))
      {
        DirectoryInfo dir = new DirectoryInfo(DirPath);
        FileInfo[] files = dir.GetFiles();

        foreach (FileInfo file in files)
        {
          StreamReader reader = file.OpenText();
          String jsonString = await reader.ReadToEndAsync();
          reader.Dispose();
          TaskModel taskModel = JsonConvert.DeserializeObject<TaskModel>(jsonString);

          taskList.Add(taskModel);
        }

      } else
      {
        Directory.CreateDirectory(DirPath);
      }
      return taskList;

    }

    public async Task<TaskModel> Read(String fileName)
    {
      TaskModel taskModel = new TaskModel();

      FileInfo file = new FileInfo(Path.Combine(DirPath, fileName + ".json"));
      if (!file.Exists)
        return taskModel;
      StreamReader reader = file.OpenText();
      String jsonString = await reader.ReadToEndAsync();
      reader.Dispose();

      taskModel = JsonConvert.DeserializeObject<TaskModel>(jsonString);
      return taskModel;
    }

    /// <summary>
    /// 新建任务
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<Boolean> Insert(TaskModel task)
    {
      FileInfo file = new FileInfo(task.Title+".json");
      if (file.Exists)
      {
        return false;
      }
      StreamWriter writer = file.CreateText();
      await writer.WriteAsync(JsonConvert.SerializeObject(task));
      writer.Dispose();
      return true;
    }

    public bool Delete(String fileName)
    {
      FileInfo file = new FileInfo(Path.Combine(DirPath, fileName + ".json"));
      if (file.Exists)
      {
        file.Delete();
      }
      return true;
    }


    public async Task<Boolean> Update(String fileName,TaskModel task)
    {
      Delete(fileName);
      await Insert(task);
      return true;
    }
  }
}
