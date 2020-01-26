namespace RabbitMQ_Messages.Models
{
    public class Counter
    {
        private int _actualValue = 0;

        public int ActualValue { get => _actualValue; }

        public void Increment()
        {
            _actualValue++;
        }
    }
}