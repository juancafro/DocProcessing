using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DocProcessing.Entities.Requests
{
    class RequestMaker
    {
        string username = "Estratec";
        string password = "Estratec.2014Pwd";
        string direction = @"C:\Users\jsanchez\Documents\Financiera OH\prueba de estilos";
        Uri FileBuilderserviceUri = new Uri(@"https://in8veklqof.execute-api.us-east-1.amazonaws.com/dev/FB_Producer");


        public string MakePostPDFRequest(PDFDocumentRequestForm form,string autherAuthentication) {
            try {
                string auth = autherAuthentication;
                string content;
                using (HttpClient client = new HttpClient())
                {

                    content = JsonConvert.SerializeObject(form);
                    var buffer = Encoding.UTF8.GetBytes(content);
                    var bytesContent = new ByteArrayContent(buffer);
                    bytesContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes(auth)));
                    using (var result = client.PostAsync(FileBuilderserviceUri, bytesContent).Result)
                    {
                        return result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex) {
                throw;

            }
        }

    }
}
