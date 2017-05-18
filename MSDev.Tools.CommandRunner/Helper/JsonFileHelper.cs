using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MSDev.Tools.CommandRunner.Models;
using Newtonsoft.Json;

namespace MSDev.Tools.CommandRunner
{
    /// <summary>
    /// 每个文件代表一个任务命令
    /// </summary>
    public class JsonFileHelper
    {
        public const string DirPath = "./commandTasks";
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
					string jsonString = await reader.ReadToEndAsync();
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
        /// <summary>
        /// 读取某一任务配置
        /// </summary>
        /// <param name="fileName">任务名称</param>
        /// <returns></returns>
        public async Task<TaskModel> Read(string fileName)
        {
            var taskModel = new TaskModel();

            var file = new FileInfo(Path.Combine(DirPath, fileName + ".json"));
            if (!file.Exists)
            {
                return taskModel;
            }

            StreamReader reader = file.OpenText();
			string jsonString = await reader.ReadToEndAsync();
            reader.Dispose();

            taskModel = JsonConvert.DeserializeObject<TaskModel>(jsonString);
            return taskModel;
        }

        /// <summary>
        /// 新建任务
        /// </summary>
        /// <param name="task">TaskModel</param>
        /// <returns></returns>
        public async Task<Boolean> Insert(TaskModel task)
        {
            FileInfo file = new FileInfo(Path.Combine(DirPath, task.Title + ".json"));
            if (file.Exists)
            {
                return false;
            }
            StreamWriter writer = file.CreateText();
            await writer.WriteAsync(JsonConvert.SerializeObject(task));
            writer.Dispose();
            return true;
        }

        /// <summary>
        /// 删除一任务
        /// </summary>
        /// <param name="fileName">任务名称</param>
        /// <returns></returns>
        public Boolean Delete(string fileName)
        {
            FileInfo file = new FileInfo(Path.Combine(DirPath, fileName + ".json"));
            if (file.Exists)
            {
                file.Delete();
            }
            return true;
        }


        public async Task<Boolean> Update(string fileName, TaskModel task)
        {
            Delete(fileName);
            await Insert(task);
            return true;
        }
    }
}
