using System.Diagnostics;
using System.Net.Http.Headers;
using MicroHeadRenderer.Types;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MicroHeadRenderer;

public class Server
{
    static SettingsHelper _settingsHelper = new SettingsHelper();
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(_settingsHelper.Settings.Port));
        
        WebApplication app = builder.Build();

        app.MapGet("/avatar/{playerToGetHead}.png", async context =>
        {
            try
            {
                string playerToGetHead = context.Request.RouteValues["playerToGetHead"]?.ToString();
                using HttpClient httpClient = new HttpClient();

                if (!Utils.IsUUID(playerToGetHead))
                {
                    string playerLookupJson = await httpClient.GetStringAsync($"{_settingsHelper.Settings.ServicesServer}/minecraft/profile/lookup/name/{playerToGetHead}");
                    UUIDLookup playerLookup = JsonConvert.DeserializeObject<UUIDLookup>(playerLookupJson);
                    playerToGetHead = playerLookup.id;
                }
                
                // fetch player info from session server
                string playerProfileUrl = $"{_settingsHelper.Settings.SessionServer}/session/minecraft/profile/{playerToGetHead}";
                string playerProfileJson = await httpClient.GetStringAsync(playerProfileUrl);
                
                // deserialize so we can actually work w/ it
                PlayerProfile playerProfileObj = JsonConvert.DeserializeObject<PlayerProfile>(playerProfileJson);
                PlayerTextures playerTextureObj = JsonConvert.DeserializeObject<PlayerTextures>(System.Text.Encoding.UTF8.GetString
                    (Convert.FromBase64String(playerProfileObj.properties
                        .First(prop => prop.name == "textures").value)));
                var skinUrl = playerTextureObj.textures.SKIN.url;
                
                // load image and manipulate
                await using Stream imageStream = await httpClient.GetStreamAsync(skinUrl);
                using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(imageStream);
            
                Rectangle hatLayerFront = new Rectangle(40, 8, 8, 8);
                Point headFrontPoint = new Point(8, 8);
                using (Image<Rgba32> hatLayerToCopy = image.Clone(ctx => ctx.Crop(hatLayerFront)))
                    image.Mutate(ctx => ctx.DrawImage(hatLayerToCopy, headFrontPoint, 1f));
                
                Rectangle crop = new Rectangle(8, 8, 8, 8);
                image.Mutate(ctx => ctx.Crop(crop));
                
                image.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(128, 128),
                    Sampler = KnownResamplers.NearestNeighbor
                }));
                
                // final head image is done. send it over to the client
                context.Response.Headers.Append("Content-Type", "image/png");
                using var outputStream = new MemoryStream();
                await image.SaveAsPngAsync(outputStream);
                outputStream.Position = 0;
                await outputStream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 500;
            }
        });
        
        app.Run();
    }
}