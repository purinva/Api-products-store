using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Api.Service.Storage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string serviceURL;
        private readonly string region;
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string bucketName;
        private readonly HttpClient httpClient;

        public FileStorageService(IOptions<TimeWebSettings> options)
        {
            this.serviceURL = options.Value.ServiceURL;
            this.region = options.Value.Region;
            this.accessKey = options.Value.AccessKey;
            this.secretKey = options.Value.SecretKey;
            this.bucketName = options.Value.BucketName;
            this.httpClient = new HttpClient();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string fileName = GenerateUniqueFileName(file.FileName);
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                byte[] fileContent = memoryStream.ToArray();

                string requestUri = $"{serviceURL}/{bucketName}/{fileName}";
                string host = new Uri(serviceURL).Host;
                string date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
                string dateShort = DateTime.UtcNow.ToString("yyyyMMdd");

                string canonicalRequest = $"PUT\n/{bucketName}/{fileName}\n\nhost:{host}\nx-amz-content-sha256:{ToHexString(HashSHA256(fileContent))}\nx-amz-date:{date}\n\nhost;x-amz-content-sha256;x-amz-date\n{ToHexString(HashSHA256(fileContent))}";

                string stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{dateShort}/{region}/s3/aws4_request\n{ToHexString(HashSHA256(Encoding.UTF8.GetBytes(canonicalRequest)))}";

                byte[] signingKey = GetSignatureKey(secretKey, dateShort, region, "s3");

                string signature = ToHexString(HMACSHA256(signingKey, stringToSign));

                using (var request = new HttpRequestMessage(HttpMethod.Put, requestUri))
                {
                    request.Headers.Host = host;
                    request.Headers.Add("x-amz-date", date);
                    request.Headers.Add("x-amz-content-sha256", ToHexString(HashSHA256(fileContent)));
                    request.Headers.Authorization = new AuthenticationHeaderValue("AWS4-HMAC-SHA256", $"Credential={accessKey}/{dateShort}/{region}/s3/aws4_request, SignedHeaders=host;x-amz-content-sha256;x-amz-date, Signature={signature}");
                    request.Content = new ByteArrayContent(fileContent);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return $"{serviceURL}/{bucketName}/{fileName}";
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error uploading file: {response.StatusCode}, Response: {responseBody}");
                    }
                }
            }
        }

        public async Task<bool> RemoveFileAsync(string fileName)
        {
            string requestUri = $"{serviceURL}/{bucketName}/{fileName}";
            string host = new Uri(serviceURL).Host;
            string date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            string dateShort = DateTime.UtcNow.ToString("yyyyMMdd");

            string canonicalRequest = $"DELETE\n/{bucketName}/{fileName}\n\nhost:{host}\nx-amz-date:{date}\n\nhost;x-amz-date\nUNSIGNED-PAYLOAD";

            string stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{dateShort}/{region}/s3/aws4_request\n{ToHexString(HashSHA256(Encoding.UTF8.GetBytes(canonicalRequest)))}";

            byte[] signingKey = GetSignatureKey(secretKey, dateShort, region, "s3");

            string signature = ToHexString(HMACSHA256(signingKey, stringToSign));

            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
            {
                request.Headers.Host = host;
                request.Headers.Add("x-amz-date", date);
                request.Headers.Authorization = new AuthenticationHeaderValue("AWS4-HMAC-SHA256", $"Credential={accessKey}/{dateShort}/{region}/s3/aws4_request, SignedHeaders=host;x-amz-date, Signature={signature}");

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error deleting file: {response.StatusCode}, Response: {responseBody}");
                }
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName);

            string uniqueId = GetTransliteration(fileNameWithoutExtension);

            string guid = Guid.NewGuid().ToString().Replace("-", "");
            uniqueId = Regex.Replace(uniqueId, @"[^a-zA-Z0-9]", "");

            uniqueId = uniqueId.Substring(0, Math.Min(uniqueId.Length, 10));

            return $"{uniqueId}_{guid}{extension}";
        }

        private string GetTransliteration(string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static byte[] HashSHA256(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        private static byte[] HMACSHA256(byte[] key, string data)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kDate = HMACSHA256(Encoding.UTF8.GetBytes("AWS4" + key), dateStamp);
            byte[] kRegion = HMACSHA256(kDate, regionName);
            byte[] kService = HMACSHA256(kRegion, serviceName);
            return HMACSHA256(kService, "aws4_request");
        }

        private static string ToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}