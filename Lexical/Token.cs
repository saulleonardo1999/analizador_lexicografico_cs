using System;

namespace compiler.Lexical
{
    class Token
    {
        public String lexema, tokenType;
        public int row, col;
        public Token()
        {
            lexema = "";
            tokenType = "";
            row = 0;
            col = 0;
        }

        public String GetToken()
        {
            return ("LEXEMA:\t" + lexema + "\nTOKEN:\t" + tokenType + "\nLINEA: " + row + " CARACTER: " + col + "\n\n");
        }
    }

}
