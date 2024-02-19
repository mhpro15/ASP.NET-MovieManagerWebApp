using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Lab3.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Amazon.SimpleSystemsManagement;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
//add systems manager

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();
builder.WebHost.ConfigureAppConfiguration(
				c => {
					c.AddSystemsManager(source => {
						source.Path = "/lab3";
						source.ReloadAfter =
							TimeSpan.FromMinutes(10);
					});
				}
			);
var sBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("AuthConnectionStrings") ?? throw new InvalidOperationException("Connection string 'AuthConnectionStrings' not found."));
sBuilder.UserID = builder.Configuration["DbUser"];
sBuilder.Password = builder.Configuration["DbPassword"];
var connection = sBuilder.ConnectionString;
builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(connection));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<AuthDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
