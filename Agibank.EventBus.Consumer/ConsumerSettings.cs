namespace Agibank.EventBus.Consumer
{
    public class ConsumerSettings
    {
        public string PathIn { get; set; }
        public string PathOut { get; set; }
        public string OutputFilename { get; set; }
        public string Extension { get; set; }
        public string Hostname { get; set; }
        public int Channels { get; set; }
        public string QueueName { get; set; }
        public int Port { get; set; }

    }
}
