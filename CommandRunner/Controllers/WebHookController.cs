using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandRunner.Models;
using CommandRunner.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandRunner.Controllers
{
  public class WebHookController : Controller
  {
    private readonly Runner _runner;

    public WebHookController(Runner runner)
    {
      _runner = runner;
    }

    [HttpGet]
    public async Task<IActionResult> Index(String result)
    {
      JsonFileHelper jfh = new JsonFileHelper();

      List<TaskModel> taskList = await jfh.ReadAllAsync();
      Console.WriteLine(JsonConvert.SerializeObject(taskList));
      ViewBag.TaskList = taskList;
      return View();
    }

    /// <summary>
    /// 直接调用执行命令
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    [HttpPost]
    //TODO: 应加权限限制
    public String RunTask(String commands)
    {
      String re = _runner.RunCommand(commands);
      return re;
    }


    /// <summary>
    /// GitLab WebHook
    /// </summary>
    /// <param name="taskName"></param>
    /// <returns></returns>

    [HttpPost]
    public async Task<Boolean> GitLab([FromBody]JObject parameter, String taskName = null)
    {

      String eventType = parameter.GetValue("object_kind").ToString();
      Console.WriteLine(parameter.ToString());
      String branch = parameter.GetValue("ref").ToString();

      if (String.IsNullOrEmpty(taskName))
      {
        return false;
      }

      if (eventType == "" && branch == "")
      {
      }
      JsonFileHelper jfh = new JsonFileHelper();
      TaskModel task = await jfh.Read(taskName);
      RunTask(task.Commands);
      return true;
    }

    /// <summary>
    /// 触发执行
    /// </summary>
    /// <param name="taskName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Boolean> AutoRunTask(String taskName = null)
    {
      if (String.IsNullOrEmpty(taskName))
      {
        return false;
      }
      JsonFileHelper jfh = new JsonFileHelper();
      TaskModel task = await jfh.Read(taskName);
      RunTask(task.Commands);
      return true;
    }



    /// <summary>
    /// Add Command
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddCommand(TaskModel task)
    {

      if (String.IsNullOrEmpty(task.Title) || String.IsNullOrEmpty(task.Commands))
      {
        return RedirectToAction("Index", new { result = "null value" });

      }
      JsonFileHelper jfh = new JsonFileHelper();
      task.Id = Guid.NewGuid();
      await jfh.Insert(task);

      return RedirectToAction("Index", new { result = "success" });
    }
  }
}