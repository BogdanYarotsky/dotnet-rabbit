using System.CommandLine;
using System.CommandLine.Binding;

namespace Rabbit;

public class GlobalOptionsBinder : BinderBase<GlobalOptions>
{
    public GlobalOptionsBinder(Option<string> hostOption, Option<int> portOption, Option<bool> secureOption,
        Option<string> queueOption, Option<string> usernameOption, Option<string> passwordOption,
        Option<bool> exclusiveOption, Option<bool> durableOption, Option<bool> autoDeleteOption,
        Option<string> exchangeOption, Option<string> vhostOption)
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
        VhostOption = vhostOption;
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
    public Option<string> VhostOption { get; }

    protected override GlobalOptions GetBoundValue(BindingContext bindingContext)
    {
        return new GlobalOptions
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
            AutoDelete = bindingContext.ParseResult.GetValueForOption(AutoDeleteOption),
            VirtualHost = bindingContext.ParseResult.GetValueForOption(VhostOption),
        };
    }
}