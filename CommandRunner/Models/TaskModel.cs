using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandRunner.Models
{
  public class TaskModel
  {
    public Guid Id { get; set; }
    public String Title { get; set; }
    public String Commands { get; set; }

  }
}
