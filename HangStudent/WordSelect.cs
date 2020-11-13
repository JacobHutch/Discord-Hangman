using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;

namespace HangStudent
{
    class WordSelect
    {
        //========================================= Helper function for word selection ============================================
        static bool hasIllegalChar(string s)
        {
            bool illegal = true;
            string legals = "abcdefghijklmnopqrstuvwxyz -";
            char[] sa = s.ToLower().ToCharArray();

            foreach (char c in sa)
            {
                for (int i = 0; i < 28; i++)
                {
                    if (c == legals[i])
                    {
                        illegal = false;
                        break;
                    }
                }
                if (illegal)
                {
                    return true;
                }
                else
                {
                    illegal = true;
                }
            }

            return false;
        }
        //=========================================================================================================================

        //================================== VARIABLES =================================================
        //private static string tmpPath = Directory.GetCurrentDirectory();
        //private string wordlistPath = tmpPath.Substring(0, tmpPath.Length - 23) + "wordlist.txt";
        private string wordlistPath = Directory.GetCurrentDirectory() + "/wordlist.txt";
        //==============================================================================================

        public WordSelect() { }


        //========================================= Picks a random word from wordlist =============================================
        public string pickRandom()
        {
            string word = "";
            Random rand = new Random();

            while (word.Length < 6)
            {
                StreamReader file = new StreamReader(wordlistPath);

                int val = rand.Next() % 10000;
                for (int i = 1; i < val; i++)
                {
                    file.ReadLine();
                }

                word = file.ReadLine();
                word.Substring(0, word.Length - 1);

                file.Close();
            }

            return word;
        }
        //=========================================================================================================================


        //====================================== Gets chosen word from user and hides it ==========================================
        public string getChosen(SocketMessage msg, string chosen)
        {
            string word = "";
            int len = chosen.Length;
            
            if (chosen[0] == '|' && chosen[1] == '|' && chosen[len-1] == '|' && chosen[len-2] == '|')
            {
                if (!hasIllegalChar( chosen.Substring(2, len-4) ))
                {
                    word = chosen.Substring(2, len - 4);
                    msg.DeleteAsync();
                }
                else
                {
                    msg.Channel.SendMessageAsync("Error: Word has illegal characters in it");
                }
            }
            else
            {
                msg.Channel.SendMessageAsync("Error: Put spoiler tags around your word ( || )");
            }

            return word;
        }
        //=========================================================================================================================


        //========================================= Get random word from quizlet link =============================================
        public string[] pickLink(SocketMessage msg, string url)
        {
            string word = "";
            string def = "";
            Random rand = new Random();

            List<string> terms = new List<string>();
            List<string> defs = new List<string>();

            string html = "";
            //string url = "https://quizlet.com/76397882/software-engineering-flash-cards/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "C# console client";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            
            Regex re = new Regex("SetPageTerm-wordText\"><span class=\"TermText notranslate lang-en\">(.*?)<(.*?)SetPageTerm-definitionText\"><span class=\"TermText notranslate lang-en\">(.*?)<");
            MatchCollection matches = re.Matches(html);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups.Count == 4)
                    {
                        word = match.Groups[1].ToString();
                        if (word.Length <= 25 && !hasIllegalChar(word))
                        {
                            terms.Add(word);
                            def = match.Groups[3].ToString();
                            defs.Add(def);
                        }
                    }
                }

                int val = rand.Next() % terms.Count;
                word = terms[val].ToLower();
                def = defs[val];
            }
            else
            {
                msg.Channel.SendMessageAsync("Error: Cannot find any terms!");
            }

            string[] term = { word, def };
            return term;
        }
        //=========================================================================================================================
    }
}
