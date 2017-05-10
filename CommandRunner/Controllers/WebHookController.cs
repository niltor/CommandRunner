using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CissAdminApi.Services;
using CommandRunner.Models;
using CommandRunner.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    public async Task<IActionResult> Index()
    {

      JsonFileHelper jfh = new JsonFileHelper();


      List<TaskModel> taskList = await jfh.Read<List<TaskModel>>("./taskCommands.json");


      Console.WriteLine(JsonConvert.SerializeObject(taskList));
      ViewBag.TaskList = taskList;
      return View();
    }

    [HttpPost]
    public String RunTask(String commands)
    {
      String re = _runner.RunCommand(commands);
      return re;
    }

    /// <summary>
    /// Add Command
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddCommand(TaskModel task)
    {
      JsonFileHelper jfh = new JsonFileHelper();
      task.Id = Guid.NewGuid();
      await jfh.Insert("./taskCommands.json", task);

      return RedirectToAction("Index", new { result = "success" });
    }
  }
}