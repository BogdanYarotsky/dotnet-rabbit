namespace Rabbit;

public class GlobalOptions
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
    public string? VirtualHost { get; set; }
}