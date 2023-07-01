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

    private Option<string> HostOption { get; }
    private Option<int> PortOption { get; }
    private Option<bool> SecureOption { get; }
    private Option<string> QueueOption { get; }
    private Option<string> UsernameOption { get; }
    private Option<string> PasswordOption { get; }
    private Option<bool> ExclusiveOption { get; }
    private Option<bool> DurableOption { get; }
    private Option<bool> AutoDeleteOption { get; }
    private Option<string> ExchangeOption { get; }
    private Option<string> VhostOption { get; }

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