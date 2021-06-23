using System;
using System.Collections.Generic;
using System.IO;
using compiler.Lexical;

namespace compiler.Syntactic
{
    class SyntacticAnalyzer
    {
        private int index;
        private readonly List<Token> tokens;
        public String error;


        private TextWriter writeSyntactic, writeErrors;
        private Tree syntacticTree = new Tree();
        private NodeTree root = new NodeTree();
        private NodeTree actualNode = new NodeTree();
        private bool errorsFlag = false;

        public SyntacticAnalyzer( List<Token> tokens )
        {
            this.tokens = tokens;
            this.index = 0;
            this.error = "No hay errores";
            writeSyntactic = new StreamWriter(Directory.GetCurrentDirectory() + "/syntactic_analysis.txt");
            writeErrors = new StreamWriter(Directory.GetCurrentDirectory() + "/errors_detected.txt");

        }
        private void SetError (String message)
        {
            error = "Se esperaba "+ message + " en la linea " + tokens[index].row + " columna " + tokens[index].col ;

        }
        private void Next()
        {
            index++;
        }
        private void Back()
        {
            index++;
        }
        private void WriteError(String msg)
        {
            Console.WriteLine(msg + " near " + tokens[index].lexema + " inline " + tokens[index].row.ToString());
            String message = msg + " near " + tokens[index].lexema + " inline " + tokens[index].row.ToString();
            try
            {
                writeErrors.WriteLine(message);
            }catch(IOException e)
            {
                Console.WriteLine(e);
            }


        }
        public void AnalSint()
        {
            root = null;
            if ( tokens[index].lexema != "program")
            {
                writeErrors.WriteLine("Function program not found line " + tokens[index].row);
            }
            else {
                for (index = 0; index < tokens.Count; index++)
                {
                    if (root == null && tokens[index].lexema == "program")
                    {
                        root = syntacticTree.Insertar(tokens[index].lexema, null);
                        actualNode = root;
                        Next();
                        if (tokens[index].lexema != "{")
                        {
                            WriteError("'{' Symbol not found");
                        }
                        else
                        {
                            Next();
                        }
                        if(tokens[index].lexema == "int" || tokens[index].lexema == "float" || tokens[index].lexema == "boolean")
                        {
                            ListaDeclaracion(syntacticTree, actualNode);
                        }
                        else
                        {
                            ListaSentencia(syntacticTree, actualNode);
                        }
                    }
                    else if (!errorsFlag )
                    {
                        ListaSentencia(syntacticTree, actualNode);
                    }
                }
            }
            if (!errorsFlag)
            {
                syntacticTree.PreOrden(root, writeSyntactic);
            }

        }
        private void ListaDeclaracion(Tree tree, NodeTree node)
        {
            try
            {
                do
                {
                    if (tokens[index].lexema == "int" || tokens[index].lexema == "float" || tokens[index].lexema == "boolean")
                    {
                        node = tree.Insertar(tokens[index].lexema, node);

                    }
                    else if (tokens[index].tokenType == "Identificador")
                    {
                        tree.Insertar(tokens[index].lexema, node);
                        if (tokens[index + 1].lexema != "," && tokens[index + 1].lexema != ";")
                        {
                            errorsFlag = true;
                            WriteError("',' or ';'");
                            break;
                        }
                    }
                    else if (tokens[index].lexema == "{")
                    {
                        errorsFlag = true;
                        WriteError("'{' Symbol unrecognizable ");
                    }
                    else if (tokens[index].lexema == "}")
                    {
                        errorsFlag = true;
                        WriteError("'{' Symbol unrecognizable ");
                    }
                    Next();
                } while (tokens[index].lexema != ";" && (tokens[index].lexema != "if" && tokens[index].lexema != "else" && tokens[index].lexema != "write" && tokens[index].lexema != "do" && tokens[index].lexema != "while" && tokens[index].lexema != "read"));
                if (tokens[index].lexema == "if" || tokens[index].lexema == "do" || tokens[index].lexema == "write" || tokens[index].lexema == "do" || tokens[index].lexema == "while" || tokens[index].lexema == "read")
                {
                    Back();
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                errorsFlag = true;
                WriteError("';' Symbol not found");
            }
        }

        private void ListaSentencia(Tree tree, NodeTree node)
        {
            switch (tokens[index].lexema)
            {
                case "{":
                    if (tokens[index - 1].lexema  == "program")
                    {
                        Next();
                    }
                    break;
                case "}":
                    Next();
                    break;
                case "if":
                    Seleccion(syntacticTree, node);
                    break;
                case "else":
                    node = tree.Insertar(tokens[index].lexema, node);
                    index += 2;
                    ListaSentencia(tree, node);
                    break;
                case "while":
                    Iteracion(syntacticTree, node);
                    break;
                case "do":
                    Repeticion(syntacticTree, node);
                    break;
                case "read":
                    SentRead(syntacticTree, node);
                    break;
                case "write":
                    SentWrite(syntacticTree, node);
                    break;
                default:
                    if(tokens[index].tokenType == "Identificador")
                    {
                        Asignacion(syntacticTree, node);
                    }

                    break;
            }
        }
        private void Seleccion(Tree tree, NodeTree node)
        {
            NodeTree auxNode = new NodeTree();
            NodeTree nodeRoot = new NodeTree();
            nodeRoot = node;
            node = tree.Insertar(tokens[index].lexema, node);
            auxNode = node;
            Next();
            bool errorFlag = false;
            try
            {
                if (tokens[index].lexema == "(")
                {
                    Next();
                    while (tokens[index].lexema != ")")
                    {
                        if (tokens[index].lexema == "true" || tokens[index].lexema == "false")
                        {
                            node = tree.Insertar(tokens[index].lexema, node);
                        }
                        else
                        {
                            node = tree.Insertar(tokens[index + 1].lexema, node);
                            tree.Insertar(tokens[index].lexema, node);
                            tree.Insertar(tokens[index + 2].lexema, node);
                            index += 2;
                        }
                        Next();

                    }
                }
                else
                {
                    errorFlag = true;
                }
                Next();
                while (tokens[index].lexema != "fi")
                {
                    if (tokens[index].lexema == "then")
                    {
                        Next();
                        if (tokens[index].lexema == "{")
                        {
                            Next();
                            ListaSentencia(tree, auxNode);
                        }
                        else
                        {
                            errorFlag = true;
                        }
                    }
                    else
                    {
                        errorFlag = true;
                    }
                    if (tokens[index].lexema == "else")
                    {
                        node = tree.Insertar(tokens[index].lexema, nodeRoot);
                        auxNode = node;
                        Next();
                        if (tokens[index].lexema == "{")
                        {
                            Next();
                            ListaSentencia(tree, auxNode);
                        }
                        else
                        {
                            errorFlag = true;
                        }

                    }
                    index++;
                }
            }catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                errorsFlag = true;
                WriteError("')' Symbol not found");
            }

        }
        private void Iteracion(Tree tree, NodeTree node)
        {
            NodeTree auxNode = new NodeTree();
            NodeTree nodeRoot = new NodeTree();
            nodeRoot = node;
            node = tree.Insertar(tokens[index].lexema, node);
            auxNode = node;
            Next();


            ///Leer b-expresion
            bool errorFlag = false;
            if (tokens[index].lexema == "(")
            {
                Next();
                while (tokens[index].lexema != ")")
                {
                    if (tokens[index].lexema == "true" || tokens[index].lexema == "false")
                    {
                        node = tree.Insertar(tokens[index].lexema, node);
                    }
                    else
                    {
                        node = tree.Insertar(tokens[index + 1].lexema, node);
                        tree.Insertar(tokens[index].lexema, node);
                        tree.Insertar(tokens[index + 2].lexema, node);
                        index += 2;
                    }
                    Next();
                }
            }
            else
            {
                errorFlag = true;
            }
            Next();
            if (tokens[index].lexema == "{")
            {
                Next();
                ListaSentencia(tree, auxNode);
            }
            else
            {
                errorFlag = true;
            }
        }
        private void Repeticion(Tree tree, NodeTree node)
        {
            NodeTree auxNode = new NodeTree();
            NodeTree nodeRoot = new NodeTree();
            nodeRoot = node;
            node = tree.Insertar(tokens[index].lexema, node);
            auxNode = node;
            index++;

            bool errorFlag = false;
            try
            {
                if (tokens[index].lexema == "{")
                {
                    index++;
                    ListaSentencia(tree, auxNode);
                }
                else
                {
                    errorFlag = true;
                }
                index += 2;
                if (tokens[index].lexema == "}")
                {
                    Next();
                }
                else
                {
                    errorFlag = true;
                }
                if (tokens[index].lexema == "until")
                {
                    node = tree.Insertar(tokens[index].lexema, nodeRoot);
                    Next();
                    while (tokens[index].lexema != ";")
                    {
                        if (tokens[index].lexema == "(")
                        {
                            Next();
                            while (tokens[index].lexema != ")")
                            {
                                if (tokens[index].lexema == "true" || tokens[index].lexema == "false")
                                {
                                    node = tree.Insertar(tokens[index].lexema, node);
                                }
                                else
                                {
                                    node = tree.Insertar(tokens[index + 1].lexema, node);
                                    tree.Insertar(tokens[index].lexema, node);
                                    tree.Insertar(tokens[index + 2].lexema, node);
                                    index += 2;
                                }
                                Next();
                            }
                        }
                        else
                        {
                            errorFlag = true;
                        }
                        Next();
                    }
                }
            }catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                errorsFlag = true;
                writeErrors.WriteLine("Hay errores que verificar");
            }
        }
        private void SentRead(Tree tree, NodeTree node)
        {
            bool errorFlag = false;
            do
            {
                if (tokens[index].lexema == "read")
                {
                    node = tree.Insertar(tokens[index].lexema, node);
                }
                else if (tokens[index].lexema != "," && tokens[index].lexema != "{" && tokens[index].lexema != "}" && tokens[index].lexema != ";")
                {
                    tree.Insertar(tokens[index].lexema, node);
                }
                Next();
            } while (tokens[index].lexema != ";" && errorFlag == false);
        }
        private void SentWrite(Tree tree, NodeTree node)
        {

            NodeTree auxNode = new NodeTree();
            node = tree.Insertar(tokens[index].lexema, node);
            Next();
            bool errorFlag = false;
            while (tokens[index].lexema != ";")
            {
                if (tokens[index].lexema == "true" || tokens[index].lexema == "false")
                {
                    node = tree.Insertar(tokens[index].lexema, node);
                }
                else
                {

                    auxNode = BExpresion(tree, node);
                    node.Hijo = auxNode;
                    index = auxNode.AuxIteracion;
                }
            }
        }

        private NodeTree BExpresion(Tree tree, NodeTree node)
        {
            Tree treeSec = new Tree();
            NodeTree tempNode = new NodeTree();

            node.Hijo = Termino(treeSec);
            node.AuxIteracion = node.Hijo.AuxIteracion;
            index = node.AuxIteracion;

            NodeTree newNode = new NodeTree();
            tempNode = new NodeTree();

            while (tokens[index].lexema == "+" || tokens[index].lexema == "-")
            {
                NodeTree auxNode = new NodeTree
                {
                    Dato = tokens[index].lexema,
                    Hijo = treeSec.raiz
                };
                treeSec.raiz = auxNode;
                newNode = auxNode;
                index++;
                tempNode = Factor();
                index++;
                treeSec.Insertar(tempNode.Dato, newNode);
                tempNode = newNode;

            }
            tempNode.AuxIteracion = index;
            return tempNode;
        }
        private NodeTree Expresion(Tree tree, NodeTree node )
        {
            Tree treeSec = new Tree();
            NodeTree tempNode = new NodeTree();
            tempNode = Termino(treeSec);
            index = tempNode.AuxIteracion;

            NodeTree newNode = new NodeTree();
            while (tokens[index].lexema == "+" || tokens[index].lexema == "-")
            {
                NodeTree auxNode = new NodeTree
                {
                    Dato = tokens[index].lexema
                };
                if (auxNode.Hijo != null)
                {
                    auxNode.Hijo = treeSec.raiz;
                }
                treeSec.raiz = auxNode;
                if (treeSec.raiz.Hijo == null)
                {
                    treeSec.Insertar(tempNode.Dato, treeSec.raiz);
                }
                newNode = auxNode;
                index++;
                tempNode = Factor();
                index++;
                treeSec.Insertar(tempNode.Dato, newNode);
                tempNode = newNode;

            }
            tempNode.AuxIteracion = index;
            return tempNode;
        }

        private NodeTree Termino(Tree treeSec)
        {
            NodeTree tempNode = new NodeTree(), newNode = new NodeTree();
            tempNode = Factor();
            if (tokens[index+1].lexema == "*" || tokens[index + 1].lexema == "/")
            {
                Next();
                while (tokens[index].lexema == "*" || tokens[index].lexema == "/")
                {
                    if (treeSec.raiz.Dato == "")
                    {
                        newNode = treeSec.Insertar(tokens[index].lexema, null);
                        treeSec.Insertar(tempNode.Dato, newNode);
                        Next();
                        treeSec.Insertar(Factor().Dato, newNode);
                        Next();
                        tempNode = newNode;
                    }
                    else
                    {
                        NodeTree auxNode = new NodeTree
                        {
                            Dato = tokens[index].lexema,
                            Hijo = treeSec.raiz
                        };
                        treeSec.raiz = auxNode;
                        newNode = auxNode;
                        Next();
                        tempNode = Factor();
                        Next();
                        treeSec.Insertar(tempNode.Dato, newNode);
                        tempNode = newNode;
                    }
                }
            }
            else
            {
                treeSec.Insertar(tempNode.Dato, null);
                Next();
            }
            tempNode.AuxIteracion = index;
            return tempNode;
        }
        private NodeTree Factor()
        {
            NodeTree tempNode = new NodeTree();////Variables temporales
            if (tokens[index].lexema == "(")
            {

            }
            else
            {
                tempNode.Dato = tokens[index].lexema;////Insertamos valor en nodo
                Next();
            }
            return tempNode;
        }
        private void Asignacion(Tree tree, NodeTree node)
        {
            NodeTree auxNode = new NodeTree(), workNode;
            Next();
            bool errorFlag = false;
            if (tokens[index].lexema == "=")
            {
                node = tree.Insertar(tokens[index].lexema, node);
                tree.Insertar(tokens[index - 1].lexema, node);
                Next();
                while (tokens[index].lexema != ";")
                {
                    if (tokens[index].lexema == "{")
                    {
                        errorsFlag = true;
                        WriteError("'{' Unrecognized Symbol");
                    }
                    else if (tokens[index].lexema == "}")
                    {
                        errorsFlag = true;
                        WriteError("'}' Unrecognized Symbol");
                    }
                    if (tokens[index].lexema == "true" || tokens[index].lexema == "false")
                    {
                        node = tree.Insertar(tokens[index].lexema, node);
                    }
                    else
                    {
                        auxNode = Expresion(tree, node);
                        if (node.Hijo == null)
                        {
                            node.Hijo = auxNode;
                        }
                        else
                        {
                            workNode = new NodeTree();
                            workNode = node.Hijo;
                            while (workNode.Hermano != null)
                            {
                                workNode = workNode.Hermano;
                            }
                            workNode.Hermano = auxNode;
                        }
                        index = auxNode.AuxIteracion;
                    }
                }
            }
            else
            {
                errorFlag = true;
            }
        }


    }

}

