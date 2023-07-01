using RabbitMQ.Client;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Text;

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

void UseQueue(Options options, Action<IModel> action)
{
    try
    {
        var connFactory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
        };

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
var portOption = NewOption("port", true, 5672);
var secureOption = NewOption("secure", true, false);
var queueOption = NewOption<string>("queue", true);
var usernameOption = NewOption("username", true, string.Empty);
var passwordOption = NewOption("password", false, string.Empty);
var exclusiveOption = NewOption("exclusive", false, false);
var durableOption = NewOption("durable", true, false);
var autodeleteOption = NewOption("autodelete", true, false);
var exchangeOption = NewOption("exchange", false, string.Empty);

var optionsBinder = new OptionsBinder(
    hostOption: hostOption,
    portOption: portOption,
    secureOption: secureOption,
    queueOption: queueOption,
    usernameOption: usernameOption,
    passwordOption: passwordOption,
    exclusiveOption: exclusiveOption,
    durableOption: durableOption,
    autoDeleteOption: autodeleteOption,
    exchangeOption: exchangeOption
);

var rootCommand = new RootCommand("CLI tool to use RabbitMQ");
rootCommand.AddGlobalOptions(
    hostOption, portOption, secureOption,
    queueOption, usernameOption, passwordOption,
    exchangeOption, durableOption, autodeleteOption, exchangeOption);

var pubCommand = new Command("publish", "publish message to specified queue");
var messageArgument = new Argument<string>();
pubCommand.AddArgument(messageArgument);

pubCommand.SetHandler((options, message) =>
{
    UseQueue(options, channel =>
    {
        channel.BasicPublish(
            exchange: options.Exchange,
            routingKey: options.Queue,
            basicProperties: channel.CreateBasicProperties(),
            body: Encoding.UTF8.GetBytes(message));
    });

}, optionsBinder, messageArgument);


// all three will have the same args in handler
var peekCommand = new Command("peek", "todo");
peekCommand.SetHandler((options) =>
{
    UseQueue(options, channel =>
    {
        // todo
        // channel.BasicConsume();
        Console.WriteLine("hi!");
    });
}, optionsBinder);

var popCommand = new Command("pop", "todo");
popCommand.SetHandler((options) =>
{
    Console.WriteLine("hi!");

}, optionsBinder);

var subCommand = new Command("subscribe", "todo");
subCommand.SetHandler((options) =>
{
    Console.WriteLine("hi!");
}, optionsBinder);

rootCommand.AddCommands(pubCommand, subCommand, peekCommand, popCommand);
await rootCommand.InvokeAsync(args);

public class Options
{
    public string? Host { get; init; }
    public int Port { get; init; }
    public bool Secure { get; init; }
    public string? Queue { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public bool Exclusive { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
    public string? Exchange { get; init; }
}

public class OptionsBinder : BinderBase<Options>
{
    public OptionsBinder(Option<string> hostOption, Option<int> portOption, Option<bool> secureOption, Option<string> queueOption, Option<string> usernameOption, Option<string> passwordOption, Option<bool> exclusiveOption, Option<bool> durableOption, Option<bool> autoDeleteOption, Option<string> exchangeOption)
    {
        HostOption = hostOption;
        PortOption = portOption;
        SecureOption = secureOption;
        QueueOption = queueOption;
        UsernameOption = usernameOption;
        PasswordOption = passwordOption;
        ExclusiveOption = exclusiveOption;
        DurableOption = durableOption;
        AutoDeleteOption = autoDeleteOption;
        ExchangeOption = exchangeOption;
    }

    public Option<string> HostOption { get; }
    public Option<int> PortOption { get; }
    public Option<bool> SecureOption { get; }
    public Option<string> QueueOption { get; }
    public Option<string> UsernameOption { get; }
    public Option<string> PasswordOption { get; }
    public Option<bool> ExclusiveOption { get; }
    public Option<bool> DurableOption { get; }
    public Option<bool> AutoDeleteOption { get; }
    public Option<string> ExchangeOption { get; }

    protected override Options GetBoundValue(BindingContext bindingContext)
    {
        return new Options
        {
            Host = bindingContext.ParseResult.GetValueForOption(HostOption),
            Port = bindingContext.ParseResult.GetValueForOption(PortOption),
            Secure = bindingContext.ParseResult.GetValueForOption(SecureOption),
            Queue = bindingContext.ParseResult.GetValueForOption(QueueOption),
            Username = bindingContext.ParseResult.GetValueForOption(UsernameOption),
            Password = bindingContext.ParseResult.GetValueForOption(PasswordOption),
            Exclusive = bindingContext.ParseResult.GetValueForOption(ExclusiveOption),
            Exchange = bindingContext.ParseResult.GetValueForOption(ExchangeOption),
            Durable = bindingContext.ParseResult.GetValueForOption(DurableOption),
            AutoDelete = bindingContext.ParseResult.GetValueForOption(AutoDeleteOption)
        };
    }
}

public static class Extensions
{
    public static void AddGlobalOptions(this RootCommand root, params Option[] options)
    {
        foreach (var option in options)
        {
            root.AddGlobalOption(option);
        }
    }

    public static void AddCommands(this RootCommand root, params Command[] commands)
    {
        foreach (var command in commands)
        {
            root.AddCommand(command);
        }
    }
}



