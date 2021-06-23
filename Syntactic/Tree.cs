using System;
using System.IO;
namespace compiler.Syntactic
{
    public class Tree
    {
        public NodeTree raiz;
        private NodeTree trabajo;
        private int i = 0;
        public Tree()
        {
            raiz = new NodeTree();
        }

        public NodeTree Insertar(string pDato, NodeTree pNodo)
        {
            //Si no hay nodo, tomamos como si fuera la raiz
            if (pNodo == null)
            {
                raiz = new NodeTree
                {
                    Dato = pDato,

                    //No hay hijo
                    Hijo = null,

                    //No hay hermano
                    Hermano = null
                };

                return raiz;
            }

            //Verificamos si no tiene hijo 
            //Insertamos el dato como hijo
            if (pNodo.Hijo == null)
            {
                NodeTree temp = new NodeTree
                {
                    Dato = pDato
                };

                //Conectamos el nuevo nodo como hijo
                pNodo.Hijo = temp;

                return temp;
            }
            else // ya tiene un hijo tenemos que insertarlo como hermano
            {
                trabajo = pNodo.Hijo;

                //Avanzamos hasta el ultimo hermano
                while (trabajo.Hermano != null)
                {
                    trabajo = trabajo.Hermano;
                }

                //Creamos nodo temporal
                NodeTree temp = new NodeTree
                {
                    Dato = pDato
                };

                //Unimos el temporal al ultimo hermano
                trabajo.Hermano = temp;

                return temp;
            }
        }
        public void PreOrden(NodeTree pNodo, TextWriter escribeSint)
        {
            if (pNodo == null)
            {
                return;
            }


            //Me proceso primero a mi
            for (int n = 0; n < i; n++)
            {
                Console.Write("  ");
                escribeSint.Write("     ");
            }

            escribeSint.WriteLine(pNodo.Dato);
            Console.WriteLine(pNodo.Dato);

            //Luego proceso a mi hijo
            if (pNodo.Hijo != null)
            {
                i++;
                PreOrden(pNodo.Hijo, escribeSint);
                i--;
            }

            //Si tengo hermanos, los proceso
            if (pNodo.Hermano != null)
                PreOrden(pNodo.Hermano, escribeSint);

        }

    }
}
