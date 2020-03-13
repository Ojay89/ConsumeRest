using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ModelLib.Model;
using Newtonsoft.Json;

namespace ConsumeRest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Item Consumer Ready to run");
            Console.WriteLine("Press a key to get all items");
            Console.ReadKey();
            string ItemWebApi = "https://restitemserviceitem.azurewebsites.net/api/items";

            GetAndPrintItems(ItemWebApi);
            PostNewItem();
            Console.ReadLine();
        }

        private static void GetAndPrintItems(string ItemWebApi)
        {
            Console.WriteLine("*****Get All Items*****");
            List<Item> items = new List<Item>();
            try
            {
                Task<List<Item>> callTask = Task.Run(() => GetItems(ItemWebApi));
                callTask.Wait();
                items = callTask.Result;
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine(items[i].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<List<Item>> GetItems(string itemWebApi)
        {
            using (HttpClient client = new HttpClient())
            {
                string eventsJsonString = await client.GetStringAsync(itemWebApi);
                if (eventsJsonString != null)
                    return (List<Item>) JsonConvert.DeserializeObject(eventsJsonString, typeof(List<Item>));
                return null;
            }
        }

        private static void PostNewItem()
        {
            Console.WriteLine("*****Post new Item*****");
            List<Item> items = new List<Item>();
            try
            {
                Task<List<Item>> callTask = Task.Run(() => PostItemHttpTask());
                callTask.Wait();
                items = callTask.Result;
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine(items[i].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<List<Item>> PostItemHttpTask()
        {
            string ItemWebApiBase = "https://restitemserviceitem.azurewebsites.net/api/items";
            Item newItem = new Item(6, "mel", "High", 100);

            using (HttpClient client = new HttpClient())
            {
                string newItemJson = JsonConvert.SerializeObject(newItem);
                var content = new StringContent(newItemJson, Encoding.UTF8, ("application/json"));
                client.BaseAddress = new Uri(ItemWebApiBase);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync("api/localitems", content);

                Console.WriteLine("*****An item posted to service*****");
                Console.WriteLine("***** Response is" + response + "*****");
                response.EnsureSuccessStatusCode();
                var httpResponseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(httpResponseBody);
            }

            Console.WriteLine("*****Get all items for verification*****");
            string ItemWebApi = "https://restitemserviceitem.azurewebsites.net/api/items";
            return await GetItems(ItemWebApi);
        }

    }
}
