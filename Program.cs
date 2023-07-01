using RabbitMQ.Client;
using System.CommandLine;
using System.Text;
using RabbitMQ.Client.Events;
using Rabbit;

Option<T> NewOption<T>(string name, bool addAlias, T? defaultValue = default)
{
    if (string.IsNullOrEmpty(name))
    {
        throw new ArgumentException("not a valid name" , name);
    }

    var option = new Option<T>($"--{name}");

    if (addAlias && name.Length > 1)
    {
        option.AddAlias($"-{name.First()}");
    }

    if (defaultValue is null)
    {
        option.IsRequired = true;
    }
    else
    {
        option.SetDefaultValue(defaultValue);
    }

    return option;
}

void UseRabbitMq(GlobalOptions options, Action<IModel> action)
{
    try
    {
        var connFactory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            VirtualHost = options.VirtualHost
        };

        if (!string.IsNullOrEmpty(options.Username))
        {
            connFactory.UserName = options.Username;
        }

        if (!string.IsNullOrEmpty(options.Password))
        {
            connFactory.Password = options.Password;
        }

        if (options.Secure)
        {
            connFactory.Ssl = new SslOption
            {
                Enabled = true,
                ServerName = options.Host
            };
        }

        using var connection = connFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(
            queue:options.Queue,
            exclusive: options.Exclusive,
            durable: options.Durable,
            autoDelete: options.AutoDelete);

        action(channel);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

var hostOption = NewOption("host", false, "localhost");
var portOption = NewOption("port", false, 5672);
var secureOption = NewOption("secure", true, false);
var queueOption = NewOption<string>("queue", true);
var usernameOption = NewOption("username", true, string.Empty);
var passwordOption = NewOption("password", true, string.Empty);
var exclusiveOption = NewOption("exclusive", false, false);
var durableOption = NewOption("durable", true, false);
var autodeleteOption = NewOption("autodelete", true, false);
var exchangeOption = NewOption("exchange", false, string.Empty);
var vhostOption = NewOption("vhost", true, "/");

var optionsBinder = new GlobalOptionsBinder(
    hostOption: hostOption,
    portOption: portOption,
    secureOption: secureOption,
    queueOption: queueOption,
    usernameOption: usernameOption,
    passwordOption: passwordOption,
    exclusiveOption: exclusiveOption,
    durableOption: durableOption,
    autoDeleteOption: autodeleteOption,
    exchangeOption: exchangeOption,
    vhostOption: vhostOption
);

var rootCommand = new RootCommand("CLI tool to use RabbitMQ");
rootCommand.AddGlobalOptions(
    hostOption, portOption, secureOption,
    queueOption, usernameOption, passwordOption,
    exchangeOption, durableOption, autodeleteOption, exclusiveOption);

var pubCommand = new Command("publish", "publish message to specified queue");
var messageOption = NewOption<string>("message", true);
pubCommand.AddOption(messageOption);

pubCommand.SetHandler((options, message) =>
{
    UseRabbitMq(options, queue =>
    {
        queue.BasicPublish(
            exchange: options.Exchange,
            routingKey: options.Queue,
            basicProperties: queue.CreateBasicProperties(),
            body: Encoding.UTF8.GetBytes(message));
    });
}, optionsBinder, messageOption);

var peekCommand = new Command("peek", "todo");
peekCommand.SetHandler((options) =>
{
    UseRabbitMq(options, queue =>
    {
        var message = queue.BasicGet(options.Queue, autoAck: false);
        if (message is null)
        {
            Console.WriteLine($"Queue {options.Queue} is empty");
            return;
        }
        Console.WriteLine(Encoding.UTF8.GetString(message.Body.Span));
        queue.BasicReject(message.DeliveryTag, requeue: true);
    });
}, optionsBinder);

var popCommand = new Command("pop", "todo");
popCommand.SetHandler((options) =>
{
    UseRabbitMq(options, queue =>
    {
        var message = queue.BasicGet(options.Queue, autoAck: false);
        if (message is null)
        {
            Console.WriteLine($"{options.Queue} queue is empty");
            return;
        }
        Console.WriteLine(Encoding.UTF8.GetString(message.Body.Span));
        queue.BasicAck(message.DeliveryTag, multiple: false);
    });

}, optionsBinder);

var subCommand = new Command("subscribe", "todo");
subCommand.SetHandler((options) =>
{
    UseRabbitMq(options, queue =>
    {
        var consumer = new EventingBasicConsumer(queue);
        consumer.Received += (_, message) =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {Encoding.UTF8.GetString(message.Body.Span)}");
        };
        queue.BasicConsume(queue: options.Queue, autoAck: true, consumer: consumer);
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    });
}, optionsBinder);

rootCommand.AddCommands(pubCommand, subCommand, peekCommand, popCommand);
await rootCommand.InvokeAsync(args);