using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace compiler{

    class MainClass{
        public static void Main(string[] args){

            string text = File.ReadAllText(args[0]);
         
            Analizer analized = new Analizer(text);
            analized.Analize();
            Console.WriteLine(analized.report);

            string path = Directory.GetCurrentDirectory() + "/analizer.txt";
            Console.Write(path);
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(analized.report);
            }
        }
    }
    class TokenType
    {
        public const string 
            TKN_ID = "Identificador",
            TKN_NUM = "Número", 

            TKN_LPAREN = "Símbolo (", 
            TKN_RPAREN = "Símbolo )",
            TKN_LKEY = "Símbolo {",
            TKN_RKEY = "Símbolo }",
            TKN_SEMICOLON = "Símbolo ;", 
            TKN_COMMA = "Símbolo ,",
            TKN_ASSIGN = "Símbolo =", 
            TKN_ADD = "Símbolo +", 
            TKN_MINUS = "Símbolo -",
            TKN_SQR = "Símbolo ^",
            TKN_MULT = "Símbolo *",
            TKN_DIV = "Símbolo /",
            TKN_COMM = "Comentario //",
            TKN_RSCOMM = "Comentario bloque abierto /*",
            TKN_LSCOMM = "Comentario bloque cerrado */",
            TKN_MAJ = "Símbolo >",
            TKN_MAJE = "Símbolo >=",
            TKN_MIN = "Símbolo <",
            TKN_MINE = "Símbolo <=",
            TKN_EQUAL = "Símbolo ==",

            TKN_DIFF = "Símbolo !=",
            TKN_INV = "Símbolo !",


            TKN_ERROR = "Error";
    }
    class Token
    {
        public string lexema, tokenType;
        public int row, col;
        public Token()
        {
            lexema = "";
            tokenType = "";
            row = 0;
            col = 0;
        }

        public string GetToken()
        {
            return (
                "LEXEMA:\t" + 
                lexema + 
                "\nTOKEN:\t" + 
                tokenType + 
                "\nLINEA: " +
                row +
                " CARACTER: " +
                col + 
                "\n\n");
        }
    }
   
    class Analizer
    {
        private readonly string text;
        public string report;
        private List<Token> tokens;
        private int index, col, row;


        enum Status{
            IN_START, IN_ID, IN_NUM, IN_LPAREN, IN_RPAREN, IN_SEMICOLON,
            IN_COMMA, IN_ASSIGN, IN_ADD, IN_MINUS, IN_EOF, IN_ERROR, IN_DONE,

            IN_MINOR, IN_MAJOR, IN_EQUAL, IN_DIFF, IN_DIV, IN_MULT, IN_DOT
        }
        private readonly string[] ReservedWords = {
            "program", "if", "else", "fi", "do", "until", "while",
            "read", "write", "float", "int", "bool", "not", "and", "or"
        };


        public Analizer (string text)
        {
            this.text = text;
            tokens = new List<Token>();
            index = -1;
            col = 1;
            row = 1;
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
            MovePosition();
            return text[index];
        }
        private void MovePosition()
        {
            col++;
            if( text[index] == '\n')
            {
                col = 1;
                row++;
            }
        }
        private void UnGetChar()
        {
            index--;
            col--;
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
                        else if (c == '{')
                        {
                            token.tokenType = TokenType.TKN_LKEY;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == '}')
                        {
                            token.tokenType = TokenType.TKN_RKEY;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == ';')
                        {
                            token.tokenType = TokenType.TKN_SEMICOLON;
                            s = Status.IN_DONE;
                            token.lexema += c;
                            token.col = col - token.lexema.Length;
                        }
                        else if (c == ',')
                        {
                            token.tokenType = TokenType.TKN_COMMA;
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
                        else if (c == '^')
                        {
                            token.tokenType = TokenType.TKN_SQR;
                            s = Status.IN_DONE;
                            token.lexema += c;
                        }
                        else if (c == '*')
                        {
                            s = Status.IN_MULT;
                            token.lexema += c;
                        }
                        else if (c == '>')
                        {
                            s = Status.IN_MAJOR;
                            token.lexema += c;
                        }
                        else if (c == '<')
                        {
                            s = Status.IN_MINOR;
                            token.lexema += c;
                        }
                        else if (c == '/')
                        {
                            s = Status.IN_DIV;
                            token.lexema += c;
                        }
                        else if (c == '=')
                        {
                            s = Status.IN_EQUAL;
                            token.lexema += c;
                        }
                        else if (c == '!')
                        {
                            s = Status.IN_DIFF;
                            token.lexema += c;
                        }
                        else
                        {
                            token.lexema += c;
                            token.tokenType = TokenType.TKN_ERROR;
                            s = Status.IN_DONE;
                        }
                        break;
                    case Status.IN_DOT:
                        c = GetChar();
                        token.lexema += c;
                        if (char.IsDigit(c))
                        {
                            s = Status.IN_NUM;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_NUM;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_NUM:
                        c = GetChar();
                        token.lexema += c;
                        if ( c == '.')
                        {
                            s = Status.IN_DOT;
                        }
                        else if (!char.IsDigit(c)){
                            token.tokenType = TokenType.TKN_NUM;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_ID:
                        c = GetChar();
                        token.lexema += c;
                        if (!(char.IsLetterOrDigit(c) || (c == '_')))
                        {
                            token.tokenType = TokenType.TKN_ID;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                            token = FindReservedWords(token.lexema);
                        }
                        break;
                    case Status.IN_MULT:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '/')
                        {
                            token.tokenType = TokenType.TKN_LSCOMM;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_MULT;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_DIV:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '*')
                        {
                            token.tokenType = TokenType.TKN_RSCOMM;
                            s = Status.IN_DONE;
                        }
                        else if (c == '/')
                        {
                            token.tokenType = TokenType.TKN_COMM;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_DIV;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_MAJOR:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '=')
                        {
                            token.tokenType = TokenType.TKN_MAJE;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_MAJ;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_MINOR:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '=')
                        {
                            token.tokenType = TokenType.TKN_MINE;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_MIN;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_EQUAL:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '=')
                        {
                            token.tokenType = TokenType.TKN_EQUAL;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_ASSIGN;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
                        }
                        break;
                    case Status.IN_DIFF:
                        c = GetChar();
                        token.lexema += c;
                        if (c == '=')
                        {
                            token.tokenType = TokenType.TKN_DIFF;
                            s = Status.IN_DONE;
                        }
                        else
                        {
                            token.tokenType = TokenType.TKN_INV;
                            s = Status.IN_DONE;
                            UnGetChar();
                            token.lexema = token.lexema.Remove(token.lexema.Length - 1);
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
            token.row = row;
            token.col = col - token.lexema.Length;
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