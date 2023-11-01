# Description:
  CLI tool to use RabbitMQ   

# Usage:
  dotnet-rabbit [command] [options]

# Options:
  --host <host>                   [default: localhost]
  
  --port <port>                   [default: 5672]
  
  -s, --secure                    [default: False]
  
  -q, --queue <queue> (REQUIRED)
  
  -u, --username <username>       []
  
  -p, --password <password>       []
  
  --exchange <exchange>           []
  
  -d, --durable                   [default: False]
  
  -a, --autodelete                [default: False]
  
  --exclusive                     [default: False]
  
  --version                       Show version information
  
  -?, -h, --help                  Show help and usage information

# Commands:
  
  publish    publish message to specified queue
  
  subscribe  Listen to all messages in a queue
  
  peek       Check the first message in specified queue
  
  pop        Read and remove the first message in a queue
