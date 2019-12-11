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
        //This method is written to consume only one message at a time.
        public void Consume(object sender, EventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };// Creates ConnectionFactory.
            //This is a "using" statement, and not an import.
            //Here, is serves to automatically close the connection and the channel once is leave the block.
            //Basically, is will automatically run channel.Close(), and connection.Close()
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Gets a single message from the queue without acknowledging.
                BasicGetResult result = channel.BasicGet(queue: "hello", autoAck: false);

                if (result == null)// Checks if the queue is empty.
                {
                    Console.WriteLine("No messages avaliable at this time.");
                }
                else
                {
                    IBasicProperties props = result.BasicProperties;// Collects message properties.
                    byte[] body = result.Body;// Collects message body.
                    string message = Encoding.UTF8.GetString(body);// Converts body from bytes to string.
                    channel.BasicAck(result.DeliveryTag, false);// Manually sends acknowledgement for message.
                    Console.WriteLine("Received {0}", message);// Prints message to console.
                    rabbitMQViewModel.Message = "Consumed " + message;// Displays message in label in the UI.
                }
            }
        }
    }
}
