using System.Text.Json;

namespace Snake
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Snake snake = new Snake()
            {
                // jmeno hada z konfiguracniho souboru appsettings.json
                Name = configuration.GetValue<string>("Snake:Name")
            };

            WebApplication app = WebApplication.Create(args);

            app.Map($"/move",
                (Game game) => JsonSerializer.Serialize(new { direction = snake.Move(game) }));
            app.Map($"/init",
                (Game game) => Results.Ok(snake.Init(game)));
            app.MapGet($"/",
                () => snake.Index());
            app.MapFallback(
                () => Results.NotFound("Not found"));

            app.Run();
        }
    }
}
