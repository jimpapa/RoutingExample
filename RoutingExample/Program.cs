using RoutingExample.CustomConstraints;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("months", typeof(MonthsCustomConstraint));
});

var app = builder.Build();

//enable Routing
app.UseRouting();

//app.Use(async (context, next) =>
//{
//    Microsoft.AspNetCore.Http.Endpoint? endPoint = context.GetEndpoint();
//    if (endPoint != null)
//    {
//        await context.Response.WriteAsync($"Endpoint: {endPoint.DisplayName}\n");
//    }
//    await next(context);
//});

//creating end points
app.UseEndpoints(endpoints =>
{
    //add your end points
    endpoints.Map("files/{filename}.{extension=txt}", async (context) =>
    {
        string? filename = Convert.ToString(context.Request.RouteValues["filename"]);
        string? extension = Convert.ToString(context.Request.RouteValues["extension"]);
        await context.Response.WriteAsync($"In files - {filename} - {extension}");
    });

    endpoints.Map("employee/profile/{EmployeeName:length(4,7):alpha=smith}", async (context) =>
    {
        string? EmployeeName = Convert.ToString(context.Request.RouteValues["EmployeeName"]);
        await context.Response.WriteAsync($"In Employee profile - {EmployeeName}");
    });

    //Eg: products/details/1
    endpoints.Map("products/details/{id:int:range(1,1000)?}", async (context) =>
    {
        if (context.Request.RouteValues.ContainsKey("id"))
        {
            int? id = Convert.ToInt32(context.Request.RouteValues["id"]);
            await context.Response.WriteAsync($"In products details - {id}");
        } else
        {
            await context.Response.WriteAsync("In products details - id is not supplied");
        }
    });

    //Eg: daily-digest-report/{reportdate}
    endpoints.Map("daily-digest-report/{reportdate:datetime?}", async (context) =>
    {
        if (context.Request.RouteValues.ContainsKey("reportdate"))
        {
            DateTime reportDate = Convert.ToDateTime(context.Request.RouteValues["reportdate"]);
            await context.Response.WriteAsync($"In daily-digest-report - {reportDate.ToShortDateString()}");
        }
        else
        {
            await context.Response.WriteAsync("daily-digest-report - reportdate is not supplied");
        }
    });

    //Eg: cities/{cityid}
    endpoints.Map("cities/{cityid:guid?}", async (context) =>
    {
        if (context.Request.RouteValues.ContainsKey("cityid"))
        {
            Guid cityId = Guid.Parse(Convert.ToString(context.Request.RouteValues["cityid"])!);
            await context.Response.WriteAsync($"In cities Information - {cityId}");
        }
        else
        {
            await context.Response.WriteAsync("In cities Information - cityid is not supplied");
        }
    });

    //sales-report/2024/jan
    endpoints.Map("sales-report/{year:int:min(1900)}/{month:months}", async context =>
    {    
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);
        string? month = Convert.ToString(context.Request.RouteValues["month"]);
        if (month == "apr" || month == "jul" || month == "oct" || month == "jan")
        {
            await context.Response.WriteAsync($"Request received at {context.Request.Path}");
        } else
        {
            await context.Response.WriteAsync($"{month} is not allowed for sales report");
        }
    });

    //sales-report/2024/jan
    endpoints.Map("sales-report/2024/jan", async context =>
    {
        await context.Response.WriteAsync($"sales-report/2024/jan");
    });

});



app.Run(async context =>
{
    await context.Response.WriteAsync($"No Route Matched at {context.Request.Path}");
});

app.Run();
