using System.Text.Json;

namespace LoggerLib
{
    public interface ILogSerializer
    {
        public string Serialize(Log log);
        public Log? Deserialize(string text);
    }
    public class JsonLogSerilizer : ILogSerializer
    {
        public string Serialize(Log log)
        {
            return JsonSerializer.Serialize<Log>(log);
        }

        public Log? Deserialize(string text)
        {
            return JsonSerializer.Deserialize<Log>(text);
        }
    }
}
