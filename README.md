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


#### Interpreting the cum and flat values
In the top command output in the pprof tool, the "flat" and "cum" columns provide information about the amount of time that was spent in each function.

The "flat" column shows the total amount of time that was spent in a function itself, without considering any time spent in its callees. This includes both the time spent executing the function's own code, as well as any time spent waiting for other resources (such as I/O or locks).

The "cum" column, on the other hand, shows the total amount of time that was spent in a function and all of its callees. This gives a more comprehensive view of how much time is being spent in a particular function, since it takes into account both the function's own code and any time spent in its subroutines.

Here's an example to illustrate the difference between the "flat" and "cum" columns:

Suppose we have two functions, `foo()` and `bar()`, where `foo()` calls `bar()`. Suppose also that foo() takes 10ms to execute, and bar() takes 5ms to execute. In this case, the top command output might look something like this:


````
Showing nodes accounting for 15ms, 100% of 15ms total
Dropped 1 nodes (cum <= 0.07ms)
Showing top 2 nodes out of 2
flat  flat%   sum%        cum   cum%
10ms 66.67% 66.67%       15ms 100.00%  foo()
5ms 33.33% 100.00%       5ms  33.33%  bar()
````



In this example, the "flat" column for foo() shows 10ms, which is the total time spent executing foo() itself (without considering any time spent in bar()). The "cum" column for foo() shows 15ms, which is the total time spent in foo() and bar() combined.

Similarly, the "flat" column for bar() shows 5ms, which is the total time spent executing bar() itself (without considering any time spent in other functions). The "cum" column for bar() shows 5ms, which is the same as the "flat" column since bar() does not call any other functions in this example.

In general, the "cum" column is a more useful metric for identifying performance bottlenecks, since it gives a more comprehensive view of how much time is being spent in a particular function and its subroutines. However, the "flat" column can also be useful for identifying functions that have a high overhead, such as functions that spend a lot of time waiting for locks or performing I/O.


