using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();
            Random rnd = new Random();
            DateTime date = new DateTime();

            string url = "https://fonwall.ru/search?q=";
            string url_copy = null;
            string url_test;
            string url_detail_pdoto = null;
            string url_img = null;
            string htmlParser = null;
            string date_now = null;
            string[] arSearchErr = { "search__no__rusult", "К сожалению, фотографий не нашлось! Вот несколько советов как эффективно найти фото:" };
            string search_href = "class=\"js-photo-link\"";
            string search_img = "class=\"image-section__image\"";
            string htmlStr = null;
            string[] strmas;
            string path = null;
            bool search_html = false; // ПРОВЕРКА ПОИСКА СТРОКИ В СТРАНИЦЕ
            int size = rnd.Next(1, 10);
            int schetchek = 1;

            do
            {
                url_copy = url + Word(); //+ RandomString(size);
                url_test = url_copy;
                //System.Diagnostics.Process.Start(url_copy); //открытие страницы в браузере
                htmlParser = Download(url_copy);
                //Console.WriteLine(htmlParser);
                foreach(string str in arSearchErr)
                {
                    int indexOfSubstring = htmlParser.IndexOf(arSearchErr[0]);
                    if(indexOfSubstring < 0)
                    {
                        search_html = true;
                        htmlStr = htmlParser;
                        //Console.WriteLine(indexOfSubstring);
                        break;
                    }
                    else if(indexOfSubstring > 0)
                    {
                        continue;
                    }
                }
                url_copy = url;
                schetchek++;
            } while (search_html == false);

            //Console.WriteLine(htmlStr);

            strmas = htmlStr.Split(new char[] { ' ' });

            for(int i=0; i < strmas.Length; i++)
            {
                if(strmas[i] == search_href)
                {
                    url_detail_pdoto = strmas[i + 1];
                    //Console.WriteLine("OKEY");
                    //Console.WriteLine(strmas[i]);
                    //Console.WriteLine(strmas[i + 1]);
                    break;
                }
            }

            //Console.WriteLine(url_detail_pdoto);

            url_detail_pdoto = url_detail_pdoto.Substring(6);
            url_detail_pdoto = url_detail_pdoto.Substring(0, url_detail_pdoto.Length - 1);
            //Console.WriteLine(url_detail_pdoto);

            //System.Diagnostics.Process.Start(url_detail_pdoto);

            htmlParser = Download(url_detail_pdoto);
            //Console.WriteLine(htmlParser);

            strmas = htmlParser.Split(new char[] { ' ' });
            for (int i = 0; i < strmas.Length; i++)
            {
                if (strmas[i] == search_img)
                {
                    url_img = strmas[i + 1];
                    //Console.WriteLine("OKEY");
                    //Console.WriteLine(strmas[i]);
                    //Console.WriteLine(strmas[i + 1]);
                    break;
                }
            }

            url_img = url_img.Substring(5);
            url_img = url_img.Substring(0, url_img.Length - 1);
            //Console.WriteLine(url_img);

            //System.Diagnostics.Process.Start(url_img);

            using (WebClient client = new WebClient())
            {
                date = DateTime.Now;
                date_now = "Photo_" + date.ToLocalTime();
                date_now = date_now.Replace(":", "_");
                path = AppDomain.CurrentDomain.BaseDirectory;
                client.DownloadFile(url_img, AppDomain.CurrentDomain.BaseDirectory + $"{date_now}.jpg");
                //Console.WriteLine($"Фото скачено в {path}{date_now}.jpg");
            }

            SetWallpaper(path + date_now + ".jpg");

            Console.WriteLine(schetchek);
            Console.WriteLine(url_test);
            Console.WriteLine("OKEY!");
        }

        [DllImport("user32.dll")]
        public static extern Int32 SystemParametersInfo(
               UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public static void SetWallpaper(String path)
        {
            //Console.WriteLine("Setting wallpaper to '" + path + "'");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static string RandomString(int size) // РАНДОМНАЯ СТРОКА  
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string Download(string uri) // ПОЛУЧЕНИЕ HTML КОДА СТРАНИЦЫ
        {
            WebClient client = new WebClient();

            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);
            string str = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return str;
        }

        public static string Word()
        {
            WebClient client = new WebClient();
            Random rnd = new Random();
            string line = null;
            string[] line_mas;
            string path = null;
            int i_random;

            path = AppDomain.CurrentDomain.BaseDirectory + "RUS.txt";
            //Console.WriteLine(path);

            using (StreamReader sr = new StreamReader(path))
            {
                line = sr.ReadToEnd();
            }

            //Console.WriteLine(line);

            line_mas = line.Split(new char[] { '\n' });
            //Console.WriteLine(line_mas.Length);
            i_random = rnd.Next(0, line_mas.Length);
            //Console.WriteLine(line_mas[i_random]);
            //for(int i = 0; i < line_mas.Length; i++)
            //{
            //    Console.WriteLine(line_mas[i]);
            //}
            return line_mas[i_random];
        }
    }
}