using DocProcessing.Entities.Data;
using DocProcessing.Entities.OutLine;
using DocProcessing.Entities.Reader;
using DocProcessing.Entities.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nustache.Core;
using DinkToPdf;
using DocProcessing.Entities;

namespace DocProcessing
{
    class Program
    {
        static string templatePath = @"C:\Users\jsanchez\Documents\02052018-Financiera OH\prueba de estilos\index.html";
        static string tableTemplatePath = @"C:\Users\jsanchez\Documents\02052018-Financiera OH\prueba de estilos\SavingsRow.html";
        static string stylesTemplatePath = @"C:\Users\jsanchez\Documents\02052018-Financiera OH\prueba de estilos\style.css";
        static string OutLinePath = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\ExampleInputs\Estructura_EECC.XLSX";
        static string usersPath = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\ExampleInputs\cliente.txt";
        static string transactionsPath = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\ExampleInputs\transacciones.txt";
        static string savingsPath = @"C:\Users\jsanchez\Documents\02052018-Financiera OH\Bases EECC MAIL_CDD.MM\AHORROMAILINGS-C0418.csv";
        static string autherCred = "Estratec:Estratec.2014Pwd";
        static string tempfilespath = Path.Combine(Environment.CurrentDirectory, "transactionsTemp");
        public int currentgroup = 0;
        static void Main(string[] args)
        {
            try
            {

                List<string> MappedContents = new List<string>();
                for (int i = 0; i <= 3; i++) {
                    MappedContents.Add(File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\index.html"));
                };
                var pdfconverter = new BasicConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.LetterPlus,
                    Margins =  { Top = 0, Bottom = 0, Left = 0, Right = 0 }
                }
                };
                foreach (string Mappedcontent in MappedContents)
                {
                    string MapPage = Mappedcontent;
                    var page = new ObjectSettings()
                    {

                        PagesCount = true,
                        HtmlContent = MapPage,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 7, Right = "Page [page] of [toPage]", Line = false, Spacing = 0 }

                    };
                    doc.Objects.Add(page);
                }
                byte[] EeCcdocument = pdfconverter.Convert(doc);
                File.WriteAllBytes(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\pruebaBG2.pdf", EeCcdocument);

                int startRow = 3;
                int startColumn = 1;
                int UserSheet = 1;
                int TransactionSheet = 2;
                string userkeyField = "NumeroCuenta";
                string TransacctionkeyField = "NúmeroDeCuenta";
                string SavingsKeyColumn = "NUM_CUENTA_PMCP";
                string mailField = "CorreoElectrónico";
                char del = '|';
                StreamReader transactionStreamReader = new StreamReader(transactionsPath);
                //string html = File.ReadAllText(Path.Combine(Environment.CurrentDirectory + "\\mappedTemplates", "pageTemplatemapped.html"));
                int columnNameIndex = 0;
                //var pdfconverter = new BasicConverter(new PdfTools());
                //var doc = new HtmlToPdfDocument()
                //{
                //    GlobalSettings = {
                //        ColorMode = ColorMode.Color,
                //        Orientation = Orientation.Portrait,
                //        PaperSize = PaperKind.LetterPlus,
                //        Margins =  { Top = 0, Bottom = 0, Left = 0, Right = 0 }
                //    }
                //};
                //var page = new ObjectSettings()
                //{

                //    PagesCount = true,
                //    HtmlContent = html,
                //    WebSettings = { DefaultEncoding = "utf-8" },
                //    HeaderSettings = { FontSize = 7, Right = "Page [page] of [toPage]", Line = false, Spacing = 0 }

                //};
                //doc.Objects.Add(page);
                //var pdfdoc = pdfconverter.Convert(doc);
                //File.WriteAllBytes(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\prueba2.pdf", pdfdoc);



                //Obtener esquemas de cada trama

                LineReader userlineReader = new LineReader(GetStringOutlineFromXlxs(OutLinePath, startRow, startColumn, UserSheet));
                LineReader transactionlineReader = new LineReader(GetStringOutlineFromXlxs(OutLinePath, startRow, startColumn, TransactionSheet));
                CsvReader savingDataReader = GetCsvReader(savingsPath, columnNameIndex, del);
                List<ClientData> clients = new List<ClientData>();
                List<SavingsData> savingsData = new List<SavingsData>();
                List<TransactionData> transactions = new List<TransactionData>();
                string transactionLine;
                string line = "";
                StreamReader TransactionReader = new StreamReader(transactionsPath);

                Directory.CreateDirectory(tempfilespath);
                int currentGroup = 0;

                //codigo para leer y archivar objetos de transaccion en archivos temporales.
                using (StreamReader sr = new StreamReader(transactionsPath))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        TransactionData transaction = new TransactionData()
                        {
                            info = transactionlineReader.readLine(line)
                        };
                        transaction.AssociateAcount = transaction.info[TransacctionkeyField].ToString();
                        switch (transaction.info["Descripcióndelaautorización"].ToString())
                        {
                            case "SALDO MES ANTERIOR":
                                currentGroup++;
                                transaction.Group = currentGroup;
                                currentGroup++;
                                break;
                            case "CUOTAS DEL MES (VER DETALLE)":
                                currentGroup++;
                                transaction.Group = currentGroup;
                                currentGroup++;
                                break;
                            case "SEGURO DE DESGRAVAMEN":
                                transaction.Group = currentGroup;
                                currentGroup = currentGroup + 2;
                                break;
                            case "CUOTAS ADELANTADAS":
                                transaction.Group = currentGroup;
                                currentGroup++;
                                break;
                            case "TOTAL PAGO DE CUOTAS DEL MES":
                                transaction.Group = currentGroup;
                                currentGroup++;
                                break;
                            default:
                                transaction.Group = currentGroup;
                                break;
                        }
                        string serialized = JsonConvert.SerializeObject(transaction);
                        Console.WriteLine(serialized);
                        string subfolderPath = Path.Combine(tempfilespath, transaction.AssociateAcount);
                        Directory.CreateDirectory(subfolderPath);
                        string totalpath = Path.Combine(subfolderPath, transaction.AssociateAcount + "_transactions.txt");
                        File.AppendAllText(totalpath, serialized + Environment.NewLine);
                    }
                }

                //codigo para ensamblar objeto ClientData.
                using (StreamReader sr = new StreamReader(usersPath))
                {
                    line = sr.ReadLine();
                    ClientData client = new ClientData()
                    {
                        info = userlineReader.readLine(line),

                    };
                    string clientsr = client.ToString();
                    client.AccountNumber = client.info[userkeyField].ToString();
                    client.Email = client.info[mailField].ToString();
                    client.Transactions = getTransactions(client.AccountNumber);
                    GenerateEECCForClient(client);
                }


                //using (DisposableCsvReader reader = new DisposableCsvReader(savingsPath,'|', 0)) {
                //    while (!(reader.EndReached)) {
                //        Dictionary<string, object> information = reader.readLine();
                //        if (information != null) {
                //            SavingsData svi= new SavingsData() { info = information};
                //            svi.CustomerAccount = svi.info[SavingsKeyColumn].ToString();
                //        }
                //    }
                //}
                //GenerateSavingsDocument(accountSavingInfo);

            }
            catch (Exception ex) {


            }

        }



        private static void GenerateEECCForClient(ClientData client)
        {
            //Main templates files
            string Footer = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\footer.html";
            string Header = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\header.html";
            string Index = @"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\index.html";

            string CuoAdeHeader = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\CuoAdeHeader.txt", Encoding.UTF8);
            string CuoAdeFooter = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\CuoAdeFooter.txt", Encoding.UTF8);
            string CuoAdeLine = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\CuoAdeLine.txt", Encoding.UTF8);

            string DetMovHeader = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetMovMesHeader.txt", Encoding.UTF8);
            string DetMovFooter = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetMovMesFooter.txt", Encoding.UTF8);
            string DetMovLine = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetMovMesLine.txt",Encoding.UTF8);

            string DetPlanCuHeader = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetPlanCuHeader.txt", Encoding.UTF8);
            string DetPlanCuFooter = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetPlanCuFooter.txt", Encoding.UTF8);
            string DetPlanCuLine = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\DetPlanCuLine.txt", Encoding.UTF8);

            string SaldAntHeader = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\SaldAntHeader.txt", Encoding.UTF8);
            string SaldAntLine = File.ReadAllText(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\OhTemplatesV3\OhTemplates\LineTemplates\SaldAntLine.txt", Encoding.UTF8);

            //Loading Table Settings
            TableSettings tableContentSettings = new TableSettings()
            {
                GroupDelimiter = "",
                PaginationLines = 15
            };

            Dictionary<string, GroupSettings> groupSettings = new Dictionary<string, GroupSettings>();
            groupSettings.Add(Groups.group1.ToString(),new GroupSettings() { Header= SaldAntHeader, LineStyle = SaldAntLine, LastLineStyle = SaldAntLine});
            groupSettings.Add(Groups.group2.ToString(), new GroupSettings() { Header = DetMovHeader, LineStyle = DetMovLine, LastLineStyle = DetMovLine});
            groupSettings.Add(Groups.group3.ToString(), new GroupSettings() { Header = DetMovHeader, LineStyle = DetMovLine, LastLineStyle = DetMovLine,SimpleHeader = SaldAntHeader });
            groupSettings.Add(Groups.group4.ToString(), new GroupSettings() { Header = DetMovHeader, LineStyle = DetMovLine, LastLineStyle = DetMovLine, SimpleHeader = SaldAntHeader });
            groupSettings.Add(Groups.group5.ToString(), new GroupSettings() { Header = DetMovHeader, LineStyle = DetMovLine, LastLineStyle = DetMovLine, SimpleHeader = SaldAntHeader });
            groupSettings.Add(Groups.group6.ToString(), new GroupSettings() { Header = DetMovHeader, LineStyle = DetMovLine, LastLineStyle = DetMovFooter, SimpleHeader = SaldAntHeader });
            groupSettings.Add(Groups.group7.ToString(), new GroupSettings() { Header = DetPlanCuHeader, LineStyle = DetPlanCuLine, LastLineStyle = DetPlanCuFooter});
            groupSettings.Add(Groups.group8.ToString(), new GroupSettings() { Header = CuoAdeHeader, LineStyle = CuoAdeLine, LastLineStyle =CuoAdeFooter});
            tableContentSettings.Groups = groupSettings;
            //Extract Transaction info from objects
            Dictionary<string, object> HeaderData = client.info;
            List<Dictionary<string, object>> transactionsdata = new List<Dictionary<string, object>>();
            foreach (TransactionData td in client.Transactions)
            {
                td.info.Add("Group", td.Group);
                transactionsdata.Add(td.info);
            }


            var pdfconverter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.LetterPlus,
                    Margins =  { Top = 0, Bottom = 0, Left = 0, Right = 0 }
                }
            };


            //map information
            string MappedHeaderpath = mapData(Header, HeaderData, "HeaderInfo");
            string MappedFooterpath = mapData(Footer, HeaderData, "FooterInfo");
            List<string> MappedContents = mapTableContent(transactionsdata, tableContentSettings);
            foreach (string Mappedcontent in MappedContents) {
                string MapPage = File.ReadAllText(mapPage(MappedHeaderpath, Mappedcontent, MappedFooterpath, Index));
                var page = new ObjectSettings()
                {

                    PagesCount = true,
                    HtmlContent = MapPage,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 7, Right = "Page [page] of [toPage]", Line = false, Spacing = 0 }

                };
                doc.Objects.Add(page);
            }
            byte[] EeCcdocument = pdfconverter.Convert(doc);
            File.WriteAllBytes(@"C:\Users\jsanchez\source\repos\DocProcessing\DocProcessing\"+client.AccountNumber+".pdf",EeCcdocument);
        }

        private static List<string> mapTableContent(List<Dictionary<string, object>> values, TableSettings tableSettings)
        {
            //Dividir
            string dir = Path.Combine(Environment.CurrentDirectory, "mappedTemplates");
            List<Dictionary<string, object>> Group1 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group2 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group3 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group4 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group5 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group6 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group7 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> Group8 = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> transactionInf in values)
            {
                switch (Int32.Parse(transactionInf["Group"].ToString()))
                {
                    case 1:
                        Group1.Add(transactionInf);
                        break;
                    case 2:
                        Group2.Add(transactionInf);
                        break;
                    case 3:
                        Group3.Add(transactionInf);
                        break;
                    case 4:
                        Group4.Add(transactionInf);
                        break;
                    case 5:
                        Group5.Add(transactionInf);
                        break;
                    case 6:
                        Group6.Add(transactionInf);
                        break;
                    case 7:
                        Group7.Add(transactionInf);
                        break;
                    case 8:
                        Group8.Add(transactionInf);
                        break;
                }
            }
            List<List<Dictionary<string, object>>> SplitedTransactions = new List<List<Dictionary<string, object>>>();
            SplitedTransactions.Add(Group1);
            SplitedTransactions.Add(Group2);
            SplitedTransactions.Add(Group3);
            SplitedTransactions.Add(Group4);
            SplitedTransactions.Add(Group5);
            SplitedTransactions.Add(Group6);
            SplitedTransactions.Add(Group7);
            SplitedTransactions.Add(Group8);

            List<string> parsedHtmlPages = new List<string>();
            string html = "";
            int lines = 0;
            foreach (List<Dictionary<string, object>> GroupTransactions in SplitedTransactions) {

                if (GroupTransactions.Count > 0) {
                    string group = GroupTransactions[0]["Group"].ToString();
                    GroupSettings groupSettings = tableSettings.Groups["group" + group];
                    if ((lines + 2) == tableSettings.PaginationLines)
                    {
                        html += groupSettings.EndOfPageStyle;
                        parsedHtmlPages.Add(html);
                        html = "";
                        lines = 0;
                    }
                    else{
                        if (groupSettings.SimpleHeader != null)
                        {
                            html += groupSettings.SimpleHeader;
                        }
                        else
                        {
                            html += groupSettings.Header;
                        }
                    }
                    for (int i = 0; i < GroupTransactions.Count; i++)
                    {
                        if (i != (GroupTransactions.Count - 1))
                        {
                            html += Render.StringToString(groupSettings.LineStyle, GroupTransactions[i]) + Environment.NewLine;
                        }
                        else
                        {
                            html += Render.StringToString(groupSettings.LastLineStyle, GroupTransactions[i]) + Environment.NewLine;
                        }
                        lines++;
                        if (lines == tableSettings.PaginationLines)
                        {
                            html += groupSettings.EndOfPageStyle;
                            parsedHtmlPages.Add(html);
                            html = "";
                            html += groupSettings.Header + Environment.NewLine;
                            lines = 0;
                        }
                    }

                }

            }
            return parsedHtmlPages;
        }

        public static string mapPage(string header, string content, string footer,string pageTemplate) {

            string headerstr = File.ReadAllText(header);
            string contentstr = content;
            string footerstr =  File.ReadAllText(footer);

            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "header",headerstr  },
                { "content",contentstr },
                { "footer",footerstr }
            };
            string outputPage = mapData(pageTemplate, values, "pageTemplate");

            return outputPage;
        }

        public static string mapData(string Template, Dictionary<string, object> data, string name)
        {
            DirectoryInfo dir=Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "mappedTemplates"));
            string outputPages = dir.FullName +"\\"+name+"mapped.html";
            Render.FileToFile(Template, data, outputPages);
            return outputPages;
        }

        public static StringOutLine GetStringOutlineFromXlxs(string path,int startRow, int startColumn, int sheet)
        {
            StringOutLine Outline = new StringOutLine(path,startRow,startColumn,sheet);
            return Outline;
        }

        public static CsvReader GetCsvReader(string path, int columnsNameRow,char delimiter) {
            List<string> KeyList = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                int count = 0;
                string line = "";
                while ((line = reader.ReadLine()) != null & count<=columnsNameRow) {
                    if (count == columnsNameRow) {
                        string[] keys=line.Split(delimiter);
                        for (int i = 0; i < keys.Length; i++) {
                            KeyList.Add(keys[i]);
                        }
                    }
                    count++;
                }
            }
            return new CsvReader(KeyList, delimiter);
        }

        public static string FormatFromDictionary(string formatString, Dictionary<string, object> ValueDict)
        {
            int i = 0;
            StringBuilder newFormatString = new StringBuilder(formatString);
            Dictionary<string, int> keyToInt = new Dictionary<string, int>();
            foreach (var tuple in ValueDict)
            {
                newFormatString = newFormatString.Replace("{" + tuple.Key + "}", "{" + i.ToString() + "}");
                keyToInt.Add(tuple.Key, i);
                i++;
            }
            return String.Format(newFormatString.ToString(), ValueDict.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray());
        }

        public static string MakeTable(List<SavingsData> accountSavingInfo,string tableTemplate) {
            string table = $"";
            foreach (SavingsData sb in accountSavingInfo)
            {
                string retail = sb.info["RETAIL"].ToString();
                string date = sb.info["FECHA"].ToString();
                string tip_tarjeta = sb.info["TIP_TARJETA"].ToString();
                string nom_producto = sb.info["NOM_PRODUCTO"].ToString();
                string cant = sb.info["CANT"].ToString();
                string p_regular = sb.info["P_REGULAR"].ToString();
                string precio_oh = sb.info["PRECIO_OH"].ToString();
                string ahorro = sb.info["AHORRO"].ToString();
                string format = String.Format(tableTemplate, retail, date, tip_tarjeta, nom_producto, cant, p_regular, precio_oh, ahorro);
                table += format;
            }
            return table;
        }

        public static string MakePDF(string completeTemplate,List<object> sourceData,string credentials) {
            PDFDocumentRequestForm form = new PDFDocumentRequestForm()
            {
                TemplateHtml = completeTemplate,
                SourceData = sourceData,
                TemplateType = "Mustache",
                IdentifierField = "NUM_CUENTA_PMCP",
                TagList = "financieraOh,carta,ahorros"
            };
            JsonConvert.SerializeObject(form);
            RequestMaker requestMaker = new RequestMaker();
            return requestMaker.MakePostPDFRequest(form, credentials);
        }

        public static List<TransactionData> getTransactions(string id) {
            List<TransactionData> transactions = new List<TransactionData>();
            string line;
            string sub_folderpath = Path.Combine(tempfilespath, id);
            using (StreamReader sr = new StreamReader(Path.Combine(sub_folderpath, id + "_transactions.txt")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    transactions.Add(JsonConvert.DeserializeObject<TransactionData>(line));
                }
            }
            File.Delete(Path.Combine(tempfilespath, id + ".txt"));
            return transactions;
        }
    }
}
