using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace GPSa
{
    class Program
    {
        private static Stream dataStream;
        private static String[] dati = new String[6];
        public static void pulisci()
        {
            dati[0] = "Via Enrico Fermi";
            dati[1] = "3";
            dati[2] = "Legnago";
            dati[3] = "Verona";
            dati[4] = "37045";
            dati[5] = "Italia";
        }

        static void Main(string[] args)
        {
            WebResponse response;
            char[] caratteri = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            char[] caratteriN = "1234567890".ToCharArray();
            char scelto;
            char sceltoN;
            
            int supporto;
            Console.WriteLine("Test da eseguire: ");
            //int imp = Int32.Parse(Console.ReadLine());
            int k = 10*6;
            char[] ar;
            pulisci();
            float[] media = new float[k / 6];
            float[] relevance = new float[k];
            string[] citta = new string[k];
            string[] via = new string[k];
            string[] civico = new string[k];
            int i = 0;

            for (i = 0; i < k; i++)
            {
                pulisci();
                Random a = new Random();
                sceltoN = caratteriN[a.Next(0, caratteriN.Length)];
                scelto = caratteri[a.Next(0, caratteri.Length)];
                supporto = (i % 6);
                switch (supporto)
                {
                    case 0:
                        ar = dati[0].ToCharArray();
                        ar[a.Next(0, dati[0].Length)] = scelto;
                        dati[0] = new string(ar);
                        break;
                    case 1:
                        ar = dati[1].ToCharArray();
                        ar[a.Next(0, dati[1].Length)] = sceltoN;
                        dati[1] = new string(ar);
                        break;
                    case 2:
                        ar = dati[2].ToCharArray();
                        ar[a.Next(0, dati[2].Length)] = scelto;
                        dati[2] = new string(ar);
                        break;
                    case 3:
                        ar = dati[3].ToCharArray();
                        ar[a.Next(0, dati[3].Length)] = scelto;
                        dati[3] = new string(ar);
                        break;
                    case 4:
                        ar = dati[4].ToCharArray();
                        ar[a.Next(0, dati[4].Length)] = sceltoN;
                        dati[4] = new string(ar);
                        break;
                    case 5:
                        ar = dati[5].ToCharArray();
                        ar[a.Next(0, dati[5].Length)] = scelto;
                        dati[5] = new string(ar);
                        break;
                    default:
                        break;
                }
                WebRequest wR = WebRequest.Create(//API connection
                    "https://geocoder.cit.api.here.com/6.2/geocode.json?app_id={ YOUR KEY }&app_code={ YOUR APP CODE }&searchtext="
                    + dati[0] //passaggio dei dati 
                    + "%20" + dati[1] + "%20" + dati[2] //%20 indica gli spazi
                    + "%20" + dati[3] + "%20" + dati[4]
                    + "%20" + dati[5]
                    );
                
                response = wR.GetResponse();//Lettura risposta
                Console.WriteLine(response);
                dataStream = response.GetResponseStream(); //Creazion datastream
                StreamReader reader = new StreamReader(dataStream); //creazione di un lettore per lo stream
                JObject o = JObject.Parse(reader.ReadToEnd()); //Parsing del JSON in oggetto
                response.Close();
    
                relevance[i] = (float)o["Response"]["View"][0]["Result"][0]["Relevance"];//Legge il campo relevance

                media[i%6] = media[i%6] + (relevance[i]*100);
                Console.WriteLine(relevance[i] * 100);
                citta[i] = (string)o["Response"]["View"][0]["Result"][0]["MatchQuality"]["City"]; //Legge l'accuratezza della città, della strada e del numero civico   
                via[i] = (string)o["Response"]["View"][0]["Result"][0]["MatchQuality"]["Street"][0];   
                civico[i] = (string)o["Response"]["View"][0]["Result"][0]["MatchQuality"]["HouseNumber"];
  
                Thread.Sleep(100);
            }      

            for (i = 0; i < k; i++)
            {
                media[i % 6] = media[i % 6] / (k / 6);
               
                Console.WriteLine("Siamo al caso " + (i % 6));
                Console.WriteLine("Relevance: " + relevance[i]);
                Console.WriteLine("Citta: " + citta[i]);
                Console.WriteLine("Civico: " + civico[i]);
            }
            float min = 100000;
            int indice = -1;
            for (i = 0; i < 6; i++)
            {
                Console.WriteLine(media[i]);
                if(media[i] < min)
                {
                    min = media[i];
                    indice = i;
                }
            }

            Console.WriteLine("Il campo più sensibile è il " + indice + " e vale " + min);

            Console.WriteLine("Premi INVIO per chiudere la finestra");
            Console.ReadLine();
        }

       

        void nascondi(){/* Console.WriteLine("Inserire la via:");
            dati[0] = Console.ReadLine();

            Console.WriteLine("Inserire il numero civico:");
            dati[1] = Console.ReadLine();

            Console.WriteLine("Inserire la città:");
            dati[2] = Console.ReadLine();

            Console.WriteLine("Inserire la provincia:");
            dati[3] = Console.ReadLine();

            Console.WriteLine("Inserire il C.A.P.:");
            dati[4] = Console.ReadLine();

            /* Console.WriteLine("Inserire lo stato:");
            dati[5] = Console.ReadLine();*/}

    }

}
