var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.MapControllerRoute(
    name:"default", 
    pattern:"{controller}/{action}/{id?}");

//app.UseEndpoints(enpoints =>
//{
//    enpoints.MapControllerRoute("default", "{controller}/{action}");
//});

app.Run();
