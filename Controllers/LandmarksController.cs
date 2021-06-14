using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cs_mediapipe_pose.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class LandmarksController : ControllerBase
  {
    private readonly ILogger<LandmarksController> _logger;

    public LandmarksController(ILogger<LandmarksController> logger)
    {
      _logger = logger;
    }

    [HttpPost]
    public dynamic GenerateFile(Landmark[][] landmarks)
    {
      try
      {
        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";
        string name = "frames.csv";
        string guid = Guid.NewGuid().ToString();
        string path = $"Storage/{guid}/";

        var sb = new StringBuilder();

        if (System.IO.File.Exists($"{path}{name}")) System.IO.File.Delete($"{path}{name}");
        System.IO.Directory.CreateDirectory(path);
        FileStream file = System.IO.File.Create($"{path}{name}");
        file.Close();

        int[] excludeList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 17, 18, 19, 20, 29, 30 };

        foreach (Landmark[] l in landmarks)
        {
          string line = "";
          for (int i = 0; i < l.Length; i++)
            if (!excludeList.Contains(i))
              line += transformValue(l[i], i);

          line = line.Substring(0, line.Length - 2);
          sb.AppendLine(line);
          TextWriter sw = new StreamWriter($"{path}{name}", true);
          sw.Write(sb.ToString());
          sw.Close();
        }

        return $"{baseUrl}/uploads/{guid}/{name}";
      }
      catch (Exception Ex)
      {
        Console.WriteLine(Ex.ToString());
        return null;
      }

    }

    private string transformValue(Landmark l, int index)
    {
      int metric = 4;
      string val = $"{index}:{Math.Round(l.x * metric, 2) }:{Math.Round(l.y * metric, 2)}:{Math.Round(l.z * metric, 2)}; ";
      val.Replace(",", ".");
      return val;
    }
  }
}
