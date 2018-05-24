using System.Threading.Tasks;

namespace BusManager.Abstraction
{
    public interface IBusManager
    {
        /// <summary>
        /// Инициализировать шину
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        void StartBus<T>(T configuration);

        /// <summary>
        /// Остановить шину
        /// </summary>
        void StopBus();

        /// <summary>
        /// Отправить событие
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventModel"></param>
        /// <returns></returns>
        Task Publish<TEvent>(object eventModel) where TEvent : class;

        /// <summary>
        /// Отправить команду
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send<TCommand>(string queueName, TCommand message) where TCommand : class;
    }
}
