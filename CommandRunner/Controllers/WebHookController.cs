using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSDev.Tools.CommandRunner;
using MSDev.Tools.CommandRunner.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandRunner.Controllers
{
    [Authorize(Policy ="Admin")]
    public class WebHookController : Controller
    {
        private readonly Runner _runner;
        private readonly JsonFileHelper _jfh;

        public WebHookController(Runner runner, JsonFileHelper jfh)
        {
            _runner = runner;
            _jfh = jfh;
        }

        [HttpGet]
        public async Task<IActionResult> Index(String result)
        {

            List<TaskModel> taskList = await _jfh.ReadAllAsync();
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
        public async Task<String> RunTask(String commands)
        {
            String re = await _runner.RunCommand(commands);
            return re;
        }


        /// <summary>
        /// GitLab WebHook
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GitLab([FromBody]JObject parameter, String taskName = null)
        {

            String eventType = parameter.GetValue("event_name").ToString();
            Console.WriteLine("EventName:" + eventType);
            String branch = parameter.GetValue("ref").ToString();
            branch = branch.Replace("refs/heads/", String.Empty);
            Console.WriteLine("Push Branch:" + branch);

            String defaultBranch = parameter.GetValue("project").Value<String>("default_branch");

            Console.WriteLine("Default Branch:" + defaultBranch);

            if (String.IsNullOrEmpty(taskName))
            {
                return NotFound();
            }

            if (eventType == "push" && branch == defaultBranch)
            {
                TaskModel task = await _jfh.Read(taskName);
                RunTask(task.Commands);
            }

            return Ok();
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
            TaskModel task = await _jfh.Read(taskName);
            RunTask(task.Commands);
            return true;
        }


        [HttpPost]

        public async Task<IActionResult> EditCommand(TaskModel task)
        {

            task.Id = Guid.NewGuid();
            Boolean re = await _jfh.Update(task.Title, task);
            return Content(re.ToString());
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
            task.Id = Guid.NewGuid();
            await _jfh.Insert(task);

            return RedirectToAction("Index", new { result = "success" });
        }

        [HttpPost]
        public IActionResult DelTask(String title)
        {
            if (String.IsNullOrEmpty(title))
            {
                return Content("null");
            } else
            {
                return Content(_jfh.Delete(title) ? "success" : "failed");
            }
        }
    }



}