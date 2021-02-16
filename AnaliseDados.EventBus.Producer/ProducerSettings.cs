namespace AnaliseDados.EventBus.Producer
{
    public class ProducerSettings
    {
        public int Threads { get; set; }
        public string PathIn { get; set; }
        public string Hostname { get; set; }
        public string Extension { get; set; }
        public int Channels { get; set; }
        public string QueueName { get; set; }
        public int Port { get; set; }
    }
}
