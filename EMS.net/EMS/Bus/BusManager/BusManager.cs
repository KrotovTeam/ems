using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusManager.Abstraction;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace BusManager
{
    public class BusManager : IBusManager
    {
        #region Settings

        private readonly string _host = "rabbitmq://localhost";
        private readonly string _userName = "guest";
        private readonly string _password = "guest";

        #endregion

        #region Private fields

        /// <summary>
        /// Контрол шины данных
        /// </summary>
        private IBusControl _bus;

        /// <summary>
        /// Конфигурация для шины данных
        /// </summary>
        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> _dictConfiguration;

        /// <summary>
        /// Локер для различных потоков
        /// </summary>
        private readonly object _locker = new object();

        #endregion Private fields

        #region Implementation of IBusManager

        /// <summary>
        /// Инициализировать шину
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        public void StartBus<T>(T configuration)
        {
            if (configuration != null)
            {
                Type configurationType = typeof(T);
                if (configurationType == typeof(Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>))
                {
                    _dictConfiguration = configuration as Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>;
                }
                else
                {
                    throw new Exception("Неверный тип конфигурации");
                }
            }
            InitAndStartBus();
        }

        /// <summary>
        /// Остановить шину
        /// </summary>
        public void StopBus()
        {
            _bus?.Stop();
        }

        /// <summary>
        /// Отправить событие
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventModel"></param>
        /// <returns></returns>
        public async Task Publish<TEvent>(object eventModel) where TEvent : class
        {
            if (!InitAndStartBus())
            {
                throw new Exception("Шина не проинициализированна");
            }
            await _bus.Publish<TEvent>(eventModel);
        }

        /// <summary>
        /// Отправить команду
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Send<TCommand>(string queueName, TCommand message) where TCommand : class
        {
            if (!InitAndStartBus())
            {
                throw new Exception("Шина не проинициализированна");
            }
            if (_bus == null)
            {
                throw new Exception("Шина не проинициализированна");
            }
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri($"{_host}/{queueName}"));
            if (sendEndpoint == null)
            {
                throw new Exception($"Не удалось найти очередь {queueName}");
            }
            await sendEndpoint.Send(message, pc => pc.SetAwaitAck(false));
        }

        #endregion Implementation of IBusManager

        #region Private methods

        private bool InitAndStartBus()
        {
            lock (_locker)
            {
                // Если шина проинициализирована - тогда выходим 
                if (_bus != null)
                {
                    return true;
                }
                _bus = ConfigureBus();
                try
                {
                    _bus.StartAsync().Wait();
                    return true;
                }
                catch (RabbitMqConnectionException)
                {
                    _bus = null;
                    return false;
                }
                catch (Exception)
                {
                    _bus = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// сконфигурировать шину
        /// </summary>
        /// <returns>IBusControl</returns>
        private IBusControl ConfigureBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_host), h =>
                {
                    h.Username(_userName);
                    h.Password(_password);
                    h.Heartbeat(600); //10 минут
                });
                if (_dictConfiguration != null && _dictConfiguration.Any())
                {
                    foreach (var kvp in _dictConfiguration)
                    {
                        cfg.ReceiveEndpoint(host, kvp.Key, kvp.Value);
                    }
                }
            });
        }

        #endregion Private methods
    }
}