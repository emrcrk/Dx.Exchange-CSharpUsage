using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net; 

namespace DxExchangeUsage
{
    class DxInstrumentAsset
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("fullName")]
        public string fullName;
        [JsonProperty("assetTypeId")]
        public int assetTypeId;
        [JsonProperty("baseCurrencyId")]
        public int baseCurrencyId;
        [JsonProperty("quotedCurrencyId")]
        public int quotedCurrencyId;
        [JsonProperty("tailDigits")]
        public int tailDigits;
        [JsonProperty("significantDigit")]
        public int significantDigit;
        [JsonProperty("status")]
        public string status;
    }
    class DxInstrumentsValue
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("productId")]
        public int productId;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("symbol")]
        public string symbol;
        [JsonProperty("assetId")]
        public int assetId;
        [JsonProperty("statusId")]
        public int statusId;
        [JsonProperty("dateInserted")]
        public int dateInserted;
        [JsonProperty("dateUpdated")]
        public int dateUpdated;
        [JsonProperty("asset")]
        public DxInstrumentAsset asset;
        [JsonProperty("minOrderQuantity")]
        public double minOrderQuantity;
        [JsonProperty("maxOrderQuantity")]
        public double maxOrderQuantity;
        [JsonProperty("meQuantityMultiplier")]
        public double meQuantityMultiplier;
        [JsonProperty("initialRate")]
        public double initialRate;

    }
    class DxLoginByTokenValue
    {
        [JsonProperty("token")]
        public string token;
        [JsonProperty("expiry")]
        public int expiry;
    }
    class DxReturnValue
    {
        [JsonProperty("id")]
        public string id;
        [JsonProperty("result")]
        public object result;
        [JsonProperty("error")]
        public string error;
    }
    class DxChangeAPI
    {
        string MainURI { get; set; }

        public string APIKey { get; set; }
        public string SecretKey { get; set; }

        public string Token { get; private set; }
        public string Error { get; private set; }
        public DxInstrumentsValue TickerData { get; private set; }

        public DxChangeAPI()
        {
            MainURI = "https://acl.dx.exchange";
        }

        public bool Logout()
        {
            var request = CreateHttpWebRequest("POST", MainURI);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", this.Token);


            var data = new Dictionary<string, object>();
            var parameter = new List<Dictionary<string, string>>();
            var parameters = new Dictionary<string, string>();
            parameter.Add(parameters);

            data.Add("jsonrpc", "2.0");
            data.Add("id", "1");
            data.Add("method", "Authorization.Logout");
            data.Add("params", parameter);
            var entity = JsonConvert.SerializeObject(data, Formatting.Indented);



            request.ContentLength = entity.Length;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(entity);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var jsonResult = JsonObjectSerialize.ObjectSerialize<DxReturnValue>(GetResponseString(request));
            Token = "";
            if (jsonResult.error != null)
            {
                Error = jsonResult.error;
                return false;
            }
            return true;
        }
        public bool Login()
        {
            var request = CreateHttpWebRequest("POST", MainURI);
            request.ContentType = "application/json";

            var data = new Dictionary<string, object>();
            var parameter = new List<Dictionary<string, string>>();
            var parameters = new Dictionary<string, string>();
            parameters.Add("token", this.APIKey);
            parameters.Add("secret", this.SecretKey);
            parameter.Add(parameters);


            data.Add("jsonrpc", "2.0");
            data.Add("id", "1");
            data.Add("method", "Authorization.LoginByToken");
            data.Add("params", parameter);
            var entity = JsonConvert.SerializeObject(data, Formatting.Indented);



            request.ContentLength = entity.Length;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(entity);
                streamWriter.Flush();
                streamWriter.Close();
            }



            var jsonResult = JsonObjectSerialize.ObjectSerialize<DxReturnValue>(GetResponseString(request));
            var tokenResult = JsonObjectSerialize.ObjectSerialize<DxLoginByTokenValue>(jsonResult.result.ToString());

            Token = tokenResult.token;
            if (jsonResult.error != null)
            {
                Error = jsonResult.error;
                return false;
            }
            return true;
        }

        public bool GetTicker(string ticker)
        {
            TickerData = new DxInstrumentsValue();
            var request = CreateHttpWebRequest("POST", MainURI);
            request.ContentType = "application/json";
            //request.Headers.Add("Authorization", this.Token);

            var data = new Dictionary<string, object>();
            var parameter = new List<Dictionary<string, string>>();
            var parameters = new Dictionary<string, string>();
            parameters.Add("symbol", ticker);
            parameter.Add(parameters);


            data.Add("jsonrpc", "2.0");
            data.Add("id", "1");
            data.Add("method", "AssetManagement.GetInstruments");
            data.Add("params", parameter);
            var entity = JsonConvert.SerializeObject(data, Formatting.Indented);



            request.ContentLength = entity.Length;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(entity);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var jsonResult = JsonObjectSerialize.ObjectSerialize<DxReturnValue>(GetResponseString(request));
            var tickerResult = JsonObjectSerialize.ObjectSerialize<Dictionary<string, List<DxInstrumentsValue>>>(jsonResult.result.ToString());

            if (jsonResult.error != null)
            {
                Error = jsonResult.error;
                return false;
            }
            else
            {
                if (tickerResult["instruments"] != null)
                {
                    TickerData = tickerResult["instruments"].First();
                }
                else
                {
                    Error = "Asset not found!";
                    return false;
                }

            }
            return true;
        }

        private HttpWebRequest CreateHttpWebRequest(string method, string relativeUrl)
        {
            var request = WebRequest.CreateHttp(relativeUrl);

            request.Method = method;

            return request;
        }

        string GetResponseString(HttpWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

    }

    class JsonObjectSerialize
    {
        public static T ObjectSerialize<T>(string jsonString)
        {
            JsonSerializer JsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };


            using (var stringReader = new StringReader(jsonString))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    var data = JsonSerializer.Deserialize(jsonTextReader, typeof(T));
                    return (T)data;
                }
            }
        }
    }
}
