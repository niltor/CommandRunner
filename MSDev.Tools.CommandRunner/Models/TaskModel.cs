using System;

namespace MSDev.Tools.CommandRunner.Models
{
  public class TaskModel
  {
    public Guid Id { get; set; }
    public String Title { get; set; }
    public String Commands { get; set; }

  }
}
