using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

using Xamarin.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace PracticeUserInputAndBinding
{
    //Using classes from MainPage.xaml.cs too.
    class RabbitMQViewModel : ViewModelBase
    {
        string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
    }
    public partial class TestPage_1 : ContentPage
    {
        RabbitMQViewModel rabbitMQViewModel;

        public TestPage_1()
        {
            InitializeComponent();
            rabbitMQViewModel = new RabbitMQViewModel();
            BindingContext = rabbitMQViewModel;
            rabbitMQViewModel.Message = "Jump";
            myList.ItemsSource = new List<string> { "Test 1", "Test 2", "Test 3", "Test 4", "Test 5", "Test 6", "Test 7", "Test 8" };
        }
        public void Send(object sender, EventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                Console.WriteLine("Sent {0}", message);
                rabbitMQViewModel.Message = "published " + message;
            }

        }
        public void Consume(object sender, EventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var queue = channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                //var consumer = new EventingBasicConsumer(channel);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue: "hello", autoAck: false, consumer: consumer); // changed autoAck: true

                //BasicDeliverEventArgs ea = consumer.Queue.Dequeue();
                //var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += (modle, ea) =>
                if(queue.MessageCount != 0)
                {
                    BasicDeliverEventArgs ea = consumer.Queue.Dequeue();


                    //var body = ea.Body;
                    var message = Encoding.UTF8.GetString(ea.Body);
                    Console.WriteLine("Received {0}", message);
                    rabbitMQViewModel.Message = "Consumed " + message;

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    Console.WriteLine("No message sent yet.");
                }
            }
        }
    }
}
