//importera namespaces

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;


namespace LabBild
{
    internal class Program
    {
        // Comp vision key and endpoint
        static string endpoint = "https://cogserviceforcompvision.cognitiveservices.azure.com/";
        static string key = "5ac5ebfd759f4af9b678b81cd1fd73e2";

        static async Task Main(string[] args)
        {
            // Skapa Client
            ComputerVisionClient client = Authenticate(endpoint, key);

            var inputUrl = "";
            // Användar val
            while (inputUrl.ToLower() != "quit")
            {
                Console.WriteLine("insert image url: ");
                inputUrl = Console.ReadLine();

                if (inputUrl == "quit")
                {
                    return;
                }

                else
                {
                    // Analysera en bild
                    await AnalyzeImageURL(client, inputUrl);
                }
            }  
        }

        // Authenticate
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        // Analyze
        public static async Task AnalyzeImageURL(ComputerVisionClient client, string imageUrl)
        {
            // Skapa en lista som definerar vad som ska tas från en bild.
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Brands
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analysera url bild.
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

            // Get image tags and confidence
            Console.WriteLine("Tags: ");
            Console.WriteLine();
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence.ToString("p")}");
                Console.WriteLine();
            }

            // Get image categories
            List<LandmarksModel> landmarks = new List<LandmarksModel> { };
            Console.WriteLine("Categories:");
            Console.WriteLine();
            foreach (var category in results.Categories)
            {
                // Print the category
                Console.WriteLine($" -{category.Name} (confidence: {category.Score.ToString("P")})");
                Console.WriteLine();

                // Get landmarks in this category
                if (category.Detail?.Landmarks != null)
                {
                    foreach (LandmarksModel landmark in category.Detail.Landmarks)
                    {
                        if (!landmarks.Any(item => item.Name == landmark.Name))
                        {
                            landmarks.Add(landmark);
                        }
                    }
                }
            }

            // Get brands in the image
            if (results.Brands.Count > 0)
            {
                Console.WriteLine("Brands:");
                Console.WriteLine();
                foreach (var brand in results.Brands)
                {
                    Console.WriteLine($" -{brand.Name} (confidence: {brand.Confidence.ToString("P")})");
                    Console.WriteLine();
                }
            }
        }
    }
}