using HtmlAgilityPack;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RozetkaParser
{
    internal partial class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            await ParseRozetka("https://hard.rozetka.com.ua/ua/mouses/c80172", Enumerable.Range(1, 5));
        }

        static async Task<string?> Get(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public static void ExportToExcel<T>(List<T> data, string filePath, string fileName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add headers
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Add data
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(data[row]);
                    }
                }

                // Save to file
                var fp = new FileInfo($"{filePath}/{fileName}.xlsx");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                package.SaveAs(fp);
            }
        }

        public static async Task ParseRozetka(string baseURL, IEnumerable<int> pages, string excelFilePath = "./excel-files", string excelFileName = "")
        {
            if (excelFileName.IsNullOrEmpty())
                excelFileName = $"file-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options);

            ProductsData productsData = new()
            {
                Data = new List<Product>() { }
            };

            string IDs = "";

            int itemIndex = 0;

            foreach (int page in pages)
            {
                driver.Navigate().GoToUrl(baseURL + "/page=$" + page);

                Thread.Sleep(2000);

                string htmlSource = driver.PageSource;

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlSource);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='g-id display-none']");

                if (nodes != null)
                {
                    Console.WriteLine("Product IDs:");
                    foreach (var node in nodes)
                    {
                        Console.WriteLine(node.InnerText);
                        IDs += $"{node.InnerText},";
                    }
                }
                else
                {
                    Console.WriteLine("No nodes found with the specified class name.");
                }

                string xhrURL = $"https://xl-catalog-api.rozetka.com.ua/v4/goods/getDetails?country=UA&lang=ua&with_groups=1&with_docket=1&with_extra_info=1&goods_group_href=1&product_ids={IDs}";
                Console.WriteLine("Calling --- " + xhrURL);
                string? rozetkaResponse = await Get(xhrURL);
                Console.WriteLine("Success");

                ProductsData? productsDataTemp = JsonConvert.DeserializeObject<ProductsData>(rozetkaResponse);

                foreach (Product item in productsDataTemp.Data)
                {
                    productsData.Data.Add(item);
                }

                IDs = "";
            }

            using (var context = new AppDbContext())
            {
                if (productsData.Data.Count > 0)
                {
                    ExportToExcel<Product>(productsData.Data, excelFilePath, excelFileName);

                    await context.AddAsync(productsData);
                }
                await context.SaveChangesAsync();
            }

            Console.WriteLine(productsData.Data[0].Title);
        }
    }
}
