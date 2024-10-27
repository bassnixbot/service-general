using System.Net.Http.Headers;
using System.Security.Cryptography;
using GeneralServices.DB;
using GeneralServices.Models;
using Newtonsoft.Json;
using UtilsLib;

namespace GeneralServices.Services;

public static class Services
{
    public static async Task<ApiResponse<string>> BuildFill(ClientInfo request)
    {
        var response = new ApiResponse<string>() { responseType = ApiResponseType.reply };
        int maxwords = 500;
        var joinedWord = $"{request.message} ";
        var temp = "";
        var fillwords = "";

        if (joinedWord[0] == '!')
        {
            return response;
        }

        do
        {
            temp = string.Concat(temp, joinedWord);

            if (temp.Length < maxwords)
                fillwords = temp;
        } while (temp.Length < maxwords);

        response.result = fillwords;
        response.success = true;
        return response;
    }

    public static async Task<ApiResponse<List<string>>> BuildPyramid(ClientInfo request)
    {
        Console.WriteLine("Hit");
        var response = new ApiResponse<List<string>>()
        {
            success = false,
            responseType = UtilsLib.ApiResponseType.message_array
        };

        var message = request.message;
        List<string> messages = message.Split(' ').ToList();
        int pyramidSize = 0;
        var isSizeArgsExist = int.TryParse(messages[0], out pyramidSize);

        if (message[0] == '!')
        {
            return response;
        }

        // if there's no argument we put the default size at 3
        if (isSizeArgsExist)
        {
            messages.RemoveAt(0);
        }
        else
        {
            pyramidSize = 3;
        }

        message = string.Join(" ", messages);

        // prepare the vars that we gonna use

        int maxPyramid = 10;
        var messageLimit = 500;

        List<string> pyramid = new();

        if (pyramidSize > maxPyramid)
        {
            response.error = HandleError("1001");
            return response;
        }

        if (message.Length * pyramidSize >= messageLimit)
        {
            response.error = HandleError("1002");
            return response;
        }

        for (var i = 1; i <= pyramidSize; i++)
        {
            List<string> pyramidItem = Enumerable.Repeat(message, i).ToList();
            pyramid.Add(string.Concat(string.Join(" ", pyramidItem)));
        }

        for (var i = pyramidSize - 1; i > 0; i--)
        {
            List<string> pyramidItem = Enumerable.Repeat(message, i).ToList();
            pyramid.Add(string.Concat(string.Join(" ", pyramidItem)));
        }

        //TODO: set a cooldown logic

        response.success = true;
        response.result = pyramid;

        return response;
    }

    public static async Task<ApiResponse<string>> BuildTuck(ClientInfo request)
    {
        var response = new ApiResponse<string>
        {
            success = true,
            responseType = ApiResponseType.reply
        };

        var message = request.message;
        List<string> messages = message.Split(' ').ToList();

        string receiver = messages[0];
        messages.RemoveAt(0);
        message = string.Join(" ", messages);

        string tuckMessage = "";

        if (messages.Count() == 0)
        {
            tuckMessage = $"You tuck {receiver} to bed 👉 FumoTuck";
        }
        else
        {
            tuckMessage = $"You tuck {receiver} to bed 👉 {message}";
        }

        response.result = tuckMessage;

        return response;
    }

    public static async Task<ApiResponse<string>> BuildUrban(ClientInfo request)
    {
        var response = new ApiResponse<string>()
        {
            success = false,
            responseType = ApiResponseType.reply
        };

        var message = request.message;
        using (var client = new HttpClient())
        {
            var baseUrl = $"https://api.urbandictionary.com";

            client.BaseAddress = new Uri(baseUrl);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            // Make the GET request
            HttpResponseMessage apiResponse = await client.GetAsync($"/v0/define?term={message}");

            if (!apiResponse.IsSuccessStatusCode)
            {
                response.error = HandleError("1003");
                return response;
            }

            var data = await apiResponse.Content.ReadAsStringAsync();
            var dataList = JsonConvert.DeserializeObject<UrbanModel>(data);
            if (dataList.list.Count() == 0)
            {
                response.error = HandleError("1004");
                return response;
            }
            var orderedData = dataList
                .list.OrderByDescending(x => x.thumbs_up - x.thumbs_down)
                .ToList();

            var acceptedResult = orderedData.First();
            acceptedResult.definition = acceptedResult
                .definition.Replace("\"", "")
                .Replace("\n", "")
                .Replace("\r", "");

            var answer =
                $"{acceptedResult.word} - {acceptedResult.definition} (👍 {acceptedResult.thumbs_up} : 👎 {acceptedResult.thumbs_down})";

            response.result = answer;
            response.success = true;
        }
        return response;
    }

    public static async Task<ApiResponse<string>> BuildLink(
        ClientInfo request,
        ApplicationDBContext context
    )
    {
        var response = new ApiResponse<string>() { success = false };

        var newLink = new Link
        {
            recid = Guid.NewGuid(),
            username = request.userInfo.userName,
            chatterid = request.userInfo.userId,
            fromChannel = request.channel,
            message = request.message,
            savedateutc = DateTime.UtcNow
        };

        context.link.Add(newLink);
        context.SaveChanges();

        response.success = true;
        response.result =
            $"{request.userInfo.userName}'s link from {request.channel} channel has been successfully saved on {DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8))} (gmt+8)";

        return response;
    }

    public static async Task<ApiResponse<string>> PickAsync(ClientInfo request)
    { 
        var response = new ApiResponse<string>
        {
            success = true,
            responseType = ApiResponseType.reply
        };
 
        var message = request.message;
        List<string> messages = message.Split(' ').ToList();
        
        var messageCount = messages.Count;

        // Generate a random number using RandomNumberGenerator
        int randomPickIndex;
        using (var rng = RandomNumberGenerator.Create())
        {
            var randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            randomPickIndex = BitConverter.ToInt32(randomNumber, 0) % messageCount;

            // Ensure index is positive
            if (randomPickIndex < 0)
                randomPickIndex = -randomPickIndex;
        }

        var pick = messages[randomPickIndex];
        
        response.result = pick;
        return response; 
    }

    private static Error HandleError(string errorCode)
    {
        var errorList = UtilsClient.GetErrorList;
        var errorDetail = errorList
            .Where(x => x.errorCode == $"GeneralService-{errorCode}")
            .SingleOrDefault();

        if (errorDetail != null)
            return errorDetail;

        return new Error
        {
            errorMessage = "An unexpected error has been occured. Please try again later."
        };
    }
}
