using System;
using System.IO;
using compiler.Lexical;
using compiler.Syntactic;

namespace compiler
{

    class MainClass{
        public static void Main(string[] args){
            string text = File.ReadAllText(args[0]);
            LexicalAnalyzer lexic = new LexicalAnalyzer(text);
            lexic.Analize();
            FileWriter("Lexical_Analysis.txt", lexic.report);
            //SyntacticAnalyzer syntactic = new SyntacticAnalyzer(lexic.tokens);
            //syntactic.AnalSint();


        }
    public static void FileWriter(String fileName, String content)
        {

            string path = Directory.GetCurrentDirectory() + "/" + fileName;
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write(content);
            }
        }
    }

}