using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace compiler{

    class MainClass{
        public static void Main(string[] args){

            string text = File.ReadAllText("/home/saul/Documentos/Csh/compiler/compiler/EmptyTextFile.txt");
         
            Analizer analized = new Analizer(text);
            analized.Analize();
            Console.WriteLine(analized.report);
           
        }
    }
    class TokenType
    {
        public const string 
            TKN_BEGIN = "Inicio", 
            TKN_END = "Final", 
            TKN_READ = "Leer", 
            TKN_WRITE = "Escribir", 
            TKN_ID = "Identificador",
            TKN_NUM = "Número", 
            TKN_LPAREN = "Símbolo (", 
            TKN_RPAREN = "Símbolo )", 
            TKN_SEMICOLON = "Símbolo ;", 
            TKN_COMMA = "Símbolo ,",
            TKN_ASSIGN = "Símbolo =", 
            TKN_ADD = "Símbolo +", 
            TKN_MINUS = "Símbolo -", 
            TKN_EOL = "Símbolo \0", 
            TKN_ERROR = "Error";
    }
    class Token
    {
        public string lexema;
        public string tokenType;
        public Token()
        {
            lexema = "";
            tokenType = "";
        }

        public string GetToken()
        {
            return ("Lexema: " + lexema + "\nToken: " + tokenType + "\n\n");
        }
    }
   
    class Analizer
    {
        private readonly string text;
        public string report;
        private List<Token> tokens;
        private int index;


        enum Status{
            IN_START, IN_ID, IN_NUM, IN_LPAREN, IN_RPAREN, IN_SEMICOLON,
            IN_COMMA, IN_ASSIGN, IN_ADD, IN_MINUS, IN_EOF, IN_ERROR, IN_DONE
        }
        private readonly string[] ReservedWords = new string[] {
            "program", "if ", "else", "fi", "do", "until", "while",
            "read", "write", "float", "int", "bool", "not", "and", "or"
        };


        public Analizer (string text)
        {
            this.text = text;
            tokens = new List<Token>();
            index = -1;
            report = "Aún no se ha efectuado el análisis o analisis nulo";
        }
        public void Analize()
        {

            Token token = new Token();
            token = FindToken();
            tokens.Add(token);
            while ( index != text.Length-1){
                token = FindToken();
                tokens.Add(token);
            }
            GenerateReport();

        }
        private void GenerateReport()
        {
            report = "";
            foreach(Token token in tokens)
            {
                report += token.GetToken();
            }
        }
        private char GetChar()
        {
            index++;
            return text[index];
        }

        private void UnGetChar()
        {
            index--;
        }
        private Token FindReservedWords(string lexema)
        {
            Token token = new Token();

            foreach(string word in ReservedWords)
            {
                if (lexema.Equals(word))
                {
                    token.lexema = word;
                    token.tokenType = "Palabra Reservada " + word;
                    return token; 
                }
            }
            token.lexema = lexema;
            token.tokenType = TokenType.TKN_ID;
            return token;

        }
        private Token FindToken(){
            Status s = Status.IN_START;
            char c;
            Token token = new Token();

            while (s != Status.IN_DONE)
            {
                switch (s)
                {
                    case Status.IN_START:
                        c = GetChar();
                        while ( IsDelimiter(c) )
                            c = GetChar();
                        
                        if ( Char.IsLetter(c))
                        {
                            s = Status.IN_ID;
                            token.lexema += c;
                        }
                        else if (Char.IsDigit(c))
                        {
                            s = Status.IN_NUM;
                            token.lexema += c;
                        }
                        else if (c == '(')
                        {
                            token.tokenType = TokenType.TKN_LPAREN;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == ')')
                        {
                            token.tokenType = TokenType.TKN_RPAREN;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == ';')
                        {
                            token.tokenType = TokenType.TKN_SEMICOLON;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == ',')
                        {
                            token.tokenType = TokenType.TKN_COMMA;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == '=')
                        {
                            token.tokenType = TokenType.TKN_ASSIGN;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == '+')
                        {
                            token.tokenType = TokenType.TKN_ADD;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == '-')
                        {
                            token.tokenType = TokenType.TKN_MINUS;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else
                        {
                            token.lexema += c;
                            token.tokenType = TokenType.TKN_ERROR;
                            s = Status.IN_DONE;
                        }
                        break;
                    case Status.IN_NUM:
                        c = GetChar();
                        token.lexema += c;
                        if (!char.IsDigit(c)){
                            token.tokenType = TokenType.TKN_NUM;
                            s = Status.IN_DONE;
                        }
                        break;
                    case Status.IN_ID:
                        c = GetChar();
                        token.lexema += c;
                        if (!(char.IsLetterOrDigit(c) || (c == '_')))
                        {
                            token.tokenType = TokenType.TKN_ID;
                            s = Status.IN_DONE;
                            token = FindReservedWords(token.lexema);
                        }
                        break;
                    default:
                        c = GetChar();
                        token.tokenType = TokenType.TKN_ERROR;
                        s = Status.IN_DONE;
                        token.lexema += c;
                        break;
                }

            }
            return token;
        }

        private bool IsDelimiter(char c)
        {
            if (c == ' ' || c == '\n'  || c == '\t' )
            {
                return true;
            }
            return false;
        }
       

    }
}
