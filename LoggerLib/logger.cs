using System.Text;
using System.Text.Json;

namespace LoggerLib
{
    public interface ILogger
    {
        public bool SendLog(Log log);
        public List<Log> RecieveLogs();
    }
    public class LocalLogger:ILogger
    {
        private ICypher cypher;
        private Log lastLog;
        private string path;
        public LocalLogger(ICypher cypher)
        {
            this.cypher = cypher;
            path = @"logs\";
        }

        public bool SendLog(Log log)
        {
            try
            {
                lastLog = log;
                string serialized = JsonSerializer.Serialize(log);
                byte[] encrypted=cypher.Encrypt(serialized);
                string fileName = log.NetInterface.Name + "_" + log.TimeStamp.Ticks;
                File.WriteAllBytes(path+fileName,encrypted);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public List<Log> RecieveLogs()
        {
            var result = new List<Log>();
            var files=Directory.GetFiles(path);
            foreach (var file in files)
            {

                    var bytes = File.ReadAllBytes(file);
                    var decrypted = cypher.Decrypt(bytes);
                    var log = JsonSerializer.Deserialize<Log>(decrypted);
                    if(log!= null) result.Add(log);
                
            }
            return result;
        }
    }
    public class EmailLogger:ILogger
    {
        public bool SendLog(Log log)
        {
            throw new NotImplementedException();
        }

        public List<Log> RecieveLogs()
        {
            throw new NotImplementedException();
        }
    }
    public class CloudLogger:ILogger
    {
        public bool SendLog(Log log)
        {
            throw new NotImplementedException();
        }

        public List<Log> RecieveLogs()
        {
            throw new NotImplementedException();
        }
    }
}