using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    // YouTube API Key and channel ID
    static string apiKey = "AIzaSyD54i9WJVPJbsMRTgJd25KKkXJd6onCSRA";
    static string channelId = "UCOgUcv_9DaivXzsN9mcHIzQ";
    static string lastVideoId = "-8cEpMwyugA";

    static async Task Main()
    {
        while (true)
        {
            await CheckUploads();
            await Task.Delay(TimeSpan.FromMinutes(5)); // check every 5 minutes
        }
    }

    static async Task CheckUploads()
    {
        var youtube = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "UploadChecker"
        });

        var request = youtube.Search.List("snippet");
        request.ChannelId = channelId;
        request.Order = SearchResource.ListRequest.OrderEnum.Date;
        request.MaxResults = 1;

        var response = await request.ExecuteAsync();
        var video = response.Items.FirstOrDefault();

        if (video != null && video.Id.VideoId != lastVideoId && video.Id.VideoId != "-8cEpMwyugA" )
        {
            lastVideoId = video.Id.VideoId;

            string thumbnail = video.Snippet.Thumbnails.High.Url;
            string title = video.Snippet.Title;

            Console.WriteLine("New video detected: " + title);

            await SendWhatsAppAlertAsync(title, thumbnail);
        }
    }

    static async Task SendWhatsAppAlertAsync(string title, string imageUrl)
    {
        string accountSid = "AC7ab9b0a4aa060c1f82b0265ae54b435d";
        string authToken = "b0821330e7863e6a9f77bdec0cc27e1a";

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
        {
            Console.WriteLine("Twilio credentials missing. Set environment variables.");
            return;
        }

        TwilioClient.Init(accountSid, authToken);

        string[] friends =
        {
            "whatsapp:+353872445341",

            
        };

        foreach (var friend in friends)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    from: new PhoneNumber("whatsapp:+14155238886"), // Twilio sandbox
                    to: new PhoneNumber(friend),
                    body: "NEW TGF VIDEO\n\n" + title,
                    mediaUrl: new List<Uri> { new Uri(imageUrl) }
                );

                Console.WriteLine($"Message sent to {friend}. SID: {message.Sid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to {friend}: {ex.Message}");
            }
        }
    }
}