namespace LoggerLib
{
    public interface ILogger
    {

    }
    public class LocalLogger:ILogger
    {
        private ICypher cypher;
        public LocalLogger(ICypher cypher)
        {
            this.cypher = cypher;
        }
    }
    public class EmailLogger:ILogger
    {

    }
    public class CloudLogger:ILogger
    {

    }
}