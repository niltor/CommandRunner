using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommandRunner.Services
{
  public class JsonFileHelper
  {
    public JsonFileHelper()
    {

    }


    public async Task<T> Read<T>(String filePath)
    {
      FileInfo file = new FileInfo(filePath);
      if (!file.Exists)
        return default(T);
      StreamReader reader = file.OpenText();
      String jsonString = await reader.ReadToEndAsync();
      reader.Dispose();
      
      return JsonConvert.DeserializeObject<T>(jsonString);
    }


    public async Task<Boolean> Insert<T>(String filePath, T content)
    {
      FileInfo file = new FileInfo(filePath);
      var origin =await Read<List<T>>(filePath);
      origin = origin ?? new List<T>();

      origin.Add(content);

      StreamWriter writer = file.CreateText();

      await writer.WriteAsync(JsonConvert.SerializeObject(origin));
      writer.Dispose();
      return true;
    }

    public async Task<Boolean> Delete<T>(String filePath, T oldContent)
    {
      FileInfo file = new FileInfo(filePath);
      var origin = Read<List<T>>(filePath) as List<T>;
      if (origin == null)
      {
        return false;
      }
      origin.Remove(oldContent);
      StreamWriter writer = file.CreateText();
      await writer.WriteAsync(JsonConvert.SerializeObject(origin));
      writer.Dispose();
      return true;
    }


    public async Task<Boolean> Update<T>(String filePath, T oldContent, T content)
    {
      await Delete(filePath,oldContent);
      await Insert<T>(filePath, content);
      return true;
    }
  }
}
