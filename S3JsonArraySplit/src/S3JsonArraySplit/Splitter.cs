using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S3JsonArraySplit;

public class Splitter
{
    public void Run(SplitRequest Request)
    {
        string sourceBucketName = Request.Bucketname;
        string sourceKey = Request.Source;
        string destinationBucketName = Request.Bucketname;
        string splitterField = Request.Fieldname;
        string region = Request.Region;
        string defaultCategory = "unassigned";
        bool addContext = Request.AddContext;

        string sourceJson = ReadS3ObjectAsync(region, sourceBucketName, sourceKey).Result;

        SplitJsonByElementAndUpload(region, sourceJson, splitterField,
            defaultCategory, destinationBucketName, addContext, 
            Request.ResultPathPrefix, Request.FilenameSyntax);
    }

    static async Task<string> ReadS3ObjectAsync(string Region, string bucketName, string key)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        using (var client = new AmazonS3Client(region)) // Change the region if needed
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (GetObjectResponse response = await client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

    static void SplitJsonByElementAndUpload(string Region, string json, 
        string elementToSplitBy, string defaultCategory, 
            string destinationBucketName, bool AddContext, 
            string ResultPathPrefix, string FilenameSyntax)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        JArray jsonArray = JArray.Parse(json);
        Dictionary<string, JArray> splitDocuments = new Dictionary<string, JArray>();

        foreach (JObject item in jsonArray)
        {
            string elementValue = item[elementToSplitBy]?.ToString() ?? defaultCategory;

            if (!splitDocuments.ContainsKey(elementValue))
            {
                splitDocuments[elementValue] = new JArray();
            }

            splitDocuments[elementValue].Add(item);
        }

        using (var client = new AmazonS3Client(region)) // Change the region if needed
        {
            foreach (var kvp in splitDocuments)
            {
                string categoryName = kvp.Key;
//                string fileName = $"split_{categoryName}.json";

                string fileName = FilenameSyntax;
                fileName = fileName.Replace("{category}",categoryName);

                string jsonContent = null;

                if (AddContext)
                {
                    // Create metadata object
                    JObject metadata = new JObject();
                    metadata["category"] = categoryName; // Adding category value to metadata

                    // Create JSON object with metadata and data
                    JObject result = new JObject();
                    result["metadata"] = metadata;
                    result["data"] = kvp.Value;

                    jsonContent = result.ToString();
                }
                else
                {
                    jsonContent = kvp.Value.ToString();
                }

                string resultPath = ResultPathPrefix + fileName;

                // Upload split JSON files to S3
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = destinationBucketName,
                    Key = resultPath,
                    ContentBody = jsonContent
                };

                client.PutObjectAsync(uploadRequest).Wait(); // Using Wait for simplicity; consider using async/await in production
                Console.WriteLine($"Split JSON for '{elementToSplitBy}'='{categoryName}' uploaded to '{fileName}'");
            }
        }
    }
}