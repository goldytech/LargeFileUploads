This repo shows how to do a large file uploads in AspNet Core 7. It also showcases how to configure Dapr for the same.

#### AspNet Core 7 Configuration
The default payload size for AspNet Core 7 is 30MB. To increase this, add the following to the `Program.cs` file:

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
});
```
[Reference](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-7.0#maximum-request-body-size)

#### Dapr Configuration
To enable large file uploads and profiling in Dapr enable this flag in the `daprd` command:
    
    
    --enable-profiling --profile-port 7777 --dapr-http-max-request-size 500
    
[Dapr Max Request Size](https://docs.dapr.io/operations/configuration/increase-request-size/)

[Dapr Enable Profiling](https://docs.dapr.io/operations/troubleshooting/profiling-debugging/)

#### Pprof Profiling commands
To profile the Dapr sidecar, use the following commands:
    
    
    go tool pprof  http://localhost:7777/debug/pprof/heap
    go tool pprof  http://localhost:7777/debug/pprof/profile
    go tool pprof  http://localhost:7777/debug/pprof/trace

After this you can use the `top` command (in pprof prompt) to see the top 10 functions that are consuming the most memory.

To output the report in pdf format (in pprof prompt), use the following command:
    
    
    pdf > report.pdf

To exit the pprof prompt, use the following command:
    
    
    quit or exit


