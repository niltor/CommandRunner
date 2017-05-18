using System;

namespace MSDev.Tools.CommandRunner.Models
{
  public class TaskModel
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Commands { get; set; }

  }
}
